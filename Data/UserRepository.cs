using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SCAP.Data
{
    public class UserRepository<TUser> : IUserRepository<TUser> where TUser : Pessoa, new()
    {
        protected readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual TUser Get(string id)
        {
            return _context.Set<TUser>().Find(id);
        }

        public virtual TUser GetByEmail(string email)
        {
            return _context.Set<TUser>().Where(u => u.Email == email).FirstOrDefault();
        }
        
        public virtual IEnumerable<TUser> GetAll()
        {
            return _context.Set<TUser>().ToList();
        }
        
        public virtual IEnumerable<TUser> GetAllAtivos()
        {
            return _context.Set<TUser>().Where(u => u.Ativo).ToList();
        }

        public virtual bool Exists(string id)
        {
            return _context.Set<TUser>().Find(id) != null;
        }

        public IEnumerable<TUser> Search(Expression<Func<TUser, bool>> predicate)
        {
            return _context.Set<TUser>().Where(predicate).AsNoTracking().ToList();
        }

        public virtual void Dispose()
        {
            _context?.Dispose();
        }
    }
}
