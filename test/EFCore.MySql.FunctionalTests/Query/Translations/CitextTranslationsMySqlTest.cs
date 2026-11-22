using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

/// <summary>
///     Tests case-insensitive string operations using MySQL/MariaDB default case-insensitive collation
///     (utf8mb4_general_ci / utf8mb4_unicode_ci), which is the MySQL equivalent of PostgreSQL's citext type.
/// </summary>
public class CitextTranslationsMySqlTest : IClassFixture<CitextTranslationsMySqlTest.CitextQueryFixture>
{
    private CitextQueryFixture Fixture { get; }

    // ReSharper disable once UnusedParameter.Local
    public CitextTranslationsMySqlTest(CitextQueryFixture fixture, ITestOutputHelper testOutputHelper)
    {
        Fixture = fixture;
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [Fact]
    public void StartsWith_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.StartsWith("some"));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void StartsWith_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "some";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.StartsWith(param));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void StartsWith_param_instance()
    {
        using var ctx = CreateContext();
        var param = "SomeTextWithExtraStuff";
        var result = ctx.SomeEntities.Single(s => param.StartsWith(s.CaseInsensitiveText));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void EndsWith_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.EndsWith("sometext"));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void EndsWith_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "sometext";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.EndsWith(param));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void EndsWith_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraStuffThenSomeText";
        var result = ctx.SomeEntities.Single(s => param.EndsWith(s.CaseInsensitiveText));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Contains_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Contains("ometex"));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Contains_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "ometex";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Contains(param));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Contains_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraSometextExtra";
        var result = ctx.SomeEntities.Single(s => param.Contains(s.CaseInsensitiveText));

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void IndexOf_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.IndexOf("ometex") == 1);

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void IndexOf_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "ometex";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.IndexOf(param) == 1);

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void IndexOf_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraSometextExtra";
        var result = ctx.SomeEntities.Single(s => param.IndexOf(s.CaseInsensitiveText) == 5);

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Replace_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Replace("Te", "Ne") == "SomeNext");

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Replace_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "Te";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Replace(param, "Ne") == "SomeNext");

        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Replace_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraSomeTextExtra";
        var result = ctx.SomeEntities.Single(s => param.Replace(s.CaseInsensitiveText, "NewStuff") == "ExtraNewStuffExtra");

        Assert.Equal(1, result.Id);
    }

    protected CitextQueryContext CreateContext()
        => Fixture.CreateContext();

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    public class CitextQueryContext(DbContextOptions options) : PoolableDbContext(options)
    {
        public DbSet<SomeArrayEntity> SomeEntities { get; set; }

        public static async Task SeedAsync(CitextQueryContext context)
        {
            context.SomeEntities.AddRange(
                new SomeArrayEntity { Id = 1, CaseInsensitiveText = "SomeText" },
                new SomeArrayEntity { Id = 2, CaseInsensitiveText = "AnotherText" });
            await context.SaveChangesAsync();
        }
    }

    public class SomeArrayEntity
    {
        public int Id { get; set; }

        // MySQL/MariaDB default collation (utf8mb4_general_ci or similar) is case-insensitive,
        // providing equivalent behavior to PostgreSQL's citext type.
        public string CaseInsensitiveText { get; set; } = null!;
    }

    public class CitextQueryFixture : SharedStoreFixtureBase<CitextQueryContext>
    {
        protected override string StoreName
            => "CitextQueryTest";

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override Task SeedAsync(CitextQueryContext context)
            => CitextQueryContext.SeedAsync(context);
    }
}
