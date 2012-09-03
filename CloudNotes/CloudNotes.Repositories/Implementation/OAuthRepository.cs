using System;
using System.Linq;
using System.Linq.Expressions;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.OAuth;

namespace CloudNotes.Repositories.Implementation
{
    /// <summary>
    /// Repository to manage all the actions related to the OAuthRegisteredApps Azure Table.
    /// The Table stores information about all the registered applications for the OAuth interaction with the Web API.
    /// </summary>
    public class OAuthRepository : IOAuthRepository
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion Fields

        #region Constructors

        public OAuthRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Loads all the entities in the OAuthRegisteredApps Azure Table.
        /// </summary>
        /// <returns>IQueryable of all the registered applications entities</returns>
        public IQueryable<OAuthEntity> Load()
        {
            return _unitOfWork.Load<OAuthEntity>("OAuthRegisteredApps");
        }

        /// <summary>
        /// Gets a single entity from the OAuthRegisteredApps Azure Table.
        /// </summary>
        /// <param name="partitionKey">The registered application partitionkey</param>
        /// <param name="rowKey">The registered application rowkey</param>
        /// <returns>A OAuthEntity if exists, null otherwise</returns>
        public OAuthEntity Get(string partitionKey, string rowKey)
        {
            var result = _unitOfWork.Get<OAuthEntity>("OAuthRegisteredApps", partitionKey, rowKey);
            return result;
        }

        /// <summary>
        /// Gets a single entity from the OAuthRegisteredApps Azure Table by filter.
        /// </summary>
        /// <param name="filter">Lambda to filter the OAuthEntity</param>
        /// <returns>A OAuthEntity if exists, null otherwise</returns>
        public OAuthEntity Get(Expression<Func<OAuthEntity, bool>> filter)
        {
            return _unitOfWork.Get("OAuthRegisteredApps", filter);
        }

        /// <summary>
        /// Creates an entity in the OAuthRegisteredApps Azure Table.
        /// </summary>
        /// <param name="entityToCreate">OAuthEntity entity to be created</param>
        public void Create(OAuthEntity entityToCreate)
        {
            _unitOfWork.Create(entityToCreate, "OAuthRegisteredApps");
        }

        /// <summary>
        /// Updates an entity in the OAuthRegisteredApps Azure Table.
        /// </summary>
        /// <param name="entityToUpdate">OAuthEntity entity to be updated</param>
        public void Update(OAuthEntity entityToUpdate)
        {
            _unitOfWork.Update("OAuthRegisteredApps", entityToUpdate);
        }

        /// <summary>
        /// Deletes an entity in the OAuthRegisteredApps Azure Table.
        /// </summary>
        /// <param name="entityToDelete">OAuthEntity entity to be deleted</param>
        public void Delete(OAuthEntity entityToDelete)
        {
            _unitOfWork.Delete<UserEntity>("OAuthRegisteredApps", entityToDelete.PartitionKey, entityToDelete.RowKey);
        }

        /// <summary>
        /// Creates an entity in the OAuthTokens Azure Table.
        /// </summary>
        /// <param name="token">TokenEntity to be created</param>
        public void CreateToken(TokenEntity token)
        {
            _unitOfWork.Create(token, "OAuthTokens");
        }

        /// <summary>
        /// Gets a TokenEntity from the OAuthTokens Azure Table.
        /// </summary>
        /// <param name="clientId">ClientId for the registered application</param>
        /// <param name="tokenType">Token value</param>
        /// <returns></returns>
        public TokenEntity GetToken(string clientId, string tokenType)
        {
            var result = _unitOfWork.Get<TokenEntity>("OAuthTokens", clientId, tokenType);
            return result;
        }

        #endregion Public methods
    }
}