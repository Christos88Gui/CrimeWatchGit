using Antlr.Runtime;
using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrimeWatch.Controllers
{
    public class StatsController : Controller
    {
        private crimewatchAzureModels db = new crimewatchAzureModels();
        

        public ActionResult Stats(int period)
        {
            List<Crimes_pm> records = new List<Crimes_pm>();
            List<County> counties = db.Counties.ToList();
            counties.Remove(counties.First(x=>x.Id==7));
            Crimes_pm lastRecord = db.Crimes_pm.OrderBy(x => x.Month).ToList().Last();
            DateTime lastDate = ReturnFirstAndLastDate(period).Last();
            DateTime firstDate = ReturnFirstAndLastDate(period).First();
            ViewBag.firstDate = firstDate.ToString("MMM yyyy");
            ViewBag.lastDate = lastDate.ToString("MMM yyyy");
            ViewBag.datesDD = ReturnDatesDD();
            foreach (County c in counties)
            {                    
                List<Crimes_pm> recordList = c.Crimes_pm.Where(x=>x.Month>= firstDate && x.Month <=lastDate).ToList();
                records.Add(new Crimes_pm { County = c, All_crimes = recordList.Sum(x => x.All_crimes) });
            }
            return View(records);
        }


        public List<DateTime> ReturnFirstAndLastDate(int period)
        {
            Crimes_pm lastRecord = db.Crimes_pm.OrderBy(x => x.Month).ToList().Last();
            DateTime lastDate = new DateTime();
            DateTime firstDate = new DateTime();
            switch (period)
            {
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
                    lastDate = lastRecord.Month;
                    firstDate = lastDate.AddMonths(-11);
                    break;
            }
            return new List<DateTime> { firstDate, lastDate };
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

    }
}