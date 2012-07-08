using System;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Xml;
using CloudNotes.Infrastructure.Exceptions;
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
            _context = new TableServiceContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials)
                           {IgnoreResourceNotFoundException = true};
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
            try
            {
                _context.SaveChangesWithRetries(SaveChangesOptions.Batch);
            }
            catch (Exception ex)
            {
                throw MapTableServiceContextException(ex);
            }
        }

        #endregion Public methods

        #region Private methods

        private static Exception MapTableServiceContextException(Exception exception)
        {
            Exception returnException = null;

            while (!(exception is DataServiceClientException) && (exception != null))
            {
                exception = exception.InnerException;    
            }

            if (exception == null)
            {
                return new Exception("Unknown error.");
            }

            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(exception.Message);
                var namespaceManager = new XmlNamespaceManager(xml.NameTable);
                namespaceManager.AddNamespace("n", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                var errorCode = xml.SelectSingleNode("/n:error/n:code", namespaceManager).InnerText;

                switch (errorCode)
                {
                    case "UpdateConditionNotSatisfied":
                        returnException = new ConcurrencyException();
                        break;
                    case "EntityAlreadyExists":
                        returnException = new EntityAlreadyExistsException();
                        break;
                    case "InvalidValueType":
                        returnException = new InvalidValueTypeException();
                        break;
                    default:
                        returnException = new Exception("An error occurred when saving changes.");
                        break;
                }
            }
            catch (Exception ex)
            {
                returnException = new Exception("An error occurred when saving changes.", ex);
            }

            return returnException;
        }

        #endregion Private methods
    }
}