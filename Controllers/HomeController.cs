using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, MemoryDB.Zile);
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
            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr,MemoryDB.Zile);
            return PartialView("_MainContent", vmi);



        }

        public ActionResult Inapoi()
        {
            DateTime ddr1 = (DateTime)HttpContext.Session["dataDeRef"];
           
            Session["dataDeRef"] = ddr1.AddDays(-7);

            // return RedirectToAction("Index");

            int IdUtil = Utils.UtilizatorLogat();
            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            //Get all the calculations again from the db (our new Calculation should be there)
            DateTime ddr = ddr1.AddDays(-7);
            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, MemoryDB.Zile);
            return PartialView("_MainContent",vmi);
        }

        public ActionResult Inainte()
        {
            DateTime ddr1 = (DateTime)HttpContext.Session["dataDeRef"];
            
            Session["dataDeRef"] = ddr1.AddDays(7);

            //return RedirectToAction("Index");


            int IdUtil = Utils.UtilizatorLogat();
            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);

            //Get all the calculations again from the db (our new Calculation should be there)
            DateTime ddr = ddr1.AddDays(7);
            ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, MemoryDB.Zile);
            return PartialView("_MainContent", vmi);
        }


        public ActionResult GenRutine()
        {


            DateTime date = DateTime.Now;


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

                for (int i = 0; i < 60; i++)
                {
                    DateTime   data = date.AddDays(-i);
                    string strData = data.ToString("yyyy-MM-dd");
                    Rutina rut = new Rutina();
                    ParcursRutina pr = db.ParcursRutina.Where(x=>x.Rutina.IdUtilizator == util.Id).FirstOrDefault(x => x.Data == strData);
                    if (pr != null)
                    {
                        rut = pr.Rutina;
                        var ras = db.RutineActiuni.Where(x => x.IdRutina == rut.Id).ToList();
                        foreach (var ra in ras)
                        {
                            db.RutineActiuni.Remove(ra);
                        }
                    }
                    else
                    {
                        rut.IdUtilizator = util.Id;
                        db.Rutine.Add(rut);
                    }
                ParcursRutina pa = new ParcursRutina();
                pa.IdRutina = rut.Id;
                pa.Data = strData;
                db.ParcursRutina.Add(pa);
            
                foreach (var item in genrut)
                    {
                        RutinaActiune ra = new RutinaActiune();
                        ra.IdActiune = item.IdActiune;
                        ra.IdRutina = rut.Id;
                        ra.IdStare = 1;
                        ra.Stare = st;
                        ra.Rutina = rut;
                        ra.Actiune = item.Actiune;
                        ra.ActiuniCumulate = 0;
                        db.RutineActiuni.Add(ra);
                        db.SaveChanges();
                        MemoryDB.Zile.Add(new Zi(ra));

                    }
                  

                    
                }
            var zile = MemoryDB.Zile;
           
            ViewModelIndex vmi = new ViewModelIndex(util, date.AddDays(-6), date, zile);

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

                return Json(new { mesaj = "Starea acțiunii a fost modificată cu succes" }, JsonRequestBehavior.AllowGet);
            }
        
    }
}