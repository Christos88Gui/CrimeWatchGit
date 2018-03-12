using CrimeWatch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CrimeWatch.Controllers
{
    public class MapController : Controller
    {
        private CrimeWatchDBEntities db = new CrimeWatchDBEntities();

        public List<Crime> ApplyFilters(String category, String police_department, String date)
        {
            List<Crime> crimes = db.Crimes.ToList();
            foreach (var crime in crimes.ToList())
            {
                if (date.Contains("All "))
                {
                    if (crime.Date.Value.Year.ToString() != date.Split(' ')[1])
                    {
                        crimes.Remove(crime);
                    }
                }
                else {
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
                if (!police_department.Equals("All Police Departments"))
                {
                    if (db.Police_Departments.Find(crime.Police_Department_Id).Name != police_department)
                    {
                        crimes.Remove(crime);
                    }
                }
            }
            return crimes;
        }


        public String ReturnMonthName(String monthIndex) {        
            switch (monthIndex)
            {
                default:
                    return DateTime.Now.Month.ToString();
                case "1":
                    return "January";
                case "2":
                    return "February";
                case "3":
                    return "March";
                case "4":
                    return "April";
                case "5":
                    return "May";
                case "6":
                    return "June";
                case "7":
                    return "July";
                case "8":
                    return "August";
                case "9":
                    return "September";
                case "10":
                    return "October";
                case "11":
                    return "November";
                case "12":
                    return "December";                                   
            }
        }

        public ActionResult MapParameters()
        {        
            return View();
        }

        // GET: Map
        public ActionResult Map(MapParametersViewModel model)
        {
            if (model.Police_Department.Equals("Select Police Department")) {
                ModelState.AddModelError(String.Empty, "Please select police department!");
                return View("MapParameters", model);
            }
            if (model.Date.Equals("Select Date"))
            {
                ModelState.AddModelError(String.Empty, "Please select Date!");
                return View("MapParameters", model);
            }
            ViewBag.type = model.Type;
            ViewBag.date = model.Date;
            ViewBag.police_department = model.Police_Department;
            List<Crime> crimes = ApplyFilters(model.Type, model.Police_Department, model.Date);
            return View(crimes);
        }


        public ActionResult Graphs(String police_department, String date, String category) {            
            if (String.IsNullOrEmpty(police_department)) {
                police_department = "All Police Departments";
            }
            if (String.IsNullOrEmpty(date))
            {
                date = "All 2017";
            }
            if (String.IsNullOrEmpty(category))
            {
                category = "All Categories";
            }
            ViewBag.police_department = police_department;
            ViewBag.date = date;
            ViewBag.category = category;

            List<Crime> crimes = ApplyFilters(category, police_department, date);
            ViewBag.crimes_per_month = new int[] { crimes.Where(x => x.Date.Value.Month == 1).Count(), crimes.Where(x => x.Date.Value.Month == 2).Count(), crimes.Where(x => x.Date.Value.Month == 3).Count(), crimes.Where(x => x.Date.Value.Month == 4).Count(), crimes.Where(x => x.Date.Value.Month == 5).Count(), crimes.Where(x => x.Date.Value.Month == 6).Count(), crimes.Where(x => x.Date.Value.Month == 7).Count(), crimes.Where(x => x.Date.Value.Month == 8).Count(), crimes.Where(x => x.Date.Value.Month == 9).Count(), crimes.Where(x => x.Date.Value.Month == 10).Count(), crimes.Where(x => x.Date.Value.Month == 11).Count(), crimes.Where(x => x.Date.Value.Month == 12).Count() };
            return View("Graphs",crimes);
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

    }
}