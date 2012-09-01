using System;
using System.Text;
using System.Web;
using System.Web.Http;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.WebRole.Helpers;
using StructureMap;

namespace CloudNotes.WebRole.Controllers
{
    public class OAuthWebApiController : ApiController
    {
        #region Fields

        private readonly IOAuthService _service;

        #endregion Fields

        #region Constructors

        public OAuthWebApiController()
        {
            _service = ObjectFactory.GetInstance<IOAuthService>();
        }

        #endregion Constructors

        #region Actions

        [HttpGet]
        public string GrantAccess([FromBody] string clientId, [FromBody] string redirectUri, [FromBody] string responseType, [FromBody] string state)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(responseType))
            {
                throw new HttpException(400, BuildInvalidRquestMessage(clientId, redirectUri, responseType));
            }

            if (responseType != "code")
            {
                throw new HttpException(400, "Unsupported Response Type");
            }

            OAuthEntity registeredApp;

            try
            {
                registeredApp = _service.Get("CloudNotesOAuth", clientId);
            }
            catch
            {
                throw new HttpException(500, "Server Error");
            }

            if (registeredApp == null)
            {
                throw new HttpException(403, "Unauthorized Client");
            }

            var accesGrantedUri = string.Format("{0}?code={1}{2}", redirectUri, registeredApp.Code, string.IsNullOrEmpty(state) ? "" : "&state=" + state);
            return accesGrantedUri;
        }

        [HttpPost]
        public string GetToken([FromBody] string code, [FromBody] string clientId, [FromBody] string clientSecret, [FromBody] string redirectUri, [FromBody] string grantType, [FromBody] string state)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(grantType))
            {
                throw new HttpException(400, BuildInvalidRquestMessage(code, clientId, clientSecret, redirectUri, grantType));
            }

            if (grantType != "authorization_code")
            {
                throw new HttpException(400, "Unsupported Grant Type");
            }

            OAuthEntity registeredApp;

            try
            {
                registeredApp = _service.Get(clientId, clientSecret);
            }
            catch
            {
                throw new HttpException(500, "Server Error");
            }

            if (registeredApp == null)
            {
                throw new HttpException(403, "Unauthorized Client");
            }

            if (registeredApp.Code != code)
            {
                throw new HttpException(400, "Invalid Grant");
            }

            var accessToken = ShortGuid.NewGuid().ToString();
            const string tokenType = "Bearer";
            var expiresIn = TimeSpan.FromHours(1).TotalSeconds;
            var token = new TokenEntity(clientId, accessToken, tokenType, expiresIn);

            _service.CreateToken(token);

            var tokenUri = string.Format("{0}#access_token={1}&token_type={2}&expires_in={3}{4}", redirectUri, accessToken, tokenType, expiresIn, string.IsNullOrEmpty(state) ? "" : "&state=" + state);
            return tokenUri;
        }

        #endregion Actions

        #region Private methods

        private string BuildInvalidRquestMessage(string code, string clientId, string clientSecret, string redirectUri, string grantType)
        {
            var buffer = new StringBuilder();
            buffer.Append("Invalid Request.");
            buffer.Append("The following parameters are missing: ");

            if (string.IsNullOrEmpty(code))
            {
                buffer.Append("code, ");
            }

            if (string.IsNullOrEmpty(clientId))
            {
                buffer.Append("ClientId, ");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                buffer.Append("ClientSecret, ");
            }

            if (string.IsNullOrEmpty(redirectUri))
            {
                buffer.Append("RedirectUri, ");
            }

            if (string.IsNullOrEmpty(grantType))
            {
                buffer.Append("GrantType, ");
            }

            return buffer.ToString();
        }

        private string BuildInvalidRquestMessage(string clientId, string redirectUri, string responseType)
        {
            var buffer = new StringBuilder();
            buffer.Append("Invalid Request.");
            buffer.Append("The following parameters are missing: ");

            if (string.IsNullOrEmpty(clientId))
            {
                buffer.Append("ClientId, ");
            }

            if (string.IsNullOrEmpty(redirectUri))
            {
                buffer.Append("RedirectUri, ");
            }

            if (string.IsNullOrEmpty(responseType))
            {
                buffer.Append("ResponseType, ");
            }

            return buffer.ToString();
        }

        #endregion Private methods
    }
}