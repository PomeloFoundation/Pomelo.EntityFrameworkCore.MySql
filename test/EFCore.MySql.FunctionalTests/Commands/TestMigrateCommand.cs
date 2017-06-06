using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Commands{

    public static class TestMigrateCommand{

        public static void Run(){
            using (var db = new AppDb()){
                db.Database.EnsureDeleted();

                Console.Write("EnsureCreate creates database...");
                Assert.True(db.Database.EnsureCreated());
                Console.WriteLine(" OK");

                Console.Write("EnsureCreate existing database...");
                Assert.False(db.Database.EnsureCreated());
                Console.WriteLine(" OK");

                Console.Write("EnsureDelete deletes database...");
                Assert.True(db.Database.EnsureDeleted());
                Console.WriteLine(" OK");

                Console.Write("EnsureCreate non-existant database...");
                Assert.False(db.Database.EnsureDeleted());
                Console.WriteLine(" OK");

                Console.Write("Migrate non-existant database...");
                db.Database.Migrate();
                Console.WriteLine(" OK");

                db.Database.EnsureDeleted();
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
                db.Database.Migrate();
                Console.WriteLine(" OK");
            }

            Console.WriteLine("All Tests Passed");
        }

    }

}
