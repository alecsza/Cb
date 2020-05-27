﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PrototipConfidenceBuilder.Models;

namespace PrototipConfidenceBuilder
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Session_Start(Object sender, EventArgs e)
        {
            DateTime dataDeReferinta = DateTime.Now;
            int nrZile = 6;
            HttpContext.Current.Session.Add("dataDeRef", dataDeReferinta);
            HttpContext.Current.Session.Add("pasZile", nrZile);
            HttpContext.Current.Session.Add("IdUtilizator", 0);
            HttpContext.Current.Session.Add("An_s", dataDeReferinta.Year);
            HttpContext.Current.Session.Add("ZiAn_s", dataDeReferinta.DayOfYear);
            MemoryDB.InitMem();
        }
    }
}
