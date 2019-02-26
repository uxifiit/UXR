using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using UXR.Models.Entities;

namespace UXR.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string ControllerName = nameof(HomeController).Replace("Controller", "");

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}
