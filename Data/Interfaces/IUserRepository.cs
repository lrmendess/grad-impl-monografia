using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SCAP.Data.Interfaces
{
    public interface IUserRepository<TUser> : IDisposable where TUser : Pessoa
    {
        TUser Get(string id);
        TUser GetByEmail(string email);
        IEnumerable<TUser> GetAll();
        IEnumerable<TUser> GetAllAtivos();
        IEnumerable<TUser> Search(Expression<Func<TUser, bool>> predicate);
        bool Exists(string id);
    }
}
