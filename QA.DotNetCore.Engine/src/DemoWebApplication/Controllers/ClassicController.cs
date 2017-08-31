using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApplication.Controllers
{
    public class ClassicController : Controller
    {
        public ActionResult Index(string id)
        {
            return View("Index");
        }

    }
}
