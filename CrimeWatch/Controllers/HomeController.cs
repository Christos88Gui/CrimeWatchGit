using CrimeWatch.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Globalization;
using PoliceUk;
using System;

namespace CrimeWatch.Controllers
{
    public class HomeController : Controller
    {
        private crimewatchAzureModels db = new crimewatchAzureModels();

        public ActionResult Index()
        {
            return View(db.Police_Departments.ToList());
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

        public ActionResult MyPortal()
        {
            return View(db.AspNetUsers.Find(User.Identity.GetUserId()));
        }
    }
}