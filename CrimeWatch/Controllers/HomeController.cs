using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RDotNet;

namespace CrimeWatch.Controllers
{
    public class HomeController : Controller
    {
        public static void ExecuteScriptFile(string scriptFilePath, string paramForScript1, string paramForScript2)
        {
            using (var en = REngine.GetInstance())
            {
                var args_r = new string[2] { paramForScript1, paramForScript2 };
                var execution = "source('" + scriptFilePath + "')";
                en.SetCommandLineArguments(args_r);
                en.Evaluate(execution);
            }
        }

        private CrimeWatchDBEntities db = new CrimeWatchDBEntities();

        public ActionResult Index()
        {
            return View(db.Police_Departments.ToList());
        }

        public ActionResult About()
        {            
            ViewBag.records = db.Crimes.Count();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MyPortal()
        {
            return View(db.AspNetUsers.Find(User.Identity.GetUserId()));
        }

        public ActionResult StatsParameters()
        {
            return View();
        }        

        // GET: Map
        public ActionResult Stats(String police_department, String date, String category)
        {     
            police_department = String.IsNullOrEmpty(police_department) ? "All Police Departments" : police_department;
            date = String.IsNullOrEmpty(date) ? "All 2017" : date;
            category = String.IsNullOrEmpty(category) ? "All Categories" : category;       
            ViewBag.category = category;
            ViewBag.date = date;
            ViewBag.police_department = police_department;

            return View(ApplyFilters(police_department, date, category));
        }

        public Tuple<List<Police_Department>, List<Crime>> ApplyFilters(String police_department, String date, String category)
        {
            List<Police_Department> police_departments = new List<Police_Department>();
            List<Crime> crimes = new List<Crime>();
            if (police_department.Equals("All Police Departments"))
            {
                police_departments = db.Police_Departments.ToList();
                crimes = db.Crimes.ToList();
            }
            else {
                police_departments = db.Police_Departments.Where(x => x.Name == police_department).ToList();
                crimes = police_departments.First(x=>x.Name.Equals(police_department)).Crimes.ToList();
            }
            if (crimes.Count != 0) {
                foreach (var crime in crimes.ToList())
                {
                    if (date.Contains("All "))
                    {
                        if (crime.Date.Value.Year.ToString() != date.Split(' ')[1])
                        {
                            crimes.Remove(crime);
                        }
                    }
                    else
                    {
                        if (!(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(crime.Date.Value.Month).Equals(date.Split(' ')[0]) && crime.Date.Value.Year.ToString().Equals(date.Split(' ')[1])))
                        {
                            crimes.Remove(crime);
                        }
                    }
                    if (!category.Equals("All Categories"))
                    {
                        if (crime.Type != category)
                        {
                            crimes.Remove(crime);
                        }
                    }              
                }
            }
            foreach (Police_Department pd in police_departments) {
                pd.Crimes = crimes.Where(x=>x.Police_Department_Id == pd.Id).ToList();
            }
            police_departments = police_departments.OrderByDescending(x => x.Crimes.Count()).ToList();
            return new Tuple<List<Police_Department>, List<Crime>>(police_departments, crimes);
        }

        public ActionResult Upload() {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase excelfile)
        {
            if (excelfile != null && excelfile.ContentLength > 0)
            {
                string path = Server.MapPath("~/Content/" + excelfile.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                excelfile.SaveAs(path);
                ImportExcel(path);
                System.IO.File.Delete(path);
            }
            return RedirectToAction("MyPortal");
        }

        public void ImportExcel(String path)
        {
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            try
            {
                for (int row = 1; row <= range.Rows.Count; row++)
                {
                    if (IsRowValid(range.Rows[row]))
                    {
                        DateTime Date = DateTime.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                        String Police_Department = ((Excel.Range)range.Cells[row, 4]).Text;
                        int Police_Department_Id = db.Police_Departments.First(x => x.Name.Equals(Police_Department)).Id;
                        float Longitude = float.Parse(((Excel.Range)range.Cells[row, 5]).Text);
                        float Latitude = float.Parse(((Excel.Range)range.Cells[row, 6]).Text);
                        String Location = ((Excel.Range)range.Cells[row, 7]).Text;
                        String LSOA_Code = ((Excel.Range)range.Cells[row, 8]).Text;
                        String LSOA_Name = ((Excel.Range)range.Cells[row, 9]).Text;
                        String Type = ((Excel.Range)range.Cells[row, 10]).Text;
                        String Outcome = ((Excel.Range)range.Cells[row, 11]).Text;

                        Crime crime = new Crime
                        {
                            Date = Date,
                            Police_Department_Id = Police_Department_Id,
                            Longitude = Longitude,
                            Latitude = Latitude,
                            Type = Type,
                            Location = String.IsNullOrEmpty(Location) ? "Unknown" : Location,
                            LSOA_Code = String.IsNullOrEmpty(LSOA_Code) ? "Unknown" : LSOA_Code,
                            LSOA_Name = String.IsNullOrEmpty(LSOA_Name) ? "Unknown" : LSOA_Name,
                            Outcome = String.IsNullOrEmpty(Outcome) ? "Unknown" : Outcome
                        };
                        db.Crimes.Add(crime);
                    }
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
        }

        [HttpPost]
        public ActionResult UploadPoliceDepartments(HttpPostedFileBase excelfile)
        {
            if (excelfile != null && excelfile.ContentLength > 0)
            {
                string path = Server.MapPath("~/Content/" + excelfile.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                excelfile.SaveAs(path);
                ImportPoliceDepartmentExcel(path);
                System.IO.File.Delete(path);
            }
            return RedirectToAction("MyPortal");
        }

        public void ImportPoliceDepartmentExcel(String path)
        {
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            try
            {
                for (int row = 1; row <= range.Rows.Count; row++)
                {
                    if (IsRowValid(range.Rows[row]))
                    {                        
                        String Name = ((Excel.Range)range.Cells[row, 1]).Text;
                        String Region = ((Excel.Range)range.Cells[row, 2]).Text;

                        Police_Department police_department = new Police_Department
                        {
                            Name = Name,
                            Region = Region
                        };
                        db.Police_Departments.Add(police_department);
                    }
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
        }

        public bool IsRowValid(Excel.Range row)
        {
            String Date = row.Cells[2].Text;
            String Police_Department = row.Cells[4].Text;
            String Longitude = row.Cells[5].Text;
            String Latitude = row.Cells[6].Text;
            String Type = row.Cells[10].Text;

            if (String.IsNullOrEmpty(Date) || String.IsNullOrEmpty(Police_Department) || String.IsNullOrEmpty(Longitude) || String.IsNullOrEmpty(Latitude) || String.IsNullOrEmpty(Type))
            {
                return false;
            }
            return true;
        }
    
    }
}