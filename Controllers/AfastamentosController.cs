using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using SCAP.ViewModels;

namespace SCAP.Controllers
{
    [Authorize(Roles = "Secretario, Professor"), ActiveUser]
    public class AfastamentosController : BaseController
    {
        private readonly IAfastamentoService _afastamentoService;
        private readonly IProfessorService _professorService;
        private readonly ISecretarioService _secretarioService;
        private readonly IEmailService _emailService;

        public AfastamentosController(IAfastamentoService afastamentoService, IProfessorService professorService,
            ISecretarioService secretarioService, IEmailService emailService,
            IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
            : base(userService, mapper, notificator)
        {
            _afastamentoService = afastamentoService;
            _professorService = professorService;
            _secretarioService = secretarioService;
            _emailService = emailService;
        }

        [Route("/")]
        [Route("afastamentos")]
        public IActionResult Index()
        {
            var userId = UserService.GetId(User);

            var afastamentos = new AfastamentoViewModelCollection
            {
                Afastamentos = Mapper.Map<IEnumerable<AfastamentoViewModel>>(_afastamentoService.GetAllWithSolicitanteAndRelator())
            };

            afastamentos.AfastamentosAsSolicitante = afastamentos.Afastamentos
                .Where(a => a.SolicitanteId == userId);

            afastamentos.AfastamentosAsRelator = afastamentos.Afastamentos
                .Where(a => a.RelatorId == userId);

            return View(afastamentos);
        }

        [Route("afastamentos/{id:guid}")]
        public IActionResult Details(Guid id)
        {
            var afastamento = _afastamentoService.GetWithAll(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<AfastamentoViewModel>(afastamento));
        }

        [Authorize(Roles = "Professor")]
        [Route("afastamentos/solicitar")]
        public IActionResult Create()
        {
            return PartialView("Modals/_CreateAfastamentoModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Professor")]
        [Route("afastamentos/solicitar")]
        public IActionResult Create(AfastamentoViewModel afastamentoViewModel)
        {
            ModelState.Remove("Situacao");
            ModelState.Remove("DecisaoReuniao");

            if (!ModelState.IsValid)
            {
                return PartialView("Modals/_CreateAfastamentoModal", afastamentoViewModel);
            }

            afastamentoViewModel.SolicitanteId = UserService.GetId(User);

            var afastamento = _afastamentoService.Add(Mapper.Map<Afastamento>(afastamentoViewModel));

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_CreateAfastamentoModal", afastamentoViewModel);
            }

            TempData["Success"] = "Afastamento criado com sucesso!";

            return JsonOk(Url.Action(nameof(Details), new { id = afastamento.Id }));
        }

