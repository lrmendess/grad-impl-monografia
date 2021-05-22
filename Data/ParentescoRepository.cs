using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data
{
    public class ParentescoRepository : EntityRepository<Parentesco>, IParentescoRepository
    {
        public ParentescoRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
