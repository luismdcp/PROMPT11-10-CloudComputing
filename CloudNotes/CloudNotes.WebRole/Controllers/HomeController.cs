using System.Configuration;
using System.Web.Mvc;
using CloudNotes.Domain.Services.Implementation;

namespace CloudNotes.WebRole.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var t = ConfigurationManager.AppSettings["StorageConnectionString"];
            UsersService service = new UsersService();
            var t1 = service.Load();
            var user = service.ManageUser();
            var t2 = service.Load();
            var temp = service.Get("users", user.RowKey);
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}