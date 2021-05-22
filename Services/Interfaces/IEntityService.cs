using SCAP.Models;
using System;
using System.Collections.Generic;

namespace SCAP.Services.Interfaces
{
    public interface IEntityService<TEntity> where TEntity : Entity
    {
        TEntity Get(Guid id);
        IEnumerable<TEntity> GetAll();
        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        void Remove(Guid id);
        bool Exists(Guid id);
    }
}
