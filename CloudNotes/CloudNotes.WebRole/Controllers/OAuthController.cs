using System.Web.Mvc;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.WebRole.Helpers;

namespace CloudNotes.WebRole.Controllers
{
    public class OAuthController : Controller
    {
        #region Fields

        private readonly IOAuthService _oAuthService;

        #endregion Fields

        #region Constructors

        public OAuthController(IOAuthService service)
        {
            _oAuthService = service;
        }

        #endregion Constructors

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string app, string redirectUri)
        {
            var clientId = app;
            var secret = ShortGuid.NewGuid().ToString();
            var code = ShortGuid.NewGuid().ToString();

            var registry = new OAuthEntity(clientId, secret, redirectUri, code) { PartitionKey = clientId, RowKey = secret };
            _oAuthService.Create(registry);

            return RedirectToAction("Index");
        }
    }
}
