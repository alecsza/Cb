using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrototipConfidanceBuilder.DataAccess
{
    public class Calculation
    {
        public int Id { get; set; }

        public int? Name { get; set; }

        public string StartDateUtc { get; set; }

        public int? Progress { get; set; }
    }

    public class Actiune
    {     
       
        public int Id { get; set; }

        public string Denumire { get; set; }

        public virtual ICollection<RutinaActiune> RutinaActiune { get; set; } = new HashSet<RutinaActiune>();
       

    }

    public class ParcursRutina
    {
        public int Id { get; set; }

        public int IdRutina { get; set; }
        public  string Data { get; set; }

        [ForeignKey("IdRutina")]
        public virtual Rutina Rutina { get; set; }
    }

    public class Rutina
    {
        public int Id { get; set; }
        public int IdUtilizator { get; set; }
        public virtual ICollection<ParcursRutina> ParcursRutina { get; set; } = new HashSet<ParcursRutina>();
        public virtual ICollection<RutinaActiune> RutinaActiune { get; set; } = new HashSet<RutinaActiune>(); 

    }

    public class Stare
    {
        public int Id { get; set; }
        public string Denumire { get; set; }

   }

    public class Utilizator
    {
        public int Id { get; set; }
        public string Nume { get; set; }
    }

    public class RutinaActiune
    {
        public int Id { get; set; }
        public int IdActiune { get; set; }
        public int IdRutina { get; set; }
        public int IdStare { get; set; }

        [ForeignKey("IdRutina")]
        public virtual Rutina Rutina { get; set; }

        [ForeignKey("IdActiune")]
        public virtual Actiune Actiune { get; set; }

        [ForeignKey("IdStare")]
        public virtual Stare Stare { get; set; }
    }

    public class GeneratorRutina
    {
        public int Id { get; set; }
        public int IdActiune { get; set; }
        public int IdUtilizator { get; set; }

        [ForeignKey("IdUtilizator")]
        public virtual Utilizator Utilizator { get; set; }

        [ForeignKey("IdActiune")]
        public virtual Actiune Actiune { get; set; }

    }

}