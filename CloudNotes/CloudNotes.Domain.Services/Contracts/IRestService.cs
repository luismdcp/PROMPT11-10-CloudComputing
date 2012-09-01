using System.Collections.Generic;

namespace CloudNotes.Domain.Services.Contracts
{
    public interface IRestService<out T>
    {
        T Get(string userId, string resourceId);
    }
}