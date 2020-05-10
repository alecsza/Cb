 using PrototipConfidenceBuilder.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PrototipConfidenceBuilder.Models
{

    public class ViewModelConfigRutina
    {

        public UtilizatorSiPagina UP { get; set; }
        public int IdParcursRutina { get; set; }
        public List<StatusRA> ListaRutinaActiuni { get; set; }

        public ViewModelConfigRutina(HashSet<Zi> lra,Utilizator util, int idParcursRutina)
        {
            UP = new UtilizatorSiPagina(util, "Instoric Rutină");
            IdParcursRutina = idParcursRutina;
            ListaRutinaActiuni = new List<StatusRA>();

            foreach(var item in lra)
            {
                StatusRA sra = new StatusRA(item);
                ListaRutinaActiuni.Add(sra);
            }
        }

    }

    public class StatusRA
    {
        public int IdRA { get; set; }
        public int IdStatusActiune { get; set; }
        public int IdRutina { get; set; }
        public string DenumireActiune { get; set; }

        public StatusRA(Zi zi)
        {
            IdRutina = zi.IdRutina;
            IdRA = zi.IdRutinaActiune;
            IdStatusActiune = (int)zi.IdStare;
            DenumireActiune = zi.DenStare;

        }
    }
    public class ViewModelIndex
    {
        public DateTime DataStart { get; set; }
        public DateTime DataStop { get; set; }

        public UtilizatorSiPagina UP { get; set; }

        public List<Tuple<int,string,DateTime>> ListaIdSiDataRutina { get; set; }
        public List<ActiunePeZile> Status7Z { get; set; }

       public ViewModelIndex(Utilizator util, DateTime StartDate, DateTime StopDate, HashSet<Zi> ListaRA)
        {
            DataStart = StartDate;
            DataStop = StopDate;
            UP = new UtilizatorSiPagina(util, "Istoric Rutină");

            double NrZile = Math.Abs( (DataStart.Date - DataStop.Date).TotalDays) ; // perfect

            ListaIdSiDataRutina = new List<Tuple<int, string, DateTime>>();
            List<Zi> Actiunieligibile = ListaRA.Where(x=>x.DataD>= StartDate.Date && x.DataD<= StopDate.Date).ToList();
            Status7Z = new List<ActiunePeZile>();

            for (int i = 0; i <= NrZile; i++)
            {
                DateTime data = StartDate.AddDays(i).Date;
                int? idRutina = Actiunieligibile.FirstOrDefault(x => x.DataD == data).IdRutina;
                string strdata = $"{data.Month}/{data.Day}";
                ListaIdSiDataRutina.Add(new Tuple<int, string,DateTime>(idRutina == null?0:(int)idRutina, strdata,data));
            }
           
            List<int> ListaActiuni = Actiunieligibile.Select(x => x.IdActiune).Distinct().ToList();
           
            foreach (int id in ListaActiuni)
            {
                var actiunePeZile = Actiunieligibile.Where(x => x.IdActiune == id).ToList();
                Status7Z.Add(new ActiunePeZile(actiunePeZile, actiunePeZile.First().DenActiune, id));
            }

        }

    }

    public class IdSidenumire
    {
        public int Id { get; set; }
        public string Denumire { get; set; }

        public IdSidenumire(int id, string den)
        {
            Id = id;
            Denumire = den;
        }

    }

    public class ActiunePeZile
    {
        public int Id { get; set; }
        public string Denumire { get; set; }

        public List<Zi> ListaZile { get; set; }

        public ActiunePeZile(HashSet<Zi> listaActiuni, DateTime dataStart,int nrZile)
        {
            
            var act  = listaActiuni.First();
            int lenActiuni = listaActiuni.Count();
            Id = act.IdRutinaActiune;
            Denumire = act.DenActiune;
            ListaZile = new List<Zi>();

               
                    DateTime dataStop = dataStart.AddDays(nrZile).Date;
                    Zi ra = listaActiuni.FirstOrDefault(x => x.DataD >= dataStart);
               
                    ListaZile= listaActiuni.Where(x => x.DataD >= dataStart && x.DataD <= dataStop).ToList();


           }
        public ActiunePeZile(List<Zi> listaActiuni, string denAct , int idAct)
        {
            ListaZile = listaActiuni;
            Id = idAct;
            Denumire = denAct;
        }


      }

    

    public class Zi
    {
        public int IdRutina { get; set; }
        public int IdParcursRutina { get; set; }
        public int IdRutinaActiune { get; set; }

        public int IdActiune { get; set; }
        public string DenActiune { get; set; }
        public string Data { get; set; }
        public DateTime DataD { get; set; }
        public int? IdStare { get; set; }
        public int ActiuniCumulate { get; set; }
        public int Idutilizator { get; set; }
        public string DenStare { get; set; }

        public Zi(DateTime data, int idRa,int idAc, int? idStare, string denStare, string denActiune, int? actiuniCumulate, int idUtilizator, int idRutina, int idParcursRutina)
        {
            DataD = data.Date;
            string zi = data.Day.ToString();
            string luna = data.Month.ToString();
            Data = $"{zi}/{luna}";
            IdStare = idStare;
            DenStare = denStare;
            IdRutinaActiune = idRa;
            IdActiune = idAc;
            DenActiune = denActiune;
            ActiuniCumulate = actiuniCumulate == null ? 0 : (int)actiuniCumulate;
            Idutilizator = idUtilizator;
        }

        public Zi(DateTime data)
        {
            string zi = data.Day.ToString();
            string luna = data.Month.ToString();
            Data = $"{luna}/{zi}";
            IdStare = 0;
            DenStare = "Acțiune oprită";
        }
        public Zi(RutinaActiune ra)
        {
            ParcursRutina pr = ra.Rutina.ParcursRutina.First();
            IdRutina = ra.IdRutina;
            IdParcursRutina = pr.Id;
            DataD = DateTime.ParseExact(pr.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date ;
            string zi = DataD.Day.ToString();
            string luna = DataD.Month.ToString();
            Data = $"{zi}/{luna}";
            IdStare = ra.IdStare;
            DenStare = ra.Stare.Denumire;
            IdRutinaActiune = ra.Id;
            IdActiune = ra.IdActiune;
            DenActiune = ra.Actiune.Denumire;
            ActiuniCumulate = ra.ActiuniCumulate == null ? 0 : (int)ra.ActiuniCumulate;
            Idutilizator = ra.Rutina.IdUtilizator;
        }
    }

  public class ProgresPeActiunePeZile
    {
        public List<ObiectRepartitie> ListaCuActiuniRepartizate { get; set; }

        public ProgresPeActiunePeZile(List<Zi> ListaZile,List<DateTime> listaDate, int idActiune)
        {

            ListaCuActiuniRepartizate = new List<ObiectRepartitie>();

                List<Zi> zile = ListaZile.Where(x => x.IdActiune == idActiune).ToList();
                string denumireActiune = zile.First().DenActiune;
              
                ListaCuActiuniRepartizate.Add(Utils.RepartitieActiunePeZile(zile,listaDate));
            
        }
    }
    public class ActiuneSiProcent
    {
        public decimal Procent { get; set; }
        public int IdActiune { get; set; }
        public string DenumireActiune { get; set; }

        public ActiuneSiProcent(decimal pr, int id, string den)
        {
            Procent = pr;
            IdActiune = id;
            DenumireActiune = den;
        }
    }

    public class UtilizatorSiPagina
    {
        public string NumeUtilizator { get; set; }
        public int IdUtilizator { get; set; }
        public string DenumirePagina { get; set; }

        public UtilizatorSiPagina(Utilizator util, string denPagina)
        {
            NumeUtilizator = util.Nume;
            IdUtilizator = util.Id;
            DenumirePagina = denPagina;
        }
    }

    public class ProgresActiuni
    {
        public string StartDate { get; set; }
        public string StopDate { get; set; }

        public UtilizatorSiPagina UP { get; set; }
        public List<ActiuneSiProcent> ProgresAct { get; set; }

        public ProgresActiuni(ParcursRutina pr,DateTime dataStart,Utilizator util,DatabaseContext db)
        {
            StartDate = dataStart.ToString("dd-MM-yyyy");
            StopDate = DateTime.Now.ToString("dd-MM-yyyy");
            UP = new UtilizatorSiPagina(util, "Parcurs Rutină");


            List<ParcursRutina> listaPr = db.ParcursRutina.Where(x=>x.Rutina.IdUtilizator == util.Id).ToList();


            ProgresAct = new List<ActiuneSiProcent>();
            if (pr != null)
            {
                foreach (var ra in pr.Rutina.RutinaActiune)
                {
                    var PrEligibile = listaPr.Where(x => x.Rutina.RutinaActiune.Select(y => y.IdActiune).Contains(ra.IdActiune))
                                             .Where(x => DateTime.ParseExact(x.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date >=
                                                      dataStart.Date);
                    int ziletrecute = PrEligibile.Count();
                    var acd = PrEligibile.Select(x => x.Rutina.RutinaActiune.Where(y => y.IdActiune == ra.IdActiune && y.IdStare == 2)).SelectMany(x => x);
                    int ac = acd.Count();
                    decimal procent = Utils.ProcentRealizatActiune(ziletrecute, ac);
                    ActiuneSiProcent asp = new ActiuneSiProcent(procent, ra.IdActiune, ra.Actiune.Denumire);

                    ProgresAct.Add(asp);
                }
            }
        }
    }

    public class ProgresPeLuna
    {
        public List<ObiectRepartitie> ListaCuActiuniRepartizate { get; set; }

        public ProgresPeLuna(List<Zi> ListaZile, List<DateTime> listaDate, List<int> ListaIdActiuni)
        {

            List<string> ListaOx = new List<string>();
            List<int> Valori = new List<int>();

            foreach (int idRA in ListaIdActiuni)
            {
                List<Zi> zile = ListaZile.Where(x => x.IdRutinaActiune == idRA).ToList();
                string denumireActiune = zile.First().DenActiune;

                ListaCuActiuniRepartizate.Add(Utils.RepartitieActiunePeZile(zile, listaDate));
            }

        }

    }

   
   
    public class ObiectRepartitie
    {
        public decimal ProcentRealizat { get; set; }
        public string Denumire { get; set; }
        public List<string> ListaOx { get; set; }
        public List<int> Valori { get; set; }

        public ObiectRepartitie(List<string> listaX, List<int> listaV, decimal procent,string denumire)
        {
            Denumire = denumire;
            ListaOx = listaX;
            Valori = listaV;
            ProcentRealizat = procent;

        }

    }


    public static class Utils
    {
        public static decimal ProcentRealizatActiune(int NrZileTrecute, int NrActiuniRealizate)
        {
            decimal procent = ((decimal)NrActiuniRealizate * 100) /(decimal) NrZileTrecute;
            return Decimal.Round( procent,2);

        }

        public static ObiectRepartitie RepartitieActiunePeZile(List<Zi> ListaZile, List<DateTime> listaDate )
        {
            List<string> ListaOx = new List<string>();
            List<int> Valori = new List<int>();

            string denumireActiune = ListaZile.First().DenActiune;
            int NrZileTrecute = listaDate.Count();
            int NrActiuniRealizate = ListaZile.Where(x => x.IdStare == 2).Count();
            decimal procent = ProcentRealizatActiune(NrZileTrecute, NrActiuniRealizate);
                foreach (DateTime data in listaDate)
                {
                    var zi = ListaZile.FirstOrDefault(x => x.DataD == data);
                    int valoare = 0;
                    if (zi != null)
                    {
                        valoare = (int)zi.ActiuniCumulate;
                    }
                    ListaOx.Add(data.ToShortDateString());
                    Valori.Add(valoare);
                }

               return new ObiectRepartitie(ListaOx, Valori, procent, denumireActiune);
            
        }

        public static void ActualizareActiuniAcumulate(DateTime data, DatabaseContext db, int idActiune, int idUtilizator)
        {
            var actiuniUtilizator = db.RutineActiuni.Where(x => x.IdActiune == idActiune && x.Rutina.IdUtilizator == idUtilizator).ToList();
            var  actiuneStrat  = actiuniUtilizator.FirstOrDefault(x =>
             DateTime.ParseExact(x.Rutina.ParcursRutina.First().Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date == data.AddDays(-1).Date);


            var listaActiuniDeActualizat = actiuniUtilizator.Where(x =>
               DateTime.ParseExact(x.Rutina.ParcursRutina.First().Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date >= data.Date).OrderBy(z=>z.Id).ToList();
            int increment =  actiuneStrat.ActiuniCumulate== null? 0: (int) actiuneStrat.ActiuniCumulate;

            var laOrd = listaActiuniDeActualizat.OrderBy(x => DateTime.ParseExact(x.Rutina.ParcursRutina.First().Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date);
            int nrElem = laOrd.Count();

            foreach (var ra in laOrd)
            {
                nrElem -= 1;
                GeneratorRutina ga = ra.Actiune.GeneratorRutina.FirstOrDefault(x => x.IdUtilizator == idUtilizator);
                increment = increment + ra.IdStare - 1;
                ra.ActiuniCumulate = increment;
                if(ga!=null && nrElem<=1)
                ga.TotalAc = increment;
            }
            db.SaveChanges();
        }

        public static int  UtilizatorLogat()
        {
            int idUtilizator = (int)  HttpContext.Current.Session["IdUtilizator"];

            return idUtilizator;
        }

        public static void ActualizareRutineLaZi(DatabaseContext context)
        {
            DateTime dataActualizare = DateTime.Now.Date;
            string dataStr = dataActualizare.ToString("yyyy-MM-dd");

          
                int idUtilLogat = Utils.UtilizatorLogat();
                var genrut = context.GeneratorRutina.Where(x => x.IdUtilizator == idUtilLogat);
                ParcursRutina prr = context.ParcursRutina.FirstOrDefault(x =>x.Data.Trim() == dataStr && x.Rutina.IdUtilizator == idUtilLogat);
                while (prr == null)
                {

                    string strData = dataActualizare.ToString("yyyy-MM-dd");
                    Rutina rut = new Rutina();
                 
                        rut.IdUtilizator = idUtilLogat;
                        context.Rutine.Add(rut);
                    

                    foreach (var item in genrut)
                    {
                        RutinaActiune ra = new RutinaActiune();
                        ra.IdActiune = item.IdActiune;
                        ra.IdRutina = rut.Id;
                        ra.IdStare = 1;
                        ra.ActiuniCumulate = item.TotalAc;
                        context.RutineActiuni.Add(ra);

                    }
                    ParcursRutina pa = new ParcursRutina();
                    pa.IdRutina = rut.Id;
                    pa.Data = strData;
                    context.ParcursRutina.Add(pa);

                    strData = dataActualizare.AddDays(-1).Date.ToString("yyyy-MM-dd");
                     prr = context.ParcursRutina.FirstOrDefault(x =>x.Data.Trim() == strData  && x.Rutina.IdUtilizator == idUtilLogat);

                    context.SaveChanges();

                
            }
        }

    }
}