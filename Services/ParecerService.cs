using FluentValidation;
using Hangfire;
using KissLog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;
using System.Linq;

namespace SCAP.Services
{
    public class ParecerService : EntityService<Parecer>, IParecerService
    {
        private readonly IAfastamentoService _afastamentoService;
        private readonly IEmailService _emailService;

        #region Scheduler Dependencies
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        #endregion

        public ParecerService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger,
            IAfastamentoService afastamentoService, IEmailService emailService,
            IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator) : base(unitOfWork, notificator, logger)
        {
            _afastamentoService = afastamentoService;
            _emailService = emailService;

            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public override Parecer Add(Parecer parecer)
        {
            parecer.DataEmissao = DateTime.Now;

            #region Validation
            var validator = new ParecerValidator();

            validator.RuleFor(p => p.ProfessorId)
                .Must(pid => UnitOfWork.Professores.Exists(pid))
                .WithMessage("Professor não encontrado");

            if (!IsValid(validator, parecer))
                return null;

            var afastamentoValidator = new AfastamentoValidator();

            afastamentoValidator.RuleFor(a => a)
                .Must(a => a.CanChangeTo(SituacaoPedidoAfastamento.AprovadoDI))
                .WithMessage("Este pedido de afastamento (ainda) não pode receber pareceres.");

            afastamentoValidator.RuleFor(a => a.SolicitanteId)
                .NotEqual(parecer.ProfessorId)
                .WithMessage("O solicitante do afastamento e o responsável pelo parecer não podem ser a mesma pessoa.");

            /**
             * Pedidos internacionais precisam primeiro receber o parecer do relator.
             * Após isso podem receber manifestações de outros professores.
             */
            afastamentoValidator.RuleFor(a => new { a.RelatorId, a.Pareceres })
                .Must(a => a.RelatorId == parecer.ProfessorId || a.Pareceres.Any(p => p.ProfessorId == a.RelatorId))
                .When(a => a.TipoAfastamento == TipoAfastamento.Internacional)
                .WithMessage("Este pedido ainda não recebeu o parecer do relator.");

            afastamentoValidator.RuleFor(a => a.Pareceres)
                .Must(p => !p.Any(p => p.ProfessorId == parecer.ProfessorId))
                .WithMessage("Você já emitiu seu parecer.");

            var afastamento = UnitOfWork.Afastamentos.GetWithPareceres(parecer.AfastamentoId);

            if (!IsValid(afastamentoValidator, afastamento))
                return null;
            #endregion

            return AddParecer(parecer, afastamento);
        }

        private Parecer AddParecer(Parecer parecer, Afastamento afastamento)
        {
            using var transaction = UnitOfWork.BeginTransaction();

            try
            {
                var result = UnitOfWork.Pareceres.Add(parecer);

                if (parecer.Julgamento == TipoParecer.Desfavoravel)
                {
                    _afastamentoService.Bloquear(afastamento.Id);

                    if (Notificator.HasNotifications())
                    {
                        transaction.Rollback(); return null;
                    }
                }

                UnitOfWork.SaveChanges();
                transaction.Commit();

                /**
                 * Envia e-mail para todos os professores em caso do relator emitir
                 * um parecer favorável.
                 */
                if (afastamento.TipoAfastamento == TipoAfastamento.Internacional &&
                    afastamento.RelatorId == parecer.ProfessorId &&
                    parecer.Julgamento == TipoParecer.Favoravel)
                {
                    _emailService.SendEmailAboutAfastamento(
                        afastamento: afastamento,
                        pessoas: UnitOfWork.Professores.GetAllAtivos().Where(p => p.Id != afastamento.SolicitanteId && p.Id != afastamento.RelatorId),
                        subject: "Novo pedido de afastamento internacional",
                        message: "Caso queira contrariar o parecer positivo do relator, emita um parecer desfavorável.").Wait();
                }

                #region Scheduler
                if (parecer.Julgamento == TipoParecer.Favoravel &&
                    afastamento.TipoAfastamento == TipoAfastamento.Internacional)
                {
                    var link = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
                        action: "Details",
                        controller: "Afastamentos",
                        values: new { id = afastamento.Id },
                        scheme: _httpContextAccessor.HttpContext.Request.Scheme);

                    BackgroundJob.Schedule(() => _afastamentoService.AprovarDIScheduler(afastamento.Id, link),
                        DateTime.Now.AddDays(10));
                }
                #endregion

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, "Ocorreu um erro inesperado.");

                return null;
            }
        }
    }
}
