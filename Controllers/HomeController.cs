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
       
        public ActionResult Index()
        {
            using (var context = new DatabaseContext())
            {
                int IdUtil = Utils.UtilizatorLogat();
                if(IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = context.Utilizatori.First(x => x.Id == IdUtil);

                //Get all the calculations again from the db (our new Calculation should be there)
                DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
                ViewModelIndex vmi = new ViewModelIndex(util, ddr.AddDays(-6), ddr, context);
                return View(vmi);

            }

        }

        public ActionResult Inapoi()
        {
            DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
           
            Session["dataDeRef"] = ddr.AddDays(-7);

            return RedirectToAction("Index");
        }

        public ActionResult Inainte()
        {
            DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
            
            Session["dataDeRef"] = ddr.AddDays(7);

            return RedirectToAction("Index");
        }


        public ActionResult GenRutine()
        {


            DateTime date = DateTime.Now;
            
            using (var context = new DatabaseContext())
            {


                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = context.Utilizatori.First(x => x.Id == IdUtil);

                var genrut = context.GeneratorRutina.Where(x => x.IdUtilizator == IdUtil);

                if (genrut.Count() < 1)
                {
                    return RedirectToAction("Index", "AdministrareRutina", new {mesaj ="Este necesar să adăugați măcar o acțiune în rutină" });
                }

                for (int i = 0; i < 60; i++)
                {
                    DateTime   data = date.AddDays(-i);
                    string strData = data.ToString("yyyy-MM-dd");
                    Rutina rut = new Rutina();
                    ParcursRutina pr = context.ParcursRutina.Where(x=>x.Rutina.IdUtilizator == util.Id).FirstOrDefault(x => x.Data == strData);
                    if (pr != null)
                    {
                        rut = pr.Rutina;
                        var ras = context.RutineActiuni.Where(x => x.IdRutina == rut.Id).ToList();
                        foreach (var ra in ras)
                        {
                            context.RutineActiuni.Remove(ra);
                        }
                    }
                    else
                    {
                        rut.IdUtilizator = util.Id;
                        context.Rutine.Add(rut);
                    }

                    foreach (var item in genrut)
                    {
                        RutinaActiune ra = new RutinaActiune();
                        ra.IdActiune = item.IdActiune;
                        ra.IdRutina = rut.Id;
                        ra.IdStare = 1;
                        ra.ActiuniCumulate = 0;
                        context.RutineActiuni.Add(ra);

                    }
                    ParcursRutina pa = new ParcursRutina();
                    pa.IdRutina = rut.Id;
                    pa.Data = strData;
                    context.ParcursRutina.Add(pa);

                    context.SaveChanges();
                    
                }
                return RedirectToAction("Index");
            }
         }

        public ActionResult SchimbaStareActiune(int IdRutinaActiune, int stare)
        {
            using( var db =  new DatabaseContext())
            {
                try
                   {
                    int IdUtil = Utils.UtilizatorLogat();
                    RutinaActiune ra = db.RutineActiuni.First(x => x.Id == IdRutinaActiune);
                    ra.IdStare = stare;
                    db.SaveChanges();
                    DateTime data = DateTime.ParseExact(ra.Rutina.ParcursRutina.First().Data.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Utils.ActualizareActiuniAcumulate(data, db, ra.IdActiune, IdUtil);
                    
                    }

                catch(Exception ex)
                {
                    return Json(new { mesaj = "a aparut o eroare" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { mesaj = "salvat" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}