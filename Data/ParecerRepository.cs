using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data
{
    public class ParecerRepository : EntityRepository<Parecer>, IParecerRepository
    {
        public ParecerRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
