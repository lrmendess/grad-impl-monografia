using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data
{
    public class SecretarioRepository : UserRepository<Secretario>, ISecretarioRepository
    {
        public SecretarioRepository(ApplicationDbContext context)
            : base(context)
        {

        }
    }
}
