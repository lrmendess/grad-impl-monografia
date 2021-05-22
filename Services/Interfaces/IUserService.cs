using Microsoft.AspNetCore.Identity;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SCAP.Services.Interfaces
{
    public interface IUserService<TUser> where TUser : Pessoa
    {
        TUser Get(string id);
        TUser GetByEmail(string email);
        IEnumerable<TUser> GetAll();
        IEnumerable<TUser> GetAllAtivos();
        void Add(TUser user);
        void Update(TUser user);
        void Remove(string id);
        bool Exists(string id);
        
        string GetId(ClaimsPrincipal user);
        string GeneratePasswordResetToken(TUser user);
        void ResetPassword(TUser user, string token, string newPassword);
    }
}
