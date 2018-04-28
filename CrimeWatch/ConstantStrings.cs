using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using CrimeWatch.Models;

namespace CrimeWatch
{
    public static class ConstantStrings
    {
        public const string API_POLICE = "https://data.police.uk/api/crimes-street/";
        public const string API_GOOGLE_GEOCODE = "https://maps.googleapis.com/maps/api/geocode/xml?";
        

        public struct EmailConstants
        {
            public const string websiteAddress = "christosathens@hotmail.gr";
            public const string websitePassword = "@then$-p@$$w0rd";
            public const string Host = "smtp.live.com";
        }

        public static IEnumerable<SelectListItem> Dates = new List<SelectListItem> {
            new SelectListItem{ Value="2017-03",Text="March 2017"},
            new SelectListItem{ Value="2017-04",Text="April 2017"},
            new SelectListItem{ Value="2017-05",Text="May 2017"},
            new SelectListItem{ Value="2017-06",Text="June 2017"},
            new SelectListItem{ Value="2017-07",Text="July 2017"},
            new SelectListItem{ Value="2017-08",Text="August 2017"},
            new SelectListItem{ Value="2017-09",Text="September 2017"},
            new SelectListItem{ Value="2017-10",Text="October 2017"},
            new SelectListItem{ Value="2017-11",Text="November 2017"},
            new SelectListItem{ Value="2017-12",Text="December 2017"},
            new SelectListItem{ Value="2018-01",Text="January 2018"},
            new SelectListItem{ Selected=true , Value="2018-02",Text="February 2018"}
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
        new SelectListItem{Selected=true, Value="",Text="All Types"},
        new SelectListItem{ Value="anti-social-behaviour",Text="Anti-social behaviour"},
        new SelectListItem{ Value="bicycle-theft",Text="Bicycle theft"},
        new SelectListItem{ Value="burglary",Text="Burglary"},
        new SelectListItem{ Value="criminal-damage-and-arson",Text="Criminal damage and arson"},
        new SelectListItem{ Value="drugs",Text="Drugs"},
        new SelectListItem{ Value="possession-of-weapons",Text="Possession of weapons"},
        new SelectListItem{ Value="other-crime",Text="Other crime"},
        new SelectListItem{ Value="other-theft",Text="Other theft"},
        new SelectListItem{ Value="public-order",Text="Public order"},
        new SelectListItem{ Value="robbery",Text="Robbery"},
        new SelectListItem{ Value="shoplifting",Text="Shoplifting"},
        new SelectListItem{ Value="theft-from-the-person",Text="Theft from the person"},
        new SelectListItem{ Value="vehicle-crime",Text="Vehicle crime"},
        new SelectListItem{ Value="violence-and-sexual-offences",Text="Violence and sexual offences"}
    };

        public static string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public static string[] crime_categories = { "Anti-social behaviour", "Bicycle theft", "Burglary", "Criminal damage and arson", "Drugs", "Possession of weapons", "Other crime", "Other theft", "Public order", "Robbery", "Shoplifting", "Theft from the person", "Vehicle crime", "Violence and sexual offences" };
        public static string[] regions = { "Avon and Somerset", "Bedfordshire", "Cambridgeshire", "Cheshire", "City of London", "Cleveland", "Cumbria", "Derbyshire", "Devon & Cornwall", "Dorset", "Durham", "Dyfed & Powys", "Essex", "Gloucestershire", "Greater Manchester", "Gwent","Hampshire", "Hertfordshire", "Humberside", "Kent", "Lancashire", "Leicestershire", "Lincolnshire", "Merseyside", "London", "Norfolk","North Wales", "North Yorkshire", "Northamptonshire", "Northumbria", "Nottinghamshire","Northern Ireland","South Wales","South Yorkshire", "Staffordshire", "Suffolk", "Surrey", "Sussex", "Thames Valley", "Warwickshire", "West Mercia", "West Midlands", "West Yorkshire", "Wiltshire" };
        public static string[] police_departments = { "Avon and Somerset Constabulary","Bedfordshire Police","Cambridgeshire Constabulary","Cheshire Constabulary","City of London Police","Cleveland Police","Cumbria Constabulary","Derbyshire Constabulary","Devon & Cornwall Police","Dorset Police","Durham Constabulary", "Dyfed-Powys Police", "Essex Police","Gloucestershire Constabulary","Greater Manchester Police","Gwent Police","Hampshire Constabulary" ,"Hertfordshire Constabulary","Humberside Police","Kent Police","Lancashire Constabulary","Leicestershire Police","Lincolnshire Police","Merseyside Police","Metropolitan Police Service","Norfolk Constabulary", "North Wales Police", "North Yorkshire Police","Northamptonshire Police","Northumbria Police","Nottinghamshire Police", "Police Service of Northern Ireland", "South Wales Police","South Yorkshire Police","Staffordshire Police","Suffolk Constabulary","Surrey Police","Sussex Police","Thames Valley Police","Warwickshire Police","West Mercia Police","West Midlands Police","West Yorkshire Police","Wiltshire Police" };
        public static DateTime firstCrimeDate = new DateTime();
        public static DateTime lastCrimeDate = new DateTime();
    }



}


