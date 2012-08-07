using System;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Reflection;
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
            _context = new TableServiceContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials) { IgnoreResourceNotFoundException = true };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<T> Load<T>(string entitySetName) where T : TableServiceEntity
        {
            return _context.CreateQuery<T>(entitySetName);
        }

        public T Get<T>(string entitySetName, string partitionKey, string rowKey) where T : TableServiceEntity
        {
            var entities = _context.CreateQuery<T>(entitySetName).Where(e => e.PartitionKey == partitionKey && e.RowKey == rowKey);
            return entities.FirstOrDefault();
        }

        public void Create<T>(T entityToCreate, string entitySetName) where T : TableServiceEntity
        {
            _context.AddObject(entitySetName, entityToCreate);
        }

        public void Update<T>(string entitySetName, T entityToUpdate) where T : TableServiceEntity
        {
            var entities = _context.CreateQuery<T>(entitySetName).Where(e => e.PartitionKey == entityToUpdate.PartitionKey && e.RowKey == entityToUpdate.RowKey);
            var entity = entities.FirstOrDefault();

            if (entity != null)
            {
                Type t = entityToUpdate.GetType();
                PropertyInfo[] pi = t.GetProperties();

                foreach (PropertyInfo p in pi)
                {
                    p.SetValue(entity, p.GetValue(entityToUpdate, null), null);
                }

                _context.UpdateObject(entity); 
            }
        }

        public void Delete<T>(string entitySetName, string partitionKey, string rowKey) where T : TableServiceEntity
        {
            var entityToDelete = Get<T>(entitySetName, partitionKey, rowKey);

            if (entityToDelete != null)
            {
                _context.DeleteObject(entityToDelete);           
            }
        }

        public void SubmitChanges()
        {
            try
            {
                _context.SaveChanges();
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
                var selectSingleNode = xml.SelectSingleNode("/n:error/n:code", namespaceManager);
                
                if (selectSingleNode != null)
                {
                    var errorCode = selectSingleNode.InnerText;

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