using System.Data.Services.Client;
using CloudNotes.Repositories.Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Implementation
{
    public class TableDataContext : TableServiceContext, IUnitOfWork
    {
        #region Properties

        private static readonly CloudStorageAccount StorageAccount;

        #endregion Properties

        #region Constructors

        static TableDataContext()
        {
            StorageAccount = CloudStorageAccount.Parse(TablesBuilder.StorageAccountConnectionString);
        }

        public TableDataContext() : base(StorageAccount.TableEndpoint.AbsoluteUri, StorageAccount.Credentials)
        {

        }

        #endregion Constructors

        #region Public methods

        public void SubmitChanges()
        {
            SaveChangesWithRetries(SaveChangesOptions.Batch);
        }

        public void SubmitChangesWithInsertOrReplace()
        {
            SaveChangesWithRetries(SaveChangesOptions.ReplaceOnUpdate);
        }

        #endregion Public methods
    }
}