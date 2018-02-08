using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace CrimeWatch.Controllers
{
    public class MapController : Controller
    {
        private CrimeWatchDBEntities db = new CrimeWatchDBEntities();

        // GET: Map
        public ActionResult Index(String type)
        {
            List<Crime> crimes = db.Crimes.ToList();
            if (!(type == "No filter" || String.IsNullOrEmpty(type)))
            {
                foreach (var crime in crimes.ToList())
                {
                    if (crime.Type != type)
                    {
                        crimes.Remove(crime);
                    }

                }
            }
            return View(crimes);
        }

        public ActionResult DeleteAll()
        {
            foreach (Crime crime in db.Crimes)
            {
                db.Crimes.Remove(crime);
            }
            db.SaveChanges();
            return RedirectToAction("About", "Home");
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

            return RedirectToAction("About", "Home");
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
                            Police_Department = Police_Department,
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