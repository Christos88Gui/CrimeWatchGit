using CrimeWatch.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CrimeWatch.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        /// <summary>
        /// Object for accessing StatsService.
        /// </summary>
        private StatsService service = new StatsService();
        
        /// <summary>
        /// Receives an Id representing the desired period and returns the Stats View 
        ///     populated with the appropriate content.
        /// </summary>
        /// <param name="period"></param>
        /// <returns>Stats View</returns>
        public ActionResult Stats(int period)
        {
            //Starting date
            ViewBag.firstDate = service.ReturnFirstAndLastDate(period).First().ToString("MMMM yyyy");
            //Finishing date
            ViewBag.lastDate = service.ReturnFirstAndLastDate(period).Last().ToString("MMMM yyyy");            
            ViewBag.datesDD = service.ReturnDatesDd();
            return View(service.Stats(period));
        }
    }
}