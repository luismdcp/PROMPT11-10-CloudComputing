using System.Collections.Generic;
using System.Web.Http;

namespace CloudNotes.WebRole.Controllers
{
    public class ListsController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}