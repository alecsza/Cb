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
        public static HashSet<Zi> GetZile()
        {
            List<Zi> zile = ((HashSet<Zi>) (HttpContext.Current.Session["Zile"])).ToList();

            return  new HashSet<Zi> (zile);
        }

        public static void AddZi(Zi zi)
        {
            List<Zi> Zile = new List<Zi>();
            try
            {
                var hSes = (HashSet<Zi>)HttpContext.Current.Session["Zile"];

                Zile = hSes.ToList();
                Zile.Add(zi);
                HttpContext.Current.Session["Zile"] = new HashSet<Zi>(Zile);
            }catch(Exception ex)
            {
                string str = $"{ex.Message}, {ex.StackTrace}, {ex.InnerException}";
            }
        }

        public static void AddZile(List<Zi> Zile)
        {
            
            try
            {
                var hSes = (HashSet<Zi>)HttpContext.Current.Session["Zile"];
                var ListZile = hSes.ToList();
                ListZile.AddRange(Zile);
                HttpContext.Current.Session["Zile"] = new HashSet<Zi>(ListZile);
            }
            catch (Exception ex)
            {
                string str = $"{ex.Message}, {ex.StackTrace}, {ex.InnerException}";
            
            }
        }




        public static void  ActualizareMemoryDB( DatabaseContext db, Utilizator util)
        {
            HashSet<Zi> Zile = GetZile();

            int dataD = DateTime.Now.AddDays(-7).DayOfYear;
            List<RutinaActiune> LRAL = new List<RutinaActiune>();
              LRAL = db.RutineActiuni.Where(x=>x.Rutina.IdUtilizator == util.Id).Where(x=>x.Rutina.ParcursRutina.First().Zi_An<= dataD).ToList();
          var LRA = LRAL.OrderByDescending(x => Convert.ToDateTime(x.Rutina.ParcursRutina.First().Data));
            
            Task.Factory.StartNew(() => {
                foreach (RutinaActiune x in LRAL)
            {
                    Zi zc = Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                    if (zc == null)
                        AddZi(new Zi(x));
            }



            });
        }

        public async static Task<List<Zi>>  AddZileToMemoryAsync(DatabaseContext db, int zi_an_s, int idUtil)
        {
            HashSet<Zi> Zile = GetZile();
            int panaLaZiua = zi_an_s - 14;
            var ziCheck = Zile.FirstOrDefault(x => x.DataD.DayOfYear == panaLaZiua);

            List<Zi> lz = new List<Zi>();
            if (ziCheck == null)
            {
                var LRA = db.ParcursRutina.Where(x => x.Zi_An < zi_an_s && x.Zi_An >= panaLaZiua && x.Rutina.IdUtilizator == idUtil).
                Select(y => y.Rutina.RutinaActiune).SelectMany(x => x).ToList();

                foreach (RutinaActiune x in LRA)
                    {
                        Zi zc = Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                        if (zc == null)
                            lz.Add(new Zi(x));
                    }

            }
            return lz;
          
        }

        public static void AddZileToMemory(DatabaseContext db, int zi_an_s, int idUtil)
        {
            int panaLaZiua = zi_an_s - 7;
            HashSet<Zi> Zile = GetZile();
            var ziCheck = Zile.FirstOrDefault(x => x.DataD.DayOfYear == panaLaZiua);

            if (ziCheck == null)
            {
                var LRA = db.ParcursRutina.Where(x => x.Zi_An <= zi_an_s && x.Zi_An > panaLaZiua && x.Rutina.IdUtilizator == idUtil).
                       Select(y => y.Rutina.RutinaActiune).SelectMany(x => x).ToList();


                foreach (RutinaActiune x in LRA)
                {
                   Zi zc = Zile.FirstOrDefault(y=>y.IdRutinaActiune == x.Id);
                    if(zc == null)
                    AddZi(new Zi(x));
                }
                Zile = GetZile();
            }

            HttpContext.Current.Session["ZiAn_s"] = panaLaZiua;
        }
    }
}