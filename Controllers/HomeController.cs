using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototipConfidanceBuilder.DataAccess;
using PrototipConfidanceBuilder.Models;

namespace PrototipConfidanceBuilder.Controllers
{

    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            using (var context = new DatabaseContext())
            {
           

                //Get all the calculations again from the db (our new Calculation should be there)
                DateTime ddr = (DateTime)HttpContext.Session["dataDeRef"];
                ViewModelIndex vmi = new ViewModelIndex(1, ddr.AddDays(-6), ddr, context);
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
                var genrut = context.GeneratoRutina.Where(x => x.IdUtilizator == 1);

                for (int i = 0; i < 60; i++)
                {
                    DateTime   data = date.AddDays(-i);
                    string strData = data.ToString("yyyy-MM-dd");
                    Rutina rut = new Rutina();
                    ParcursRutina pr = context.ParcursRutina.FirstOrDefault(x => x.Data == strData);
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
                        rut.IdUtilizator = genrut.First().IdUtilizator;
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
                    RutinaActiune ra = db.RutineActiuni.First(x => x.Id == IdRutinaActiune);
                    ra.IdStare = stare;
                    db.SaveChanges();
                    DateTime data = DateTime.ParseExact(ra.Rutina.ParcursRutina.First().Data.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    Utils.ActualizareActiuniAcumulate(data, db, ra.IdActiune, 1);
                    
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