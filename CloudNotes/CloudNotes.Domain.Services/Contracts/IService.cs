using System.Linq;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface IService<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> Load();
        TEntity Get(string partitionKey, string rowKey);
        void Add(TEntity entityToAdd);
        void Update(TEntity entityToUpdate);
        void Delete(TEntity entityToDelete);
    }
}