using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototipConfidanceBuilder.DataAccess;
using PrototipConfidanceBuilder.Models;

namespace PrototipConfidanceBuilder.Controllers
{
    public class AdministrareRutinaController : Controller
    {
        // GET: AdministrareRutina
        public ActionResult Index()
        {
            int IdUtilizator = Utils.UtilizatorLogat();
            using (var db = new DatabaseContext())
            {
                List<GeneratorRutina> LGR = db.GeneratoRutina.Where(x => x.IdUtilizator == IdUtilizator).ToList();
                ViewModelAdminGeneratorRutine vm = new ViewModelAdminGeneratorRutine(LGR, IdUtilizator);

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

                    GeneratorRutina generatorRutina = db.GeneratoRutina.First(x => x.Id == idGRA);
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
                    db.GeneratoRutina.Add(generatorRutina);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return Json(new { mesaj = $"a apărut o eroare, așa din senin.... {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}