using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using SCAP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCAP.Controllers
{
    [Authorize(Roles = "Secretario"), ActiveUser]
    public class ParentescosController : BaseController
    {
        private readonly IProfessorService _professorService;
        private readonly IParentescoService _parentescoService;

        public ParentescosController(IProfessorService professorService, IParentescoService parentescoService,
            IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
            : base(userService, mapper, notificator)
        {
            _professorService = professorService;
            _parentescoService = parentescoService;
        }

        [Route("usuarios/{professorId:guid}/parentescos/novo")]
        public IActionResult Create(string professorId)
        {
            var professor = _professorService.GetWithParentescos(professorId);

            if (professor == null)
            {
                return NotFound();
            }

            return ParentescoModal(professor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("usuarios/{professorId:guid}/parentescos/novo")]
        public IActionResult Create(string professorId, ParentescoViewModel parentescoViewModel)
        {
            var professor = _professorService.GetWithParentescos(professorId);

            if (professor == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return ParentescoModal(professor);
            }
            
            _parentescoService.Add(Mapper.Map<Parentesco>(parentescoViewModel));

            if (ErrorsOccurred())
            {
                return ParentescoModal(professor);
            }

            TempData["Success"] = "Parentesco criado com sucesso!";

            return JsonOk(Url.Action("Details", "Users", new { id = professor.Id }));
        }

        private IActionResult ParentescoModal(Professor professor)
        {
            /**
             * Exceto parentes e o próprio solicitante.
             */
            var except = professor.Parentescos
                .Select(p => p.Parente.Id)
                .Concat(new List<string> { professor.Id });

            var professores = _professorService.GetAll()
                .Where(p => !except.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    Label = $"{p.UserName} - {p.Nome} {p.Sobrenome}"
                });

            ViewBag.ParenteId = new SelectList(professores, "Id", "Label");

            return PartialView("Modals/_CreateParentescoModal", new ParentescoViewModel { ParenteDeId = professor.Id });
        }

        [Route("parentescos/{id:guid}/excluir")]
        public IActionResult Delete(Guid id)
        {
            var parentesco = _parentescoService.Get(id);

            if (parentesco == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_DeleteParentescoModal", Mapper.Map<ParentescoViewModel>(parentesco));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        [Route("parentescos/{id:guid}/excluir")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var parentesco = _parentescoService.Get(id);

            if (parentesco == null)
            {
                return NotFound();
            }

            _parentescoService.Remove(id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_DeleteParentescoModal", Mapper.Map<ParentescoViewModel>(parentesco));
            }

            TempData["Success"] = "Parentesco excluído com sucesso!";

            return JsonOk(Url.Action("Details", "Users", new { id = parentesco.ParenteDeId }));
        }
    }
}
