using Microsoft.AspNetCore.Mvc;

namespace QA.DotNetCore.OnScreenAdmin.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
