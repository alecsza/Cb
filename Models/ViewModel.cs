using PrototipConfidanceBuilder.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PrototipConfidanceBuilder.Models
{

    public class ViewModelConfigRutina
    {
        public int IdParcursRutina { get; set; }
        public List<StatusRA> ListaRutinaActiuni { get; set; }

        public ViewModelConfigRutina(List<RutinaActiune> lra, int idParcursRutina)
        {
            IdParcursRutina = idParcursRutina;
            ListaRutinaActiuni = new List<StatusRA>();

            foreach(var item in lra)
            {
                StatusRA sra = new StatusRA(item.Id, item.IdStare, item.IdRutina, item.Actiune.Denumire);
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

        public StatusRA(int idRa, int idSts, int idR, string denAct)
        {
            IdRutina = idR;
            IdRA = idRa;
            IdStatusActiune = idSts;
            DenumireActiune = denAct;

        }
    }
    public class ViewModelIndex
    {
        public DateTime DataStart { get; set; }
        public DateTime DataStop { get; set; }

        public List<Tuple<int,string,DateTime>> ListaIdSiDataRutina { get; set; }
        public List<ActiunePeZile> Status7Z { get; set; }

       public ViewModelIndex(int IdUtil, DateTime StartDate, DateTime StopDate, DatabaseContext db)
        {
            DataStart = StartDate;
            DataStop = StopDate;
            double NrZile = Math.Abs( (DataStart.Date - DataStop.Date).TotalDays) ; // perfect

            var ListaParcursRutina = db.ParcursRutina.Where(x=>x.Rutina.IdUtilizator==1).ToList();


            ListaIdSiDataRutina = new List<Tuple<int, string, DateTime>>();
            List<Tuple<ParcursRutina, RutinaActiune>> RutineEligibile = new List<Tuple<ParcursRutina, RutinaActiune>>();
            Status7Z = new List<ActiunePeZile>();

            for (int i = 0; i <= NrZile; i++)
            {
                DateTime data = StartDate.AddDays(i).Date;

                var pr = ListaParcursRutina.FirstOrDefault(x => DateTime.ParseExact(x.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date == data);
                
                if (pr !=null)
                {
                    var listaActiuni = pr.Rutina.RutinaActiune.Select(y => new Tuple<ParcursRutina, RutinaActiune>(pr, y)).ToList();
                    RutineEligibile.AddRange(listaActiuni);
                    
                }
                string strdata = $"{data.Month}/{data.Day}";
                ListaIdSiDataRutina.Add(new Tuple<int, string,DateTime>(pr==null?0:pr.IdRutina, strdata,data));
            }
           
            List<int> ListaActiuni = RutineEligibile.Select(x => x.Item2.IdActiune).Distinct().ToList();
           
            foreach (int id in ListaActiuni)
            {
                var actiunePeZile = RutineEligibile.Where(x => x.Item2.IdActiune == id).ToList();
                Status7Z.Add(new ActiunePeZile(actiunePeZile, StartDate,6));
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

        public ActiunePeZile(List<Tuple<ParcursRutina, RutinaActiune>> listaActiuni, DateTime dataStart,int nrZile)
        {
            var act  = listaActiuni.First();
            int lenActiuni = listaActiuni.Count();
            Id = act.Item2.Id;
            Denumire = act.Item2.Actiune.Denumire.Trim();
            ListaZile = new List<Zi>();

                for( int i = 0;  i<=nrZile; i++)
                {
                    DateTime data = dataStart.AddDays(i).Date;
                    var ra = listaActiuni.FirstOrDefault(x => DateTime.ParseExact(x.Item1.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date == data);
                if (ra != null)
                {
                    ListaZile.Add(new Zi(DateTime.ParseExact(ra.Item1.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date, ra.Item2.Id, ra.Item2.IdActiune, ra.Item2.IdStare, ra.Item2.Stare.Denumire, ra.Item2.Actiune.Denumire, ra.Item2.ActiuniCumulate));

                }
                else
                {
                    ListaZile.Add(new Zi(data.Date));
                }

                }
            
        }

    }

    public class Zi
    {
        public int IdRutinaActiune { get; set; }

        public int IdActiune { get; set; }
        public string DenActiune { get; set; }
        public string Data { get; set; }
        public DateTime DataD { get; set; }
        public int? IdStare { get; set; }
        public int ActiuniCumulate { get; set; }
        public string DenStare { get; set; }

        public Zi(DateTime data, int idRa,int idAc, int? idStare, string denStare, string denActiune, int? actiuniCumulate)
        {
            DataD = data.Date;
            string zi = data.Day.ToString();
            string luna = data.Month.ToString();
            Data = $"{luna}/{zi}";
            IdStare = idStare;
            DenStare = denStare;
            IdRutinaActiune = idRa;
            IdActiune = idAc;
            DenActiune = denActiune;
            ActiuniCumulate = actiuniCumulate == null ? 0 : (int)actiuniCumulate;
        }

        public Zi(DateTime data)
        {
            string zi = data.Day.ToString();
            string luna = data.Month.ToString();
            Data = $"{luna}/{zi}";
            IdStare = 0;
            DenStare = "Acțiune oprită";
        }
    }

  public class ProgresPeActiunePeZile
    {
        public List<ObiectRepartitie> ListaCuActiuniRepartizate { get; set; }

        public ProgresPeActiunePeZile(List<Zi> ListaZile,List<DateTime> listaDate, List<int> ListaIdActiuni)
        {

            ListaCuActiuniRepartizate = new List<ObiectRepartitie>();

            foreach ( int idRA in ListaIdActiuni)
            {
                List<Zi> zile = ListaZile.Where(x => x.IdActiune == idRA).ToList();
                string denumireActiune = zile.First().DenActiune;
              
                ListaCuActiuniRepartizate.Add(Utils.RepartitieActiunePeZile(zile,listaDate));
            }

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


    public class ProgresActiuni
    {
        public List<ActiuneSiProcent> ProgresAct { get; set; }

        public ProgresActiuni(ParcursRutina pr,DatabaseContext db)
        {
            List<ParcursRutina> listaPr = db.ParcursRutina.ToList();
            ProgresAct = new List<ActiuneSiProcent>();
            foreach (var ra in pr.Rutina.RutinaActiune)
            {
                int ziletrecute = listaPr.Where(x=>x.Rutina.RutinaActiune.Select(y => y.IdActiune).Contains(ra.IdActiune))
                                                  .Where(x => DateTime.ParseExact(x.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date <= DateTime.ParseExact(pr.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date).Count();
                int ac = ra.ActiuniCumulate == null ? 0 : (int)ra.ActiuniCumulate;
                decimal procent = Utils.ProcentRealizatActiune(ziletrecute, ac);
                ActiuneSiProcent asp = new ActiuneSiProcent(procent, ra.IdActiune, ra.Actiune.Denumire);

                ProgresAct.Add(asp);
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

    public class ProgresPe6Luni
    {
        public List<string> ListaOx { get; set; }
        public List<string> ListOy { get; set; }
        public List<string> Valori { get; set; }

        public ProgresPe6Luni(List<ProgresPeLuna> ListaPLuni, int NrUnitati)
        {

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

public class ProgresPeUltimaLuna
    {

    }
public class ProgressPeTotal
    {

    }

    public static class Utils
    {
        public static decimal ProcentRealizatActiune(int NrZileTrecute, int NrActiuniRealizate)
        {
            decimal procent = ((decimal)NrActiuniRealizate * 100) /(decimal) NrZileTrecute;
            return procent;

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

            foreach (var ra in listaActiuniDeActualizat.OrderBy(x => DateTime.ParseExact(x.Rutina.ParcursRutina.First().Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date))
            {
                increment = increment + ra.IdStare - 1;
                ra.ActiuniCumulate = increment;
            }
            db.SaveChanges();
        }

        

    }
}