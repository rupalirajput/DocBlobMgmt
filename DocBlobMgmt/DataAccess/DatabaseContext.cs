using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Web;

namespace WebApplication.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<DocStatusView> DocStatusViews { get; set; }
        public DbSet<Document> Documents { get; set; }

        public DatabaseContext() : base(GetConnection(), false)
        {

        }

        public static DbConnection GetConnection()
        {
            // Formulating a db access withing the soln base dir, so we can avoid setting up a DB infra to run this.
            var conn = new SQLiteConnection();
            conn.ConnectionString = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\DataAccess\\Database.db;";
            conn.Open();
            return conn;

            //var connection = ConfigurationManager.ConnectionStrings["SQLiteConnection"];
            //var factory = DbProviderFactories.GetFactory(connection.ProviderName);
            //var dbCon = factory.CreateConnection();
            //dbCon.ConnectionString = connection.ConnectionString;
            //return dbCon;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new DocStatusViewMap());
            modelBuilder.Configurations.Add(new DocumentMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}