using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrimeWatch.Models;
using Microsoft.Office.Interop.Excel;

namespace CrimeWatch.Controllers
{
    //Allows only administrators to call its mehtods
    //[Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        /// <summary>
        /// Instance of the CrimeWatch model.
        /// </summary>
        private readonly CrimeWatchModel _db = new CrimeWatchModel();

        /// <summary>
        /// Redirects to the administrator page.
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminPage()
        {            
            //All records in the database.
            var records = _db.Records.ToList();
            //All user accounts in the datase.
            var users = _db.AspNetUsers.ToList();
            //Return admin page passing all users and records.
            return View(Tuple.Create(records, users));            
        }

        /// <summary>
        /// Removes a user from the system
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns></returns>
        public ActionResult DeleteUser(string email)
        { 
            //Find the user by email
            var user = _db.AspNetUsers.First(x=>x.Email==email);
            if (user != null)
            {
                _db.AspNetUsers.Remove(user);
            }
            //Cathes possible exception
            try
            {
                _db.SaveChanges();
            }
            catch 
            {
                return View("Error");
            }
            return RedirectToAction("AdminPage");
        }

        /// <summary>
        /// Deletes monthly records from the database
        /// </summary>
        /// <param name="id">The records Id</param>
        /// <returns></returns>
        public ActionResult DeleteRecord(int id)
        {
            //Finds the desired record
            var record = _db.Records.Find(id);
            //If it exists it gets removed
            if (record != null)
            {
                _db.Records.Remove(record);
            }
            try
            {
                _db.SaveChanges();
            }
            //Cathes possible exception
            catch
            {
                return View("Error");
            }
            return RedirectToAction("AdminPage");
        }


        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                throw new Exception();
            }
            var path = Server.MapPath("~/Content/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            excelfile.SaveAs(path);
            var application = new Application();
            var workbook = application.Workbooks.Open(path);
            Worksheet worksheet = workbook.ActiveSheet;
            var range = worksheet.UsedRange;
            try
            {
                for (var row = 1; row <= range.Rows.Count; row++)
                {
                    var record = new Record
                    {
                        CountyId = int.Parse(((Range)range.Cells[row, 1]).Text) + 90,
                        Date = DateTime.ParseExact(((Range)range.Cells[row, 2]).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        AllCrimes = int.Parse(((Range)range.Cells[row, 3]).Text)
                    };
                    _db.Records.Add(record);
                }
                _db.SaveChanges();
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

        [HttpPost]
        public ActionResult ImportPD(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                throw new Exception();
            }
            var path = Server.MapPath("~/Content/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            excelfile.SaveAs(path);
            var application = new Application();
            var workbook = application.Workbooks.Open(path);
            Worksheet worksheet = workbook.ActiveSheet;
            var range = worksheet.UsedRange;
            try
            {
                for (var row = 1; row <= range.Rows.Count; row++)
                {
                    PoliceDepartment pd = new PoliceDepartment()
                    {
                        Id = row,
                        Name = ((Range)range.Cells[row, 1]).Text
                    };
                    _db.PoliceDepartments.Add(pd);
                }
                
                _db.SaveChanges();
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

    }
}