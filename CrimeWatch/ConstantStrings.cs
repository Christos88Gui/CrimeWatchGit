using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CrimeWatch.Models;

namespace CrimeWatch
{
    public static class ConstantStrings
    {
        private static CrimeWatchDBEntities db = new CrimeWatchDBEntities();

        public struct EmailConstants
        {
            public const string websiteAddress = "christosathens@hotmail.gr";
            public const string websitePassword = "@then$-p@$$w0rd";
            public const string Host = "smtp.live.com";
        }

        public static IEnumerable<SelectListItem> Dates = new List<SelectListItem> {
            new SelectListItem{Selected=true, Value="Select Date",Text="Select Date"},
            new SelectListItem{ Value="All 2017",Text="All 2017"},
            new SelectListItem{ Value="All 2018",Text="All 2018"}
            //new SelectListItem{ Value="January 2017",Text="January 2017"},
            //new SelectListItem{ Value="February 2017",Text="February 2017"},
            //new SelectListItem{ Value="March 2017",Text="March 2017"},
            //new SelectListItem{ Value="April 2017",Text="April 2017"},
            //new SelectListItem{ Value="May 2017",Text="May 2017"},
            //new SelectListItem{ Value="June 2017",Text="June 2017"},
            //new SelectListItem{ Value="July 2017",Text="July 2017"},
            //new SelectListItem{ Value="August 2017",Text="August 2017"},
            //new SelectListItem{ Value="September 2017",Text="September 2017"},
            //new SelectListItem{ Value="October 2017",Text="October 2017"},
            //new SelectListItem{ Value="November 2017",Text="November 2017"},
            //new SelectListItem{ Value="December 2017",Text="December 2017"},
            //new SelectListItem{ Value="All 2017",Text="All 2017"},
            //new SelectListItem{ Value="January 2018",Text="January 2018"},
            //new SelectListItem{ Value="February 2018",Text="February 2018"},
            //new SelectListItem{ Value="All 2018",Text="All 2018"}
    };

