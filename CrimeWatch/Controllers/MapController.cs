using PoliceUk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using CrimeWatch.Models;

namespace CrimeWatch.Controllers
{
    public class MapController : Controller
    {
        private crimewatchAzureModels db = new crimewatchAzureModels();

        public ActionResult Parameters()
        {     
            ViewBag.dateDD = ReturnDateDD();
            ViewBag.typeDD = ReturnTypeDD();

            return View();
        }


        public ActionResult Map(string address, string date, string type)
        {
            
            List<XElement> LatLng = ReturnLatLng(address);                        
            var crimesLocation = new Geoposition(double.Parse(LatLng.First().Value), double.Parse(LatLng.ElementAt(1).Value));
            DateTime crimesDate = DateTime.ParseExact(date, "MMMM yyyy", CultureInfo.InvariantCulture);
            var policeClient = new PoliceUkClient();
            PoliceUk.Entities.StreetLevel.StreetLevelCrimeResults results = policeClient.StreetLevelCrimes(crimesLocation, crimesDate);
            List<Crime> crimes = new List<Crime>();
            foreach (PoliceUk.Entities.StreetLevel.Crime crime in results.Crimes)
            { 
                string category = crime.Category.ToUpper().First() + crime.Category.Remove(0,1).Replace("-"," ");
                crimes.Add(new Crime { Latitude = crime.Location.Latitude, Longitude = crime.Location.Longitude, Type = category, Date = crimesDate, Location = crime.Location.Street.Name ,Outcome = crime.OutcomeStatus is null? "Unknown":crime.OutcomeStatus.Category});
            }
            if (!type.Equals("All crime") && !type.Equals(""))
                crimes = crimes.Where(x => x.Type == type).ToList();

            crimes = SumCrimeTypes(crimes);           
            ViewBag.dateDD = ReturnDateDD();
            ViewBag.typeDD = ReturnTypeDD();
            ViewBag.type = type;
            ViewBag.date = date;
            ViewBag.address = address;
            ViewBag.lat = float.Parse(LatLng.First().Value);
            ViewBag.lng = float.Parse(LatLng.ElementAt(1).Value);
            return View(crimes);
        }

        public List<Crime> SumCrimeTypes(List<Crime> crimes) {
            foreach (Crime crime in crimes)
            {
                if (crimes.Where(x=>x.Latitude.Equals(crime.Latitude) && x.Longitude.Equals(crime.Longitude)).ToList().Count>1 && !crime.Type.Contains('('))
                {
                    List<Crime> sameLatLngCrimes = crimes.Where(c => c.Latitude == crime.Latitude && c.Longitude == crime.Longitude).ToList();
                    
                    List<string> types = new List<string>();
                    foreach (Crime crime2 in sameLatLngCrimes)
                    {
                        types.Add(crime2.Type);
                    }
                    List<string> typesDistinct = new List<string>();
                    foreach (string s in types.Distinct().ToList())
                    {
                        typesDistinct.Add(s);
                        typesDistinct.Add(sameLatLngCrimes.Where(x => x.Type.Equals(s)).Count().ToString());

                    }
                    string newType = "";
                    for (int j = 0; j < typesDistinct.Count; j += 2)
                    {
                        newType = newType + typesDistinct[j] + " (" + typesDistinct[j + 1] + "), ";
                    }
                    foreach (Crime c in sameLatLngCrimes)
                    {
                        c.Type = newType.Remove(newType.Length - 2);
                        c.Outcome = "Multiple";
                    }
                }
            }
            return crimes;
        }

        public List<XElement> ReturnLatLng(string address)
        {
            string requestUri = ConstantStrings.API_GOOGLE_GEOCODE + "address=" + address.Replace(" ", "+") + "&key=AIzaSyD_rXEE2-a_KrNsOkLH0sxMr96NVZjaiTI";
            WebRequest request = WebRequest.Create(requestUri);
            WebResponse responseAddr = request.GetResponse();
            XDocument xdoc = XDocument.Load(responseAddr.GetResponseStream());
            XElement result = xdoc.Element("GeocodeResponse").Element("result");
            XElement locationElement = result.Element("geometry").Element("location");
            XElement lat = locationElement.Element("lat");
            XElement lng = locationElement.Element("lng");
            return new List<XElement> { lat, lng };
        }

        public ActionResult DeleteAll()
        {
            foreach (Models.Crime crime in db.Crimes)
            {
                db.Crimes.Remove(crime);
            }
            db.SaveChanges();
            return RedirectToAction("About", "Home");
        }


        public List<string> ReturnDateDD()
        {
            List<string> datesToString = new List<string>();
            List<DateTime> dates = new List<DateTime>();
            foreach (var record in db.Crimes_pm)
            {
                dates.Add(record.Month);
            }
            dates = dates.Distinct().OrderByDescending(x => x).ToList();
            foreach (var date in dates)
            {
                datesToString.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month) + " " + date.Year);
            }
            return datesToString;
        }

        public List<string> ReturnTypeDD()
        {
            List<string> typesToString = new List<string>();
            PoliceUkClient client = new PoliceUkClient();
            List<PoliceUk.Entities.Category> categories = client.CrimeCategories(DateTime.UtcNow).ToList();
            foreach (var category in categories)
            {
                typesToString.Add(category.Name);
            }
            return typesToString;
        }
    }
}