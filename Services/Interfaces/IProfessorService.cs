using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Services.Interfaces
{
    public interface IProfessorService : IUserService<Professor>
    {
        Professor GetChefe();
        bool IsChefe(Professor professor);
        Professor GetWithParentescos(string id);
    }
}
