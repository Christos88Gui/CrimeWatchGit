using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CrimeWatch.Services;
using PoliceUk.Entities.StreetLevel;

namespace CrimeWatch.Controllers
{
    public class MapController : Controller
    {
        private readonly CommonFunctions _cf = new CommonFunctions();
        private readonly MapService service = new MapService();

        /// <summary>
        /// Redirects to Map Parameters View
        /// </summary>
        /// <returns></returns>
        public ActionResult Parameters()
        {     
            //Passes the dates and crime categories dropdowns to View
            ViewBag.dateDD = _cf.ReturnDateDd();
            ViewBag.typeDD = _cf.ReturnTypeDd();
            return View();
        }

        /// <summary>
        /// Redirects to Map View. Passes all neccessary elements via ViewBag
        /// </summary>
        /// <param name="address"></param>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Map(string address, string date, string type) {
            //Passes dates dropdown
            ViewBag.dateDD = _cf.ReturnDateDd();
            //Passes crime categories dropdown
            ViewBag.typeDD = _cf.ReturnTypeDd();
            //Passes dates dropdown
            ViewBag.type = type;
            ViewBag.date = date;
            ViewBag.address = address;            
            List<Crime> crimes = service.Map(address, date, type);
            var latLng = service.ReturnLatLng(address);
            ViewBag.lat = float.Parse(latLng.First().Value);
            ViewBag.lng = float.Parse(latLng.ElementAt(1).Value);

            return View(crimes);
        }
    }
}