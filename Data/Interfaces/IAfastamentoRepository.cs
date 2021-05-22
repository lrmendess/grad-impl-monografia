using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data.Interfaces
{
    public interface IAfastamentoRepository : IEntityRepository<Afastamento>
    {
        Afastamento GetWithPareceres(Guid id);
        Afastamento GetWithAll(Guid id);
        IEnumerable<Afastamento> GetAllWithSolicitanteAndRelator();
        Afastamento GetWithDocumentos(Guid id);
        Afastamento GetWithPareceresAndDocumentos(Guid id);
    }
}
