using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;

namespace CloudNotes.WebRole.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IClaimsIdentity claimsIdentity = ((IClaimsPrincipal)(Thread.CurrentPrincipal)).Identities[0];
            Claim id = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType ==
              "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            Claim provider = claimsIdentity.Claims.FirstOrDefault(claim => claim.ClaimType ==
             "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider");
            ViewBag.Message = id.Value + " - " + provider.Value;

            return View();
        }
        
        public ActionResult About()
        {
            return View();
        }
    }
}