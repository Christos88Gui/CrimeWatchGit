using CrimeWatch.Models;
using CrimeWatch.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CrimeWatch.Controllers
{
    [Authorize]
    public class GraphsController : Controller
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();
        private readonly CommonFunctions _cf = new CommonFunctions();
        private readonly GraphsService _service = new GraphsService();


        /// <summary>
        /// Redirects to graph parameters View.
        /// </summary>
        /// <returns></returns>
        public ActionResult Parameters()
        {
            //Passes the dropdown of counties to the view
            ViewBag.countiesDD = _service.ReturnCountiesDd();
            return View();
        }

        /// <summary>
        /// Receives user input and returns appropriate data via the ViewBag functionality.
        /// </summary>
        /// <param name="countyName">The name of the desired county.</param>
        /// <param name="periodId">Produce graphs related to this period. 1 = Last year, 2 = 2 years ago, 3 = 3 years ago</param>
        /// <returns>Redirects to the Graph View.</returns>
        public ActionResult Graphs(string countyName, int periodId)
        {
            setViewBags(countyName,periodId);
            var graphData = _service.ReturnGraphData(countyName, periodId);
            return View(graphData);
        }

        /// <summary>
        /// Handles the static data to be passed to the view such as dropdowns
        /// </summary>
        /// <param name="countyName">The name of the selected county</param>
        /// <param name="periodId">The period the graphs relate to</param>
        public void setViewBags(string countyName, int periodId) {
            //Pass dropdown data to View.
            ViewBag.countiesDD = _service.ReturnCountiesDd();
            ViewBag.datesDD = _service.ReturnDatesDd();
            ViewBag.categories = _cf.ReturnTypeDd();
            //Pass first and last date of the period.
            ViewBag.firstDate = _service.ReturnFirstAndLastDate(periodId).First().ToString("MMM yyyy");
            ViewBag.lastDate = _service.ReturnFirstAndLastDate(periodId).Last().ToString("MMM yyyy");
            //If countyName indicates to return data from all counties:
            if (String.IsNullOrEmpty(countyName))
            {
                ViewBag.police_department = "All police forces";
                ViewBag.population = _db.Counties.Sum(x => x.Population).Value;
                ViewBag.countyName = "All counties";
            }
            //Otherwise return data from just one county:
            else
            {
                ViewBag.police_department = _db.Counties.FirstOrDefault(x => x.Name.Equals(countyName)).PoliceDepartment.Name;
                ViewBag.population = _db.Counties.FirstOrDefault(x => x.Name.Equals(countyName)).Population;
                ViewBag.countyName = countyName;
            }
        }
    }
}