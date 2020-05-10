using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototipConfidenceBuilder.Models;
using PrototipConfidenceBuilder.DataAccess;

namespace PrototipConfidenceBuilder.Controllers
{
    public class AutentificareController : Controller
    {
        // GET: Autentificare
        DatabaseContext db = new DatabaseContext();
        public ActionResult Index(string mesaj="")
        {

            return View("Index", (object)mesaj);
        }

        public ActionResult Logare(string util, string parola)
        {
            
                List<Utilizator> Utilizatori = db.Utilizatori.ToList();

                Utilizator Util = Utilizatori.FirstOrDefault(x => x.Nume == util && x.Parola == parola);

                if(Util!= null)
                {
                    System.Web.HttpContext.Current.Session["IdUtilizator"] = Util.Id;
                    Utils.ActualizareRutineLaZi(db);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                   string mesaj = "Utilizatorul sau parola sunt incorecte";
                   return RedirectToAction("Index","Autentificare", new { mesaj });
                }
            
        }

        public ActionResult Inregistrare(string util, string parola)
        {
           
                List<Utilizator> Utilizatori = db.Utilizatori.ToList();

                Utilizator Util = Utilizatori.FirstOrDefault(x => x.Nume == util );

                if (Util != null)
                {
                    string mesaj = "Exista un utilizator cu acest nume";
                    return RedirectToAction("Index", "Autentificare" , new { mesaj } );
                }
                else
                {
                    Util = new Utilizator();
                    Util.Nume = util;
                    Util.Parola = parola;
                    db.Utilizatori.Add(Util);
                    db.SaveChanges();
                    string mesaj = "Este necesar să adăugați cel putin o acțiune în rutina dumneavoastră";
                    System.Web.HttpContext.Current.Session["IdUtilizator"] = Util.Id;
                    return RedirectToAction("Index", "AdministrareRutina", new { mesaj });
                }
            
        }
    }
}