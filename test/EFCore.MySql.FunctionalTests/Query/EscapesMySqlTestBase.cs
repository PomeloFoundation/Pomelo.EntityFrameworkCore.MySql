using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.MusicStore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public abstract class EscapesMySqlTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : EscapesMySqlTestBase<TFixture>.EscapesMySqlFixtureBase, new()
    {
        protected EscapesMySqlTestBase(TFixture fixture)
        {
            Fixture = fixture;
            fixture.ListLoggerFactory.Clear();
        }

        [ConditionalFact]
        public virtual void Input_query_escapes_parameter()
        {
            ExecuteWithStrategyInTransaction(
                context =>
                {
                    context.Artists.Add(new Artist
                    {
                        Name = @"Back\slash's Garden Party",
                    });

                    context.SaveChanges();
                },
                context =>
                {
                    var artists = context.Artists.Where(x => x.Name.EndsWith(" Garden Party")).ToList();
                    Assert.Single(artists);
                    Assert.True(artists[0].Name == @"Back\slash's Garden Party");
                });
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_query_escapes_literal(bool async)
        {
            using (var context = CreateContext())
            {
                var query = context.Artists.Where(c => c.Name == @"Back\slasher's");

                var artists = async
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Single(artists);
            }
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_query_escapes_parameter(bool async)
        {
            using (var context = CreateContext())
            {
                var artistName = @"Back\slasher's";

                var query = context.Artists.Where(c => c.Name == artistName);

                var artists = async
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Single(artists);
            }
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_contains_query_escapes(bool async)
        {
            using (var context = CreateContext())
            {
                var artistNames = new[]
                {
                    @"Back\slasher's",
                    @"John's Chill Box"
                };

                var query = context.Artists.Where(a => artistNames.Contains(a.Name));

                var artists = async
                    ? await query.ToListAsync()
                    : query.ToList();

                Assert.Equal(2, artists.Count);
            }
        }

        public static IEnumerable<object[]> IsAsyncData = new[] { new object[] { false }, new object[] { true } };

        protected TFixture Fixture { get; }
        protected MusicStoreContext CreateContext() => Fixture.CreateContext();

        protected void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected virtual void ExecuteWithStrategyInTransaction(
            Action<MusicStoreContext> testOperation,
            Action<MusicStoreContext> nestedTestOperation1 = null,
            Action<MusicStoreContext> nestedTestOperation2 = null)
        {
            TestHelpers.ExecuteWithStrategyInTransaction(
                CreateContext,
                UseTransaction,
                testOperation,
                nestedTestOperation1,
                nestedTestOperation2);
        }

        protected virtual void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public abstract class EscapesMySqlFixtureBase : SharedStoreFixtureBase<MusicStoreContext>
        {
            protected override string StoreName { get; } = "EscapesMusicStore";
            protected override bool UsePooling => false;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                MySqlTestHelpers.Instance.EnsureSufficientKeySpace(modelBuilder.Model, TestStore);
            }

            protected override void Seed(MusicStoreContext context)
            {
                context.Artists.AddRange(
                    new Artist { ArtistId = 1, Name = @"Back\slasher's" },
                    new Artist { ArtistId = 2, Name = @"Ice Cups" },
                    new Artist { ArtistId = 3, Name = @"John's Chill Box" }
                );

                context.SaveChanges();
            }
        }
    }
}
