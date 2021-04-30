using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands{

    public class TestMigrateCommand : ITestMigrateCommand
    {
        private static readonly string[] _expectedTableNames = {
            "__EFMigrationsHistory",
            "AspNetRoleClaims",
            "AspNetRoles",
            "AspNetUserClaims",
            "AspNetUserLogins",
            "AspNetUserRoles",
            "AspNetUsers",
            "AspNetUserTokens",
            "BlogPost",
            "Blogs",
            "Categories",
            "CrmAdminMenu",
            "CrmAdminRole",
            "CrmAdmins",
            "CrmMenus",
            "CrmRoles",
            "DataTypesSimple",
            "DataTypesVariable",
            "GeneratedConcurrencyCheck",
            "GeneratedContacts",
            "GeneratedRowVersion",
            "GeneratedTime",
            "People",
            "PeopleFamilies",
            "ProductCategory",
            "Products",
            "Sequence",
        };

        private AppDb _db;

        public TestMigrateCommand(AppDb db)
        {
            _db = db;
        }

        public void Run(){
            _db.Database.EnsureDeleted();
            Assert.False(DatabaseExists());

            Console.Write("EnsureCreate creates database...");
            Assert.True(_db.Database.EnsureCreated());
            Assert.True(DatabaseExists());
            Assert.False(DatabaseEmpty());
            Assert.True(EnsureCreatedSucceededAsExpected());
            Console.WriteLine(" OK");

            Console.Write("EnsureCreate existing database...");
            Assert.False(_db.Database.EnsureCreated());
            Assert.True(DatabaseExists());
            Assert.False(DatabaseEmpty());
            Assert.True(EnsureCreatedSucceededAsExpected());
            Console.WriteLine(" OK");

            Console.Write("EnsureDelete deletes database...");
            Assert.True(_db.Database.EnsureDeleted());
            Assert.False(DatabaseExists());
            Console.WriteLine(" OK");

            Console.Write("EnsureCreate non-existant database...");
            Assert.False(_db.Database.EnsureDeleted());
            Assert.False(DatabaseExists());
            Console.WriteLine(" OK");

            _db.Database.EnsureDeleted();
            Assert.False(DatabaseExists());
            Console.Write("Create blank database...");
            var csb = new MySqlConnectionStringBuilder(AppConfig.Config["Data:ConnectionString"]);
            var dbName = csb.Database.Replace('`', ' ');
            csb.Database = "";
            using (var connection = new MySqlConnection(csb.ConnectionString)){
                connection.Open();
                using (var cmd = connection.CreateCommand()){
                    cmd.CommandText = $"CREATE DATABASE `{dbName}` CHARACTER SET utf8 COLLATE utf8_unicode_ci";
                    cmd.ExecuteNonQuery();
                }
            }
            Assert.True(DatabaseExists());
            Assert.True(DatabaseEmpty());
            Console.WriteLine("OK");

            Console.Write("Migrate blank database...");
            _db.Database.Migrate();
            Assert.False(DatabaseEmpty());
            Assert.True(MigrationsSucceededAsExpected());
            Console.WriteLine(" OK");

            _db.Database.EnsureDeleted();
            Assert.False(DatabaseExists());
            Console.Write("Migrate non-existant database...");
            _db.Database.Migrate();
            Assert.False(DatabaseEmpty());
            Assert.True(MigrationsSucceededAsExpected());
            Console.WriteLine(" OK");

            Console.WriteLine("All Tests Passed");
        }

        private bool EnsureCreatedSucceededAsExpected()
            => DatabaseContainsTablesExact(_expectedTableNames.Skip(1).ToArray());

        private bool MigrationsSucceededAsExpected()
            => DatabaseContainsTablesExact(_expectedTableNames);

        private bool DatabaseEmpty()
            => DatabaseContainsTablesExact(Array.Empty<string>());

        private static bool DatabaseContainsTablesExact(string[] allExpectedTables)
        {
            var tableNames = new List<string>();

            using var connection = new MySqlConnection(AppConfig.Config["Data:ConnectionString"]);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SHOW TABLES;";

            using var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var tableName = dataReader.GetString(0);
                tableNames.Add(tableName);
            }

            tableNames.Sort();

            return allExpectedTables.SequenceEqual(tableNames);
        }

        private bool DatabaseExists()
        {
            var databaseNames = new List<string>();

            var csb = new MySqlConnectionStringBuilder(AppConfig.Config["Data:ConnectionString"]);
            var dbName = csb.Database.Replace('`', ' ');
            csb.Database = "";

            using var connection = new MySqlConnection(csb.ConnectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SHOW DATABASES;";

            using var dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                var tableName = dataReader.GetString(0);
                databaseNames.Add(tableName);
            }

            return databaseNames.Contains(dbName);
        }
    }
}
