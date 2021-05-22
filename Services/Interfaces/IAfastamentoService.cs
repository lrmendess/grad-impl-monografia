using Microsoft.AspNetCore.Http;
using SCAP.Models;
using System;
using System.Collections.Generic;

namespace SCAP.Services.Interfaces
{
    public interface IAfastamentoService : IEntityService<Afastamento>
    {
        Afastamento GetWithPareceres(Guid id);
        Afastamento GetWithAll(Guid id);
        IEnumerable<Afastamento> GetAllWithSolicitanteAndRelator();

        void ConfirmarSubmissaoDocumentos(Guid id);
        void Encaminhar(Guid id, string relatorId);
        void Bloquear(Guid id);
        void Desbloquear(Guid id, bool aprovar);
        void AprovarDI(Guid id);
        void AprovarDIScheduler(Guid id, string link);
        void AprovarCT(Guid id, IFormFile file);
        void AprovarPRPPG(Guid id, IFormFile file);
        void Reprovar(Guid id, IFormFile file = null);
        void Arquivar(Guid id);
        void ArquivarScheduler(Guid id);
        void Cancelar(Guid id);
    }
}
