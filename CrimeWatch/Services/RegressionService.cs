using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression.Linear;
using System;
using System.Collections.Generic;
using System.Linq;
using CrimeWatch.Models;

namespace CrimeWatch.Services
{
    public class RegressionService
    {
        private readonly CrimeWatchModel _db = new CrimeWatchModel();
        private readonly OrdinaryLeastSquares _ols = new OrdinaryLeastSquares() { UseIntercept = true };
        private readonly CommonFunctions _cf = new CommonFunctions();
        private SimpleLinearRegression slr = new SimpleLinearRegression();

        /// <summary>
        /// Receives a list of Records and trains a Simple Linear Regression algorithm upon them.
        /// </summary>
        /// <param name="records"></param>
        public SimpleLinearRegression TrainLinearRegression(List<Record> records)
        {
            //Orders records by ascending date.
            records = records.OrderBy(x => x.Date).ToList();
            var trainingInputs = new double[records.Count];
            var trainingOutputs = new double[records.Count];
            //Fills training arrays.
            for (var i = 0; i < records.Count; i++)
            {
                trainingInputs[i] = i;
                trainingOutputs[i] = records.ElementAt(i).AllCrimes;
            }
            //Trains the global object slr.
            slr = _ols.Learn(trainingInputs, trainingOutputs);
            return slr;
        }


        
        /// <summary>
        /// Receives a county, trains SVR algorithm with data from the county and returns the traied model.
        /// </summary>
        /// <param name="county"></param>
        /// <returns>Trained SVR object</returns>
        public SupportVectorMachine<IKernel> TrainSVR(County county)
        {
            //Creates an optimization object and sets its parameters.
            var teacher = new SequentialMinimalOptimizationRegression()
            {
                //Radial basis function kernel is used.
                Kernel = new Gaussian() { Gamma = 0.01 },
                Complexity = 350,
                Epsilon = 0.25
            };
            //Inputs to train SVR. The inputs are the month and the year.
            var trainingInputs = new double[county.Records.Count][];
            //Training outputs of SVR. They are the number of crimes divided by 1000.
            var trainingOutputs = new double[county.Records.Count];
            foreach (var record in county.Records)
            {
                //Fills inputs and outputs array with values.
                var index = county.Records.ToList().IndexOf(record);
                //trainingInputs[index] = new double[] { record.Date.Month, record.Date.Year };
                trainingInputs[index] =  new double[]{ record.Date.Month,record.Date.Year };
                trainingOutputs[index] = record.AllCrimes / 1000.0;
            }
            //Trains the algorithm.
            var svr = teacher.Learn(trainingInputs, trainingOutputs);
            return svr;
        }

        /// <summary>
        /// Returns data to populate Scatter plot
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public double[][] ReturnScatterPlotData(List<Record> pastRecords, List<Record> predictedRecords)
        {
            if (!pastRecords.Any() || !predictedRecords.Any())
            {
                var empty = new double[1][]; empty[0] = new double[] { 0, 0, 0 };
                return empty;
            }
            //var records = _db.Counties.First(x => x.Name == countyName).Records.ToList();
            var scatterPlotData = new double[pastRecords.Count + predictedRecords.Count][];
            foreach (var r in pastRecords)
            {
                var i = pastRecords.IndexOf(r);
                scatterPlotData[i] = new double[] { _cf.ReturnDiff(r.Date), r.AllCrimes, -5 };
            }
            foreach (var r in predictedRecords)
            {
                var i = pastRecords.Count + predictedRecords.IndexOf(r);              
                scatterPlotData[i] = new double[] { _cf.ReturnDiff(r.Date), -5, r.AllCrimes };
            }
            return scatterPlotData;
        }


        /// <summary>
        /// Receives a county and a number of months. 
        /// Returns the predicted monthly records of that county for the months specified.
        /// </summary>
        /// <param name="county">Number of months</param>
        /// <param name="months">County to predict</param>
        /// <returns></returns>
        public List<Record> ReturnPredictedRecords(County county, int months)
        {
            //Trains SVR model
            var svm = TrainSVR(county);
            var returnedRecords = new List<Record>();
            var predictionSet = new double[months][];
            //Start date for predictions
            var startDate = county.Records.OrderBy(x => x.Date).Last().Date.AddMonths((1));
            //End date for predictions
            var endDate = county.Records.OrderBy(x => x.Date).Last().Date.AddMonths((months));
            int counter = 0;
            //Months to predict
            for (DateTime dateTime = startDate; dateTime <= endDate; dateTime = dateTime.AddMonths((1)))
            {
                predictionSet[counter] = new double[] { dateTime.Month, dateTime.Year };
                counter++;
            }
            //Predicted records
            double[] outputs = svm.Score(predictionSet);
            for (int i = 0; i < outputs.Length; i++)
            {
                //Assign the predicted months ranks Low,Normal or High 
                CrimeRate crime_rate = ReturnPredictedRank(county, (int)(outputs[i] * 1000));
                returnedRecords.Add(new Record()
                {
                    Date = new DateTime((int)predictionSet[i][1], (int)predictionSet[i][0], 1),
                    AllCrimes = (int)(outputs[i] * 1000),
                    CrimeRateId = crime_rate.Rank
                });
            }
            return returnedRecords;
        }


        public CrimeRate ReturnPredictedRank(County county, double predictedCrimes)
        {
            var rank1 = county.Records.Where(x => x.CrimeRate.Rank == 2).OrderBy(x => x.AllCrimes).ToList();
            var rank2 = county.Records.Where(x => x.CrimeRate.Rank == 3).OrderBy(x => x.AllCrimes).ToList();
            var rank3 = county.Records.Where(x => x.CrimeRate.Rank == 4).OrderBy(x => x.AllCrimes).ToList();

            if (predictedCrimes < rank1.Last().AllCrimes) return _db.CrimeRates.First(x => x.Rank == 2);
            else if (predictedCrimes < rank2.Last().AllCrimes) return _db.CrimeRates.First(x => x.Rank == 3);
            else return _db.CrimeRates.First(x => x.Rank == 4);
        }


    }
}