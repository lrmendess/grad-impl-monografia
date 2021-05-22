using Microsoft.AspNetCore.Http;
using KissLog;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;
using System.Collections.Generic;
using SCAP.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Hangfire;
using FluentValidation;

namespace SCAP.Services
{
    public class AfastamentoService : EntityService<Afastamento>, IAfastamentoService
    {
        private readonly IDocumentoService _documentoService;
        private readonly IEmailService _emailService;

        #region Scheduler Dependencies
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        #endregion

        public AfastamentoService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger,
            IDocumentoService documentoService, IEmailService emailService,
            IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator) : base(unitOfWork, notificator, logger)
        {
            _documentoService = documentoService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public Afastamento GetWithAll(Guid id) =>
            UnitOfWork.Afastamentos.GetWithAll(id);

        public Afastamento GetWithPareceres(Guid id) =>
            UnitOfWork.Afastamentos.GetWithPareceres(id);

        public IEnumerable<Afastamento> GetAllWithSolicitanteAndRelator() =>
            UnitOfWork.Afastamentos.GetAllWithSolicitanteAndRelator();

        public override Afastamento Add(Afastamento afastamento)
        {
            afastamento.DataPedido = DateTime.Now;
            afastamento.Situacao = SituacaoPedidoAfastamento.Iniciado;

            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a =>
                {
                    var solicitante = UnitOfWork.Professores.GetWithAfastamentos(a?.SolicitanteId);

                    return solicitante != null && !solicitante.Afastamentos.Any(a =>
                        a.DataInicio.Date <= afastamento.DataFimEvento.Date &&
                        a.DataFim.Date >= afastamento.DataInicioEvento.Date &&
                       (a.Situacao != SituacaoPedidoAfastamento.Cancelado   &&
                        a.Situacao != SituacaoPedidoAfastamento.Reprovado));
                })
                .WithMessage("Já existe um pedido de afastamento em andamento no intervalo estipulado.");

            if (!IsValid(validator, afastamento))
                return null;
            #endregion

            var result = base.Add(afastamento);

            if (Notificator.HasErrors())
                return null;

            BackgroundJob.Schedule(() => ArquivarScheduler(result.Id), result.DataFim.AddDays(1));

            return result;
        }

