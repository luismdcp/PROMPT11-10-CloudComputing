using System.Linq;
using System.Security.Principal;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.Domain.Services.Implementation
{
    public class UsersService : IUsersService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersRepository _repository;

        #endregion Fields

        #region Constructors

        public UsersService(IUnitOfWork unitOfWork, IUsersRepository usersRepository)
        {
            _unitOfWork = unitOfWork;
            _repository = usersRepository;
        }

        #endregion Constructors

        #region Public methods

        public IQueryable<User> Load()
        {
            return _repository.Load();
        }

        public User Get(string partitionKey, string rowKey)
        {
            return _repository.Get(partitionKey, rowKey);
        }

        public void Create(User entityToCreate)
        {
            _repository.Create(entityToCreate);
            _unitOfWork.SubmitChanges();
        }

        public void Update(User entityToUpdate)
        {
            _repository.Update(entityToUpdate);
            _unitOfWork.SubmitChanges();
        }

        public void Delete(User entityToDelete)
        {
            _repository.Delete(entityToDelete);
            _unitOfWork.SubmitChanges();
        }

        public bool UserExists(IPrincipal principal)
        {
            return _repository.UserExists(principal);
        }

        public User GetByUniqueIdentifier(string uniqueIdentifier)
        {
            return _repository.GetByUniqueIdentifier(uniqueIdentifier);
        }

        public void GetUserAuthenticationInfo(IPrincipal principal, out string name, out string email, out string uniqueIdentifier)
        {
            string userUniqueIdentifier = string.Empty;
            string userEmailAddress = string.Empty;
            string userName = string.Empty;
            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var emailAddressClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");
                var nameClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

                userUniqueIdentifier = string.Format("{0}_{1}", nameIdentifierClaim.Value, identityProviderClaim.Value);
                userEmailAddress = emailAddressClaim == null ? string.Empty : emailAddressClaim.Value;
                userName = nameClaim == null ? string.Empty : nameClaim.Value;
            }

            name = userName;
            email = userEmailAddress;
            uniqueIdentifier = userUniqueIdentifier;
        }

        public void FillUniqueIdentifier(User user, IPrincipal principal)
        {
            string userUniqueIdentifier = string.Empty;

            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal != null)
            {
                var claimsIdentity = claimsPrincipal.Identities[0];
                var nameIdentifierClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var identityProviderClaim = claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");

                userUniqueIdentifier = string.Format("{0}_{1}", nameIdentifierClaim.Value, identityProviderClaim.Value);
            }

            user.UserUniqueIdentifier = userUniqueIdentifier;
        }

        public void LoadNoteOwner(Note note)
        {
            _repository.LoadNoteOwner(note);
        }

        public void LoadTaskListOwner(TaskList taskList)
        {
            _repository.LoadTaskListOwner(taskList);
        }

        public void LoadNoteAssociatedUsers(Note note)
        {
            _repository.LoadNoteAssociatedUsers(note);
        }

        public void LoadTaskListAssociatedUsers(TaskList taskList)
        {
            _repository.LoadTaskListAssociatedUsers(taskList);
        }

        #endregion Public methods
    }
}