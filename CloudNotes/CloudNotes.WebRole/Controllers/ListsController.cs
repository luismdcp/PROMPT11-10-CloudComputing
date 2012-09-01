using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Models.WebAPI;
using StructureMap;

namespace CloudNotes.WebRole.Controllers
{
    public class ListsController : ApiController
    {
        #region Fields

        private readonly IListsRestService _listsService;

        #endregion Fields

        #region Constructors

        public ListsController()
        {
            _listsService = ObjectFactory.GetInstance<IListsRestService>();
        }

        #endregion Constructors

        #region Actions

        [HttpGet]
        public IEnumerable<List> All(string userId)
        {
            var lists = _listsService.GetAll(userId).ToList();

            for (int i = 0; i < lists.Count; i++)
            {
                var uri = Url.Route("WebAPI", new { controller = "Lists", action = "Details", userId = lists[i].PartitionKey, resourceId = lists[i].RowKey });
                var absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

                var selfLink = new Link
                                {
                                    Name = "self",
                                    Rel = "http://api.relations.wrml.org/common/self",
                                    Href = absoluteUrl
                                };

                lists[i].Links.Add(selfLink);
            }

            return lists;
        }

        [HttpGet]
        public List Details(string userId, string resourceId)
        {
            var list = _listsService.Get(userId, resourceId);

            if (list == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var uri = Url.Route("WebAPI", new { controller = "Lists", action = "Details", userId, resourceId });
            var absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var selfLink = new Link
                            {
                                Name = "self",
                                Rel = "http://api.relations.wrml.org/common/self",
                                Href = absoluteUrl
                            };

            uri = Url.Route("WebAPI", new { controller = "Lists", action = "All", userId });
            absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

            var allLink = new Link
                            {
                                Name = "self",
                                Rel = "http://api.relations.wrml.org/common/all",
                                Href = absoluteUrl
                            };

            list.Links.Add(selfLink);
            list.Links.Add(allLink);

            return list;
        }

        #endregion Actions
    }
}