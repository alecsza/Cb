using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PrototipConfidenceBuilder.DataAccess;
using PrototipConfidenceBuilder.Models;

namespace PrototipConfidenceBuilder.Controllers
{

    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        public ActionResult Index()
        {
           
                int IdUtil = Utils.UtilizatorLogat();
                if(IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

                //Get all the calculations again from the db (our new Calculation should be there)
                DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
                ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, MemoryDB.GetZile());
                return View(vmi);

        }
        public ActionResult PartialIndex()
        {

            int IdUtil = Utils.UtilizatorLogat();
            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            //Get all the calculations again from the db (our new Calculation should be there)
            DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr,MemoryDB.GetZile());
            return PartialView("_MainContent", vmi);

        }

        public ActionResult Inapoi()
        {
            DateTime ddr1 = (DateTime)HttpContext.Session["dataDeRef"];

         

            Session["dataDeRef"] = ddr1.AddDays(-7);

            // return RedirectToAction("Index");

            int IdUtil = Utils.UtilizatorLogat();
            var ras = new List<Zi>();


            ras = MemoryDB.AddZileToMemoryAsync(db, ddr1.AddDays(-7).DayOfYear, IdUtil).Result;
            MemoryDB.AddZile(ras);
            Session["ZiAn_s"] = ddr1.AddDays(-21);

            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            //Get all the calculations again from the db (our new Calculation should be there)
            DateTime ddr = ddr1.AddDays(-7);
         
            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, MemoryDB.GetZile());
            return PartialView("_MainContent",vmi);
        }

        public ActionResult Inainte()
        {
            DateTime ddr1 = (DateTime)HttpContext.Session["dataDeRef"];
            
            Session["dataDeRef"] = ddr1.AddDays(7);

            Session["ZiAn_s"] = ddr1.AddDays(7).DayOfYear;

            //return RedirectToAction("Index");


            int IdUtil = Utils.UtilizatorLogat();
            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            //Get all the calculations again from the db (our new Calculation should be there)
            DateTime ddr = ddr1.AddDays(7);
            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, MemoryDB.GetZile());
            return PartialView("_MainContent", vmi);
        }


        public ActionResult GenRutine()
        {

            DateTime date = DateTime.Now.AddDays(-60);


            Stare st = db.Stari.First(x => x.Id == 1);

            int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

                var genrut = db.GeneratorRutina.Where(x => x.IdUtilizator == IdUtil);

                if (genrut.Count() < 1)
                {
                    return RedirectToAction("PartialIndex", "AdministrareRutina", new {mesaj ="Este necesar să adăugați măcar o acțiune în rutină" });
                }

                for (int i = 53; i <= 59; i++)
                {
                    DateTime data = date.AddDays(i);
                    Utils.GenRutina(data, util, db, genrut.ToList(), st);
              
                }
            Session["ZiAn_s"] = date.AddDays(53).DayOfYear;
            ParcursRutina pa = Utils.GenRutina(date.AddDays(60), util, db, genrut.ToList(), st);
            util.UltimParcursRutina = pa;
            db.SaveChanges();
            var ras = new List<Zi>();

          
              ras =  Utils.GenRutine(date, util, db, genrut.ToList(), st).Result;

              
            
            
            Session["ZiAn_s"] = date.DayOfYear;
            MemoryDB.AddZile(ras);




            var zile = MemoryDB.GetZile();

            DateTime ddr = DateTime.Now.Date;

            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, zile);

            return PartialView("_MainContent", vmi);

        }

        public ActionResult SchimbaStareActiune(int IdRutinaActiune, int stare)
        {
           
                try
                   {
                  Utils.ActualizareActiuniAcumulate(db, stare, IdRutinaActiune);
                    
                    }

                catch(Exception ex)
                {
                    return Json(new { mesaj = "a aparut o eroare" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { mesaj = "Starea acțiunii a fost modoficată cu succes" }, JsonRequestBehavior.AllowGet);
            }
        
    }
}