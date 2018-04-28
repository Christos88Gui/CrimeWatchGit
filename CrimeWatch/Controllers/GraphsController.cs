using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrimeWatch.Controllers
{
    public class GraphsController : Controller
    {
        private crimewatchAzureModels db = new crimewatchAzureModels();

        public ActionResult Parameters()
        {
            ViewBag.countiesDD = ReturnCountiesDD();
            return View();
        }

        public ActionResult Graphs(string countyName, int period)
        {
            MapController mapObj = new MapController();
            ViewBag.countiesDD = ReturnCountiesDD();
            ViewBag.datesDD = ReturnDatesDD();
            ViewBag.categories = mapObj.ReturnTypeDD();            
            ViewBag.firstDate = ReturnFirstAndLastDate(period).First().ToString("MMM yyyy");
            ViewBag.lastDate = ReturnFirstAndLastDate(period).Last().ToString("MMM yyyy");
            if (String.IsNullOrEmpty(countyName))
            {
                ViewBag.police_department = "All police forces";
                ViewBag.population = db.Counties.Sum(x => x.Population).Value;
                ViewBag.countyName = "All counties";
            }
            else
            {
                ViewBag.police_department = db.Counties.FirstOrDefault(x => x.Name.Equals(countyName)).Police_Departments.Name;
                ViewBag.population = db.Counties.FirstOrDefault(x=>x.Name.Equals(countyName)).Population;
                ViewBag.countyName = countyName;
            }
            List<Crimes_pm> graphData = ReturnGraphData(countyName,period);
            return View(graphData);
        }

        public List<Crimes_pm> ReturnGraphData(string countyName, int period) {
            List<DateTime> datesRange = new List<DateTime>();
            List<Crimes_pm> records = new List<Crimes_pm>();
            DateTime firstDate = ReturnFirstAndLastDate(period).First();
            DateTime lastDate = ReturnFirstAndLastDate(period).Last();

            for (DateTime date = firstDate; date <= lastDate; date = date.AddMonths(1))
                datesRange.Add(date);

            if (String.IsNullOrEmpty(countyName))
            {
                foreach (DateTime d in datesRange)
                {
                    List<Crimes_pm> recordsByDate = db.Crimes_pm.Where(x => x.Month.Equals(d)).ToList();
                    records.Add(
                    new Crimes_pm
                    {
                        All_crimes = recordsByDate.Sum(x => x.All_crimes),
                        Anti_social_behaviour = recordsByDate.Sum(x => x.Anti_social_behaviour),
                        Bicycle_theft = recordsByDate.Sum(x => x.Bicycle_theft),
                        Burglary = recordsByDate.Sum(x => x.Burglary),
                        Criminal_damage_and_arson = recordsByDate.Sum(x => x.Criminal_damage_and_arson),
                        Drugs = recordsByDate.Sum(x => x.Drugs),
                        Other_crime = recordsByDate.Sum(x => x.Other_crime),
                        Other_theft = recordsByDate.Sum(x => x.Other_theft),
                        Possession_of_weapons = recordsByDate.Sum(x => x.Possession_of_weapons),
                        Public_order = recordsByDate.Sum(x => x.Public_order),
                        Robbery = recordsByDate.Sum(x => x.Robbery),
                        Shoplifting = recordsByDate.Sum(x => x.Shoplifting),
                        Theft_from_the_person = recordsByDate.Sum(x => x.Theft_from_the_person),
                        Vehicle_crime = recordsByDate.Sum(x => x.Vehicle_crime),
                        Violence_and_sexual_offences = recordsByDate.Sum(x => x.Violence_and_sexual_offences),
                        Month = d
                    });
                }
            }
            else
            {
                County county = db.Counties.FirstOrDefault(x => x.Name.Equals(countyName));
                records = county.Crimes_pm.Where(x=>x.Month>=firstDate && x.Month <= lastDate).ToList();
            }
            return records.OrderBy(x=>x.Month).ToList();
        }

        public List<DateTime> ReturnFirstAndLastDate(int period) {
            Crimes_pm lastRecord = db.Crimes_pm.OrderBy(x => x.Month).ToList().Last();
            DateTime lastDate = new DateTime();
            DateTime firstDate = new DateTime();
            switch (period) {
                case 1:
                    lastDate = lastRecord.Month;
                    firstDate = lastDate.AddMonths(-11);
                    break;
                case 2:
                    lastDate = lastRecord.Month.AddMonths(-12);
                    firstDate = lastDate.AddMonths(-11);
                    break;
                case 3:
                    firstDate = lastRecord.Month.AddMonths(-35);
                    lastDate = firstDate.AddMonths(11);
                    break;
                default:
                    break;
            }                
            return new List<DateTime> { firstDate,lastDate };
        }

        public List<string> ReturnDatesDD()
        {
            List<string> datesDD = new List<string>();
            Crimes_pm lastRecord = db.Crimes_pm.OrderBy(x => x.Month).ToList().Last();
            DateTime lastDate = lastRecord.Month;
            DateTime firstDate = lastDate.AddMonths(-11);
            datesDD.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(firstDate.Month) + " " + firstDate.Year + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastDate.Month) + " " + lastDate.Year);
            lastDate = lastRecord.Month.AddMonths(-12);
            firstDate = lastDate.AddMonths(-11);
            datesDD.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(firstDate.Month) + " " + firstDate.Year + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastDate.Month) + " " + lastDate.Year);
            firstDate = lastRecord.Month.AddMonths(-35);
            lastDate = firstDate.AddMonths(11);
            datesDD.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(firstDate.Month) + " " + firstDate.Year + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastDate.Month) + " " + lastDate.Year);
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