using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Domain.Services.Implementation;
using CloudNotes.Repositories.Implementation;

namespace CloudNotes.WebRole.Handlers
{
    public class OAuthAuthenticationHandler : HttpMessageHandler
    {
        #region Fields

        private readonly IOAuthService _service;

        #endregion Fields

        #region Constructor

        public OAuthAuthenticationHandler()
        {
            // StructureMap conflicted wiht the MvcRouteUnitTester and made the unit tests fail.
            //_service = ObjectFactory.GetInstance<IOAuthService>();
            var unitOfWork = new AzureTablesUnitOfWork("DefaultEndpointsProtocol=http;AccountName=luisprompt;AccountKey=EGzUtKSo0RvBnMX4gqde8TQx0LErE9PQ5KFbo++WeURl8zPidAtt3tzQnzCSp7IH1rW8Ecct3OmNdB3gLwhA1Q==");
            _service = new OAuthService(unitOfWork);
        }

        #endregion Constructor

        #region Public methods

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authenticationHeader = request.Headers.Authorization;
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);

            if (authenticationHeader != null)
            {
                var scheme = authenticationHeader.Scheme;
                var token = authenticationHeader.Parameter;

                var validToken = _service.GetToken("CloudNotes", token);

                if (scheme != "Bearer" || validToken == null)
                {
                    response = request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                response = request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(response);

            return tcs.Task;
        }

        #endregion Public methods
    }
}