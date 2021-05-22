using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using SCAP.ViewModels;

namespace SCAP.Controllers
{
    [Authorize(Roles = "Secretario, Professor"), ActiveUser]
    public class PareceresController : BaseController
    {
        private readonly IAfastamentoService _afastamentoService;
        private readonly IParecerService _parecerService;

        public PareceresController(IAfastamentoService afastamentoService, IParecerService parecerService,
            IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
            : base(userService, mapper, notificator)
        {
            _afastamentoService = afastamentoService;
            _parecerService = parecerService;
        }

        [Route("afastamentos/{afastamentoId:guid}/pareceres/novo")]
        public IActionResult Create(Guid afastamentoId)
        {
            var afastamento = _afastamentoService.Get(afastamentoId);
            
            if (afastamento == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Professor"))
            {
                return PartialView("Modals/_CreateParecerModal", new ParecerViewModel { AfastamentoId = afastamento.Id });
            }

            if (User.IsInRole("Secretario"))
            {
                return SecretarioModal(afastamento);
            }

            return Forbid();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("afastamentos/{afastamentoId:guid}/pareceres/novo")]
        public IActionResult Create(Guid afastamentoId, ParecerViewModel parecerViewModel)
        {
            if (afastamentoId != parecerViewModel.AfastamentoId)
            {
                return NotFound();
            }

            if (User.IsInRole("Professor"))
            {
                return AddParecerProfessor(parecerViewModel);
            }

            if (User.IsInRole("Secretario"))
            {
                return AddParecerSecretario(parecerViewModel);
            }

            return Forbid();
        }

        private IActionResult AddParecerProfessor(ParecerViewModel parecerViewModel)
        {
            var afastamento = _afastamentoService.Get(parecerViewModel.AfastamentoId);

            if (afastamento == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return PartialView("Modals/_CreateParecerModal", parecerViewModel);
            }

            parecerViewModel.ProfessorId = UserService.GetId(User);

            _parecerService.Add(Mapper.Map<Parecer>(parecerViewModel));

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_CreateParecerModal", parecerViewModel);
            }

            TempData["Success"] = "Parecer deferido com sucesso!";

            return JsonOk(Url.Action("Details", "Afastamentos", new { id = afastamento.Id }));
        }

        private IActionResult AddParecerSecretario(ParecerViewModel parecerViewModel)
        {
            var afastamento = _afastamentoService.Get(parecerViewModel.AfastamentoId);

            if (afastamento == null)
            {
                return NotFound();
            }

            ModelState.Remove("DocumentoParecer.Titulo");

            if (!ModelState.IsValid)
            {
                return SecretarioModal(afastamento);
            }

            var file = parecerViewModel.DocumentoParecer.File;
            
            if (parecerViewModel.Julgamento == 1)
            {
                if (afastamento.Situacao == SituacaoPedidoAfastamento.AprovadoDI)
                {
                    _afastamentoService.AprovarCT(afastamento.Id, file);
                }
                else if (afastamento.Situacao == SituacaoPedidoAfastamento.AprovadoCT)
                {
                    _afastamentoService.AprovarPRPPG(afastamento.Id, file);
                }
                else
                {
                    return Forbid();
                }
            }
            else if (parecerViewModel.Julgamento == 2)
            {
                _afastamentoService.Reprovar(afastamento.Id, file);
            }
            else
            {
                return Forbid();
            }

            if (ErrorsOccurred())
            {
                return SecretarioModal(afastamento);
            }

            TempData["Success"] = "Parecer deferido com sucesso!";

            return JsonOk(Url.Action("Details", "Afastamentos", new { id = afastamento.Id }));
        }

        private IActionResult SecretarioModal(Afastamento afastamento)
        {
            string modal;

            if (afastamento.Situacao == SituacaoPedidoAfastamento.AprovadoDI)
            {
                modal = "_CreateParecerCTModal";
            }
            else if (afastamento.Situacao == SituacaoPedidoAfastamento.AprovadoCT)
            {
                modal = "_CreateParecerPRPPGModal";
            }
            else
            {
                return Forbid();
            }

            return PartialView($"Modals/{modal}", new ParecerViewModel
            {
                AfastamentoId = afastamento.Id,
                DocumentoParecer = new DocumentoViewModel
                {
                    AfastamentoId = afastamento.Id
                }
            });
        }
    }
}
