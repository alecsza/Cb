using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototipConfidenceBuilder.DataAccess;
using PrototipConfidenceBuilder.Models;

namespace PrototipConfidenceBuilder.Controllers
{
    public class AdministrareRutinaController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: AdministrareRutina
        public ActionResult Index(string mesaj = "")
        {
            int IdUtilizator = Utils.UtilizatorLogat();

          
                int IdUtil = Utils.UtilizatorLogat();
                if (IdUtil == 0)
                {
                    return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
                }
                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);
                List<GeneratorRutina> LGR = db.GeneratorRutina.Where(x => x.IdUtilizator == IdUtilizator).ToList();
                ViewModelAdminGeneratorRutine vm = new ViewModelAdminGeneratorRutine(LGR, util, mesaj);

                return View("Index",vm);
            
           
        }


        public ActionResult PartialIndex(string mesaj = "")
        {
            int IdUtilizator = Utils.UtilizatorLogat();

            int IdUtil = Utils.UtilizatorLogat();
            if (IdUtil == 0)
            {
                return RedirectToAction("Index", "Autentificare", (object)"Este necesar sa vă autentificați");
            }
            Utilizator util = db.Utilizatori.First(x => x.Id == IdUtil);
            List<GeneratorRutina> LGR = db.GeneratorRutina.Where(x => x.IdUtilizator == IdUtilizator).ToList();
            ViewModelAdminGeneratorRutine vm = new ViewModelAdminGeneratorRutine(LGR, util, mesaj);

            return PartialView("_MainContent", vm);


        }



        public ActionResult SchimbaStare(  int idGRA, int stare)
        {
            int IdUtilizator = Utils.UtilizatorLogat();
            try
            {
              

                    GeneratorRutina generatorRutina = db.GeneratorRutina.First(x => x.Id == idGRA);
                    generatorRutina.Activ = stare;
                    db.SaveChanges();

                    return Json(new { mesaj = "Starea acțiunii a fost midificată cu succes" }, JsonRequestBehavior.AllowGet);
                }
            
            catch (Exception ex)
            {
                return Json(new { mesaj = "a aparut o eroare" }, JsonRequestBehavior.AllowGet);
            }

          }
        


        public ActionResult AdaugaActiune(string denumire)
        {
            int IdUtilizator = Utils.UtilizatorLogat();
            try
            {
               
                    Actiune act = db.Actiuni.FirstOrDefault(x => x.Denumire == denumire.Trim());
                    if(act== null)
                    {
                        act = new Actiune();
                        act.Denumire = denumire;
                        db.Actiuni.Add(act);
                    }
                    GeneratorRutina generatorRutina = new GeneratorRutina();
                    generatorRutina.IdActiune = act.Id;
                    generatorRutina.IdUtilizator = IdUtilizator;
                    generatorRutina.TotalAc = 0;
                    generatorRutina.Activ = 1;
                    db.GeneratorRutina.Add(generatorRutina);
                    db.SaveChanges();

                    //return RedirectToAction("Index");

                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtilizator);
                List<GeneratorRutina> LGR = db.GeneratorRutina.Where(x => x.IdUtilizator == IdUtilizator).ToList();
                ViewModelAdminGeneratorRutine vm = new ViewModelAdminGeneratorRutine(LGR, util, "Acțiune adăugată cu succes");

                return PartialView("_MainContent", vm);

            }
            catch (Exception ex)
            {
                string mesaj = mesaj = $"a apărut o eroare, așa din senin.... {ex.Message}";

                Utilizator util = db.Utilizatori.First(x => x.Id == IdUtilizator);
                List<GeneratorRutina> LGR = db.GeneratorRutina.Where(x => x.IdUtilizator == IdUtilizator).ToList();
                ViewModelAdminGeneratorRutine vm = new ViewModelAdminGeneratorRutine(LGR, util, mesaj);

                return PartialView("_MainContent", vm);
            }


        }
    }
}