using Microsoft.EntityFrameworkCore.Storage;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAfastamentoRepository Afastamentos { get; }
        IDocumentoRepository Documentos { get; }
        IMandatoRepository Mandatos { get; }
        IParecerRepository Pareceres { get; }
        IParentescoRepository Parentescos { get; }
        IProfessorRepository Professores { get; }
        ISecretarioRepository Secretarios { get; }

        int SaveChanges();
        IDbContextTransaction BeginTransaction();

        IEntityRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
        IUserRepository<TUser> UserRepository<TUser>() where TUser : Pessoa;
    }
}
