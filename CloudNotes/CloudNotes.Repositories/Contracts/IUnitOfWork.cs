using System.Linq;

namespace CloudNotes.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        IQueryable<T> Load<T>(string entitySetName);
        T Get<T>(string entitySetName, string partitionKey, string rowKey);
        void Add<T>(T entityToAdd, string entitySetName);
        void Update<T>(T entityToUpdate);
        void Delete<T>(T entityToDelete);
        void SubmitChanges();
    }
}