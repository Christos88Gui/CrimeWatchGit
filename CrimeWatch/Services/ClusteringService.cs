using Accord.MachineLearning;
using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrimeWatch.Services
{
    public class ClusteringService
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();

        /// <summary>
        /// Initialize a KMeans object and uses it to perform clustering on observations recieved as parameters.
        /// </summary>
        /// <param name="observations">Array of observations to be clustered</param>
        /// <param name="clusterNum">Desired number of clusters</param>
        /// <returns>An arrary of integers representing the cluster number of each observation</returns>
        public int[] KMeansClustering(double[][] observations, int clusterNum)
        {
            //To ensure reprodusability.
            Accord.Math.Random.Generator.Seed = 8;
            var kmeans = new KMeans(k: clusterNum);
            var clusters = kmeans.Learn(observations);
            //Assigned labels for each observation.
            var labels = clusters.Decide(observations);
            return labels;
        }

        /// <summary>
        /// Lists all counties and passes their average crimes per 1000 residents to KMeansClustering().
        /// Receives a list of labels and assigns each county to the respective cluster. It afterwards correct the cluster numbers.
        /// </summary>
        public void ClusterCounties()
        {
            //Retrieves counties to be included in clustering
            List<County> counties = _db.Counties.Where(x => x.Included == true).ToList();
            var observations = new double[counties.Count][];
            foreach (var county in counties)
            {
                var index = counties.IndexOf(county);
                //Clusters counties by their crimes per 1000 residents
                var crimesPer1000Residents = GetCrimesPer1000Residents(county);
                observations[index] = new double[] { crimesPer1000Residents };
            }
            //Passes counties' crimes per 1000 people to KMEans asking for 5 clusters 'Very Low' - 'Very High'.
            var labels = KMeansClustering(observations, 5);
            //Assign the labels.
            foreach (var county in counties)
            {
                var index = counties.IndexOf(county);
                county.CrimeRateId = labels[index] + 1;
            }
            //Save changes to databse.
            _db.SaveChanges();
            //Reasign labels in correct way.
            CorrectCountyLabels(labels);
        }

        public void CorrectCountyLabels(int[] labels)
        {
            foreach (var county in _db.Counties.Where(x => x.Included == true))
            {
                var index = _db.Counties.Where(x => x.Included == true).ToList().IndexOf(county);
                switch (labels[index] + 1)
                {
                    case 1:
                        county.CrimeRateId = 5;
                        break;
                    case 2:
                        county.CrimeRateId = 1;
                        break;
                    case 3:
                        county.CrimeRateId = 2;
                        break;
                    case 4:
                        county.CrimeRateId = 3;
                        break;
                    case 5:
                        county.CrimeRateId = 4;
                        break;
                }
            }
            //Save changes to databse.
            _db.SaveChanges();
        }


        /// <summary>
        /// Loops over months of each county and passes them to KMeansClustering method for clustering.
        /// Receives the results for each county and calls CorrectLabels to fix the random labels assigned.
        /// </summary>
        public void ClusterRecords()
        {
            foreach (var county in _db.Counties)
            {
                var records = county.Records.ToList();
                //Forms an observation array and fills it with the number of crimes for each month.
                var observations = new double[records.Count][];
                foreach (var record in records)
                {
                    var i = records.IndexOf(record);
                    observations[i] = new double[] { record.AllCrimes };
                }
                //Calls KMeansClustering requesting 3 clusters ('Low','Medium','High')
                var labels = KMeansClustering(observations, 3);
                for (var i = 0; i < labels.Length; i++)
                {
                    var label = labels[i];
                    records[i].CrimeRateId = _db.CrimeRates.First(x => x.Rank == label + 2).Id;
                }
            }
            _db.SaveChanges();
            CorrectRecordLabels();
        }

        /// <summary>
        /// Kmeans clusters data but does not assign the correct labels to each cluster (i.e. All 'High' criminality months may be assigned label 0).
        /// This method corrects the labels of all monthly records in the database.
        /// </summary>
        public void CorrectRecordLabels()
        {
            foreach (var county in _db.Counties)
            {
                var cluster1 = county.Records.Where(x => x.CrimeRate.Rank == 2).ToList();
                var cluster2 = county.Records.Where(x => x.CrimeRate.Rank == 3).ToList();
                var cluster3 = county.Records.Where(x => x.CrimeRate.Rank == 4).ToList();

                var records = new List<Record> { cluster1.First(), cluster2.First(), cluster3.First() };
                records = records.OrderBy(x => x.AllCrimes).ToList();

                if (cluster1.Any(x => x.Id == records.First().Id)) { foreach (var r in cluster1) { r.CrimeRateId = 2; } }
                else if (cluster1.Any(x => x.Id == records.ElementAt(1).Id)) { foreach (var r in cluster1) { r.CrimeRateId = 3; } }
                else if (cluster1.Any(x => x.Id == records.ElementAt(2).Id)) { foreach (var r in cluster1) { r.CrimeRateId = 4; } }

                if (cluster2.Any(x => x.Id == records.First().Id)) { foreach (var r in cluster2) { r.CrimeRateId = 2; } }
                else if (cluster2.Any(x => x.Id == records.ElementAt(1).Id)) { foreach (var r in cluster2) { r.CrimeRateId = 3; } }
                else if (cluster2.Any(x => x.Id == records.ElementAt(2).Id)) { foreach (var r in cluster2) { r.CrimeRateId = 4; } }

                if (cluster3.Any(x => x.Id == records.First().Id)) { foreach (var r in cluster3) { r.CrimeRateId = 2; } }
                else if (cluster3.Any(x => x.Id == records.ElementAt(1).Id)) { foreach (var r in cluster3) { r.CrimeRateId = 3; } }
                else if (cluster3.Any(x => x.Id == records.ElementAt(2).Id)) { foreach (var r in cluster3) { r.CrimeRateId = 4; } }
            }
            _db.SaveChanges();
        }


        /// <summary>
        /// Receives a county and returns its average crimes per 1000 people.
        /// </summary>
        /// <param name="county"></param>
        /// <returns>The desired county</returns>
        public int GetCrimesPer1000Residents(County county)
        {
            DateTime firstDate = ReturnFirstAndLastDate().First();
            DateTime lastDate = ReturnFirstAndLastDate().Last();
            var sumOfCrimes = county.Records.Where(x => x.Date >= firstDate && x.Date <= lastDate).Sum(x => x.AllCrimes);
            //Calculate crimes/1000 people by multiplying crimes by 1000 and dividing by population.
            var cirmesPer1000Residents = sumOfCrimes * 1000 / county.Population.Value;
            return cirmesPer1000Residents;
        }

        /// <summary>
        /// Returns first and last date of the desired period.
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public List<DateTime> ReturnFirstAndLastDate()
        {
            var lastRecord = _db.Records.OrderBy(x => x.Date).ToList().Last();
            var lastDate = new DateTime();
            var firstDate = new DateTime();
            lastDate = lastRecord.Date;
            firstDate = lastDate.AddMonths(-11);
            return new List<DateTime> { firstDate, lastDate };
        }
    }
}