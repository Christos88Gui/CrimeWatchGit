using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CrimeWatch.Models;
using PoliceUk;

namespace CrimeWatch
{
    public class CommonFunctions
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();


        public List<string> ReturnDatesDd()
        {
            var datesDd = new List<string>();
            var lastRecord = _db.Records.OrderBy(x => x.Date).ToList().Last();
            var firstDate = lastRecord.Date.AddMonths(1);
            var lastDate = firstDate.AddMonths(+11);
            for (var date = firstDate; date.Date <= lastDate; date = date.AddMonths(1))
                datesDd.Add(date.ToString("MMMM yyyy"));

            return datesDd;
        }

        public double ReturnDiff(DateTime date)
        {
            return (date - ConstantStrings.FirstDate.AddMonths(-1)).TotalDays;
        }

        public List<string> ReturnCountiesDd()
        {
            var countyNames = new List<string>();
            foreach (var county in _db.Counties)
            {
                countyNames.Add(county.Name);
            }
            return countyNames;
        }

        public List<string> ReturnDateDd()
        {
            var datesToString = new List<string>();
            var dates = new List<DateTime>();
            foreach (var record in _db.Records)
            {
                dates.Add(record.Date);
            }
            dates = dates.Distinct().OrderByDescending(x => x).ToList();
            foreach (var date in dates)
            {
                datesToString.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month) + " " + date.Year);
            }
            return datesToString;
        }

        public List<string> ReturnTypeDd()
        {
            var typesToString = new List<string>();
            PoliceUkClient client = new PoliceUkClient();
            List<PoliceUk.Entities.Category> categories = client.CrimeCategories(DateTime.UtcNow).ToList();
            foreach (var category in categories)
            {
                typesToString.Add(category.Name);
            }
            return typesToString;
        }

    }
}