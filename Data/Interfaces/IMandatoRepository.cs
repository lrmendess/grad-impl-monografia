using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data.Interfaces
{
    public interface IMandatoRepository : IEntityRepository<Mandato>
    {
        Mandato GetWithProfessor(Guid id);
        IEnumerable<Mandato> GetAllWithProfessor();
        IEnumerable<Mandato> GetMandatosVigentes(DateTime date);
    }
}
