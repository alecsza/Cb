﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrototipConfidanceBuilder.Models;
using PrototipConfidanceBuilder.DataAccess;

namespace PrototipConfidanceBuilder.Controllers
{
    public class AutentificareController : Controller
    {
        // GET: Autentificare
        public ActionResult Index(string mesaj="")
        {

            return View("Index", (object)mesaj);
        }

        public ActionResult Logare(string util, string parola)
        {
            using(DatabaseContext db = new DatabaseContext())
            {
                List<Utilizator> Utilizatori = db.Utilizatori.ToList();

                Utilizator Util = Utilizatori.FirstOrDefault(x => x.Nume == util && x.Parola == parola);

                if(Util!= null)
                {
                    System.Web.HttpContext.Current.Session["IdUtilizator"] = Util.Id;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                   string mesaj = "Utilizatorul sau parola sunt incorecte";
                    return RedirectToAction("Index","Autentificare", new { mesaj });
                }
            }
        }

        public ActionResult Inregistrare(string util, string parola)
        {
            using (DatabaseContext db = new DatabaseContext())
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
                    System.Web.HttpContext.Current.Session["IdUtilizator"] = Util.Id;
                    return RedirectToAction("Index", "Rutina");
                }
            }
        }
    }
}