using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            ViewBag.Header = "Header";
            ViewBag.SubHeader = "Sub Header";
            return View();
        }
        public ActionResult RequestAccess()
        {
            return View();
        }
        public ActionResult RequestMessage()
        {
            return View();
        }
    }
}