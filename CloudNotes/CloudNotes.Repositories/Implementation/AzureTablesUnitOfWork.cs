using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using CloudNotes.Repositories.Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Implementation
{
    public class AzureTablesUnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly TableServiceContext _context;

        #endregion Fields

        #region Constructors

        public AzureTablesUnitOfWork()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            _context = new TableServiceContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials);
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<T> Load<T>(string entitySetName)
        {
            return _context.CreateQuery<T>(entitySetName);
        }

        public T Get<T>(string entitySetName, string partitionKey, string rowKey)
        {
            var query = _context.CreateQuery<T>(entitySetName);
            query.AddQueryOption("PartitionKey", partitionKey);
            query.AddQueryOption("RowKey", rowKey);

            return query.Execute().FirstOrDefault();
        }

        public void Add<T>(T entityToAdd, string entitySetName)
        {
            _context.AddObject(entitySetName, entityToAdd);
        }

        public void Update<T>(T entityToUpdate)
        {
            _context.UpdateObject(entityToUpdate);
        }

        public void Delete<T>(T entityToDelete)
        {
            _context.DeleteObject(entityToDelete);
        }

        public void SubmitChanges()
        {
            _context.SaveChangesWithRetries(SaveChangesOptions.Batch);
        }

        #endregion Public methods
    }
}