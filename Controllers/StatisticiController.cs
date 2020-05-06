using PrototipConfidenceBuilder.DataAccess;
using PrototipConfidenceBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace PrototipConfidenceBuilder.Controllers
{
    public class StatisticiController : Controller
    {

        // GET: Statistici

        public ActionResult Chart(int idActiune)
        {
            DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
            int pasZile = (int)HttpContext.Session["pasZile"];

            using (var context = new DatabaseContext())
            {

                //Get all the calculations again from the db (our new Calculation should be there)
             
                ViewModelIndex vmi = new ViewModelIndex(1, ddr.AddDays(-pasZile+1), ddr, context);
                var listaZile = vmi.Status7Z.Select(x => x.ListaZile).SelectMany(x=>x).ToList();
                var listaDate = vmi.ListaIdSiDataRutina.Select(x => x.Item3).ToList();
               ProgresPeActiunePeZile progresActiunePeZile = new ProgresPeActiunePeZile(listaZile, listaDate, idActiune);
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
            int pasZile = (int)HttpContext.Session["pasZile"];
            using (DatabaseContext db =  new DatabaseContext())
            {
                ParcursRutina pr = db.ParcursRutina.First(x => Convert.ToDateTime(x.Data) == dinData && x.Rutina.IdUtilizator == idutilizator);
                ProgresActiuni pa = new ProgresActiuni(pr,dinData.AddDays(-pasZile +1),1,db);
                return View ("Index", pa);

            }
        
        }


        public ActionResult Index()
        {
            int idutilizator = 1;
            DateTime dinData = (DateTime)HttpContext.Session["dataDeRef"];
            int pasZile = (int)HttpContext.Session["pasZile"];
            string dinDataStr = dinData.ToString("yyyy-MM-dd");
            using (DatabaseContext db = new DatabaseContext())
            {
                ParcursRutina pr = db.ParcursRutina.First(x => x.Data.Trim() == dinDataStr && x.Rutina.IdUtilizator == idutilizator);
                ProgresActiuni pa = new ProgresActiuni(pr, dinData.AddDays(-pasZile + 1),1, db);
                return View("Index", pa);

            }

        }

        public ActionResult ProgresPe7Zile()
        {

            using (DatabaseContext db = new DatabaseContext())
            {
                int nrTotalZile = db.ParcursRutina.Count();
                int actualizare = nrTotalZile > 7 ? 7 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }
        }
        public ActionResult ProgresPe30Zile()
        {

            using (DatabaseContext db = new DatabaseContext())
            {
                int nrTotalZile = db.ParcursRutina.Count();
                int actualizare = nrTotalZile > 30 ? 30 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }
        }

        public ActionResult ProgresPe90Zile()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int nrTotalZile = db.ParcursRutina.Count();
                int actualizare = nrTotalZile > 90 ? 90 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }

            
        }

        public ActionResult ProgresPe6Luni()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int nrTotalZile = db.ParcursRutina.Count();
                int actualizare = nrTotalZile > 180 ? 180 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }


        }

        public ActionResult ProgresComplet()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int nrTotalZile = db.ParcursRutina.Count();
                System.Web.HttpContext.Current.Session["pasZile"] = nrTotalZile;
                return RedirectToAction("Index");

            }


        }
    }
}