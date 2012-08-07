using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        IQueryable<T> Load<T>(string entitySetName) where T : TableServiceEntity;
        T Get<T>(string entitySetName, string partitionKey, string rowKey) where T : TableServiceEntity;
        void Create<T>(T entityToCreate, string entitySetName) where T : TableServiceEntity;
        void Update<T>(string entitySetName, T entityToUpdate) where T : TableServiceEntity;
        void Delete<T>(string entitySetName, string partitionKey, string rowKey) where T : TableServiceEntity;
        void SubmitChanges();
    }
}