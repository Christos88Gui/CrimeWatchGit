using CrimeWatch.Models;
using Accord.Statistics.Models.Regression.Linear;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Globalization;

namespace CrimeWatch.Controllers
{
    public class RegressionController : Controller
    {
        private crimewatchAzureModels db = new crimewatchAzureModels();
        public OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
        public MultipleLinearRegression regression = new MultipleLinearRegression();

        public ActionResult Parameters() {
            ViewBag.datesDD = ReturnDatesDD();
            ViewBag.countiesDD = ReturnCountiesDD();
            return View();
        }

        public ActionResult PredictCrimes(string countyName, string dateStr) {
            // Compute the output for a given input:
            County county = db.Counties.First(x=>x.Name.Equals(countyName));
            trainRegression(county);
            DateTime date = DateTime.ParseExact(dateStr, "MMMM yyyy", CultureInfo.InvariantCulture);
            double[] input = new double[]{ date.Month, date.Year };
            double y = regression.Transform(input); // The answer will be 28.088
            // We can also extract the slope and the intercept term
            // for the line. Those will be -0.26 and 50.5, respectively.
            double[] s = regression.Weights;     // -0.264706
            double c = regression.Intercept; // 50.588235
            return RedirectToAction("MyPortal","Home");
        }

        public void trainRegression(County county) {
            List<Crimes_pm> records = county.Crimes_pm.ToList();
            int recordsCount = records.Count();
            // Declare some sample test data.
            double[][] inputs = new double[recordsCount][];
            double[] outputs = new double[recordsCount];
            for (int i = 0; i < recordsCount; i++)
            {
                Crimes_pm record = records.ElementAt(i);
                double year = Convert.ToDouble(record.Month.Year);
                double month = Convert.ToDouble(record.Month.Month);
                double[] arr = new double[] { month, year};
                inputs[i] = arr;
                outputs[i] = record.All_crimes;
            }
            // Use Ordinary Least Squares to learn the regression
            ols = new OrdinaryLeastSquares();
            // Use OLS to learn the simple linear regression
            regression = ols.Learn(inputs, outputs);            
        }

        public List<string> ReturnDatesDD()
        {
            List<string> datesDD = new List<string>();
            Crimes_pm lastRecord = db.Crimes_pm.OrderBy(x => x.Month).ToList().Last();
            DateTime firstDate = lastRecord.Month.AddMonths(1);
            DateTime lastDate = firstDate.AddMonths(+11);
            for (var date = firstDate; date.Date <= lastDate; date = date.AddMonths(1))
                datesDD.Add(date.ToString("MMMM yyyy"));

            return datesDD;
        }

        public List<string> ReturnCountiesDD()
        {
            List<string> countyNames = new List<string>();
            foreach (County county in db.Counties)
            {
                countyNames.Add(county.Name);
            }
            return countyNames;
        }

    }
}


