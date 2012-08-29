using System;
using System.Linq;
using System.Linq.Expressions;

namespace CloudNotes.Repositories.Contracts
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Load();
        TEntity Get(string partitionKey, string rowKey);
        TEntity Get(Expression<Func<TEntity, bool>> filter);
        void Create(TEntity entityToCreate);
        void Update(TEntity entityToUpdate);
        void Delete(TEntity entityToDelete);
    }
}