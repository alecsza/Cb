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
        // GET: AdministrareRutina
        public ActionResult Index(string mesaj = "")
        {
            int IdUtilizator = Utils.UtilizatorLogat();
            using (var db = new DatabaseContext())
            {
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
           
        }

        public ActionResult SchimbaStare(  int idGRA, int stare)
        {
            int IdUtilizator = Utils.UtilizatorLogat();
            try
            {
                using (var db = new DatabaseContext())
                {

                    GeneratorRutina generatorRutina = db.GeneratorRutina.First(x => x.Id == idGRA);
                    generatorRutina.Activ = stare;
                    db.SaveChanges();

                    return Json(new { mesaj = "salvat" }, JsonRequestBehavior.AllowGet);
                }
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
                using (var db = new DatabaseContext())
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

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string mesaj = mesaj = $"a apărut o eroare, așa din senin.... {ex.Message}";

                return RedirectToAction("Index", new { mesaj });
            }


        }
    }
}