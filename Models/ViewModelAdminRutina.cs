using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrototipConfidanceBuilder.DataAccess;

namespace PrototipConfidanceBuilder.Models
{
    public class ViewModelAdminGeneratorRutine
    {
        public int IdUtilizator { get; set; }

        public List<ViewActiunGenerator> ListaActiune { get; set; }

        public ViewModelAdminGeneratorRutine(List<GeneratorRutina> LGR, int idUtilizator)
        {
            ListaActiune = new List<ViewActiunGenerator>();
            IdUtilizator = idUtilizator;
            foreach(GeneratorRutina gr in LGR)
            {
                ViewActiunGenerator vag = new ViewActiunGenerator(gr);
                ListaActiune.Add(vag);


            }
        }
    }

    public class ViewActiunGenerator
    {
        public int IdActiune { get; set; }
        public int IdGeneratoractiune { get; set; }
        public string DenumireActiune { get; set; }
        public int TotalActiuniAcumulateAleUtilizatorului { get; set; }
        public int StareActiune { get; set; }

        public ViewActiunGenerator(GeneratorRutina gr)
        {
            IdActiune = gr.IdActiune;
            DenumireActiune = gr.Actiune.Denumire;
            TotalActiuniAcumulateAleUtilizatorului =(int) gr.TotalAc;
            StareActiune = (int)gr.Activ;
            IdGeneratoractiune = gr.Id;
        }
    }
}