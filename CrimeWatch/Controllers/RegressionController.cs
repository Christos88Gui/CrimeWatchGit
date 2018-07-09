using CrimeWatch.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CrimeWatch.Services;

namespace CrimeWatch.Controllers
{
    [Authorize]
    public class RegressionController : Controller
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();
        private readonly CommonFunctions _cf = new CommonFunctions();
        private readonly RegressionService _regression = new RegressionService();
        private readonly KMeansService _kMEans= new KMeansService();

        public Tests t = new Tests();


        /// <summary>
        /// Receives a county name by the user, trains an svr model 
        /// and returns 12 predicted record for the next 12 months
        /// </summary>
        /// <param name="countyName">The requested county</param>
        /// <returns></returns>
        public ActionResult Prediction(string countyName)
        {
            ViewBag.datesDD = _cf.ReturnDatesDd(); ViewBag.countiesDD = _cf.ReturnCountiesDd();
            //Sets the properties of the graph in Prediction View
            if (string.IsNullOrEmpty(countyName))
            {
                var crimes = new double[1][]; crimes[0] = new double[] { 0, 0 , 0};
                ViewBag.crimes = crimes;
                ViewBag.title = "Forecast Crimes";
                ViewBag.prediction = "";
                ViewBag.countyName = "";
                return View();
            }

            ViewBag.countyName = countyName;
            ViewBag.title = countyName;
            var county = _db.Counties.First(x => x.Name.Equals(countyName));
            //Finds the 12 most recent records of the county
            List<Record> pastRecords = county.Records.OrderByDescending(x => x.Date).Take(12).ToList();
            //Retrieves the 12 next predicted records
            List<Record> predictedRecords = _regression.ReturnPredictedRecords(county, 12);
            //Passes both record lists to the view
            ViewBag.crimes = _regression.ReturnScatterPlotData(county.Records.OrderBy(x=>x.Date).ToList(),predictedRecords);            
            return View(pastRecords.Concat(predictedRecords).ToList());
        }
    }
}


