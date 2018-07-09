using PoliceUk;
using PoliceUk.Entities.StreetLevel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CrimeWatch.Services
{
    public class MapService
    {
        /// <summary>
        /// Receives user input, makes call to Police API and returns the appropriate crimes
        /// </summary>
        /// <param name="address"></param>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Crime> Map(string address, string date, string type)
        {
            //Call to ReturnLatLong method to get the coords
            var latLng = ReturnLatLng(address);
            //Parse the responce
            var crimesLocation = new Geoposition(double.Parse(latLng.First().Value), double.Parse(latLng.ElementAt(1).Value));
            DateTime crimesDate = DateTime.ParseExact(date, "MMMM yyyy", CultureInfo.InvariantCulture);
            //Initialize a Police client object.
            var policeClient = new PoliceUkClient();
            //Calls police API and retrieves the crimes
            var crimes = policeClient.StreetLevelCrimes(crimesLocation, crimesDate).Crimes.ToList();
            //If category is specified, filter the crimes
            if (!type.Equals("All crime") && !type.Equals(""))
            {
                var typeFormat2 = type.ToLower().Replace(" ", "-");
                crimes = crimes.Where(x => x.Category == typeFormat2).ToList();
            }
            //Format the category of each crime to be readable
            foreach (var crime in crimes)
            {
                crime.Category = crime.Category.ToUpper().First() + crime.Category.Remove(0, 1).Replace("-", " ");
                if (crime.OutcomeStatus is null)
                    crime.OutcomeStatus = new PoliceUk.Entities.OutcomeStatus { Date = "Unknown", Category = "Unknown" };
            }

            crimes = SumCrimeTypes(crimes);

            return crimes;
        }

        /// <summary>
        /// Receives an address or postcode and returns its lat long coordinates
        /// </summary>
        /// <param name="address">The receives postcide/address</param>
        /// <returns>XElement with the coordinates</returns>
        public List<XElement> ReturnLatLng(string address)
        {
            //Makes call to Google Geocode API
            var requestUri = ConstantStrings.ApiGoogleGeocode +
                "address=" + address.Replace(" ", "+") + "&key=AIzaSyD_rXEE2-a_KrNsOkLH0sxMr96NVZjaiTI";
            //Sends the request
            var request = WebRequest.Create(requestUri);
            //Get the result
            var responseAddr = request.GetResponse();
            var xdoc = XDocument.Load(responseAddr.GetResponseStream());
            //Interpret the result
            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            //Take only the lat long attributes
            var lat = locationElement.Element("lat");
            var lng = locationElement.Element("lng");
            return new List<XElement> { lat, lng };
        }

        public List<Crime> SumCrimeTypes(List<Crime> crimes)
        {
            foreach (var crime in crimes)
            {
                if (crimes.Where(x => x.Location.Latitude.Equals(crime.Location.Latitude) && x.Location.Longitude.Equals(crime.Location.Longitude)).ToList().Count > 1
                    && !crime.Category.Contains('('))
                {
                    var sameLatLngCrimes = crimes.Where(c => c.Location.Latitude == crime.Location.Latitude && c.Location.Longitude == crime.Location.Longitude).ToList();

                    var types = new List<string>();
                    foreach (var crime2 in sameLatLngCrimes)
                    {
                        types.Add(crime2.Category);
                    }
                    var typesDistinct = new List<string>();
                    foreach (var s in types.Distinct().ToList())
                    {
                        typesDistinct.Add(s);
                        typesDistinct.Add(sameLatLngCrimes.Where(x => x.Category.Equals(s)).Count().ToString());

                    }
                    var newType = "";
                    for (var j = 0; j < typesDistinct.Count; j += 2)
                    {
                        newType = newType + typesDistinct[j] + " (" + typesDistinct[j + 1] + "), ";
                    }
                    foreach (var c in sameLatLngCrimes)
                    {
                        c.Category = newType.Remove(newType.Length - 2);
                        c.OutcomeStatus.Category = "Multiple";
                    }
                }
            }
            return crimes;
        }

    }
}