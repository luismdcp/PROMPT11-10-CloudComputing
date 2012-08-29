using System;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using CloudNotes.Infrastructure.Exceptions;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Helpers;
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
            _context = new TableServiceContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials) { IgnoreResourceNotFoundException = true, IgnoreMissingProperties = true };
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<T> Load<T>(string entitySetName)
        {
            return _context.CreateQuery<T>(entitySetName).AsTableServiceQuery();
        }

        public IQueryable<T> Load<T>(string entitySetName, Expression<Func<T, bool>> filter)
        {
            return _context.CreateQuery<T>(entitySetName).Where(filter).AsTableServiceQuery();
        }

        public T Get<T>(string entitySetName, string partitionKey, string rowKey)
        {
            var query = string.Format("PartitionKey == \"{0}\" And RowKey == \"{1}\"", partitionKey, rowKey);
            return _context.CreateQuery<T>(entitySetName).Where(query).FirstOrDefault();
        }

        public T Get<T>(string entitySetName, Expression<Func<T, bool>> filter)
        {
            return _context.CreateQuery<T>(entitySetName).Where(filter).FirstOrDefault();
        }

        public void Create<T>(T entityToCreate, string entitySetName)
        {
            _context.AddObject(entitySetName, entityToCreate);
        }

        public void Update<T>(string entitySetName, T entityToUpdate)
        {
            var t = entityToUpdate.GetType();
            var properties = t.GetProperties();

            var partitionKey = (string) properties.Single(pi => pi.Name == "PartitionKey").GetValue(entityToUpdate, null);
            var rowKey = (string) properties.Single(pi => pi.Name == "RowKey").GetValue(entityToUpdate, null); ;

            var entity = Get<T>(entitySetName, partitionKey, rowKey);
            
            if (entity != null)
            {
                foreach (var p in properties)
                {
                    p.SetValue(entity, p.GetValue(entityToUpdate, null), null);
                }

                _context.UpdateObject(entity);
            }
        }

        public void Delete<T>(string entitySetName, string partitionKey, string rowKey)
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