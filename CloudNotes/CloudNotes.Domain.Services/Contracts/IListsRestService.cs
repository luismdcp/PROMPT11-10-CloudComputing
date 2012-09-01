using System.Collections.Generic;
using CloudNotes.Domain.Services.Models.WebAPI;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface IListsRestService : IRestService<List>
    {
        IEnumerable<List> GetAll(string userId);
    }
}