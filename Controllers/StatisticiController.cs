using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace PrototipConfidanceBuilder.Controllers
{
    public class StatisticiController : Controller
    {
        // GET: Statistici

        public ActionResult Index()
        {
          
            int[] nums = { 10, 15, 16, 8, 6 };
            return Json(new { Vect1 = nums }, JsonRequestBehavior.AllowGet);
        }
    }
}