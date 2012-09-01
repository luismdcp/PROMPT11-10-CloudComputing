using System;
using System.Linq;
using System.Linq.Expressions;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Implementation;

namespace CloudNotes.Domain.Services.Implementation
{
    /// <summary>
    /// Service to register applications and manage tokens. 
    /// </summary>
    public class OAuthService : IOAuthService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly OAuthRepository _repository;

        #endregion Fields

        #region Constructors

        public OAuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = new OAuthRepository(_unitOfWork);
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Load all the registered applications.
        /// </summary>
        /// <returns>IQueryable of all registered applications</returns>
        public IQueryable<OAuthEntity> Load()
        {
            return _repository.Load();
        }

        /// <summary>
        /// Get a registered application by its partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The registered application partition key</param>
        /// <param name="rowKey">The registered application row key</param>
        /// <returns>The Registered application if exists, null otherwise</returns>
        public OAuthEntity Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        /// <summary>
        /// Get a register application by a filter.
        /// </summary>
        /// <param name="filter">Expression lambda filter</param>
        /// <returns>The Registered application if exists, null otherwise</returns>
        public OAuthEntity Get(Expression<Func<OAuthEntity, bool>> filter)
        {
            return _repository.Get(filter);
        }

        /// <summary>
        /// Register a new application.
        /// </summary>
        /// <param name="entityToCreate">Application to register</param>
        public void Create(OAuthEntity entityToCreate)
        {
            _repository.Create(entityToCreate);
        }

        /// <summary>
        /// Update a registered application.
        /// </summary>
        /// <param name="entityToUpdate">Registered application to update</param>
        public void Update(OAuthEntity entityToUpdate)
        {
            _repository.Update(entityToUpdate);
        }

        /// <summary>
        /// Delete a registered application.
        /// </summary>
        /// <param name="entityToDelete">Registered application to delete</param>
        public void Delete(OAuthEntity entityToDelete)
        {
            _repository.Delete(entityToDelete);
        }

        /// <summary>
        /// Create an OAuth token.
        /// </summary>
        /// <param name="token">Token to create</param>
        public void CreateToken(TokenEntity token)
        {
            _repository.CreateToken(token);
        }

        /// <summary>
        /// Get a token by clientId and tokenType
        /// </summary>
        /// <param name="clientId">ClientId given to the registered application</param>
        /// <param name="accessToken">Token value</param>
        /// <returns>Token if exists, null otherwise</returns>
        public TokenEntity GetToken(string clientId, string accessToken)
        {
            return _repository.GetToken(clientId, accessToken);
        }

        #endregion Public methods
    }
}