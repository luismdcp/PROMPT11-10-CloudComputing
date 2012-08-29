using System;
using System.Linq;
using System.Linq.Expressions;

namespace CloudNotes.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        IQueryable<T> Load<T>(string entitySetName);
        IQueryable<T> Load<T>(string entitySetName, Expression<Func<T, bool>> filter);
        T Get<T>(string entitySetName, string partitionKey, string rowKey);
        T Get<T>(string entitySetName, Expression<Func<T, bool>> filter);
        void Create<T>(T entityToCreate, string entitySetName);
        void Update<T>(string entitySetName, T entityToUpdate);
        void Delete<T>(string entitySetName, string partitionKey, string rowKey);
        void SubmitChanges();
    }
}