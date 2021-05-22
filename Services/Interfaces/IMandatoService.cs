using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Services.Interfaces
{
    public interface IMandatoService : IEntityService<Mandato>
    {
        IEnumerable<Mandato> GetAllWithProfessor();
        IEnumerable<Mandato> GetMandatosVigentes();
        Mandato GetWithProfessor(Guid id);
        void Interromper(Guid id);
    }
}
