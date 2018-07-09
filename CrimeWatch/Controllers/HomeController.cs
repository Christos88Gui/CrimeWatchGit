using CrimeWatch.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Web;
using System.Globalization;
using System.Web.Security;

namespace CrimeWatch.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();


        /// <summary>
        /// Redirects to the home page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public void AddUserToRole()
        {
            Roles.CreateRole("Administrator");   
            
            Roles.AddUserToRole("admin@admin.com", "Administrator");

        }

        /// <summary>
        /// Redirects to user portal View (Dashboard).
        /// </summary>
        /// <returns>The Id of logged in user</returns>
        public ActionResult MyPortal()
        {
            return View(_db.AspNetUsers.Find(User.Identity.GetUserId()));
        }
    }
}