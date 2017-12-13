using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace CrimeWatch.Controllers
{
    public class MapController : Controller
    {
        private DefaultDB db = new DefaultDB();

        // GET: Map
        public ActionResult Index() => View(db.Crimes.ToList());

        public ActionResult test() {
            ViewBag.records = db.Crimes.Count();
            return View();
        }

       
        public ActionResult ImportIt()
        {
            string path = Server.MapPath("~/Content/excel_file.xlxs");
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            List<Crime> crimes = new List<Crime>();

            for (int row = 2; row < range.Rows.Count; row++)
            {

                Crime crime = new Crime
                {
                    Date = DateTime.Parse(((Excel.Range)range.Cells[row, 1]).Text),
                    Police_Department = ((Excel.Range)range.Cells[row, 2]).Text,
                    Longitude = float.Parse(((Excel.Range)range.Cells[row, 4]).Text),
                    Latitude = float.Parse(((Excel.Range)range.Cells[row, 5]).Text),
                    Location = ((Excel.Range)range.Cells[row, 6]).Text,
                    LSOA_Code = ((Excel.Range)range.Cells[row, 7]).Text,
                    LSOA_Name = ((Excel.Range)range.Cells[row, 8]).Text,
                    Crime_Type = ((Excel.Range)range.Cells[row, 9]).Text,
                    Outcome = ((Excel.Range)range.Cells[row, 10]).Text
                };
                db.Crimes.Add(crime);                
            }
            db.SaveChanges();
            ViewBag.records = db.Crimes.Count();
            return View("test");
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile) {

            if (excelfile == null)
            {
                ViewBag.Error = "Pick a file man!";
            }
            else {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    
                    string path = Server.MapPath("~/Content/text.xlxs");
                    if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                    excelfile.SaveAs(path);

                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    List<Crime> crimes = new List<Crime>();

                    for (int row = 2; row < range.Rows.Count; row++) {
                        
                        Crime crime = new Crime
                        {
                            Date = DateTime.Parse(((Excel.Range)range.Cells[row, 1]).Text),
                            Police_Department = ((Excel.Range)range.Cells[row, 2]).Text,
                            Longitude = float.Parse(((Excel.Range)range.Cells[row, 4]).Text),
                            Latitude = float.Parse(((Excel.Range)range.Cells[row, 5]).Text),
                            Location = ((Excel.Range)range.Cells[row, 6]).Text,
                            LSOA_Code = ((Excel.Range)range.Cells[row, 7]).Text,
                            LSOA_Name = ((Excel.Range)range.Cells[row, 8]).Text,
                            Crime_Type = ((Excel.Range)range.Cells[row, 9]).Text,
                            Outcome = ((Excel.Range)range.Cells[row, 10]).Text
                            
                        };
                        ViewBag.Error = "Okay done boss!The records now are " + db.Crimes.Count();

                    }
                }
                else
                {
                    ViewBag.Error = "didnt go well....";
                }
            }                            
            return View("test");
        }


        //[HttpPost]
        //public ActionResult FindLocation(String postcode) {
            
        //    string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?postcode={0}&sensor=false", Uri.EscapeDataString(postcode));
            
        //    WebRequest request = WebRequest.Create(requestUri);
        //    WebResponse response = request.GetResponse();
        //    XDocument xdoc = XDocument.Load(response.GetResponseStream());

        //    XElement result = xdoc.Element("GeocodeResponse").Element("result");
        //    XElement locationElement = result.Element("geometry").Element("location");
        //    XElement lat = locationElement.Element("lat");
        //    XElement lng = locationElement.Element("lng");

        //    ViewBag.latitude = (int)lat;
        //    ViewBag.longitude = (int)lng;

        //    return View("Map");
        //}
    }
}