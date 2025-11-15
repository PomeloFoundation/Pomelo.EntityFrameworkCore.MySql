using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

/// <summary>
///     Tests operations on the PostgreSQL citext type.
/// </summary>
public class CitextTranslationsTest : IClassFixture<CitextTranslationsTest.CitextQueryFixture>
{
    private CitextQueryFixture Fixture { get; }

    // ReSharper disable once UnusedParameter.Local
    public CitextTranslationsTest(CitextQueryFixture fixture, ITestOutputHelper testOutputHelper)
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
        AssertSql(
            """
SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE s."CaseInsensitiveText" LIKE 'some%'
LIMIT 2
""");
    }

    [Fact]
    public void StartsWith_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "some";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.StartsWith(param));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param_startswith='some%'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE s."CaseInsensitiveText" LIKE @param_startswith
LIMIT 2
""");
    }

    [Fact]
    public void StartsWith_param_instance()
    {
        using var ctx = CreateContext();
        var param = "SomeTextWithExtraStuff";
        var result = ctx.SomeEntities.Single(s => param.StartsWith(s.CaseInsensitiveText));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='SomeTextWithExtraStuff'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE left(@param, length(s."CaseInsensitiveText"))::citext = s."CaseInsensitiveText"
LIMIT 2
""");
    }

    [Fact]
    public void EndsWith_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.EndsWith("sometext"));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE s."CaseInsensitiveText" LIKE '%sometext'
LIMIT 2
""");
    }

    [Fact]
    public void EndsWith_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "sometext";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.EndsWith(param));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param_endswith='%sometext'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE s."CaseInsensitiveText" LIKE @param_endswith
LIMIT 2
""");
    }

    [Fact]
    public void EndsWith_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraStuffThenSomeText";
        var result = ctx.SomeEntities.Single(s => param.EndsWith(s.CaseInsensitiveText));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='ExtraStuffThenSomeText'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE right(@param, length(s."CaseInsensitiveText"))::citext = s."CaseInsensitiveText"
LIMIT 2
""");
    }

    [Fact]
    public void Contains_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Contains("ometex"));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE s."CaseInsensitiveText" LIKE '%ometex%'
LIMIT 2
""");
    }

    [Fact]
    public void Contains_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "ometex";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Contains(param));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param_contains='%ometex%'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE s."CaseInsensitiveText" LIKE @param_contains
LIMIT 2
""");
    }

    [Fact]
    public void Contains_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraSometextExtra";
        var result = ctx.SomeEntities.Single(s => param.Contains(s.CaseInsensitiveText));

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='ExtraSometextExtra'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE strpos(@param, s."CaseInsensitiveText") > 0
LIMIT 2
""");
    }

    [Fact]
    public void IndexOf_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.IndexOf("ometex") == 1);

        Assert.Equal(1, result.Id);
        AssertSql(
            """
SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE strpos(s."CaseInsensitiveText", 'ometex') - 1 = 1
LIMIT 2
""");
    }

    [Fact]
    public void IndexOf_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "ometex";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.IndexOf(param) == 1);

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='ometex'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE strpos(s."CaseInsensitiveText", @param) - 1 = 1
LIMIT 2
""");
    }

    [Fact]
    public void IndexOf_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraSometextExtra";
        var result = ctx.SomeEntities.Single(s => param.IndexOf(s.CaseInsensitiveText) == 5);

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='ExtraSometextExtra'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE strpos(@param, s."CaseInsensitiveText") - 1 = 5
LIMIT 2
""");
    }

    [Fact]
    public void Replace_literal()
    {
        using var ctx = CreateContext();
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Replace("Te", "Ne") == "SomeNext");

        Assert.Equal(1, result.Id);
        AssertSql(
            """
SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE replace(s."CaseInsensitiveText", 'Te', 'Ne') = 'SomeNext'
LIMIT 2
""");
    }

    [Fact]
    public void Replace_param_pattern()
    {
        using var ctx = CreateContext();
        var param = "Te";
        var result = ctx.SomeEntities.Single(s => s.CaseInsensitiveText.Replace(param, "Ne") == "SomeNext");

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='Te'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE replace(s."CaseInsensitiveText", @param, 'Ne') = 'SomeNext'
LIMIT 2
""");
    }

    [Fact]
    public void Replace_param_instance()
    {
        using var ctx = CreateContext();
        var param = "ExtraSometextExtra";
        var result = ctx.SomeEntities.Single(s => param.Replace(s.CaseInsensitiveText, "NewStuff") == "ExtraNewStuffExtra");

        Assert.Equal(1, result.Id);
        AssertSql(
            """
@param='ExtraSometextExtra'

SELECT s."Id", s."CaseInsensitiveText"
FROM "SomeEntities" AS s
WHERE replace(@param, s."CaseInsensitiveText", 'NewStuff') = 'ExtraNewStuffExtra'
LIMIT 2
""");
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

        [Column(TypeName = "citext")]
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
