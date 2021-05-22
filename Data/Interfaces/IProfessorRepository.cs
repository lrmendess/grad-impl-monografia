using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data.Interfaces
{
    public interface IProfessorRepository : IUserRepository<Professor>
    {
        Professor GetChefe();
        bool IsChefe(Professor professor);
        Professor GetWithAll(string id);
        Professor GetWithParentescos(string id);
        Professor GetWithAfastamentos(string id);
    }
}
