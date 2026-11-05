using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.Query
{
    public sealed class MySqlFromSqlComposabilityTest : TestWithFixture<MySqlFromSqlComposabilityTest.MySqlFromSqlComposabilityFixture>
    {
        public MySqlFromSqlComposabilityTest(MySqlFromSqlComposabilityFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public void Stored_procedure_call_without_composition_works()
        {
            using var context = Fixture.CreateContext();

            var containers = context.Set<Model.Container>()
                .FromSqlRaw("CALL `GetContainers`()")
                .ToList();

            Assert.Equal(3, containers.Count);
        }

        [ConditionalFact]
        public void Composing_over_stored_procedure_call_throws_non_composable()
        {
            using var context = Fixture.CreateContext();

            var exception = Assert.Throws<InvalidOperationException>(
                () => context.Set<Model.Container>()
                    .FromSqlRaw("CALL `GetContainers`()")
                    .Where(c => c.Number > 10)
                    .ToList());

            Assert.Equal(RelationalStrings.FromSqlNonComposable, exception.Message);
        }

        [ConditionalFact]
        public void Composing_over_non_query_statement_throws_non_composable()
        {
            using var context = Fixture.CreateContext();

            var exception = Assert.Throws<InvalidOperationException>(
                () => context.Set<Model.Container>()
                    .FromSqlRaw("SHOW TABLES")
                    .OrderBy(c => c.Id)
                    .ToList());

            Assert.Equal(RelationalStrings.FromSqlNonComposable, exception.Message);
        }

        [ConditionalFact]
        public void Composing_over_select_with_leading_comments_works()
        {
            using var context = Fixture.CreateContext();

            var containers = context.Set<Model.Container>()
                .FromSqlRaw("/* all containers */\n-- as a plain query\nSELECT * FROM `Container`")
                .Where(c => c.Number > 10)
                .OrderBy(c => c.Id)
                .ToList();

            Assert.Equal(new[] { 2, 3 }, containers.Select(c => c.Id));
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CommonTableExpressions))]
        [ConditionalFact]
        public void Composing_over_common_table_expression_works()
        {
            using var context = Fixture.CreateContext();

            var containers = context.Set<Model.Container>()
                .FromSqlRaw("WITH `cte` AS (SELECT * FROM `Container`) SELECT * FROM `cte`")
                .Where(c => c.Number > 10)
                .OrderBy(c => c.Id)
                .ToList();

            Assert.Equal(new[] { 2, 3 }, containers.Select(c => c.Id));
        }

        [ConditionalFact]
        public void Composing_over_parenthesized_query_expression_works()
        {
            using var context = Fixture.CreateContext();

            var containers = context.Set<Model.Container>()
                .FromSqlRaw(
                    "(SELECT * FROM `Container` WHERE `Id` = 1) UNION ALL (SELECT * FROM `Container` WHERE `Id` = 3)")
                .Where(c => c.Number > 10)
                .OrderBy(c => c.Id)
                .ToList();

            Assert.Equal(new[] { 3 }, containers.Select(c => c.Id));
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ValuesWithRows))]
        [ConditionalFact]
        public void Composing_over_table_statement_works()
        {
            using var context = Fixture.CreateContext();

            var containers = context.Set<Model.Container>()
                .FromSqlRaw("TABLE `Container`")
                .Where(c => c.Number > 10)
                .OrderBy(c => c.Id)
                .ToList();

            Assert.Equal(new[] { 2, 3 }, containers.Select(c => c.Id));
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ValuesWithRows))]
        [ConditionalFact]
        public void Composing_over_values_statement_works()
        {
            using var context = Fixture.CreateContext();

            var numbers = context.Database
                .SqlQueryRaw<int>("VALUES ROW(10), ROW(20), ROW(30)")
                .Select(v => v)
                .ToList();

            Assert.Equal(3, numbers.Count);
        }

        public class MySqlFromSqlComposabilityFixture
            : MySqlTestFixtureBase<MySqlFromSqlComposabilityFixture.MySqlFromSqlComposabilityContext>
        {
            protected override string SetupDatabaseScript
                => "CREATE PROCEDURE `GetContainers`() SELECT * FROM `Container`;";

            public class MySqlFromSqlComposabilityContext : ContextBase
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Model.Container>(
                        entity =>
                        {
                            entity.HasData(
                                new Model.Container { Id = 1, Name = "Heavymetal", Number = 10 },
                                new Model.Container { Id = 2, Name = "Plastic", Number = 20 },
                                new Model.Container { Id = 3, Name = "Plastic-Metal-Compound", Number = 30 });
                        });
                }
            }
        }

        private static class Model
        {
            public class Container
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public int Number { get; set; }
            }
        }
    }
}
