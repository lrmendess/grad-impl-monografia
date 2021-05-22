using Microsoft.AspNetCore.Http;
using SCAP.Models;

namespace SCAP.Services.Interfaces
{
    public interface IDocumentoService : IEntityService<Documento>
    {
        Documento Add(Documento documento, IFormFile file);
    }
}
