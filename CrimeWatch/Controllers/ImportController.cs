using CrimeWatch.Models;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Globalization;

namespace CrimeWatch.Controllers
{
    public class ImportController : Controller
    {
        private crimewatchAzureModels db = new crimewatchAzureModels();

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportCrimes(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                throw new Exception();
            }
            string path = Server.MapPath("~/Content/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            excelfile.SaveAs(path);
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            try
            {
                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    if (IsRowValid(range.Rows[row]))
                    {
                        DateTime Date = DateTime.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                        String Police_Department = ((Excel.Range)range.Cells[row, 4]).Text;
                        int Police_Department_Id = db.Police_Departments.First(x => x.Name.Equals(Police_Department)).Id;
                        int County_Id = db.Police_Departments.First(x => x.Name.Equals(Police_Department)).Counties.First().Id;
                        float Longitude = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 5]).Text) ? 0 : float.Parse(((Excel.Range)range.Cells[row, 5]).Text);
                        float Latitude = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 6]).Text) ? 0 : float.Parse(((Excel.Range)range.Cells[row, 6]).Text);
                        String Location = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 7]).Text) ? "Unknown" : ((Excel.Range)range.Cells[row, 7]).Text;
                        String LSOA_Code = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 8]).Text) ? "Unknown" : ((Excel.Range)range.Cells[row, 8]).Text;
                        String LSOA_Name = ((Excel.Range)range.Cells[row, 9]).Text;
                        String Type = ((Excel.Range)range.Cells[row, 10]).Text;
                        String Outcome = ((Excel.Range)range.Cells[row, 11]).Text;

                        Crime crime = new Crime
                        {
                            Date = Date,
                            Police_Department_Id = Police_Department_Id,
                            County_Id = County_Id,
                            Longitude = Longitude,
                            Latitude = Latitude,
                            Type = Type,
                            Location = String.IsNullOrEmpty(Location) ? "Unknown" : Location,
                            LSOA_Code = String.IsNullOrEmpty(LSOA_Code) ? "Unknown" : LSOA_Code,
                            LSOA_Name = LSOA_Name,
                            Outcome = String.IsNullOrEmpty(Outcome) ? "Unknown" : Outcome
                        };
                        db.Crimes.Add(crime);
                    }
                }
                db.SaveChanges();
                DateTime dateOfNewExcel = DateTime.Parse(((Excel.Range)range.Cells[2, 2]).Text);
                DeleteLastYearsRecords(dateOfNewExcel);
                workbook.Close(true);
                application.Quit();
            }
            catch (Exception)
            {
                workbook.Close(true);
                application.Quit();
            }
            System.IO.File.Delete(path);
            return RedirectToAction("MyPortal", "Home");
        }

        [HttpPost]
        public ActionResult ImportPoliceDepartments(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                throw new Exception();
            }
            string path = Server.MapPath("~/Content/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            excelfile.SaveAs(path);
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            try
            {
                for (int row = 1; row <= range.Rows.Count; row++)
                {
                    String PD_Name = ((Excel.Range)range.Cells[row, 1]).Text;
                    String County_Name = ((Excel.Range)range.Cells[row, 2]).Text;
                    String County_Name2 = ((Excel.Range)range.Cells[row, 3]).Text;
                    Police_Departments police_department = new Police_Departments { Name = PD_Name, Id = row };
                    County county = new County { Name = County_Name, Police_Department_Id = row };
                    County county2 = new County { Name = County_Name2, Police_Department_Id = row };

                    db.Police_Departments.Add(police_department);
                    db.Counties.Add(county);
                    db.Counties.Add(county2);
                }
                db.SaveChanges();
                workbook.Close(true);
                application.Quit();
            }
            catch (Exception)
            {
                workbook.Close(true);
                application.Quit();
            }
            System.IO.File.Delete(path);
            return RedirectToAction("MyPortal");
        }

        public bool IsRowValid(Excel.Range row)
        {
            String Police_Department = row.Cells[4].Text;
            String LSOA_Name = row.Cells[9].Text;
            String Type = row.Cells[10].Text;

            if (String.IsNullOrEmpty(Police_Department) || String.IsNullOrEmpty(Type) || String.IsNullOrEmpty(LSOA_Name))
            {
                return false;
            }
            return true;
        }

        public ActionResult setCountiesRight()
        {
            List<Crime> AvonSomerset = new List<Crime>();
            List<Crime> DevonCornwall = new List<Crime>();
            List<Crime> Hampshire = new List<Crime>();
            List<Crime> Leicestershire = new List<Crime>();
            List<Crime> Northumbria = new List<Crime>();
            List<Crime> Sussex = new List<Crime>();
            List<Crime> ThamesValley = new List<Crime>();
            List<Crime> WestMercia = new List<Crime>();

            return View("MyPortal", "Home");
        }

        [HttpPost]
        public ActionResult FindCounties(HttpPostedFileBase excelfile)
        {
            List<string> allcounties = new List<string>();
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                throw new Exception();
            }
            string path = Server.MapPath("~/Content/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            excelfile.SaveAs(path);
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            try
            {
                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    if (IsRowValid(range.Rows[row]))
                    {
                        string c = ((Excel.Range)range.Cells[row, 9]).Text;
                        allcounties.Add(c.Substring(0, c.LastIndexOf(" ")));
                    }
                }
                workbook.Close(true);
                application.Quit();
            }
            catch (Exception)
            {
                workbook.Close(true);
                application.Quit();
            }
            System.IO.File.Delete(path);
            List<string> distinct = allcounties.Distinct().ToList();
            return RedirectToAction("MyPortal");
        }

        public void DeleteLastYearsRecords(DateTime dateOfNewExcel)
        {
            int month = dateOfNewExcel.Month;
            int year = dateOfNewExcel.Year;
            List<Crime> crimesToRemove = db.Crimes.Where(x => x.Date.Value.Month == month && x.Date.Value.Year == year - 1).ToList();
            foreach (Crime crime in crimesToRemove)
            {
                db.Crimes.Remove(crime);
            }
            db.SaveChanges();
        }

        public ActionResult delete()
        {
            foreach (Crime crime in db.Crimes)
            {
                if (crime.Date.Value.Year == 2015)
                {
                    db.Crimes.Remove(crime);
                }
            }
            db.SaveChanges();
            return RedirectToAction("MyPortal", "Home");
        }


        [HttpPost]
        public ActionResult ImportCrimes_pm(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                throw new Exception();
            }
            string path = Server.MapPath("~/Content/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            excelfile.SaveAs(path);
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            try
            {
                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    if (IsRowValid(range.Rows[row]))
                    {
                        DateTime Date = DateTime.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                        String Police_Department = ((Excel.Range)range.Cells[row, 4]).Text;
                        int Police_Department_Id = db.Police_Departments.First(x => x.Name.Equals(Police_Department)).Id;
                        int County_Id = db.Police_Departments.First(x => x.Name.Equals(Police_Department)).Counties.First().Id;
                        float Longitude = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 5]).Text) ? 0 : float.Parse(((Excel.Range)range.Cells[row, 5]).Text);
                        float Latitude = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 6]).Text) ? 0 : float.Parse(((Excel.Range)range.Cells[row, 6]).Text);
                        String Location = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 7]).Text) ? "Unknown" : ((Excel.Range)range.Cells[row, 7]).Text;
                        String LSOA_Code = String.IsNullOrEmpty(((Excel.Range)range.Cells[row, 8]).Text) ? "Unknown" : ((Excel.Range)range.Cells[row, 8]).Text;
                        String LSOA_Name = ((Excel.Range)range.Cells[row, 9]).Text;
                        String Type = ((Excel.Range)range.Cells[row, 10]).Text;
                        String Outcome = ((Excel.Range)range.Cells[row, 11]).Text;

                        Crime crime = new Crime
                        {
                            Date = Date,
                            Police_Department_Id = Police_Department_Id,
                            County_Id = County_Id,
                            Longitude = Longitude,
                            Latitude = Latitude,
                            Type = Type,
                            Location = String.IsNullOrEmpty(Location) ? "Unknown" : Location,
                            LSOA_Code = String.IsNullOrEmpty(LSOA_Code) ? "Unknown" : LSOA_Code,
                            LSOA_Name = LSOA_Name,
                            Outcome = String.IsNullOrEmpty(Outcome) ? "Unknown" : Outcome
                        };
                        db.Crimes.Add(crime);
                    }
                }
                db.SaveChanges();
                DateTime dateOfNewExcel = DateTime.Parse(((Excel.Range)range.Cells[2, 2]).Text);
                DeleteLastYearsRecords(dateOfNewExcel);
                workbook.Close(true);
                application.Quit();
            }
            catch (Exception)
            {
                workbook.Close(true);
                application.Quit();
            }
            System.IO.File.Delete(path);
            return RedirectToAction("MyPortal", "Home");
        }


    }
}