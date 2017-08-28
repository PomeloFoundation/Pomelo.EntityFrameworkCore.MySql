using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands{

    public class TestMigrateCommand : ITestMigrateCommand {

        private AppDb _db;

        public TestMigrateCommand(AppDb db)
        {
            _db = db;
        }

        public void Run(){
            _db.Database.EnsureDeleted();

            Console.Write("EnsureCreate creates database...");
            Assert.True(_db.Database.EnsureCreated());
            Console.WriteLine(" OK");

            Console.Write("EnsureCreate existing database...");
            Assert.False(_db.Database.EnsureCreated());
            Console.WriteLine(" OK");

            Console.Write("EnsureDelete deletes database...");
            Assert.True(_db.Database.EnsureDeleted());
            Console.WriteLine(" OK");

            Console.Write("EnsureCreate non-existant database...");
            Assert.False(_db.Database.EnsureDeleted());
            Console.WriteLine(" OK");

            Console.Write("Migrate non-existant database...");
            _db.Database.Migrate();
            Console.WriteLine(" OK");

            _db.Database.EnsureDeleted();
            Console.Write("Create blank database...");
            var csb = new MySqlConnectionStringBuilder(AppConfig.Config["Data:ConnectionString"]);
            var dbName = "`" + csb.Database.Replace('`', ' ') + "`";
            csb.Database = "";
            using (var connection = new MySqlConnection(csb.ConnectionString)){
                connection.Open();
                using (var cmd = connection.CreateCommand()){
                    cmd.CommandText = $"CREATE DATABASE {dbName} CHARACTER SET utf8 COLLATE utf8_unicode_ci";
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("OK");

            Console.Write("Migrate blank database...");
            _db.Database.Migrate();
            Console.WriteLine(" OK");

            Console.WriteLine("All Tests Passed");
        }

    }

}
