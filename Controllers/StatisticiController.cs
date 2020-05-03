using PrototipConfidanceBuilder.DataAccess;
using PrototipConfidanceBuilder.Models;
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

        public ActionResult Chart()
        {
            DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
            int pasZile = (int)HttpContext.Session["pasZile"];

            using (var context = new DatabaseContext())
            {


                //Get all the calculations again from the db (our new Calculation should be there)
             
                ViewModelIndex vmi = new ViewModelIndex(1, ddr.AddDays(-pasZile+1), ddr, context);
                var listaZile = vmi.Status7Z.Select(x => x.ListaZile).SelectMany(x=>x).ToList();
                var listaDate = vmi.ListaIdSiDataRutina.Select(x => x.Item3).ToList();
                var listaIdActiuni = vmi.Status7Z.Select(x => x.ListaZile.Select(y => y.IdActiune)).SelectMany(x => x).Distinct().ToList();
                ProgresPeActiunePeZile progresActiunePeZile = new ProgresPeActiunePeZile(listaZile, listaDate, listaIdActiuni);
                var primaActiune = progresActiunePeZile.ListaCuActiuniRepartizate.First();
                var vectorOx = primaActiune.ListaOx.ToArray();
                var vectorValori = primaActiune.Valori.ToArray();
                string denumireActiune = primaActiune.Denumire;
                int[] nums = {10, 15, 16, 8, 6};
                // fisierul si linia la care se gasejta javascriptul care se ocupa cu procesarea vectorilor
                //D:\Repos\Cb\Content\js\demo\chart-area-demo.js
                // linia 33, pe acolo
                return Json(new { vOx = vectorOx, vValori = vectorValori, denActiune = denumireActiune }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult ProcentrealizatDinRutina()
        {
            int idutilizator = 1;
            DateTime dinData = (DateTime)HttpContext.Session["dataDeRef"];
            using (DatabaseContext db =  new DatabaseContext())
            {
                ParcursRutina pr = db.ParcursRutina.First(x => Convert.ToDateTime(x.Data) == dinData && x.Rutina.IdUtilizator == idutilizator);
                ProgresActiuni pa = new ProgresActiuni(pr,db);
                return View ("Index", pa);

            }
        

        }


        public ActionResult Index()
        {
            int idutilizator = 1;
            DateTime dinData = (DateTime)HttpContext.Session["dataDeRef"];
            string dinDataStr = dinData.ToString("yyyy-MM-dd");
            using (DatabaseContext db = new DatabaseContext())
            {
                ParcursRutina pr = db.ParcursRutina.First(x => x.Data.Trim() == dinDataStr && x.Rutina.IdUtilizator == idutilizator);
                ProgresActiuni pa = new ProgresActiuni(pr, db);
                return View("Index", pa);

            }

        }
    }
}