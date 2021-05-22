using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using SCAP.ViewModels;

namespace SCAP.Controllers
{
    [Authorize, ActiveUser]
    public class MandatosController : BaseController
    {
        private readonly IMandatoService _mandatoService;
        private readonly IProfessorService _professorService;

        public MandatosController(IMandatoService mandatoService, IProfessorService professorService,
            IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
            : base(userService, mapper, notificator)
        {
            _mandatoService = mandatoService;
            _professorService = professorService;
        }

        [Route("mandatos")]
        public IActionResult Index()
        {
            var today = DateTime.Now.Date;
            var vigentes = Mapper.Map<IEnumerable<MandatoViewModel>>(_mandatoService.GetMandatosVigentes());
            var mandatos = Mapper.Map<IEnumerable<MandatoViewModel>>(_mandatoService.GetAllWithProfessor());

            var mandatoViewModelCollection = new MandatoViewModelCollection
            {
                Chefes = vigentes,
                Agendados = mandatos.Where(m => m.DataInicio.Date > today),
                Arquivados = mandatos.Where(m => m.DataFim.Date < today || m.Interrompido)
            };

            return View(mandatoViewModelCollection);
        }

        [Authorize(Roles = "Secretario")]
        [Route("mandatos/novo")]
        public IActionResult Create()
        {
            return ModalCreate();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Secretario")]
        [Route("mandatos/novo")]
        public IActionResult Create(MandatoViewModel mandatoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return ModalCreate(mandatoViewModel);
            }

            _mandatoService.Add(Mapper.Map<Mandato>(mandatoViewModel));

            if (ErrorsOccurred())
            {
                return ModalCreate(mandatoViewModel);
            }

            TempData["Success"] = "Mandato criado com sucesso!";

            return JsonOk(Url.Action(nameof(Index)));
        }

        private IActionResult ModalCreate(MandatoViewModel mandatoViewModel = null)
        {
            var professores = _professorService.GetAll()
                .Where(p => p.Ativo)
                .Select(p => new { p.Id, Label = $"{p.UserName} - {p.Nome} {p.Sobrenome}" });
            
            ViewBag.ProfessorId = new SelectList(professores, "Id", "Label", mandatoViewModel?.ProfessorId);

            var now = DateTime.Now.Date;

            return PartialView("Modals/_CreateMandatoModal", mandatoViewModel ?? new MandatoViewModel
            {
                DataInicio = now,
                DataFim = now.AddYears(2).AddDays(-1)
            });
        }

        [Authorize(Roles = "Secretario")]
        [Route("mandatos/{id:guid}/interromper")]
        public IActionResult Interromper(Guid id)
        {
            var mandato = _mandatoService.Get(id);

            if (mandato == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_InterromperMandatoModal", Mapper.Map<MandatoViewModel>(mandato));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Secretario")]
        [ActionName("Interromper")]
        [Route("mandatos/{id:guid}/interromper")]
        public IActionResult InterromperConfirmed(Guid id)
        {
            var mandato = _mandatoService.GetWithProfessor(id);

            if (mandato == null)
            {
                return NotFound();
            }

            _mandatoService.Interromper(id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_InterromperMandatoModal", Mapper.Map<MandatoViewModel>(mandato));
            }

            TempData["Success"] = "Mandato finalizado com sucesso!";

            return JsonOk(Url.Action(nameof(Index)));
        }

        [Authorize(Roles = "Secretario")]
        [Route("mandatos/{id:guid}/excluir")]
        public IActionResult Delete(Guid id)
        {
            var mandato = _mandatoService.Get(id);

            if (mandato == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_DeleteMandatoModal", Mapper.Map<MandatoViewModel>(mandato));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Secretario")]
        [ActionName("Delete")]
        [Route("mandatos/{id:guid}/excluir")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var mandato = _mandatoService.GetWithProfessor(id);

            if (mandato == null)
            {
                return NotFound();
            }

            _mandatoService.Remove(id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_DeleteMandatoModal", Mapper.Map<MandatoViewModel>(mandato));
            }

            TempData["Success"] = "Mandato excluído com sucesso!";

            return JsonOk(Url.Action(nameof(Index)));
        }
    }
}