        [Authorize(Roles = "Professor")]
        [Route("afastamentos/{id:guid}/cancelar")]
        public IActionResult CancelarAfastamento(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            if (!IsOwnerOfId(afastamento.SolicitanteId))
            {
                return Forbid();
            }

            return PartialView("Modals/_CancelarAfastamentoModal", Mapper.Map<AfastamentoViewModel>(afastamento));
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Professor")]
        [Route("afastamentos/{id:guid}/cancelar")]
        [HttpPost, ActionName("CancelarAfastamento")]
        public IActionResult CancelarAfastamentoConfirmed(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            if (!IsOwnerOfId(afastamento.SolicitanteId))
            {
                return Forbid();
            }

            _afastamentoService.Cancelar(afastamento.Id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_CancelarAfastamentoModal", Mapper.Map<AfastamentoViewModel>(afastamento));
            }

            return JsonOk(Url.Action(nameof(Details), new { id }));
        }

        [Authorize(Roles = "Secretario")]
        [Route("afastamentos/{id:guid}/arquivar")]
        public IActionResult ArquivarAfastamento(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_ArquivarAfastamentoModal", Mapper.Map<AfastamentoViewModel>(afastamento));
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Secretario")]
        [Route("afastamentos/{id:guid}/arquivar")]
        [HttpPost, ActionName("ArquivarAfastamento")]
        public IActionResult ArquivarAfastamentoConfirmed(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            _afastamentoService.Arquivar(afastamento.Id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_ArquivarAfastamentoModal", Mapper.Map<AfastamentoViewModel>(afastamento));
            }

            return JsonOk(Url.Action(nameof(Details), new { id }));
        }

        [Authorize(Roles = "Secretario")]
        [Route("afastamentos/{id:guid}/desbloquear")]
        public IActionResult DesbloquearAfastamento(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_DesbloquearAfastamentoModal", Mapper.Map<AfastamentoViewModel>(afastamento));
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Secretario")]
        [Route("afastamentos/{id:guid}/desbloquear")]
        [HttpPost, ActionName("DesbloquearAfastamento")]
        public IActionResult DesbloquearAfastamentoConfirmed(Guid id, AfastamentoViewModel afastamentoViewModel)
        {
            if (id != afastamentoViewModel.Id)
            {
                return NotFound();
            }

            var afastamento = _afastamentoService.Get(afastamentoViewModel.Id);

            if (afastamento == null)
            {
                return NotFound();
            }

            _afastamentoService.Desbloquear(id, afastamentoViewModel.DecisaoReuniao == 1);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_DesbloquearAfastamentoModal", Mapper.Map<AfastamentoViewModel>(afastamento));
            }

            return JsonOk(Url.Action(nameof(Details), new { id }));
        }

        [Authorize(Roles = "Professor")]
        [Route("afastamentos/{id:guid}/encaminhar")]
        public IActionResult EncaminharAfastamento(Guid id)
        {
            var chefe = _professorService.Get(UserService.GetId(User));

            if (!_professorService.IsChefe(chefe))
            {
                return Forbid();
            }

            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            var afastamentoViewModel = Mapper.Map<AfastamentoViewModel>(afastamento);

            if (afastamentoViewModel.RelatorId != null)
            {
                ViewBag.SelectDisabled = true;
                ModelState.AddModelError("RelatorId", "Um relator já foi definido.");
            }
            else
            {
                ViewBag.SelectDisabled = false;
            }

            return EncaminharAfastamentoModal(afastamentoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Professor")]
        [Route("afastamentos/{id:guid}/encaminhar")]
        public IActionResult EncaminharAfastamento(Guid id, AfastamentoViewModel afastamentoViewModel)
        {
            if (id != afastamentoViewModel.Id)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(afastamentoViewModel.RelatorId))
            {
                ModelState.AddModelError("RelatorId", "O campo Relator é obrigatório.");
                return EncaminharAfastamentoModal(afastamentoViewModel);
            }

            _afastamentoService.Encaminhar(id, afastamentoViewModel.RelatorId);

            if (ErrorsOccurred())
            {
                return EncaminharAfastamentoModal(afastamentoViewModel);
            }

            return JsonOk(Url.Action(nameof(Details), new { id }));
        }

        private IActionResult EncaminharAfastamentoModal(AfastamentoViewModel afastamentoViewModel)
        {
            ViewBag.RelatorId = SelectListRelatores(afastamentoViewModel.SolicitanteId);
            return PartialView("Modals/_EncaminharAfastamentoModal", afastamentoViewModel);
        }

        private SelectList SelectListRelatores(string solicitanteId)
        {
            var chefe = _professorService.GetChefe();
            var solicitante = _professorService.GetWithParentescos(solicitanteId);

            /**
             * Todos os professores, exceto o chefe, solicitante e seus parentes.
             */
            var except = solicitante.Parentescos
                .Select(p => p.Parente.Id)
                .Concat(new List<string> { solicitante.Id, chefe?.Id });

            var professores = _professorService.GetAll()
                .Where(p => p.Ativo && !except.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    Label = $"{p.UserName} - {p.Nome} {p.Sobrenome}"
                });

            return new SelectList(professores, "Id", "Label");
        }

        [Authorize(Roles = "Professor")]
        [Route("afastamentos/{id:guid}/confirmar-submissao-documentos")]
        public IActionResult ConfirmarSubmissaoDocumentos(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            if (!IsOwnerOfId(afastamento.SolicitanteId))
            {
                return Forbid();
            }

            return PartialView("Modals/_ConfirmarSubmissaoDocumentosModal", Mapper.Map<AfastamentoViewModel>(afastamento));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Professor")]
        [ActionName("ConfirmarSubmissaoDocumentos")]
        [Route("afastamentos/{id:guid}/confirmar-submissao-documentos")]
        public IActionResult ConfirmarSubmissaoDocumentosPost(Guid id)
        {
            var afastamento = _afastamentoService.Get(id);

            if (afastamento == null)
            {
                return NotFound();
            }

            if (!IsOwnerOfId(afastamento.SolicitanteId))
            {
                return Forbid();
            }

            _afastamentoService.ConfirmarSubmissaoDocumentos(id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_ConfirmarSubmissaoDocumentosModal", Mapper.Map<AfastamentoViewModel>(afastamento));
            }

            if (afastamento.TipoAfastamento == TipoAfastamento.Nacional)
            {
                TempData["Success"] = "Todos os professores foram notificados sobre o seu pedido.";
            }
            else
            {
                TempData["Success"] = "O chefe de departamento foi notificado e irá escolher um relator para o seu pedido.";
            }

            return JsonOk(Url.Action(nameof(Details), new { id }));
        }

        [Route("afastamentos/{id:guid}/AtaAprovacaoDI")]
        public IActionResult GerarAtaAprovacaoDI(Guid id)
        {
            var afastamento = _afastamentoService.GetWithAll(id);

            if (afastamento == null ||
                afastamento.Situacao != SituacaoPedidoAfastamento.AprovadoDI &&
                afastamento.Situacao != SituacaoPedidoAfastamento.AprovadoCT &&
                afastamento.Situacao != SituacaoPedidoAfastamento.AprovadoPRPPG &&
                afastamento.Situacao != SituacaoPedidoAfastamento.Arquivado)
            {
                return NoContent();
            }

            return new ViewAsPdf("Ata/AtaAprovacaoDI", Mapper.Map<AfastamentoViewModel>(afastamento));
        }
    }
}
