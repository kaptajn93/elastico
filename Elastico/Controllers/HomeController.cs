using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elastico.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult SearchEntry()
        {
            ViewBag.Title = "SearchEntry";

            return View();
        }
        public ActionResult SearchLemma()
        {
            ViewBag.Title = "SearchLemma";

            return View();
        }
    }
}
