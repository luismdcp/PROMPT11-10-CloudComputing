using System.Linq;

namespace CloudNotes.Repositories.Contracts
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Load();
        TEntity Get(string partitionKey, string rowKey);
        void Add(TEntity entityToAdd);
        void Update(TEntity entityToUpdate);
        void Delete(TEntity entityToDelete);
    }
}