using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PrototipConfidenceBuilder.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Calculation> Calculations { get; set; }
        public DbSet<Actiune> Actiuni { get; set; }
        public DbSet<ParcursRutina> ParcursRutina { get; set; }
        public DbSet<Rutina> Rutine { get; set; }
        public DbSet<Stare> Stari { get; set; }
        public DbSet<Utilizator> Utilizatori { get; set; }
        public DbSet<RutinaActiune> RutineActiuni { get; set; }
        public DbSet<GeneratorRutina> GeneratorRutina { get; set; }


        public DatabaseContext() : base(GetConnection(), false)
        {

        }

        public static DbConnection GetConnection()
        {
            var connection = ConfigurationManager.ConnectionStrings["SQLiteConnection"];
            var factory = DbProviderFactories.GetFactory(connection.ProviderName);
            var dbCon = factory.CreateConnection();
            dbCon.ConnectionString = connection.ConnectionString;
            return dbCon;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new CalculationMap());
            modelBuilder.Configurations.Add(new ActiuneMap());
            modelBuilder.Configurations.Add(new ParcursRutinaMap());
            modelBuilder.Configurations.Add(new RutinaMap());
            modelBuilder.Configurations.Add(new StareMap());
            modelBuilder.Configurations.Add(new UtilizatorMap());
            modelBuilder.Configurations.Add(new RutinaActiuneMap());
            modelBuilder.Configurations.Add(new GeneratorRutinaMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}