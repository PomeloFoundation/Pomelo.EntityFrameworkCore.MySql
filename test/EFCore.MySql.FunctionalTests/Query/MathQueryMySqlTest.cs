using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class MathQueryMySqlTest : IClassFixture<MathQueryMySqlTest.MathQueryMySqlFixture>
{
    private MathQueryMySqlFixture Fixture { get; }

    public MathQueryMySqlTest(MathQueryMySqlFixture fixture)
    {
        Fixture = fixture;

        // ReSharper disable once VirtualMemberCallInConstructor
        ClearLog();
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_clientside()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(_ => 5 / 2)
            .Single();

        Assert.Equal(2, result);

        AssertSql(
"""
SELECT 2
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_cast_decimal1_clientside()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(_ => (decimal)5 / 2)
            .Single();

        Assert.Equal(2.5M, result);

        AssertSql(
"""
SELECT 2.5
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_cast_decimal2_clientside()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(_ => (decimal)(5 / 2))
            .Single();

        Assert.Equal(2.0M, result);

        AssertSql(
"""
SELECT 2.0
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_constants()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(_ => EF.Constant(5) / EF.Constant(2))
            .Single();

        Assert.Equal(2, result);

        AssertSql(
"""
SELECT 5 / 2
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_cast_decimal1_constants()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(_ => EF.Constant((decimal)5) / EF.Constant(2))
            .Single();

        Assert.Equal(2.5M, result);

        AssertSql(
"""
SELECT 5.0 / CAST(2 AS decimal(65,30))
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_cast_decimal2_constants()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(_ => (decimal)(EF.Constant(5) / EF.Constant(2)))
            .Single();

        Assert.Equal(2.0M, result);

        AssertSql(
"""
SELECT CAST(5 / 2 AS numeric)
FROM "Dummy" AS d
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_parameters()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(d => d.IntLeftOperand / d.IntRightOperand)
            .Single();

        Assert.Equal(2, result);

        AssertSql(
"""
SELECT `d`.`IntLeftOperand` / `d`.`IntRightOperand`
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_cast_decimal1_parameters()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(d => (decimal)d.IntLeftOperand / d.IntRightOperand)
            .Single();

        Assert.Equal(2.5M, result);

        AssertSql(
"""
SELECT CAST(`d`.`IntLeftOperand` AS decimal(65,30)) / CAST(`d`.`IntRightOperand` AS decimal(65,30))
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_cast_decimal2_parameters()
    {
        using var context = CreateContext();

        var result = context.Set<Dummy>()
            .Select(d => (decimal)(d.IntLeftOperand / d.IntRightOperand))
            .Single();

        Assert.Equal(2.0M, result);

        AssertSql(
"""
SELECT CAST(d."IntLeftOperand" / d."IntRightOperand" AS numeric)
FROM "Dummy" AS d
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_clientside()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => 5 / 2 * 2)
            .Single();

        Assert.Equal(4, result);

        AssertSql(
"""
SELECT 4
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_constants()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => EF.Constant(5) / EF.Constant(2) * EF.Constant(2))
            .Single();

        Assert.Equal(4, result);

        AssertSql(
"""
SELECT (5 / 2) * 2
FROM "Dummy" AS d
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_cast_decimal1_clientside()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => (decimal)5 / 2 * 2)
            .Single();

        Assert.Equal(5.0M, result);

        AssertSql(
"""
SELECT 5.0
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_cast_decimal1_constants()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => (decimal)EF.Constant(5) / EF.Constant(2) * EF.Constant(2))
            .Single();

        Assert.Equal(5.0M, result);

        AssertSql(
"""
SELECT (CAST(5 AS decimal(65,30)) / CAST(2 AS decimal(65,30))) * CAST(2 AS decimal(65,30))
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_cast_decimal2_clientside()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => (decimal)(5 / 2 * 2))
            .Single();

        Assert.Equal(4.0M, result);

        AssertSql(
"""
SELECT 4.0
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_cast_decimal2_constants()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => (decimal)(EF.Constant(5) / EF.Constant(2) * EF.Constant(2)))
            .Single();

        Assert.Equal(4.0M, result);

        AssertSql(
"""
SELECT CAST((5 / 2) * 2 AS numeric)
FROM "Dummy" AS d
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_parameters()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". But here it is "5".
        var result = context.Set<Dummy>()
            .Select(d => d.IntLeftOperand / d.IntRightOperand * d.IntRightOperand)
            .Single();

        Assert.Equal(4, result);

        AssertSql(
"""
SELECT (d."IntLeftOperand" / d."IntRightOperand") * d."IntRightOperand"
FROM "Dummy" AS d
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_cast_decimal1_parameters()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => (decimal)d.IntLeftOperand / d.IntRightOperand * d.IntRightOperand)
            .Single();

        Assert.Equal(5.0M, result);

        AssertSql(
"""
SELECT (CAST(`d`.`IntLeftOperand` AS decimal(65,30)) / CAST(`d`.`IntRightOperand` AS decimal(65,30))) * CAST(`d`.`IntRightOperand` AS decimal(65,30))
FROM `Dummy` AS `d`
LIMIT 2
""");
    }

    [ConditionalFact]
    public virtual void Divide_integer_by_integer_then_multiply_cast_decimal2_parameters()
    {
        using var context = CreateContext();

        // "(int)5 / (int)2 * (int)2" should be "4". Which it is.
        var result = context.Set<Dummy>()
            .Select(d => (decimal)(d.IntLeftOperand / d.IntRightOperand * d.IntRightOperand))
            .Single();

        Assert.Equal(4.0M, result);

        AssertSql(
"""
SELECT CAST((d."IntLeftOperand" / d."IntRightOperand") * d."IntRightOperand" AS numeric)
FROM "Dummy" AS d
LIMIT 2
""");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    protected virtual void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();

    protected virtual DbContext CreateContext()
        => Fixture.CreateContext();

    public class MathQueryMySqlFixture : SharedStoreFixtureBase<MathQueryContext>, ITestSqlLoggerFactory
    {
        protected override string StoreName
            => "Math";

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

        protected override async Task SeedAsync(MathQueryContext context)
        {
            context.Add(
                new Dummy
                {
                    DummyId = 1,
                    IntLeftOperand = 5,
                    IntRightOperand = 2,
                    IntOne = 1,
                    IntZero = 0,
                });

            await context.SaveChangesAsync();
        }
    }

    public class MathQueryContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dummy>();
        }
    }

    public class Dummy
    {
        public int DummyId { get; set; }
        public int IntLeftOperand { get; set; }
        public int IntRightOperand { get; set; }
        public int IntOne { get; set; }
        public int IntZero { get; set; }
    }
}
