using System;
using System.Collections.Generic;
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




        public ActionResult AdaugaRutina(DateTime? date)
        {
            DateTime data = date == null ? DateTime.Now : (DateTime) date;
            using (var context = new DatabaseContext())
            {
                var genrut = context.GeneratoRutina.Where(x => x.IdUtilizator == 1);

                Rutina rut = new Rutina();
                rut.IdUtilizator = genrut.First().IdUtilizator;
                context.Rutine.Add(rut);
               
                foreach(var item in genrut)
                {
                    RutinaActiune ra = new RutinaActiune();
                    ra.IdActiune = item.IdActiune;
                    ra.IdRutina = rut.Id;
                    ra.IdStare = 1;
                    context.RutineActiuni.Add(ra);

                }
                ParcursRutina pa = new ParcursRutina();
                pa.IdRutina = rut.Id;
                pa.Data = data.Date.ToString();
                context.ParcursRutina.Add(pa);

                context.SaveChanges();
                return RedirectToAction("Index");
            }
         }

        public ActionResult  GetEditRutina(int IdParcursRutina)
        {
            using (var db = new DatabaseContext())
            {
                var ActiuniRutina = db.ParcursRutina.First(x => x.Id == IdParcursRutina).Rutina.RutinaActiune.ToList();
                ViewModelConfigRutina vmc = new ViewModelConfigRutina(ActiuniRutina, IdParcursRutina);
                return PartialView("_EditareStari", vmc);
               // return PartialView("_EditareStari");
            } 
           
        }
    }
}