using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrimeWatch.Models;

namespace CrimeWatch.Services
{
    public class StatsService
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();

        public List<Record> Stats(int period)
        {
            var records = new List<Record>();
            var counties = _db.Counties.ToList();
            counties.Remove(counties.First(x => x.Name == "City of London"));
            var lastRecord = _db.Records.OrderBy(x => x.Date).ToList().Last();
            var lastDate = ReturnFirstAndLastDate(period).Last();
            var firstDate = ReturnFirstAndLastDate(period).First();  
            foreach (var c in counties)
            {
                var recordList = c.Records.Where(x => x.Date >= firstDate && x.Date <= lastDate).ToList();
                records.Add(new Record { County = c, AllCrimes = recordList.Sum(x => x.AllCrimes) });
            }
            return records;
        }

        public List<DateTime> ReturnFirstAndLastDate(int period)
        {
            var lastRecord = _db.Records.OrderBy(x => x.Date).ToList().Last();
            var lastDate = new DateTime();
            var firstDate = new DateTime();
            switch (period)
            {
                case 1:
                    lastDate = lastRecord.Date;
                    firstDate = lastDate.AddMonths(-11);
                    break;
                case 2:
                    lastDate = lastRecord.Date.AddMonths(-12);
                    firstDate = lastDate.AddMonths(-11);
                    break;
                case 3:
                    firstDate = lastRecord.Date.AddMonths(-35);
                    lastDate = firstDate.AddMonths(11);
                    break;
                default:
                    lastDate = lastRecord.Date;
                    firstDate = lastDate.AddMonths(-11);
                    break;
            }
            return new List<DateTime> { firstDate, lastDate };
        }


        public List<string> ReturnDatesDd()
        {
            var datesDd = new List<string>();
            var lastRecord = _db.Records.OrderBy(x => x.Date).ToList().Last();
            var lastDate = lastRecord.Date;
            var firstDate = lastDate.AddMonths(-11);
            datesDd.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(firstDate.Month) + " " + firstDate.Year + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastDate.Month) + " " + lastDate.Year);
            lastDate = lastRecord.Date.AddMonths(-12);
            firstDate = lastDate.AddMonths(-11);
            datesDd.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(firstDate.Month) + " " + firstDate.Year + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastDate.Month) + " " + lastDate.Year);
            firstDate = lastRecord.Date.AddMonths(-35);
            lastDate = firstDate.AddMonths(11);
            datesDd.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(firstDate.Month) + " " + firstDate.Year + " - " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastDate.Month) + " " + lastDate.Year);

            return datesDd;
        }

    }
}