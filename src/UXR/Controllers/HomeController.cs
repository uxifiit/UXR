using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UXR.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string ControllerName = nameof(HomeController).Replace("Controller", "");

        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}
