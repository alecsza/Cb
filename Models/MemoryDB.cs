using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrototipConfidenceBuilder.Models;
using PrototipConfidenceBuilder.DataAccess;
using System.Threading.Tasks;

namespace PrototipConfidenceBuilder.Models
{
    public static class MemoryDB
    {
        public static  HashSet<Zi> Zile { get; set; }



        public static void InitMem()
        {
            if (MemoryDB.Zile== null)
                MemoryDB.Zile = new HashSet<Zi>();
        }
        public static void  ActualizareMemoryDB( DatabaseContext db, Utilizator util)
        {
            int dataD = DateTime.Now.AddDays(-7).DayOfYear;
            List<RutinaActiune> LRAL = new List<RutinaActiune>();
              LRAL = db.RutineActiuni.Where(x=>x.Rutina.IdUtilizator == util.Id).Where(x=>x.Rutina.ParcursRutina.First().Zi_An<= dataD).ToList();
          var LRA = LRAL.OrderByDescending(x => Convert.ToDateTime(x.Rutina.ParcursRutina.First().Data));
            
            Task.Factory.StartNew(() => {
                foreach (RutinaActiune x in LRAL)
            {
                    Zi zc = MemoryDB.Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                    if (zc == null)
                        MemoryDB.Zile.Add(new Zi(x));
            }



            });
        }

        public static void  AddZileToMemoryAsync(DatabaseContext db, int zi_an_s, int idUtil)
        {
            int panaLaZiua = zi_an_s - 14;
            var ziCheck = MemoryDB.Zile.Where(x => x.Idutilizator == idUtil).FirstOrDefault(x => x.DataD.DayOfYear == panaLaZiua);

            if (ziCheck == null)
            {
                var LRA = db.ParcursRutina.Where(x => x.Zi_An < zi_an_s && x.Zi_An >= panaLaZiua && x.Rutina.IdUtilizator == idUtil).
                Select(y => y.Rutina.RutinaActiune).SelectMany(x => x);


                Task.Factory.StartNew(() =>
                {
                    foreach (RutinaActiune x in LRA)
                    {
                        Zi zc = MemoryDB.Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                        if (zc == null)
                            MemoryDB.Zile.Add(new Zi(x));
                    }

                });
            }
            HttpContext.Current.Session["ZiAn_s"] = panaLaZiua;
        }

        public static void AddZileToMemory(DatabaseContext db, int zi_an_s, int idUtil)
        {
            int panaLaZiua = zi_an_s - 7;

            var ziCheck = MemoryDB.Zile.Where(x => x.Idutilizator == idUtil).FirstOrDefault(x => x.DataD.DayOfYear == panaLaZiua);

            if (ziCheck == null)
            {
                var LRA = db.ParcursRutina.Where(x => x.Zi_An <= zi_an_s && x.Zi_An > panaLaZiua && x.Rutina.IdUtilizator == idUtil).
                       Select(y => y.Rutina.RutinaActiune).SelectMany(x => x);


                foreach (RutinaActiune x in LRA)
                {
                   Zi zc = MemoryDB.Zile.FirstOrDefault(y=>y.IdRutinaActiune == x.Id);
                    if(zc == null)
                    MemoryDB.Zile.Add(new Zi(x));
                }
            }

            HttpContext.Current.Session["ZiAn_s"] = panaLaZiua;
        }
    }
}