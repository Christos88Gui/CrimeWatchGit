using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CrimeWatch.Controllers
{
    public class HomeController : Controller
    {
        private CrimeWatchDBEntities db = new CrimeWatchDBEntities();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.records = db.Crimes.Count();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}