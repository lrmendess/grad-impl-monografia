using System;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using SCAP.ViewModels;

namespace SCAP.Controllers
{
    [Authorize(Roles = "Secretario, Professor"), ActiveUser]
    public class DocumentosController : BaseController
    {
        private readonly IDocumentoService _documentoService;
        private readonly IAfastamentoService _afastamentoService;

        public DocumentosController(IDocumentoService documentoService, IAfastamentoService afastamentoService,
            IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
            : base(userService, mapper, notificator)
        {
            _documentoService = documentoService;
            _afastamentoService = afastamentoService;
        }

        [Route("documentos/{name}/download")]
        public IActionResult Download(string name)
        {
            if (name == null)
            {
                return NoContent();
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", name);

            return PhysicalFile(path, "application/octet-stream", name);
        }

        [Route("afastamentos/{afastamentoId:guid}/documentos/novo")]
        public IActionResult Create(Guid afastamentoId)
        {
            var afastamento = _afastamentoService.Get(afastamentoId);

            if (afastamento == null)
            {
                return NotFound();
            }

            if (!IsSecretarioOrOwnerOfId(afastamento.SolicitanteId))
            {
                return Forbid();
            }

            return PartialView("Modals/_CreateDocumentoModal", new DocumentoViewModel { AfastamentoId = afastamentoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("afastamentos/{afastamentoId:guid}/documentos/novo")]
        public IActionResult Create(DocumentoViewModel documentoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Modals/_CreateDocumentoModal", documentoViewModel);
            }

            var afastamento = _afastamentoService.Get(documentoViewModel.AfastamentoId);

            if (!IsSecretarioOrOwnerOfId(afastamento?.SolicitanteId))
            {
                return Forbid();
            }

            _documentoService.Add(Mapper.Map<Documento>(documentoViewModel), documentoViewModel.File);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_CreateDocumentoModal", documentoViewModel);
            }

            TempData["Success"] = "Documento adicionado com sucesso!";

            return JsonOk(Url.Action("Details", "Afastamentos", new { id = documentoViewModel.AfastamentoId }));
        }

        [Route("documentos/{id:guid}/excluir")]
        public IActionResult Delete(Guid id)
        {
            var documento = _documentoService.Get(id);

            if (documento == null)
            {
                return NotFound();
            }

            var afastamento = _afastamentoService.Get(documento.AfastamentoId);

            if (!IsSecretarioOrOwnerOfId(afastamento?.SolicitanteId))
            {
                return Forbid();
            }

            return PartialView("Modals/_DeleteDocumentoModal", Mapper.Map<DocumentoViewModel>(documento));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        [Route("documentos/{id:guid}/excluir")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var documento = _documentoService.Get(id);

            if (documento == null)
            {
                return NotFound();
            }

            var afastamento = _afastamentoService.Get(documento.AfastamentoId);

            if (!IsSecretarioOrOwnerOfId(afastamento?.SolicitanteId))
            {
                return Forbid();
            }

            _documentoService.Remove(id);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_DeleteDocumentoModal", Mapper.Map<DocumentoViewModel>(documento));
            }

            TempData["Success"] = "Documento excluído com sucesso!";

            return JsonOk(Url.Action("Details", "Afastamentos", new { id = afastamento.Id }));
        }
    }
}
