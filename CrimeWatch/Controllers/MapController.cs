using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CrimeWatch.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult Index()
        {
            ViewBag.CurrentAddress = "";
            return View();
        }
        
        [HttpPost]
        public ActionResult FindLocation(String postcode) {
            
            string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?postcode={0}&sensor=false", Uri.EscapeDataString(postcode));
            
            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());

            XElement result = xdoc.Element("GeocodeResponse").Element("result");
            XElement locationElement = result.Element("geometry").Element("location");
            XElement lat = locationElement.Element("lat");
            XElement lng = locationElement.Element("lng");

            ViewBag.latitude = (int)lat;
            ViewBag.longitude = (int)lng;

            return View("Map");
        }
    }
}