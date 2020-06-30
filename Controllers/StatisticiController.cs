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
        DatabaseContext db = MemoryDB.db;

        public ActionResult Chart(int idActiune)
        {
            DateTime ddr = DateTime.Now.Date;
            int pasZile = (int)HttpContext.Session["pasZile"];

          
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);


                //Get all the calculations again from the db (our new Calculation should be there)

                ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-pasZile), ddr, MemoryDB.GetZile());
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

        public ActionResult ProcentrealizatDinRutina()
        {
            DateTime dinData = DateTime.Now;
            int pasZile = (int)HttpContext.Session["pasZile"];
           
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);
                List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
                ParcursRutina pr = db.ParcursRutina.First(x => Convert.ToDateTime(x.Data) == dinData && x.Rutina.IdUtilizator == util.Id);
                ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-pasZile ),util,MemoryDB.GetZile(), lgr);
                return PartialView("_MainContent", pa);



        }

        public ActionResult PartialIndex()
        {

            DateTime dinData = DateTime.Now;
            int pasZile = (int)HttpContext.Session["pasZile"];
            string dinDataStr = dinData.ToString("yyyy-MM-dd");

            int IdUtil = Utils.UtilizatorLogat();
            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            if (util.UltimParcursRutina == null)
            {
                ParcursRutina u_pr = null;
                List<ParcursRutina> lpr = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == util.Id).ToList();
                if (lpr.Count() > 0)
                {
                    u_pr = lpr.First(x => x.Data == dinDataStr);

                }
                util.UltimParcursRutina = u_pr;
                db.SaveChanges();
            }
            ParcursRutina pr = util.UltimParcursRutina;

            List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
            ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-pasZile), util, MemoryDB.GetZile(), lgr);
            return PartialView("_MainContent", pa);

        }

        public ActionResult Index()
        {

            DateTime dinData = DateTime.Now;
            int pasZile = (int)HttpContext.Session["pasZile"];
            string dinDataStr = dinData.ToString("yyyy-MM-dd");
            
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);
            
                ParcursRutina pr = util.UltimParcursRutina;

            List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
            ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-pasZile ),util, MemoryDB.GetZile(), lgr);
                return View("Index", pa);

        }

        public ActionResult ProgresPe7Zile()
        {

            using (DatabaseContext db = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                int nrTotalZile = MemoryDB.GetZile().Where(x => x.Idutilizator == IdUtil).Select(x => x.IdParcursRutina).Distinct().Count();
                int actualizare = nrTotalZile > 6 ? 7 : nrTotalZile;
                System.Web.HttpContext.Current.Session["pasZile"] = actualizare;

                DateTime dinData = DateTime.Now;
               
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

                ParcursRutina pr = util.UltimParcursRutina;

                List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
                ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-actualizare), util, MemoryDB.GetZile(),lgr);
                return PartialView("_MainContent", pa);


            }
        }
        public ActionResult ProgresPe30Zile()
        {

           int IdUtil = Utils.UtilizatorLogat();
                
            DateTime dinData = DateTime.Now;
         
          
              

            int pasZile = (int)HttpContext.Session["pasZile"];
            int zi_an_s = (int)HttpContext.Session["ZiAn_s"];
            int nrTotalZileDeAdaugat =zi_an_s - dinData.AddDays(-30).DayOfYear ;
         
            var zile = MemoryDB.GetZile().ToList();
            if (nrTotalZileDeAdaugat > 0)
            {
                List<Zi> lz = new List<Zi>();
                lz = MemoryDB.AddZileToMemorySync(db, dinData.AddDays(-30).DayOfYear, IdUtil, nrTotalZileDeAdaugat);
                zile.AddRange(lz);

            }
            int nrTotalZile = zile.Where(x => x.Idutilizator == IdUtil).Select(x => x.IdParcursRutina).Distinct().Count();
            int actualizare = nrTotalZile > 30 ? 30 : nrTotalZile;
            System.Web.HttpContext.Current.Session["pasZile"] = actualizare;

            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            ParcursRutina pr = util.UltimParcursRutina;

            List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
            ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-actualizare), util,new HashSet<Zi>( zile), lgr);
            return PartialView("_MainContent", pa);


        }

        public ActionResult ProgresPe90Zile()
        {
            
                int IdUtil = Utils.UtilizatorLogat();
                DateTime dinData = DateTime.Now;


            int pasZile = (int)HttpContext.Session["pasZile"];
            int zi_an_s = (int)HttpContext.Session["ZiAn_s"];
            int nrTotalZileDeAdaugat = zi_an_s- dinData.AddDays(-90).DayOfYear ;
            var zile = MemoryDB.GetZile().ToList();

            if (nrTotalZileDeAdaugat > 0)
            {
                List<Zi> lz = new List<Zi>();
                lz = MemoryDB.AddZileToMemorySync(db, dinData.AddDays(-90).DayOfYear, IdUtil, nrTotalZileDeAdaugat);
            
                zile.AddRange(lz);
            }
            int nrTotalZile = zile.Where(x => x.Idutilizator == IdUtil).Select(x => x.IdParcursRutina).Distinct().Count();
           
            int actualizare = nrTotalZile > 90 ? 90 : nrTotalZile;
            System.Web.HttpContext.Current.Session["pasZile"] = actualizare;



            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            //if (util.UltimParcursRutina == null)
            //{
            //    ParcursRutina u_pr = null;
            //    List<ParcursRutina> lpr = db.ParcursRutina.Where(x => x.Rutina.IdUtilizator == util.Id).ToList();
            //    if (lpr.Count() > 0)
            //    {
            //        u_pr = lpr.First(x => x.Data == dinDataStr);

            //    }
            //    util.UltimParcursRutina = u_pr;
            //    db.SaveChanges();
            //}
            ParcursRutina pr = util.UltimParcursRutina;

            List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
            ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-actualizare), util, new HashSet<Zi>( zile),lgr);
            return PartialView("_MainContent", pa);



        }

        public ActionResult ProgresPe6Luni()
        {
            
            int IdUtil = Utils.UtilizatorLogat();
            DateTime dinData = DateTime.Now;
            string dinDataStr = dinData.ToString("yyyy-MM-dd");
           int pasZile = (int)HttpContext.Session["pasZile"];
           int zi_an_s = (int)HttpContext.Session["ZiAn_s"];
           int nrTotalZileDeAdaugat  = zi_an_s- dinData.AddDays(-180).DayOfYear  ;
            var zile = MemoryDB.GetZile().ToList();

            if (nrTotalZileDeAdaugat > 0)
            {
                List<Zi> lz = new List<Zi>();
                lz =   MemoryDB.AddZileToMemorySync(db, dinData.AddDays(-180).DayOfYear, IdUtil, nrTotalZileDeAdaugat);
            
                zile.ToList().AddRange(lz);
            }
            int nrTotalZile = zile.Where(x => x.Idutilizator == IdUtil).Select(x => x.IdParcursRutina).Distinct().Count();


            int actualizare = nrTotalZile > 180 ? 180 : nrTotalZile;
            System.Web.HttpContext.Current.Session["pasZile"] = actualizare;
           
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            ParcursRutina pr = util.UltimParcursRutina;

            List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
            ProgresActiuni pa = new ProgresActiuni( dinData.AddDays(-actualizare), util,new HashSet<Zi>( zile),lgr);
            return PartialView("_MainContent", pa);



        }

        public ActionResult ProgresComplet()
        {
            
                int IdUtil = Utils.UtilizatorLogat();

            DateTime dinData = DateTime.Now;
            string dinDataStr = dinData.ToString("yyyy-MM-dd");

            int pasZile = (int)HttpContext.Session["pasZile"];
            int zi_an_s = (int)HttpContext.Session["ZiAn_s"];
            int nrTotalZileDeAdaugat = zi_an_s- dinData.AddDays(-pasZile).DayOfYear ;

            
            var zile = MemoryDB.GetZile().ToList();

            if (nrTotalZileDeAdaugat > 0)
            {
                List<Zi> lz = new List<Zi>();
                lz =   MemoryDB.AddZileToMemorySync(db, dinData.AddDays(-180).DayOfYear, IdUtil, nrTotalZileDeAdaugat);
                zile.AddRange(lz);
            }
            int nrTotalZile = zile.Where(x=>x.Idutilizator== IdUtil).Select(x => x.IdParcursRutina).Distinct().Count(); 
            System.Web.HttpContext.Current.Session["pasZile"] = nrTotalZile;

            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

         
            ParcursRutina pr = util.UltimParcursRutina;

            List<GeneratorRutina> lgr = db.GeneratorRutina.Where(x => x.IdUtilizator == util.Id).ToList();
            ProgresActiuni pa = new ProgresActiuni(dinData.AddDays(-nrTotalZile), util,new HashSet<Zi>( zile), lgr);
            return PartialView("_MainContent", pa); ;



        }
    }
}