        public void ConfirmarSubmissaoDocumentos(Guid id)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a.Situacao)
                .Equal(SituacaoPedidoAfastamento.Iniciado)
                .WithMessage("Este pedido de afastamento já foi liberado.");

            validator.RuleFor(a => a.Documentos)
                .Must(d => d.Any())
                .WithMessage("Nenhum documento foi submetido.");

            var afastamento = UnitOfWork.Afastamentos.GetWithDocumentos(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            if (afastamento.TipoAfastamento == TipoAfastamento.Nacional)
            {
                afastamento.Situacao = SituacaoPedidoAfastamento.Liberado;
                
                base.Update(afastamento);
                
                if (Notificator.HasErrors())
                    return;

                #region Scheduler
                var link = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
                    action: "Details",
                    controller: "Afastamentos",
                    values: new { id },
                    scheme: _httpContextAccessor.HttpContext.Request.Scheme);

                BackgroundJob.Schedule(() => AprovarDIScheduler(afastamento.Id, link), DateTime.Now.AddDays(10));
                #endregion

                _emailService.SendEmailAboutAfastamento(
                    afastamento: afastamento,
                    pessoas: UnitOfWork.Professores.GetAll()
                        .Where(p => p.Id != afastamento.SolicitanteId && p.Ativo),
                    subject: "Novo pedido de afastamento nacional").Wait();
            }
            else
            {
                var chefe = UnitOfWork.Professores.GetChefe();

                if (chefe == null)
                {
                    Error(string.Empty, "Não há um chefe disponível para encaminhar o seu pedido no momento. Contate a secretaria o mais breve possível para que definam um novo chefe e você possa confirmar a submissão dos documentos.");
                    return;
                }

                _emailService.SendEmailAboutAfastamento(
                    afastamento: afastamento,
                    pessoa: chefe,
                    subject: "Novo pedido de afastamento internacional",
                    message: "Encaminhe o pedido para um relator.").Wait();
            }
        }
        
        public void Encaminhar(Guid id, string relatorId)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a.TipoAfastamento)
                .Equal(TipoAfastamento.Internacional)
                .WithMessage("Apenas pedidos internacionais podem ser encaminhados.");

            validator.RuleFor(a => a.SolicitanteId)
                .NotEqual(relatorId)
                .WithMessage("Solicitante e relator não podem ser a mesma pessoa.");

            validator.RuleFor(a => a.RelatorId)
                .Empty()
                .WithMessage("Um relator já foi definido.");

            validator.RuleFor(a => a.Documentos)
                .Must(d => d.Any())
                .WithMessage("Nenhum documento foi submetido.");

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.Liberado))
                .WithMessage("Este pedido de afastamento (ainda) não pode ser encaminhado.");

            var chefe = UnitOfWork.Professores.GetChefe();

            validator.RuleFor(a => chefe)
                .NotEmpty()
                .WithMessage("Não há um mandato de chefe de departamento em vigor para que o pedido possa ser encaminhado. Você não devia estar fazendo isso, amiguinho...");

            validator.RuleFor(a => relatorId)
                .NotEqual(chefe?.Id)
                .WithMessage("O chefe de departamento não pode ser o relator.");

            var relator = UnitOfWork.Professores.GetWithParentescos(relatorId);

            validator.RuleFor(a => relator)
                .NotEmpty()
                .WithMessage("Relator não encontrado");

            validator.RuleFor(a => relator.Ativo)
                .Equal(true)
                .WithMessage("Relator indisponível.");

            validator.RuleFor(a => a.SolicitanteId)
                .Must(sid => !relator.Parentescos.Any(p => p.ParenteId == sid))
                .WithMessage("O relator não pode ser parente do solicitante.");

            var afastamento = UnitOfWork.Afastamentos.GetWithDocumentos(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            afastamento.RelatorId = relatorId;
            afastamento.Situacao = SituacaoPedidoAfastamento.Liberado;

            base.Update(afastamento);

            if (Notificator.HasErrors())
                return;

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: relator,
                subject: "Você foi escolhido como relator de um novo pedido de afastamento",
                message: "O seu parecer como relator é necessário.").Wait();
        }

        public void Bloquear(Guid id)
        {
            #region Validation
            var validator = new AfastamentoValidator();
            var afastamento = UnitOfWork.Afastamentos.Get(id);

            // Só ignora...
            if (afastamento.Situacao == SituacaoPedidoAfastamento.Bloqueado)
                return;

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.Bloqueado))
                .WithMessage("Este pedido de afastamento (ainda) não pode ser bloqueado.");

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            afastamento.Situacao = SituacaoPedidoAfastamento.Bloqueado;

            base.Update(afastamento);

            if (Notificator.HasErrors())
                return;

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: UnitOfWork.Professores.Get(afastamento.SolicitanteId),
                subject: "Seu pedido de afastamento foi bloqueado").Wait();
        }
        
        public void Desbloquear(Guid id, bool aprovar)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a.Situacao)
                .Equal(SituacaoPedidoAfastamento.Bloqueado)
                .WithMessage("Este pedido de afastamento não está bloqueado.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            if (aprovar)
            {
                AprovarDI(afastamento.Id);
                return;
            }

            Reprovar(afastamento.Id);
        }

        public void AprovarDI(Guid id)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.AprovadoDI))
                .WithMessage("Este pedido de afastamento (ainda) não pode ser aprovado pelo DI.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            afastamento.Situacao = SituacaoPedidoAfastamento.AprovadoDI;

            base.Update(afastamento);

            if (Notificator.HasErrors())
                return;

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: UnitOfWork.Professores.Get(afastamento.SolicitanteId),
                subject: "Seu pedido de afastamento foi aprovado pelo DI").Wait();

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoas: UnitOfWork.Secretarios.GetAllAtivos(),
                message: "Faça o download da ata de aprovação para dar continuidade aos trâmites fora do DI.",
                subject: "Ata de aprovação disponível").Wait();
        }

        public void AprovarDIScheduler(Guid id, string link)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.AprovadoDI));

            validator.RuleFor(a => a.Pareceres)
                .Must(p => !p.Any(p => p.Julgamento == TipoParecer.Desfavoravel));

            var afastamento = UnitOfWork.Afastamentos.GetWithPareceres(id);

            validator.When(a => a.TipoAfastamento == TipoAfastamento.Internacional, () =>
            {
                validator.RuleFor(a => a.Pareceres)
                    .Must(pareceres => pareceres.Any(p => p.ProfessorId == afastamento?.RelatorId));
            });

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            AprovarDI(afastamento.Id);

            if (Notificator.HasErrors())
                return;

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: UnitOfWork.Professores.Get(afastamento.SolicitanteId),
                subject: "Seu pedido de afastamento foi aprovado pelo DI",
                link: link).Wait();

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoas: UnitOfWork.Secretarios.GetAllAtivos(),
                message: "Faça o download da ata de aprovação para dar continuidade aos trâmites fora do DI.",
                subject: "Ata de aprovação disponível",
                link: link).Wait();
        }

        public void AprovarCT(Guid id, IFormFile file)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.AprovadoCT))
                .WithMessage("Este pedido de afastamento (ainda) não pode ser aprovado pelo CT.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            var documento = new Documento
            {
                Titulo = "Parecer do CT",
                AfastamentoId = afastamento.Id
            };

            using var transaction = UnitOfWork.BeginTransaction();

            try
            {
                _documentoService.Add(documento, file);

                if (Notificator.HasErrors())
                {
                    transaction.Rollback(); return;
                }

                afastamento.Situacao = SituacaoPedidoAfastamento.AprovadoCT;

                UnitOfWork.Afastamentos.Update(afastamento);
                UnitOfWork.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, "Ocorreu um erro inesperado.");

                ScapFileUtils.DeleteFile(documento.NomeArquivo);

                return;
            }

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: UnitOfWork.Professores.Get(afastamento.SolicitanteId),
                subject: "Seu pedido de afastamento foi aprovado pelo CT").Wait();
        }

        public void AprovarPRPPG(Guid id, IFormFile file)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.AprovadoPRPPG))
                .WithMessage("Este pedido de afastamento (ainda) não pode ser aprovado pelo CT.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            var documento = new Documento
            {
                Titulo = "Parecer da PRPPG",
                AfastamentoId = afastamento.Id
            };

            using var transaction = UnitOfWork.BeginTransaction();

            try
            {
                _documentoService.Add(documento, file);

                if (Notificator.HasErrors())
                {
                    transaction.Rollback(); return;
                }

                afastamento.Situacao = SituacaoPedidoAfastamento.AprovadoPRPPG;

                UnitOfWork.Afastamentos.Update(afastamento);
                UnitOfWork.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, "Ocorreu um erro inesperado.");

                ScapFileUtils.DeleteFile(documento.NomeArquivo);

                return;
            }

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: UnitOfWork.Professores.Get(afastamento.SolicitanteId),
                subject: "Seu pedido de afastamento foi aprovado pela PRPPG").Wait();
        }

        public void Reprovar(Guid id, IFormFile file = null)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.Reprovado))
                .WithMessage("Este pedido de afastamento (ainda) não pode ser aprovado pelo CT.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            var dest = (afastamento.Situacao == SituacaoPedidoAfastamento.AprovadoDI ? "do CT" : "da PRPPG");
            var documento = new Documento
            {
                Titulo = $"Parecer {dest}",
                AfastamentoId = afastamento.Id
            };

            var transaction = UnitOfWork.BeginTransaction();

            try
            {
                /**
                 * Adiciona documento apenas para pareceres internacionais que não estejam bloqueados.
                 * Pedidos que são desbloqueados para reprovação não incluem documento, apenas a decisão
                 * da reunião da DI, implicando num afastamento nacional.
                 */
                if (afastamento.TipoAfastamento == TipoAfastamento.Internacional &&
                    afastamento.Situacao != SituacaoPedidoAfastamento.Bloqueado)
                {
                    _documentoService.Add(documento, file);

                    if (Notificator.HasErrors())
                    {
                        transaction.Rollback(); return;
                    }
                }

                afastamento.Situacao = SituacaoPedidoAfastamento.Reprovado;

                UnitOfWork.Afastamentos.Update(afastamento);
                UnitOfWork.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, "Ocorreu um erro inesperado.");

                /**
                 * Remove o arquivo criado se o afastamento for internacional.
                 */
                if (afastamento.TipoAfastamento == TipoAfastamento.Internacional)
                    ScapFileUtils.DeleteFile(documento.NomeArquivo);

                return;
            }

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoa: UnitOfWork.Professores.Get(afastamento.SolicitanteId),
                subject: "Seu pedido de afastamento foi reprovado").Wait();
        }
        
        public void Arquivar(Guid id)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.Arquivado))
                .WithMessage("Este pedido de afastamento não pode ser arquivado pois (ainda) não foi totalmente aprovado.");

            validator.RuleFor(a => a.DataFim)
                .LessThanOrEqualTo(DateTime.Now.Date)
                .WithMessage("O período de afastamento ainda não terminou.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            afastamento.Situacao = SituacaoPedidoAfastamento.Arquivado;

            base.Update(afastamento);
        }

        public void ArquivarScheduler(Guid id)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.Situacao == SituacaoPedidoAfastamento.AprovadoDI)
                .When(a => a.TipoAfastamento == TipoAfastamento.Nacional)
                .OnFailure(a => Cancelar(a.Id));

            validator.RuleFor(a => a)
                .Must(a => a.Situacao == SituacaoPedidoAfastamento.AprovadoPRPPG)
                .When(a => a.TipoAfastamento == TipoAfastamento.Internacional)
                .OnFailure(a => Cancelar(a.Id));

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            Arquivar(id);
        }

        public void Cancelar(Guid id)
        {
            #region Validation
            var validator = new AfastamentoValidator();

            validator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.Cancelado))
                .WithMessage("Este pedido de afastamento já foi finalizado.");

            var afastamento = UnitOfWork.Afastamentos.Get(id);

            if (!IsValid(validator, afastamento))
                return;
            #endregion

            afastamento.Situacao = SituacaoPedidoAfastamento.Cancelado;

            base.Update(afastamento);

            if (Notificator.HasErrors())
                return;

            _emailService.SendEmailAboutAfastamento(
                afastamento: afastamento,
                pessoas: UnitOfWork.Secretarios.GetAllAtivos(),
                subject: "Pedido de afastamento cancelado").Wait();
        }
    }
}
