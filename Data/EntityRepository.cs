﻿using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SCAP.Data
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly ApplicationDbContext _context;

        public EntityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual TEntity Add(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public virtual TEntity Get(Guid id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public virtual TEntity Update(TEntity entity)
        {
            return _context.Set<TEntity>().Update(entity).Entity;
        }

        public virtual void Remove(Guid id)
        {
            _context.Set<TEntity>().Remove(new TEntity { Id = id });
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsNoTracking().ToList();
        }

        public bool Exists(Guid id)
        {
            return _context.Set<TEntity>().Find(id) != null;
        }

        public IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate).AsNoTracking().ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
