using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class DateOnlyQueryMySqlTest : DateOnlyQueryMySqlTestBase<DateOnlyQueryMySqlTest.DateOnlyQueryMySqlFixture>
{
    public DateOnlyQueryMySqlTest(
        DateOnlyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DayNumber(bool isAsync)
    {
        var todayDateTime = DateOnly.FromDateTime(DateTime.Today);

        await AssertSingle(
            isAsync,
            ss => ss.Set<Model.IceCream>()
                .Where(i => i.BestServedBefore.DayNumber - todayDateTime.DayNumber < 30));

        AssertSql(
            @"@__todayDateTime_DayNumber_0='738170'

SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE ((TO_DAYS(`i`.`BestServedBefore`) - 366) - @__todayDateTime_DayNumber_0) < 30
LIMIT 2");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DayNumber_offset_same_as_CLR(bool isAsync)
    {
        var matchaExpireDayNumber = new DateOnly(2299, 12, 31).DayNumber;

        await AssertSingle(
            isAsync,
            ss => ss.Set<Model.IceCream>()
                .Where(i => i.BestServedBefore.DayNumber == matchaExpireDayNumber));

        AssertSql(
            @"@__matchaExpireDayNumber_0='839691'

SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE (TO_DAYS(`i`.`BestServedBefore`) - 366) = @__matchaExpireDayNumber_0
LIMIT 2");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task ToDateTime_with_nondefault_TimeOnly(bool isAsync)
    {
        var matchExpireDateTime = new DateOnly(2299, 12, 31).ToDateTime(new TimeOnly(12, 21, 42));

        await AssertSingle(
            isAsync,
            ss => ss.Set<Model.IceCream>()
                .Where(i => i.BestServedBefore.ToDateTime(new TimeOnly(12, 21, 42)) == matchExpireDateTime));

        AssertSql(
            @"@__matchExpireDateTime_0='2299-12-31T12:21:42.0000000' (DbType = DateTime)

SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE ADDTIME(CAST(`i`.`BestServedBefore` AS datetime(6)), TIME '12:21:42') = @__matchExpireDateTime_0
LIMIT 2");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task ToDateTime_with_default_TimeOnly(bool isAsync)
    {
        var matchExpireDateTime = new DateOnly(2299, 12, 31).ToDateTime(new TimeOnly());

        await AssertSingle(
            isAsync,
            ss => ss.Set<Model.IceCream>()
                .Where(i => i.BestServedBefore.ToDateTime(new TimeOnly()) == matchExpireDateTime));

        AssertSql(
            @"@__matchExpireDateTime_0='2299-12-31T00:00:00.0000000' (DbType = DateTime)

SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE CAST(`i`.`BestServedBefore` AS datetime(6)) = @__matchExpireDateTime_0
LIMIT 2");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DayNumber_FromDateTime(bool isAsync)
    {
        await AssertSingle(
            isAsync,
            ss => ss.Set<Model.IceCream>()
                .Where(i => i.BestServedBefore.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber < 30));

        AssertSql(
            @"SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE ((TO_DAYS(`i`.`BestServedBefore`) - 366) - (TO_DAYS(DATE(CURDATE())) - 366)) < 30
LIMIT 2");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateDiffDay(bool isAsync)
    {
        await using var context = CreateContext();

        var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
        var result = context.Set<Model.IceCream>()
            .Where(i => EF.Functions.DateDiffDay(todayDateOnly, i.BestServedBefore) < 30)
            .ToList();

        Assert.Single(result);

        AssertSql(
            @"@__todayDateOnly_1='01/16/2022' (DbType = Date)

SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE TIMESTAMPDIFF(DAY, @__todayDateOnly_1, `i`.`BestServedBefore`) < 30");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task DateDiffDay_ToDateTime(bool isAsync)
    {
        await using var context = CreateContext();

        var result = context.Set<Model.IceCream>()
            .Where(i => EF.Functions.DateDiffDay(DateTime.Today, i.BestServedBefore.ToDateTime(new TimeOnly())) < 30)
            .ToList();

        Assert.Single(result);

        AssertSql(
            @"SELECT `i`.`IceCreamId`, `i`.`BestServedBefore`, `i`.`Name`
FROM `IceCream` AS `i`
WHERE TIMESTAMPDIFF(DAY, CURDATE(), CAST(`i`.`BestServedBefore` AS datetime(6))) < 30");
    }

    private string AssertSql(string expected)
    {
        Fixture.TestSqlLoggerFactory.AssertBaseline(new[] { expected });
        return expected;
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    public class DateOnlyQueryMySqlFixture : DateOnlyQueryMySqlFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}

public abstract class DateOnlyQueryMySqlTestBase<TFixture> : QueryTestBase<TFixture>
    where TFixture : DateOnlyQueryMySqlTestBase<TFixture>.DateOnlyQueryMySqlFixtureBase, new()
{
    protected DateOnlyQueryMySqlTestBase(TFixture fixture)
        : base(fixture)
    {
        fixture.ListLoggerFactory.Clear();
    }

    protected virtual DbContext CreateContext() => Fixture.CreateContext();

    public abstract class DateOnlyQueryMySqlFixtureBase : SharedStoreFixtureBase<PoolableDbContext>, IQueryFixtureBase
    {
        protected override string StoreName => "DateOnlyQueryTest";
        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            modelBuilder.Entity<Model.IceCream>()
                .HasData(DateOnlyQueryData.CreateIceCreams());
        }

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder).ConfigureWarnings(wcb => wcb.Throw());

        public override PoolableDbContext CreateContext()
        {
            var context = base.CreateContext();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        public Func<DbContext> GetContextCreator()
            => CreateContext;

        public ISetSource GetExpectedData()
            => new DateOnlyQueryData();

        public IReadOnlyDictionary<Type, object> GetEntitySorters()
            => new Dictionary<Type, Func<object, object>> { { typeof(Model.IceCream), e => ((Model.IceCream)e)?.IceCreamId }, }.ToDictionary(
                e => e.Key, e => (object)e.Value);

        public IReadOnlyDictionary<Type, object> GetEntityAsserters()
            => new Dictionary<Type, Action<object, object>>
            {
                {
                    typeof(Model.IceCream),
                    (e, a) =>
                    {
                        Assert.Equal(e == null, a == null);

                        if (a != null)
                        {
                            var ee = (Model.IceCream)e;
                            var aa = (Model.IceCream)a;

                            Assert.Equal(ee.IceCreamId, aa.IceCreamId);
                            Assert.Equal(ee.Name, aa.Name);
                            Assert.Equal(ee.BestServedBefore, aa.BestServedBefore);
                        }
                    }
                },
            }.ToDictionary(e => e.Key, e => (object)e.Value);
    }

    private class DateOnlyQueryData : ISetSource
    {
        private readonly IReadOnlyList<Model.IceCream> _iceCreams;

        public DateOnlyQueryData()
        {
            _iceCreams = CreateIceCreams();
        }

        public IQueryable<TEntity> Set<TEntity>()
            where TEntity : class
        {
            if (typeof(TEntity) == typeof(Model.IceCream))
            {
                return (IQueryable<TEntity>)_iceCreams.AsQueryable();
            }

            throw new InvalidOperationException("Invalid entity type: " + typeof(TEntity));
        }

        public static IReadOnlyList<Model.IceCream> CreateIceCreams()
            => new List<Model.IceCream>
            {
                new Model.IceCream
                {
                    IceCreamId = 1,
                    Name = "Vanilla",
                    BestServedBefore = DateOnly.FromDateTime(DateTime.Today).AddDays(7),
                },
                new Model.IceCream
                {
                    IceCreamId = 2,
                    Name = "Chocolate",
                    BestServedBefore = DateOnly.FromDateTime(DateTime.Today).AddDays(100),
                },
                new Model.IceCream
                {
                    IceCreamId = 3,
                    Name = "Matcha",
                    BestServedBefore = new DateOnly(2299, 12, 31),
                },
            };
    }

    public static class Model
    {
        public class IceCream
        {
            public int IceCreamId { get; set; }
            public string Name { get; set; }
            public DateOnly BestServedBefore { get; set; }
        }
    }
}
