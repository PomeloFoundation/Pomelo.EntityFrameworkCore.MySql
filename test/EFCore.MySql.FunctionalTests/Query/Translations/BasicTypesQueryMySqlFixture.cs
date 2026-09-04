using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.BasicTypesModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Query.Translations;

public class BasicTypesQueryMySqlFixture : BasicTypesQueryFixtureBase, ITestSqlLoggerFactory
{
    private BasicTypesData _expectedData;

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        // MySQL datetime without precision truncates sub-second values.
        // Use datetime(6) to preserve full .NET precision (down to 100ns ticks mapped to microseconds).
        modelBuilder.Entity<BasicTypesEntity>().Property(b => b.DateTime).HasColumnType("datetime(6)");
        modelBuilder.Entity<BasicTypesEntity>().Property(b => b.DateTimeOffset).HasColumnType("datetime(6)");
        modelBuilder.Entity<BasicTypesEntity>().Property(b => b.TimeOnly).HasColumnType("time(6)");
        modelBuilder.Entity<BasicTypesEntity>().Property(b => b.TimeSpan).HasColumnType("time(6)");

        modelBuilder.Entity<NullableBasicTypesEntity>().Property(b => b.DateTime).HasColumnType("datetime(6)");
        modelBuilder.Entity<NullableBasicTypesEntity>().Property(b => b.DateTimeOffset).HasColumnType("datetime(6)");
        modelBuilder.Entity<NullableBasicTypesEntity>().Property(b => b.TimeOnly).HasColumnType("time(6)");
        modelBuilder.Entity<NullableBasicTypesEntity>().Property(b => b.TimeSpan).HasColumnType("time(6)");
    }

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

        // MySQL supports full microsecond precision (6 fractional digits) for datetime/time columns.
        // We chop sub-microsecond precision (100ns ticks) because MySQL's maximum fractional precision is microseconds (6 digits),
        // while .NET uses 100ns ticks (7 fractional digits).
        foreach (var item in data.BasicTypesEntities)
        {
            if (item.DateTime == default)
            {
                item.DateTime += TimeSpan.FromSeconds(1);
            }

            item.DateTime = new DateTime(StripSubMicrosecond(item.DateTime.Ticks));

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

            item.DateTimeOffset = new DateTimeOffset(StripSubMicrosecond(item.DateTimeOffset.Ticks), TimeSpan.Zero);
        }

        // Do the same for the nullable counterparts
        foreach (var item in data.NullableBasicTypesEntities)
        {
            if (item.DateTime.HasValue)
            {
                item.DateTime = new DateTime(StripSubMicrosecond(item.DateTime.Value.Ticks));
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
