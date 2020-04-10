using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class MatchQueryMySqlTest : MatchQueryMySqlTestBase<MatchQueryMySqlTest.MatchQueryMySqlFixture>
    {
        public MatchQueryMySqlTest(MatchQueryMySqlFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public virtual void Match_in_natural_language_mode()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First", MySqlMatchSearchMode.NaturalLanguage));

            Assert.Equal(3, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First')");
        }

        [ConditionalFact]
        public virtual void Match_in_natural_language_mode_keywords_separated()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First, Second", MySqlMatchSearchMode.NaturalLanguage));

            Assert.Equal(6, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First, Second')");
        }

        [ConditionalFact]
        public virtual void Match_in_natural_language_mode_multiple_keywords()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First Herb", MySqlMatchSearchMode.NaturalLanguage));

            Assert.Equal(9, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First Herb')");
        }

        [ConditionalFact]
        public virtual void Match_in_natural_language_mode_multiple_keywords_separated()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First, Second", MySqlMatchSearchMode.NaturalLanguage));

            Assert.Equal(6, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First, Second')");
        }

        [ConditionalFact]
        public virtual void Match_in_boolean_mode()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First*", MySqlMatchSearchMode.Boolean));

            Assert.Equal(3, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First*' IN BOOLEAN MODE)");
        }

        [ConditionalFact]
        public virtual void Match_in_boolean_mode_with_search_mode_parameter()
        {
            using var context = CreateContext();

            var searchMode = MySqlMatchSearchMode.Boolean;
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First*", searchMode));

            Assert.Equal(3, count);

            AssertSql(
                @"@__searchMode_1='2'

SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE ((@__searchMode_1 = 0) AND MATCH (`h`.`Name`) AGAINST ('First*')) OR (((@__searchMode_1 = 1) AND MATCH (`h`.`Name`) AGAINST ('First*' WITH QUERY EXPANSION)) OR ((@__searchMode_1 = 2) AND MATCH (`h`.`Name`) AGAINST ('First*' IN BOOLEAN MODE)))");
        }

        [ConditionalFact]
        public virtual void Match_in_boolean_mode_keywords()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First* Herb*", MySqlMatchSearchMode.Boolean));

            Assert.Equal(9, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First* Herb*' IN BOOLEAN MODE)");
        }

        [ConditionalFact]
        public virtual void Match_in_boolean_mode_keyword_excluded()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "Herb* -Second", MySqlMatchSearchMode.Boolean));

            Assert.Equal(6, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('Herb* -Second' IN BOOLEAN MODE)");
        }

        [ConditionalFact]
        public virtual void Match_with_query_expansion()
        {
            using var context = CreateContext();
            var count = context.Set<Herb>().Count(herb => EF.Functions.Match(herb.Name, "First", MySqlMatchSearchMode.NaturalLanguageWithQueryExpansion));

            Assert.Equal(9, count);

            AssertSql(@"SELECT COUNT(*)
FROM `Herb` AS `h`
WHERE MATCH (`h`.`Name`) AGAINST ('First' WITH QUERY EXPANSION)");
        }

        private void AssertSql(params string[] expected) => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class MatchQueryMySqlFixture : MatchQueryMySqlFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }

    public abstract class MatchQueryMySqlTestBase<TFixture> : QueryTestBase<TFixture>
        where TFixture : MatchQueryMySqlTestBase<TFixture>.MatchQueryMySqlFixtureBase, new()
    {
        protected MatchQueryMySqlTestBase(TFixture fixture)
            : base(fixture)
        {
            fixture.ListLoggerFactory.Clear();
        }

        protected virtual DbContext CreateContext() => Fixture.CreateContext();

        public abstract class MatchQueryMySqlFixtureBase : SharedStoreFixtureBase<PoolableDbContext>, IQueryFixtureBase
        {
            protected MatchQueryMySqlFixtureBase()
            {
                var entitySorters = new Dictionary<Type, Func<dynamic, object>>
                {
                    {typeof(Herb), e => e?.Id}
                }.ToDictionary(e => e.Key, e => (object)e.Value);

                var entityAsserters = new Dictionary<Type, Action<dynamic, dynamic>>
                {
                    {
                        typeof(Herb), (e, a) =>
                        {
                            Assert.Equal(e == null, a == null);
                            if (a != null)
                            {
                                Assert.Equal(e.Id, a.Id);
                                Assert.Equal(e.Name, a.Name);
                            }
                        }
                    }
                }.ToDictionary(e => e.Key, e => (object)e.Value);

                QueryAsserter = new QueryAsserter<PoolableDbContext>(
                    CreateContext,
                    new MatchQueryData(),
                    entitySorters,
                    entityAsserters);
            }

            protected override string StoreName { get; } = "MatchQueryTest";
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;

            public QueryAsserterBase QueryAsserter { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.Entity<Herb>(
                    herb =>
                    {
                        herb.HasData(MatchQueryData.CreateHerbs());
                        herb.HasKey(h => h.Id);
                        herb.HasIndex(h => h.Name).ForMySqlIsFullText();
                    });
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                return base.AddOptions(builder).ConfigureWarnings(wcb => wcb.Throw());
            }

            public override PoolableDbContext CreateContext()
            {
                var context = base.CreateContext();
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return context;
            }
        }

        public class MatchQueryData : ISetSource
        {
            private readonly IReadOnlyList<Herb> _herbs;

            public MatchQueryData()
            {
                _herbs = CreateHerbs();
            }

            public IQueryable<TEntity> Set<TEntity>() where TEntity : class
            {
                if (typeof(TEntity) == typeof(Herb))
                {
                    return (IQueryable<TEntity>)_herbs.AsQueryable();
                }

                throw new InvalidOperationException("Invalid entity type: " + typeof(TEntity));
            }

            public static IReadOnlyList<Herb> CreateHerbs()
            {
                return new List<Herb>
                {
                    new Herb {Id = 1, Name = "First Herb Name 1"},
                    new Herb {Id = 2, Name = "First Herb Name 2"},
                    new Herb {Id = 3, Name = "First Herb Name 3"},
                    new Herb {Id = 4, Name = "Second Herb Name 1"},
                    new Herb {Id = 5, Name = "Second Herb Name 2"},
                    new Herb {Id = 6, Name = "Second Herb Name 3"},
                    new Herb {Id = 7, Name = "Third Herb Name 1"},
                    new Herb {Id = 8, Name = "Third Herb Name 2"},
                    new Herb {Id = 9, Name = "Third Herb Name 3"}
                };
            }
        }

        public class Herb
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
