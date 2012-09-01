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
    /// <summary>
    /// Implementation of the UnitOfWork Pattern for operations with Azure Tables.
    /// </summary>
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

        public AzureTablesUnitOfWork(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _context = new TableServiceContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials) { IgnoreResourceNotFoundException = true, IgnoreMissingProperties = true };
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Loads all the entities from a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entitySetName">Azure Table name</param>
        /// <returns>IQueryable of objects related to the Azure Tables</returns>
        public IQueryable<T> Load<T>(string entitySetName)
        {
            return _context.CreateQuery<T>(entitySetName).AsTableServiceQuery();
        }

        /// <summary>
        /// Filter the entities from a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entitySetName">Azure Table name</param>
        /// <param name="filter">Lambda to filter the result</param>
        /// <returns>Filtered IQueryable of objects related to the Azure Tables</returns>
        public IQueryable<T> Load<T>(string entitySetName, Expression<Func<T, bool>> filter)
        {
            return _context.CreateQuery<T>(entitySetName).Where(filter).AsTableServiceQuery();
        }

        /// <summary>
        /// Gets a single entity from a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entitySetName">Azure Table name</param>
        /// <param name="partitionKey">The entity partition key</param>
        /// <param name="rowKey">The entity row key</param>
        /// <returns>A single entity from the Azure Table or null if it doesn't exists</returns>
        public T Get<T>(string entitySetName, string partitionKey, string rowKey)
        {
            var query = string.Format("PartitionKey == \"{0}\" And RowKey == \"{1}\"", partitionKey, rowKey);
            return _context.CreateQuery<T>(entitySetName).Where(query).FirstOrDefault();
        }

        /// <summary>
        /// Gets a filtered single entity from a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entitySetName">Azure Table name</param>
        /// <param name="filter">Lambda to filter the result</param>
        /// <returns>A single entity from the Azure Table or null if it doesn't exists</returns>
        public T Get<T>(string entitySetName, Expression<Func<T, bool>> filter)
        {
            return _context.CreateQuery<T>(entitySetName).Where(filter).FirstOrDefault();
        }

        /// <summary>
        /// Creates an entity in a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entityToCreate">Entity to create in a Azure Table</param>
        /// <param name="entitySetName">Azure Table name</param>
        public void Create<T>(T entityToCreate, string entitySetName)
        {
            _context.AddObject(entitySetName, entityToCreate);
        }

        /// <summary>
        /// Updates an entity in a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entitySetName">Azure Table name</param>
        /// <param name="entityToUpdate">Entity to update in a Azure Table</param>
        public void Update<T>(string entitySetName, T entityToUpdate)
        {
            var t = entityToUpdate.GetType();
            var properties = t.GetProperties();

            var partitionKey = (string) properties.Single(pi => pi.Name == "PartitionKey").GetValue(entityToUpdate, null);
            var rowKey = (string) properties.Single(pi => pi.Name == "RowKey").GetValue(entityToUpdate, null);

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

        /// <summary>
        /// Deletes an entity in a Azure Table.
        /// </summary>
        /// <typeparam name="T">Type of objects related to the Azure Tables</typeparam>
        /// <param name="entitySetName">Azure Table name</param>
        /// <param name="partitionKey">The entity partition key</param>
        /// <param name="rowKey">The entity row key</param>
        public void Delete<T>(string entitySetName, string partitionKey, string rowKey)
        {
            var entityToDelete = Get<T>(entitySetName, partitionKey, rowKey);

            if (entityToDelete != null)
            {
                _context.DeleteObject(entityToDelete);           
            }
        }

        /// <summary>
        /// Submits all the changes in the Azure Tables.
        /// </summary>
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

        /// <summary>
        /// Creates a more specifica Exception from a Azure Table Context Exception.
        /// </summary>
        /// <param name="exception">The Azure Table Context Exception</param>
        /// <returns>Specific Exception</returns>
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