using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression.Linear;
using CrimeWatch.Models;

namespace CrimeWatch.Services
{

    public class Tests
    {
        private CrimeWatchModel _db = new CrimeWatchModel();
        private RegressionService _regression = new RegressionService();
        private readonly OrdinaryLeastSquares _ols = new OrdinaryLeastSquares() { UseIntercept = true };

        public OrdinaryLeastSquares Ols => _ols;

        /// <summary>
        /// Tests the database correctly stores data
        /// </summary>
        public void PopulateDatabase()
        {

            County county1 = _db.Counties.Add(new County() { Name = "Example1", PoliceDepartmentId = 0, Population = 100000 });
            County county2 = _db.Counties.Add(new County() { Name = "Example2", PoliceDepartmentId = 2, Population = 200000 });
            County county3 = _db.Counties.Add(new County() { Name = "Example3", PoliceDepartmentId = 3, Population = 300000 });

            PoliceDepartment pd1 = _db.PoliceDepartments.Add(new PoliceDepartment() { Name = "Example1" });
            PoliceDepartment pd2 = _db.PoliceDepartments.Add(new PoliceDepartment() { Name = "Example2" });
            PoliceDepartment pd3 = _db.PoliceDepartments.Add(new PoliceDepartment() { Name = "Example3" });

            Record record1 = _db.Records.Add(new Record() { AllCrimes = 100000, CrimesPer1000 = 5400, Date = DateTime.UtcNow });
            Record record2 = _db.Records.Add(new Record() { AllCrimes = 200000, CrimesPer1000 = 5400, Date = DateTime.UtcNow });
            Record record3 = _db.Records.Add(new Record() { AllCrimes = 300000, CrimesPer1000 = 5400, Date = DateTime.UtcNow });

            CrimeRate rank1 = _db.CrimeRates.Add(new CrimeRate() { Name = "LOW", Rank = 1 });
            CrimeRate rank2 = _db.CrimeRates.Add(new CrimeRate() { Name = "MEDIUM", Rank = 2 });
            CrimeRate rank3 = _db.CrimeRates.Add(new CrimeRate() { Name = "HIGH", Rank = 3 });

            CrimeRate crimerate1 = _db.CrimeRates.Add(new CrimeRate() { Name = "LOW", Rank = 4 });
            CrimeRate crimerate2 = _db.CrimeRates.Add(new CrimeRate() { Name = "MEDIUM", Rank = 5 });
            CrimeRate crimerate3 = _db.CrimeRates.Add(new CrimeRate() { Name = "HIGH", Rank = 6 });

            _db.SaveChanges();
        }



        public void TestSvrAccuracy(int monthsToPredict)
        {
            //Total Number of montths predicted correctly
            double correctGuesses = 0;
            //Total number of attemts
            double totalGuesses = 0;
            //Actual Low , predicted Low
            double aLpL = 0;            
            double aLpN = 0;            
            double aLpH = 0;
            //Actual Normal , predicted Low
            double aNpL = 0;
            double aNpN = 0;
            double aNpH = 0;
            //Actual Normal , predicted Low
            double aHpL = 0;
            double aHpN = 0;
            double aHpH = 0;
            //Precision and recall lists.
            List<double> Precisions = new List<double>();
            List<double> Recalls = new List<double>();
            //Variable to measure the sum of all deviations to be used in MAE, MSE , RMSE
            double summedDeviations = 0;
            //Variable to measure the sum of all squared deviations to be used in MAE, MSE , RMSE
            double summedSquaredDeviations = 0;
            // Mean Absolute Error, Mean Squared Error, and Root Mean Squared Error
            double MAE = 0;
            double MSE = 0;
            double RMSE = 0;
            var counties = _db.Counties.Where(x => x.Id != 97 && x.Id != 111 && x.Id != 125 && x.Id != 130 && x.Id != 131).ToList();

            foreach (var county in counties)
            {
                var totalRecords = county.Records.OrderBy(x => x.Date).ToList();
                var trainingInputs = new double[totalRecords.Count - monthsToPredict][];
                var trainingOutputs = new double[totalRecords.Count - monthsToPredict];
                var testingInputs = new double[monthsToPredict][];
                var testingOutputs = new double[monthsToPredict];
                for (var i = 0; i < totalRecords.Count; i++)
                {
                    if (i < trainingInputs.Length)
                    {
                        trainingInputs[i] = new double[] { totalRecords[i].Date.Month, totalRecords[i].Date.Year };
                        trainingOutputs[i] = totalRecords[i].AllCrimes / 1000.0;
                    }
                    else
                    {
                        testingInputs[i - trainingInputs.Length] = new double[] { totalRecords[i].Date.Month, totalRecords[i].Date.Year };
                        testingOutputs[i - trainingInputs.Length] = totalRecords[i].AllCrimes / 1000.0;
                    }
                }
                var teacher = new SequentialMinimalOptimizationRegression()
                {
                    Kernel = new Gaussian() { Gamma = 0.01 },
                    Complexity = 280,
                    Epsilon = 0.25
                };
                var svr = teacher.Learn(trainingInputs, trainingOutputs);
                var output = svr.Score(testingInputs);

                for (var i = 0; i < output.Length; i++)
                {
                    double prediction = output[i] * 1000;
                    int predictionRank = _regression.ReturnPredictedRank(county, output[i] * 1000).Rank.Value;
                    double actual = testingOutputs[i] * 1000;
                    int actualRank = _regression.ReturnPredictedRank(county, testingOutputs[i] * 1000).Rank.Value;
                    if (actualRank == 2 && predictionRank == 2) { aLpL++; correctGuesses++; };
                    if (actualRank == 2 && predictionRank == 3) { aLpN++; };
                    if (actualRank == 2 && predictionRank == 4) { aLpH++; };
                    if (actualRank == 3 && predictionRank == 2) { aNpL++; };
                    if (actualRank == 3 && predictionRank == 3) { aNpN++; correctGuesses++; };
                    if (actualRank == 3 && predictionRank == 4) { aNpH++; };
                    if (actualRank == 4 && predictionRank == 2) { aHpL++; };
                    if (actualRank == 4 && predictionRank == 3) { aHpN++; };
                    if (actualRank == 4 && predictionRank == 4) { aHpH++; correctGuesses++; };
                    summedDeviations += Math.Abs(prediction - actual);
                    summedSquaredDeviations = summedDeviations * summedDeviations;
                    Debug.WriteLine(actual + "        " + actualRank + "        " + prediction + "        " + predictionRank);
                }
                totalGuesses += testingInputs.Length;
            }
            var accuracy = (double)aLpL + aNpN + aHpH / totalGuesses;
            var precisionL = (double)(aLpL / aLpL + aLpN + aLpH);
            var precisionN = (double)(aNpN / aNpL + aNpN + aNpH);
            var precisionH = (double)(aHpH / aHpL + aHpN + aHpH);
            MAE = summedDeviations / totalGuesses;
            MSE = summedSquaredDeviations / totalGuesses;
            RMSE = Math.Sqrt(MSE);
        }

