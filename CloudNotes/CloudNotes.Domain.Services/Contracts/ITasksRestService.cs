using System.Collections.Generic;
using CloudNotes.Domain.Services.Models.WebAPI;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface ITasksRestService : IRestService<Task>
    {
        IEnumerable<Task> GetAll(string userId, string resourceId);
        string Move(Task task, List destinationList);
    }
}