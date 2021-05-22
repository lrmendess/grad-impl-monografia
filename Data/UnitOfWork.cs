using Microsoft.EntityFrameworkCore.Storage;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;

namespace SCAP.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, dynamic> _repositories;

        public IAfastamentoRepository Afastamentos { get; }
        public IDocumentoRepository Documentos { get; }
        public IMandatoRepository Mandatos { get; }
        public IParecerRepository Pareceres { get; }
        public IParentescoRepository Parentescos { get; }
        public IProfessorRepository Professores { get; }
        public ISecretarioRepository Secretarios { get; }

        public UnitOfWork(ApplicationDbContext context,
            IAfastamentoRepository afastamentoRepository,
            IDocumentoRepository documentoRepository,
            IMandatoRepository mandatoRepository,
            IParecerRepository parecerRepository,
            IParentescoRepository parentescoRepository,
            IProfessorRepository professorRepository,
            ISecretarioRepository secretarioRepository)
        {
            _context = context;
            _repositories = new Dictionary<string, dynamic>();

            Afastamentos = afastamentoRepository;
            Documentos = documentoRepository;
            Mandatos = mandatoRepository;
            Pareceres = parecerRepository;
            Parentescos = parentescoRepository;
            Professores = professorRepository;
            Secretarios = secretarioRepository;
        }

        /**
         * Repositório genérico para modelos de domínio "comuns".
         */
        public IEntityRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            var type = typeof(TEntity).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IEntityRepository<TEntity>)_repositories[type];
            }

            var repositoryType = typeof(EntityRepository<>).MakeGenericType(typeof(TEntity));

            _repositories.Add(type, Activator.CreateInstance(repositoryType, _context));

            return _repositories[type];
        }

        /**
         * Repositório genérico para usuários.
         */
        public IUserRepository<TUser> UserRepository<TUser>() where TUser : Pessoa
        {
            var type = typeof(TUser).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IUserRepository<TUser>)_repositories[type];
            }

            var repositoryType = typeof(UserRepository<>).MakeGenericType(typeof(TUser));

            _repositories.Add(type, Activator.CreateInstance(repositoryType, _context));

            return _repositories[type];
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
