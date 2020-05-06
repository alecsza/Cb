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
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = context.Utilizatori.First(x => x.Id == IdUtil);

                //Get all the calculations again from the db (our new Calculation should be there)

                ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-pasZile), ddr, context);
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
            DateTime panaLaData = DateTime.Now;
            int pasZile = (int)HttpContext.Session["pasZile"];
            using (DatabaseContext db =  new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);
                ParcursRutina pr = db.ParcursRutina.First(x => Convert.ToDateTime(x.Data) == panaLaData && x.Rutina.IdUtilizator == util.Id);
                ProgresActiuni pa = new ProgresActiuni(pr,panaLaData.AddDays(-pasZile ),util,db);
                return View ("Index", pa);

            }
        
        }


        public ActionResult Index()
        {

            DateTime panaLaData = DateTime.Now;
            int pasZile = (int)HttpContext.Session["pasZile"];
            string panaLaDataStr = panaLaData.ToString("yyyy-MM-dd");
            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);
              
                if(util.UltimParcursRutina == null)
                {
                    ParcursRutina u_pr = null;
                    List<ParcursRutina> lpr = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == util.Id).ToList();
                    if (lpr.Count() > 0)
                    {
                        u_pr = lpr.First(x => x.Data == panaLaDataStr);

                    }
                    util.UltimParcursRutina = u_pr;
                    db.SaveChanges();
                }
                ParcursRutina pr = util.UltimParcursRutina;
            ProgresActiuni pa = new ProgresActiuni(pr, panaLaData.AddDays(-pasZile ),util, db);
                return View("Index", pa);

            }


        }

        public ActionResult ProgresPe7Zile()
        {

            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                int nrTotalZile = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == IdUtil).Count();
                int actualizare = nrTotalZile > 6 ? 6 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }
        }
        public ActionResult ProgresPe30Zile()
        {

            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                int nrTotalZile = db.ParcursRutina.Where(x=>x.Rutina.IdUtilizator == IdUtil).Count();
                int actualizare = nrTotalZile > 30 ? 30 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }
        }

        public ActionResult ProgresPe90Zile()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                int nrTotalZile = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == IdUtil).Count();
                int actualizare = nrTotalZile > 90 ? 90 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }

        }

        public ActionResult ProgresPe6Luni()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                int nrTotalZile = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == IdUtil).Count();
                int actualizare = nrTotalZile > 180 ? 180 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
                return RedirectToAction("Index");

            }

        }

        public ActionResult ProgresComplet()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                int nrTotalZile = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == IdUtil).Count();
                System.Web.HttpContext.Current.Session["pasZile"] = nrTotalZile;
                return RedirectToAction("Index");

            }


        }
    }
}