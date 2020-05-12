using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrototipConfidenceBuilder.Models;
using PrototipConfidenceBuilder.DataAccess;

namespace PrototipConfidenceBuilder.Models
{
    public static class MemoryDB
    {
        public static  HashSet<Zi> Zile { get; set; }
        
        public static void  ActualizareMemoryDB( DatabaseContext db)
        {
            List<RutinaActiune> LRA = new List<RutinaActiune>();
            LRA = db.RutineActiuni.ToList();

            MemoryDB.Zile =    new HashSet<Zi> (LRA.Select(x => new Zi(x)));
        }
    }
}