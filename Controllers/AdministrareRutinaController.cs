using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototipConfidanceBuilder.DataAccess;
using PrototipConfidanceBuilder.Models;

namespace PrototipConfidanceBuilder.Controllers
{
    public class AdministrareRutinaController : Controller
    {
        // GET: AdministrareRutina
        public ActionResult Index()
        {
            int IdUtilizator = Utils.UtilizatorLogat();
            using (var db = new DatabaseContext())
            {

                return View();
            }
           
        }
    }
}