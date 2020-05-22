 using PrototipConfidenceBuilder.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PrototipConfidenceBuilder.Models
{

    //public class ViewModelConfigRutina
    //{

    //    public UtilizatorSiPagina UP { get; set; }
    //    public int IdParcursRutina { get; set; }
    //    public List<StatusRA> ListaRutinaActiuni { get; set; }

    //    public ViewModelConfigRutina(HashSet<Zi> lra,Utilizator util, int idParcursRutina)
    //    {
    //        UP = new UtilizatorSiPagina(util, "Instoric Rutină");
    //        IdParcursRutina = idParcursRutina;
    //        ListaRutinaActiuni = new List<StatusRA>();

    //        foreach(var item in lra)
    //        {
    //            StatusRA sra = new StatusRA(item);
    //            ListaRutinaActiuni.Add(sra);
    //        }
    //    }

    //}

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

       public ViewModelIndex(Utilizator util, DateTime StartDate, DateTime StopDate, HashSet<Zi> HashsetRA)
        {
            List<Zi> ListaRA = HashsetRA.Where(x => x.Idutilizator == util.Id).ToList();
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
                Zi actEligibila = Actiunieligibile.FirstOrDefault(x => x.DataD == data);
                int idRutina = actEligibila == null ? 0 : (int)actEligibila.IdRutina;
                string strdata = $"{data.Month}/{data.Day}";
                ListaIdSiDataRutina.Add(new Tuple<int, string,DateTime>(idRutina, strdata,data));
            }
           
            List<int> ListaActiuni = Actiunieligibile.Select(x => x.IdActiune).Distinct().ToList();
           
            foreach (int id in ListaActiuni)
            {
                var actiunePeZile = Actiunieligibile.Where(x => x.IdActiune == id).OrderBy(x=>x.DataD).ToList();
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

        public ActiunePeZile(HashSet<Zi> hashActiuni, DateTime dataStart,int nrZile)
        {
            int idUtil = Utils.UtilizatorLogat();
            List<Zi> listaActiuni = hashActiuni.Where(x => x.Idutilizator == idUtil).ToList();
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
            var prs = ra.Rutina.ParcursRutina.ToList();
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

        public void ActualizareActiuniCUmulate(int inc)
        {
            ActiuniCumulate += inc;
        }
        public void ModificareStare(int stare)
        {
            IdStare = stare;
        }
    }

  public class ProgresPeActiunePeZile
    {
        public List<ObiectRepartitie> ListaCuActiuniRepartizate { get; set; }

        public ProgresPeActiunePeZile(List<Zi> listaZile,List<DateTime> listaDate, int idActiune)
        {
            int idUtil = Utils.UtilizatorLogat();
            List<Zi> ListaZile = listaZile.Where(x => x.Idutilizator == idUtil).ToList();
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

        public ProgresActiuni(DateTime dataStart,Utilizator util, HashSet<Zi> ra, List<GeneratorRutina> lgr)
        {
            StartDate = dataStart.ToString("dd-MM-yyyy");
            StopDate = DateTime.Now.ToString("dd-MM-yyyy");
            UP = new UtilizatorSiPagina(util, "Parcurs Rutină");

            List<Zi> listRaEligibile = ra.Where(x=>x.DataD >= dataStart.Date && x.Idutilizator == util.Id).ToList();

            
            ProgresAct = new List<ActiuneSiProcent>();
            if (lgr.Count()>0 )
            {
                foreach (var act in lgr)
                {
                    var parcursActiuni = listRaEligibile.Where(x => x.IdActiune == act.IdActiune);
                    int ziletrecute = parcursActiuni.Count();
                    var acd = parcursActiuni.Where(x=>x.IdStare == 2);
                    int ac = acd.Count();
                    decimal procent = Utils.ProcentRealizatActiune(ziletrecute, ac);
                    ActiuneSiProcent asp = new ActiuneSiProcent(procent, act.IdActiune, act.Actiune.Denumire);

                    ProgresAct.Add(asp);
                }
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

        public static void ActualizareActiuniAcumulate(DatabaseContext db, int stare, int IdRutinaActiune)
        {
            int modifActCum = stare==2?1:-1;
            Zi ActModif = MemoryDB.Zile.First(x => x.IdRutinaActiune == IdRutinaActiune);
            ActModif.ModificareStare(stare);
            ActModif.ActualizareActiuniCUmulate(modifActCum);
            List<RutinaActiune> DBlistRa = db.RutineActiuni.Where(x => x.IdActiune == ActModif.IdActiune && x.Rutina.IdUtilizator == ActModif.Idutilizator).ToList();
            RutinaActiune raModif =  DBlistRa.First(x => x.Id == IdRutinaActiune);
            raModif.IdStare = stare;
            raModif.ActiuniCumulate += modifActCum;
             
                
             foreach(var item in   MemoryDB.Zile.Where(x => x.DataD >ActModif.DataD).Where(x => x.IdActiune == ActModif.IdActiune))
            {
                item.ActualizareActiuniCUmulate(modifActCum);
                DBlistRa.First(x => x.Id == item.IdRutinaActiune).ActiuniCumulate += modifActCum;
            }
            db.SaveChanges();

        }

        public static int  UtilizatorLogat()
        {
            int idUtilizator = (int)  HttpContext.Current.Session["IdUtilizator"];

            return idUtilizator;
        }

        public static ParcursRutina GenRutina(DateTime data, Utilizator util, DatabaseContext context,List<GeneratorRutina> genrut, Stare st)
        {
                string dataStr = data.ToString("yyyy-MM-dd");
            
                    Rutina rut = new Rutina();
                    rut.IdUtilizator = util.Id;
                    context.Rutine.Add(rut);
                    ParcursRutina pa = new ParcursRutina();
                    pa.IdRutina = rut.Id;
                    pa.Data = dataStr;
                    context.ParcursRutina.Add(pa);

                    foreach (var item in genrut)
                    {

                        RutinaActiune ra = new RutinaActiune();
                        ra.IdActiune = item.IdActiune;
                        ra.IdRutina = rut.Id;
                        ra.IdStare = st.Id;
                        ra.Stare = st;
                        ra.Rutina = rut;
                        ra.Actiune = item.Actiune;
                        ra.ActiuniCumulate = item.TotalAc;
                        context.RutineActiuni.Add(ra);
                        context.SaveChanges();
                        MemoryDB.Zile.Add(new Zi(ra));

                    }
                    return pa;
        }

        public static void GenRutine(DateTime data, Utilizator util, DatabaseContext context, List<GeneratorRutina> genrut, Stare st)
        {
            string dataStr = data.ToString("yyyy-MM-dd");
            for (int i = 0; i < 53; i++)
            {

                Utils.GenRutina(data.AddDays(i), util, context, genrut, st);
           
            }
        }



        public static void ActualizareRutineLaZi(DatabaseContext context)
        {
            DateTime dataActualizare = DateTime.Now.Date;
            int idUtilLogat = Utils.UtilizatorLogat();
            Utilizator util = context.Utilizatori.First(x => x.Id == idUtilLogat);

            ParcursRutina ultimparcursRutina = util.UltimParcursRutina;

            if (ultimparcursRutina != null) {
                DateTime dataUltimaParcursRutina = DateTime.ParseExact(ultimparcursRutina.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date;
                DateTime dataverificare = dataUltimaParcursRutina.AddDays(1);
            var genrut = context.GeneratorRutina.Where(x => x.IdUtilizator == idUtilLogat).ToList();
                Stare st = context.Stari.First(x=>x.Id==1);
                while (dataverificare < dataActualizare)
                {
                    Utils.GenRutina(dataverificare, util, context, genrut, st);
                    dataverificare.AddDays(1);
                }
               ParcursRutina pa= Utils.GenRutina(dataverificare, util, context, genrut, st);
                util.UltimParcursRutina = pa;
            }
        }

    }
}