using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.BasicTypesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class BasicTypesQueryMySqlFixture : BasicTypesQueryFixtureBase, ITestSqlLoggerFactory
{
    private BasicTypesData _expectedData;

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    protected override Task SeedAsync(BasicTypesContext context)
    {
        _expectedData ??= LoadAndTweakData();
        context.AddRange(_expectedData.BasicTypesEntities);
        context.AddRange(_expectedData.NullableBasicTypesEntities);
        return context.SaveChangesAsync();
    }

    public override ISetSource GetExpectedData()
        => _expectedData ??= LoadAndTweakData();

    private BasicTypesData LoadAndTweakData()
    {
        var data = (BasicTypesData)base.GetExpectedData();

        foreach (var item in data.BasicTypesEntities)
        {
            // For all relevant temporal types, chop sub-microsecond precision which PostgreSQL does not support.
            // Temporal types which aren't set (default) get mapped to -infinity on PostgreSQL; this value causes many tests to fail.

            if (item.DateTime == default)
            {
                item.DateTime += TimeSpan.FromSeconds(1);
            }

            // PostgreSQL maps DateTime to timestamptz by default, but that represents UTC timestamps which require DateTimeKind.Utc.
            item.DateTime = DateTime.SpecifyKind(new DateTime(StripSubMicrosecond(item.DateTime.Ticks)), DateTimeKind.Utc);

            if (item.DateOnly == default)
            {
                item.DateOnly = item.DateOnly.AddDays(1);
            }

            item.TimeOnly = new TimeOnly(StripSubMicrosecond(item.TimeOnly.Ticks));
            item.TimeSpan = new TimeSpan(StripSubMicrosecond(item.TimeSpan.Ticks));

            if (item.DateTimeOffset == default)
            {
                item.DateTimeOffset += TimeSpan.FromSeconds(1);
            }

            // PostgreSQL doesn't have a real DateTimeOffset type; we map .NET DateTimeOffset to timestamptz, which represents a UTC
            // timestamp, and so we only support offset=0.
            // Also chop sub-microsecond precision which PostgreSQL does not support.
            item.DateTimeOffset = new DateTimeOffset(StripSubMicrosecond(item.DateTimeOffset.Ticks), TimeSpan.Zero);
        }

        // Do the same for the nullable counterparts
        foreach (var item in data.NullableBasicTypesEntities)
        {
            if (item.DateTime.HasValue)
            {
                item.DateTime = DateTime.SpecifyKind(new DateTime(StripSubMicrosecond(item.DateTime.Value.Ticks)), DateTimeKind.Utc);
            }

            if (item.TimeOnly.HasValue)
            {
                item.TimeOnly = new TimeOnly(StripSubMicrosecond(item.TimeOnly.Value.Ticks));
            }

            if (item.TimeSpan.HasValue)
            {
                item.TimeSpan = new TimeSpan(StripSubMicrosecond(item.TimeSpan.Value.Ticks));
            }

            if (item.DateTimeOffset.HasValue)
            {
                item.DateTimeOffset = new DateTimeOffset(StripSubMicrosecond(item.DateTimeOffset.Value.Ticks), TimeSpan.Zero);
            }
        }

        return data;

        static long StripSubMicrosecond(long ticks) => ticks - (ticks % (TimeSpan.TicksPerMillisecond / 1000));
    }

    public TestSqlLoggerFactory TestSqlLoggerFactory
        => (TestSqlLoggerFactory)ListLoggerFactory;
}
