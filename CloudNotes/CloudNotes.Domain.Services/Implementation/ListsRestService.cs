using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Models.WebAPI;
using CloudNotes.Domain.Services.Models.WebAPI.Extensions;

namespace CloudNotes.Domain.Services.Implementation
{
    /// <summary>
    /// Service to be used by the Web API for Lists operations.
    /// </summary>
    public class ListsRestService : IListsRestService
    {
        #region Fields

        private readonly ITaskListsService _taskListsService;
        private readonly IUsersService _usersService;

        #endregion Fields

        #region Constructors

        public ListsRestService(ITaskListsService taskListsService, IUsersService usersService)
        {
            _taskListsService = taskListsService;
            _usersService = usersService;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Get all the Lists that were shared to the User.
        /// </summary>
        /// <param name="userId">The User row key</param>
        /// <returns>All the Lists that were shared to the User</returns>
        public IEnumerable<List> GetAll(string userId)
        {
            var userKeys = userId.Split('-');
            var userPartitionKey = userKeys[1];
            var user = _usersService.Get(userPartitionKey, userId);
            var taskLists = _taskListsService.GetShared(user);

            return taskLists.Select(t => t.MapToList());
        }

        /// <summary>
        /// Get a List by his partiton key and row key.
        /// </summary>
        /// <param name="userId">List partition key</param>
        /// <param name="resourceId">List row key</param>
        /// <returns></returns>
        public List Get(string userId, string resourceId)
        {
            var list = _taskListsService.Get(t => t.PartitionKey == userId && t.RowKey == resourceId);
            return list != null ? list.MapToList() : null;
        }

        #endregion Public methods
    }
}