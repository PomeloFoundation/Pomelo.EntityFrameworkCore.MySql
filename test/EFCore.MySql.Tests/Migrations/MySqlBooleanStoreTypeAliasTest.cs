using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.Migrations
{
    public sealed class MySqlBooleanStoreTypeAliasTest
        : TestWithFixture<MySqlBooleanStoreTypeAliasTest.MySqlBooleanStoreTypeAliasFixture>
    {
        public MySqlBooleanStoreTypeAliasTest(MySqlBooleanStoreTypeAliasFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalTheory]
        [InlineData("boolean")]
        [InlineData("BOOLEAN")]
        [InlineData("bool")]
        [InlineData("BOOL")]
        public void Boolean_alias_store_type_is_not_suffixed_with_size(string storeType)
        {
            using var context = Fixture.CreateContext();

            var mapping = context.GetService<IRelationalTypeMappingSource>().FindMapping(typeof(bool), storeType);

            Assert.NotNull(mapping);
            Assert.Equal(storeType, mapping.StoreType);
            Assert.Equal(typeof(bool), mapping.ClrType);
        }

        [ConditionalFact]
        public void Default_and_explicit_tinyint_store_types_are_unchanged()
        {
            using var context = Fixture.CreateContext();
            var typeMappingSource = context.GetService<IRelationalTypeMappingSource>();

            Assert.Equal("tinyint(1)", typeMappingSource.FindMapping(typeof(bool)).StoreType);
            Assert.Equal("tinyint(1)", typeMappingSource.FindMapping(typeof(bool), "tinyint(1)").StoreType);
        }

        [ConditionalFact]
        public void Columns_configured_with_boolean_aliases_keep_the_configured_store_type()
        {
            using var context = Fixture.CreateContext();

            var columns = context.GetService<IDesignTimeModel>().Model
                .GetRelationalModel()
                .Tables.Single()
                .Columns.ToDictionary(c => c.Name, c => c.StoreType);

            Assert.Equal("boolean", columns[nameof(Model.Switch.Boolean)]);
            Assert.Equal("bool", columns[nameof(Model.Switch.Bool)]);
            Assert.Equal("tinyint(1)", columns[nameof(Model.Switch.Default)]);
        }

        [ConditionalFact]
        public void Table_with_boolean_alias_columns_can_be_created_and_round_trips_values()
        {
            using var context = Fixture.CreateContext();

            // The fixture has already created the table. Before the aliases were mapped, that failed with a syntax error near "(1)".
            var switches = context.Set<Model.Switch>().OrderBy(s => s.Id).ToList();

            Assert.Equal(2, switches.Count);
            Assert.True(switches[0].Boolean);
            Assert.False(switches[0].Bool);
            Assert.False(switches[1].Boolean);
            Assert.True(switches[1].Bool);

            Assert.Equal(1, context.Set<Model.Switch>().Count(s => s.Boolean && !s.Bool));
        }

        public class MySqlBooleanStoreTypeAliasFixture
            : MySqlTestFixtureBase<MySqlBooleanStoreTypeAliasFixture.MySqlBooleanStoreTypeAliasContext>
        {
            public class MySqlBooleanStoreTypeAliasContext : ContextBase
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Model.Switch>(
                        entity =>
                        {
                            entity.Property(e => e.Boolean).HasColumnType("boolean");
                            entity.Property(e => e.Bool).HasColumnType("bool");

                            entity.HasData(
                                new Model.Switch { Id = 1, Boolean = true, Bool = false, Default = true },
                                new Model.Switch { Id = 2, Boolean = false, Bool = true, Default = false });
                        });
                }
            }
        }

        private static class Model
        {
            public class Switch
            {
                public int Id { get; set; }
                public bool Boolean { get; set; }
                public bool Bool { get; set; }
                public bool Default { get; set; }
            }
        }
    }
}
