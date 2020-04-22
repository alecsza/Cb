using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace PrototipConfidanceBuilder.DataAccess
{
    public class CalculationMap : EntityTypeConfiguration<Calculation>
    {
        public CalculationMap()
        {
            ToTable("Calculation");

            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Name).IsRequired();
            Property(p => p.StartDateUtc).IsOptional();

        }
    }

        public class ActiuneMap : EntityTypeConfiguration<Actiune>
        {
            public ActiuneMap()
            {
                ToTable("Actiune");

                Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
               
                Property(p => p.Denumire).IsRequired();
            }

        }

        public class ParcursRutinaMap : EntityTypeConfiguration<ParcursRutina>
        {
            public ParcursRutinaMap()
            {
                ToTable("ParcursRutina");

                Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(p => p.Data).IsRequired();
                Property(p => p.IdRutina).IsRequired();
        }
        }

        public class RutinaMap : EntityTypeConfiguration<Rutina>
        {
            public RutinaMap()
            {
                ToTable("Rutina");

                Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(p => p.IdUtilizator).IsRequired();
            }

        }

        public class StareMap: EntityTypeConfiguration<Stare>
        {
            public StareMap()
            {
                ToTable("Stare");

                Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(p => p.Denumire).IsRequired();

            }
        }

        public class UtilizatorMap: EntityTypeConfiguration<Utilizator>
        {
            public UtilizatorMap()
            {
                ToTable("Utilizator");

                Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(p => p.Nume).IsRequired();
            }
        }

    public class RutinaActiuneMap : EntityTypeConfiguration<RutinaActiune>
    {
        public RutinaActiuneMap()
        {
            ToTable("RutinaActiune");

            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.IdRutina).IsRequired();
            Property(p => p.IdActiune).IsRequired();
            Property(p => p.IdStare).IsRequired();
        }
    }

    public class GeneratorRutinaMap: EntityTypeConfiguration<GeneratorRutina>
    {
        public GeneratorRutinaMap()
        {
            ToTable("GeneratorRutina");

            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(p => p.IdUtilizator).IsRequired();
            Property(p => p.IdActiune).IsRequired();
        }
    }

}
