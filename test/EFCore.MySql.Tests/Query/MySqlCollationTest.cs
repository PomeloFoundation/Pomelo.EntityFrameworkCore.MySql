using System.Linq;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Query
{
    public sealed class MySqlCollationTest : TestWithFixture<MySqlCollationTest.MySqlCollationFixture>
    {
        public MySqlCollationTest(MySqlCollationFixture fixture)
            : base(fixture)
        {
        }

        // TODO: 9.0
        // For some reason, this test does not throw on MariaDB 11.5 (Ubuntu/Windows) or MariaDB 11.4 Ubuntu (but throws on Windows).
        // The test is supposed to throw.
        [SupportedServerVersionLessThanCondition("11.4.0-mariadb")]
        [ConditionalFact]
        public void Where_with_incompatible_collations_fails()
        {
            using var context = Fixture.CreateContext();

            SetConnectionCharSetAndCollation(context);

            var connection = (MySqlConnection)context.Database.GetDbConnection();
            var csb = new MySqlConnectionStringBuilder(connection.ConnectionString);

            // using (var command = connection.CreateCommand())
            // {
            //     command.CommandText = "SELECT @@character_set_server, @@collation_server, @@character_set_client, @@character_set_connection, @@collation_connection";
            //     using (var reader = command.ExecuteReader())
            //     {
            //         if (reader.Read())
            //         {
            //             var character_set_server = (string)reader["@@character_set_server"];
            //             var collation_server = (string)reader["@@collation_server"];
            //             var character_set_client = (string)reader["@@character_set_client"];
            //             var character_set_connection = (string)reader["@@character_set_connection"];
            //             var collation_connection = (string)reader["@@collation_connection"];
            //         }
            //     }
            // }

            Assert.Throws<MySqlException>(
                () => context.Set<Model.Container>()
                    .Where(e => EF.Functions.Like(e.Name + e.Number, "%Metal%"))
                    .ToList());
        }

        [ConditionalFact]
        public void Where_with_incompatible_collations_succeeds_with_explicit_collation_case_sensitive()
        {
            using var context = Fixture.CreateContext();

            SetConnectionCharSetAndCollation(context);

            var metalContainers = context.Set<Model.Container>()
                .Where(e => EF.Functions.Like(EF.Functions.Collate(e.Name, "latin1_general_cs") + e.Number, "%Metal%"))
                .ToList();

            Assert.Single(metalContainers);
            Assert.Equal(3, metalContainers[0].Id);
        }

        [ConditionalFact]
        public void Where_with_incompatible_collations_succeeds_with_explicit_collation_case_insensitive()
        {
            using var context = Fixture.CreateContext();

            SetConnectionCharSetAndCollation(context);

            var metalContainers = context.Set<Model.Container>()
                .Where(e => EF.Functions.Like(EF.Functions.Collate(e.Name, "latin1_general_ci") + e.Number, "%Metal%"))
                .OrderBy(e => e.Id)
                .ToList();

            Assert.Equal(2, metalContainers.Count);
            Assert.Equal(1, metalContainers[0].Id);
            Assert.Equal(3, metalContainers[1].Id);
        }

        private static void SetConnectionCharSetAndCollation(MySqlCollationFixture.MySqlCollationContext context)
        {
            context.Database.OpenConnection();
            var connection = context.Database.GetDbConnection();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SET NAMES 'latin1' COLLATE 'latin1_general_ci';";
                command.ExecuteNonQuery();
            }
        }

        public class MySqlCollationFixture : MySqlTestFixtureBase<MySqlCollationFixture.MySqlCollationContext>
        {
            public class MySqlCollationContext : ContextBase
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Model.Container>(
                        entity =>
                        {
                            entity.Property(e => e.Name)
                                .UseCollation("latin1_general_cs");

                            entity.HasData(
                                new Model.Container { Id = 1, Name = "Heavymetal", Number = 10},
                                new Model.Container { Id = 2, Name = "Plastic", Number = 20},
                                new Model.Container { Id = 3, Name = "Plastic-Metal-Compound", Number = 30});
                        });
                }
            }
        }

        private static class Model
        {
            public class Container
            {
                public int Id { get ; set; }
                public string Name { get ; set; }
                public int Number { get; set; }
            }
        }
    }
}
