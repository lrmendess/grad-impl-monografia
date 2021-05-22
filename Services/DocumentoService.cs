using Microsoft.AspNetCore.Http;
using KissLog;
using SCAP.Data.Interfaces;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;

namespace SCAP.Services
{
    public class DocumentoService : EntityService<Documento>, IDocumentoService
    {
        public DocumentoService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger)
            : base(unitOfWork, notificator, logger) { }

        public Documento Add(Documento documento, IFormFile file)
        {
            documento.DataSubmissao = DateTime.Now;
            documento.NomeArquivo = ScapFileUtils.GenerateFileName(file);

            #region Validation
            if (!IsValid(new DocumentoValidator(), documento))
                return null;
            #endregion

            if (!UnitOfWork.Afastamentos.Exists(documento.AfastamentoId))
            {
                Error(string.Empty, $"{nameof(Afastamento)} não encontrado.");
                return null;
            }

            if (!ScapFileUtils.UploadFile(file, documento.NomeArquivo).Result)
            {
                Error(string.Empty, "Ocorreu um erro inesperado.");
                return null;
            }

            return base.Add(documento);
        }

        public override void Remove(Guid id)
        {
            var documento = UnitOfWork.Documentos.Get(id);

            #region Validation
            if (!IsValid(new DocumentoValidator(), documento))
                return;
            #endregion

            base.Remove(id);

            if (Notificator.HasNotifications())
                return;

            /**
             * Nesse caso não há necessidade de informar o usuário sobre o problema
             * na deleção, pois basta que ele não tenha mais acesso ao documento.
             */
            try
            {
                ScapFileUtils.DeleteFile(documento.NomeArquivo);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