    public static IEnumerable<SelectListItem> Police_Departments = new List<SelectListItem> {
        new SelectListItem{Selected=true, Value="All Police Departments",Text="All Police Departments"},
        new SelectListItem{ Value="Avon and Somerset Constabulary",Text="Avon and Somerset Constabulary"},
        new SelectListItem{ Value="Bedfordshire Police",Text="Bedfordshire Police"},
        new SelectListItem{ Value="British Transport Police",Text="British Transport Police"},
        new SelectListItem{ Value="Cambridgeshire Constabulary",Text="Cambridgeshire Constabulary"},
        new SelectListItem{ Value="Cheshire Constabulary",Text="Cheshire Constabulary"},
        new SelectListItem{ Value="City of London Police",Text="City of London Police"},
        new SelectListItem{ Value="Cleveland Police",Text="Cleveland Police"},
        new SelectListItem{ Value="Cumbria Constabulary",Text="Cumbria Constabulary"},
        new SelectListItem{ Value="Derbyshire Constabulary",Text="Derbyshire Constabulary"},
        new SelectListItem{ Value="Devon & Cornwall Police",Text="Devon & Cornwall Police"},
        new SelectListItem{ Value="Dorset Police",Text="Dorset Police"},
        new SelectListItem{ Value="Durham Constabulary",Text="Durham Constabulary"},
        new SelectListItem{ Value="Dyfed-Powys Police",Text="Dyfed-Powys Police"},
        new SelectListItem{ Value="Gloucestershire Constabulary",Text="Gloucestershire Constabulary"},
        new SelectListItem{ Value="Greater Manchester Police",Text="Greater Manchester Police"},
        new SelectListItem{ Value="Gwent Police",Text="Gwent Police"},
        new SelectListItem{ Value="Hampshire Constabulary",Text="Hampshire Constabulary"},
        new SelectListItem{ Value="Hertfordshire Constabulary",Text="Hertfordshire Constabulary"},
        new SelectListItem{ Value="Humberside Police",Text="Humberside Police"},
        new SelectListItem{ Value="Kent Police",Text="Kent Police"},
        new SelectListItem{ Value="Lancashire Constabulary",Text="Lancashire Constabulary"},
        new SelectListItem{ Value="Leicestershire Police",Text="Leicestershire Police"},
        new SelectListItem{ Value="Lincolnshire Police",Text="Lincolnshire Police"},
        new SelectListItem{ Value="Merseyside Police",Text="Merseyside Police"},
        new SelectListItem{ Value="Metropolitan Police Service",Text="Metropolitan Police Service"},
        new SelectListItem{ Value="Norfolk Constabulary",Text="Norfolk Constabulary"},
        new SelectListItem{ Value="North Wales Police",Text="North Wales Police"},
        new SelectListItem{ Value="North Yorkshire Police",Text="North Yorkshire Police"},
        new SelectListItem{ Value="Northamptonshire Police",Text="Northamptonshire Police"},
        new SelectListItem{ Value="Northumbria Police",Text="Northumbria Police"},
        new SelectListItem{ Value="Nottinghamshire Police",Text="Nottinghamshire Police"},
        new SelectListItem{ Value="South Wales Police",Text="South Wales Police"},
        new SelectListItem{ Value="South Yorkshire Police",Text="South Yorkshire Police"},
        new SelectListItem{ Value="Staffordshire Police",Text="Staffordshire Police"},
        new SelectListItem{ Value="Suffolk Police",Text="Suffolk Police"},
        new SelectListItem{ Value="Surrey Police",Text="Surrey Police"},
        new SelectListItem{ Value="Sussex Police",Text="Sussex Police"},
        new SelectListItem{ Value="Thames Valley Police",Text="Thames Valley Police"},
        new SelectListItem{ Value="Warwickshire Police",Text="Warwickshire Police"},
        new SelectListItem{ Value="West Mercia Police",Text="West Mercia Police"},
        new SelectListItem{ Value="West Midlands Police",Text="West Midlands Police"},
        new SelectListItem{ Value="West Yorkshire Police",Text="West Yorkshire Police"},
        new SelectListItem{ Value="Wiltshire Police",Text="Wiltshire Police"}
    };
        public static IEnumerable<SelectListItem> Crime_Types = new List<SelectListItem> {
        new SelectListItem{Selected=true, Value="All Categories",Text="All Categories (Optional)"},
        new SelectListItem{ Value="Anti-social behaviour",Text="Anti-social behaviour"},
        new SelectListItem{ Value="Bicycle theft",Text="Bicycle theft"},
        new SelectListItem{ Value="Burglary",Text="Burglary"},
        new SelectListItem{ Value="Criminal damage and arson",Text="Criminal damage and arson"},
        new SelectListItem{ Value="Drugs",Text="Drugs"},
        new SelectListItem{ Value="Possession of weapons",Text="Possession of weapons"},
        new SelectListItem{ Value="Other crime",Text="Other crime"},
        new SelectListItem{ Value="Other theft",Text="Other theft"},
        new SelectListItem{ Value="Public order",Text="Public order"},
        new SelectListItem{ Value="Robbery",Text="Robbery"},
        new SelectListItem{ Value="Shoplifting",Text="Shoplifting"},
        new SelectListItem{ Value="Theft from the person",Text="Theft from the person"},
        new SelectListItem{ Value="Vehicle crime",Text="Vehicle crime"},
        new SelectListItem{ Value="Violence and sexual offences",Text="Violence and sexual offences"}
    };

        public static string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public static string[] Crime_Categories = { "Anti-social behaviour", "Bicycle theft", "Burglary", "Criminal damage and arson", "Drugs", "Possession of weapons", "Other crime", "Other theft", "Public order", "Robbery", "Shoplifting", "Theft from the person", "Vehicle crime", "Violence and sexual offences" };


        public static String[] police_departments = new string[] { "Avon and Somerset Constabulary", "Bedfordshire Police","Cambridgeshire Constabulary","Cheshire Constabulary","City of London Police","Cleveland Police","Cumbria Constabulary","Derbyshire Constabulary","Devon & Cornwall Police","Dorset Police","Durham Constabulary","Essex Police","Gloucestershire Constabulary","Greater Manchester Police","Hampshire Constabulary" ,"Hertfordshire Constabulary","Humberside Police","Kent Police","Lancashire Constabulary","Leicestershire Police","Lincolnshire Police","Merseyside Police","Metropolitan Police Service","Norfolk Constabulary","North Yorkshire Police","Northamptonshire Police","Northumbria Police","Nottinghamshire Police","South Yorkshire Police","Staffordshire Police","Suffolk Constabulary","Surrey Police","Sussex Police","Thames Valley Police","Warwickshire Police","West Mercia Police","West Midlands Police","West Yorkshire Police","Wiltshire Police" };
    }

}


