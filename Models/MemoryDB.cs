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
        public static DatabaseContext db { get; set; }
        public static HashSet<Zi> GetZile()
        {
            HashSet<Zi> zile = ((HashSet<Zi>) (HttpContext.Current.Session["Zile"]));

            return  zile;
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

        public async static Task<List<Zi>> ZileAsync(DatabaseContext db, int dinZiua, int idUtil, int nzile = 0)
        {
            HashSet<Zi> Zile = GetZile();
            int panaLaZiua = dinZiua + 14;
            if (nzile > 0)
                panaLaZiua = dinZiua + nzile;

            var ziCheck = Zile.FirstOrDefault(x => x.DataD.DayOfYear == dinZiua);

            List<Zi> lz = new List<Zi>();
            if (ziCheck == null)

            {
                var LRA = db.ParcursRutina.Where(x => x.Zi_An < panaLaZiua && x.Zi_An >= dinZiua && x.Rutina.IdUtilizator == idUtil).
                Select(y => y.Rutina.RutinaActiune).SelectMany(x => x).ToList();

                foreach (RutinaActiune x in LRA)
                {
                    Zi zc = Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                    if (zc == null)
                        lz.Add(new Zi(x));
                }

            }
            HttpContext.Current.Session["ZiAn_s"] = dinZiua;
            return lz;

        }
        public async static Task  AddZileToMemoryAsync(DatabaseContext db, int dinZiua, int idUtil,int nzile=0)
        {
           var zile = await ZileAsync(db, dinZiua, idUtil, nzile = 0);
            MemoryDB.AddZile(zile);


        }

        public static List<Zi> AddZileToMemorySync(DatabaseContext db, int dinZiua, int idUtil, int nzile = 0)
        {
            HashSet<Zi> Zile = GetZile();
            int panaLaZiua = dinZiua + 14;
            if (nzile > 0)
                panaLaZiua = dinZiua + nzile;

            var ziCheck = Zile.FirstOrDefault(x => x.DataD.DayOfYear == dinZiua);
            List<Zi> lz = new List<Zi>();

            if (ziCheck == null)

            {
                var LRA = db.ParcursRutina.Where(x => x.Zi_An < panaLaZiua && x.Zi_An >= dinZiua && x.Rutina.IdUtilizator == idUtil).
                Select(y => y.Rutina.RutinaActiune).SelectMany(x => x).ToList();

                foreach (RutinaActiune x in LRA)
                {
                    Zi zc = Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                    if (zc == null)
                        lz.Add(new Zi(x));
                }

            }
            MemoryDB.AddZile(lz);
            HttpContext.Current.Session["ZiAn_s"] = dinZiua;
            return lz;


        }

        public async static Task<List<Zi>> ActualizareZileToMemoryAsync(DatabaseContext db, int idUtil)
        {
            HashSet<Zi> Zile = GetZile();
        
            List<Zi> lz = new List<Zi>();
          
                var LRA = db.ParcursRutina.Where(x=> x.Rutina.IdUtilizator == idUtil).
                Select(y => y.Rutina.RutinaActiune).SelectMany(x => x).ToList();

                foreach (RutinaActiune x in LRA)
                {
                    Zi zc = Zile.FirstOrDefault(y => y.IdRutinaActiune == x.Id);
                    if (zc == null)
                        lz.Add(new Zi(x));
                }

            
            return lz;

        }

        public async static Task<Zi>  AddZiAsync(Zi zi)
        {
            List<Zi> Zile = new List<Zi>();
            try
            {
                var hSes = (HashSet<Zi>)HttpContext.Current.Session["Zile"];

                Zile = hSes.ToList();
                Zile.Add(zi);
                HttpContext.Current.Session["Zile"] = new HashSet<Zi>(Zile);
                return zi;
            }
            catch (Exception ex)
            {
                string str = $"{ex.Message}, {ex.StackTrace}, {ex.InnerException}";
                return null;
            }
        }


        public static void AddZileToMemory(DatabaseContext db, int zi_an_s, int idUtil)
        {
            int dinziua = zi_an_s - 7;
            HashSet<Zi> Zile = GetZile();
            var ziCheck = Zile.FirstOrDefault(x => x.DataD.DayOfYear == dinziua);

            if (ziCheck == null)
            {
                var LRA = db.ParcursRutina.Where(x =>  x.Zi_An > dinziua && x.Rutina.IdUtilizator == idUtil).
                       Select(y => y.Rutina.RutinaActiune).SelectMany(x => x).ToList();


                foreach (RutinaActiune x in LRA)
                {
                   Zi zc = Zile.FirstOrDefault(y=>y.IdRutinaActiune == x.Id);
                    if(zc == null)
                    AddZi(new Zi(x));
                }
                Zile = GetZile();
            }

            HttpContext.Current.Session["ZiAn_s"] = dinziua;
        }



    }
}