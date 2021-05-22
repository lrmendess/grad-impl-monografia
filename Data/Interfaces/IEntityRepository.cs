using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SCAP.Data.Interfaces
{
    public interface IEntityRepository<TEntity> : IDisposable where TEntity : Entity
    {
        TEntity Get(Guid id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);
        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        void Remove(Guid id);
        bool Exists(Guid id);
    }
}