        /// <summary>
        /// Compares Linear and SVR models
        /// </summary>
        /// <param name="monthsToPredict"></param>
        public void CompareModels(int monthsToPredict)
        {
            List<double> SlrMAEs = new List<double>();
            List<double> SlrRMSEs = new List<double>();
            List<double> SvrMAEs = new List<double>();
            List<double> SvrRMSEs = new List<double>();
            foreach (var county in _db.Counties)
            {
                double SlrAbsoluteErrors = 0;
                double SvrAbsoluteErrors = 0;
                double SlrSquaredErros = 0;
                double SvrSquaredErros = 0;
                //All records of the county ordered by date.
                List<Record> totalRecords = county.Records.OrderBy(x => x.Date).ToList();
                //Training inputs and outputs.
                var trainingInputs = new double[totalRecords.Count - monthsToPredict][];
                var trainingOutputs = new double[totalRecords.Count - monthsToPredict];
                //Testing inputs and outputs.
                var testingInputs = new double[monthsToPredict][];
                var testingOutputs = new double[monthsToPredict];
                //Fill the arrays trainning and testing arrays with inputs and outputs.
                for (var i = 0; i < totalRecords.Count; i++)
                {
                    if (i < trainingInputs.Length)
                    {
                        trainingInputs[i] = new double[] { i };
                        //Values are downscaled dividing by 1000
                        trainingOutputs[i] = totalRecords[i].AllCrimes / 1000.0;
                    }
                    else
                    {
                        testingInputs[i - trainingInputs.Length] = new double[] { totalRecords[i].Date.Month, totalRecords[i].Date.Year };
                        //Values are downscaled dividing by 1000
                        testingOutputs[i - trainingInputs.Length] = totalRecords[i].AllCrimes / 1000.0;
                    }
                }
                var slr = Ols.Learn(trainingInputs, trainingOutputs);
                var outputSlr = slr.Transform(testingInputs);
                var teacher = new SequentialMinimalOptimizationRegression()
                {
                    Kernel = new Gaussian() { Gamma = 0.01 },
                    Complexity = 280,
                    Epsilon = 0.25
                };
                var svr = teacher.Learn(trainingInputs, trainingOutputs);
                var outputSvr = svr.Score(testingInputs);
                for (int i = 0; i < testingOutputs.Length; i++)
                {

                    SvrAbsoluteErrors += Math.Abs(testingOutputs[i] - outputSvr[i]);
                    SlrAbsoluteErrors += Math.Abs(testingOutputs[i] - outputSlr[i]);
                    SvrSquaredErros += Math.Abs(testingOutputs[i] - outputSvr[i]) * Math.Abs(testingOutputs[i] - outputSvr[i]);
                    SlrSquaredErros += Math.Abs(testingOutputs[i] - outputSlr[i]) * Math.Abs(testingOutputs[i] - outputSlr[i]);
                }
                SlrMAEs.Add(SlrAbsoluteErrors / (double)testingOutputs.Length);
                SvrMAEs.Add(SvrAbsoluteErrors / (double)testingOutputs.Length);
                SvrRMSEs.Add(SlrSquaredErros / ((double)testingOutputs.Length));
                SvrRMSEs.Add(SlrSquaredErros / ((double)testingOutputs.Length));
            }
        }
    }
}