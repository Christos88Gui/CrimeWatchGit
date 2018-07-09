using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CrimeWatch.Services
{
    public class GraphsService
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();
        private readonly CommonFunctions _cf = new CommonFunctions();


        /// <summary>
        /// Returns aggregated information for all records to be included in Graphs View.
        /// </summary>
        /// <param name="countyName">County to produce aggregated information.</param>
        /// <param name="periodId">Which periodId to include.</param>
        /// <returns></returns>
        public List<Record> ReturnGraphData(string countyName, int periodId)
        {
            var datesRange = new List<DateTime>();
            var records = new List<Record>();
            var firstDate = ReturnFirstAndLastDate(periodId).First();
            var lastDate = ReturnFirstAndLastDate(periodId).Last();
            //Construct a list with all considered dates.
            for (var date = firstDate; date <= lastDate; date = date.AddMonths(1))
                datesRange.Add(date);

            if (String.IsNullOrEmpty(countyName))
            {
                foreach (var d in datesRange)
                {
                    var recordsByDate = _db.Records.Where(x => x.Date.Equals(d)).ToList();
                    records.Add(
                    new Record
                    {
                        AllCrimes = recordsByDate.Sum(x => x.AllCrimes),
                        AntiSocialBehaviour = recordsByDate.Sum(x => x.AntiSocialBehaviour),
                        BicycleTheft = recordsByDate.Sum(x => x.BicycleTheft),
                        Burglary = recordsByDate.Sum(x => x.Burglary),
                        CriminalDamageAndArson = recordsByDate.Sum(x => x.CriminalDamageAndArson),
                        Drugs = recordsByDate.Sum(x => x.Drugs),
                        OtherCrime = recordsByDate.Sum(x => x.OtherCrime),
                        OtherTheft = recordsByDate.Sum(x => x.OtherTheft),
                        PossessionOfWeapons = recordsByDate.Sum(x => x.PossessionOfWeapons),
                        PublicOrder = recordsByDate.Sum(x => x.PublicOrder),
                        Robbery = recordsByDate.Sum(x => x.Robbery),
                        Shoplifting = recordsByDate.Sum(x => x.Shoplifting),
                        TheftFromThePerson = recordsByDate.Sum(x => x.TheftFromThePerson),
                        VehicleCrime = recordsByDate.Sum(x => x.VehicleCrime),
                        ViolenceAndSexualOffences = recordsByDate.Sum(x => x.ViolenceAndSexualOffences),
                        Date = d
                    });
                }
            }
            else
            {
                var county = _db.Counties.FirstOrDefault(x => x.Name.Equals(countyName));
                records = county.Records.Where(x => x.Date >= firstDate && x.Date <= lastDate).ToList();
            }
            return records.OrderBy(x => x.Date).ToList();
        }

        /// <summary>
        /// Returns first and last date of the desired period.
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public List<DateTime> ReturnFirstAndLastDate(int periodId)
        {
            var lastRecord = _db.Records.OrderBy(x => x.Date).ToList().Last();
            var lastDate = new DateTime();
            var firstDate = new DateTime();
            switch (periodId)
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
                    break;
            }
            return new List<DateTime> { firstDate, lastDate };
        }

        /// <summary>
        /// Returns dates dropdown in the required format for Graphs view.
        /// </summary>
        /// <returns>List of dates in string format.</returns>
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

        /// <summary>
        /// Returns the names of all counties for dropdown data.
        /// </summary>
        /// <returns>List of all names.</returns>
        public List<string> ReturnCountiesDd()
        {
            var countyNames = new List<string>();
            foreach (var county in _db.Counties)
            {
                countyNames.Add(county.Name);
            }
            return countyNames;
        }
    }
}