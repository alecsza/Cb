using PrototipConfidanceBuilder.DataAccess;
using System;
using System.Collections.Generic;
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

        public List<Tuple<int,DateTime>> ListaIdSiDataRutina { get; set; }
        public List<ActiunePeZile> Status7Z { get; set; }
       public ViewModelIndex(int IdUtil, DateTime StartDate, DateTime StopDate, DatabaseContext db)
        {
            DataStart = StartDate;
            DataStop = StopDate;
            var ListaParcursRutina = db.ParcursRutina.Where(x=>x.Rutina.IdUtilizator==1).ToList();




            ListaIdSiDataRutina = new List<Tuple<int, DateTime>>();
            List<Tuple<ParcursRutina, RutinaActiune>> RutineEligibile = new List<Tuple<ParcursRutina, RutinaActiune>>();
            Status7Z = new List<ActiunePeZile>();

            for (int i = 0; i < 7; i++)
            {
                DateTime data = StartDate.AddDays(i).Date;

                var pr = ListaParcursRutina.FirstOrDefault(x => DateTime.ParseExact(x.Data.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).Date == data);
                

                if (pr !=null)
                {
                    var listaActiuni = pr.Rutina.RutinaActiune.Select(y => new Tuple<ParcursRutina, RutinaActiune>(pr, y)).ToList();
                    RutineEligibile.AddRange(listaActiuni);
                    
                }
                ListaIdSiDataRutina.Add(new Tuple<int, DateTime>(pr==null?0:pr.IdRutina, data));
            }
           

            List<int> ListaActiuni = RutineEligibile.Select(x => x.Item2.IdActiune).Distinct().ToList();
           

            foreach (int id in ListaActiuni)
            {
                var actiunePeZile = RutineEligibile.Where(x => x.Item2.IdActiune == id).ToList();
                Status7Z.Add(new ActiunePeZile(actiunePeZile, StartDate));
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

        public ActiunePeZile(List<Tuple<ParcursRutina, RutinaActiune>> listaActiuni, DateTime dataStart)
        {
            var act  = listaActiuni.First();
            int lenActiuni = listaActiuni.Count();
            Id = act.Item2.Id;
            Denumire = act.Item2.Actiune.Denumire.Trim();
            ListaZile = new List<Zi>();

      
          
                for( int i = 0;  i<7; i++)
                {
                    DateTime data = dataStart.AddDays(i).Date;
                    var ra = listaActiuni.FirstOrDefault(x => Convert.ToDateTime(x.Item1.Data).Date == data);
                if (ra != null)
                {
                    ListaZile.Add(new Zi(Convert.ToDateTime(ra.Item1.Data), ra.Item2.IdStare, ra.Item2.Stare.Denumire));

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
        public string Data { get; set; }
        public int? IdStare { get; set; }
        public string DenStare { get; set; }

        public Zi(DateTime data, int? idStare, string denStare)
        {
            string zi = data.Day.ToString();
            string luna = data.Month.ToString();
            Data = $"{luna}/{zi}";
            IdStare = idStare;
            DenStare = denStare;
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
}