namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.NTS;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Shouldly;
using Xunit;

public class CustomValueConvertersForNonNTSClrTypesMySqlTest
{
    [Fact]
    public async Task NonNTS_ClrType_CanBe_Mapped_to_CustomValueConverters()
    {
        var services = new ServiceCollection()
            .AddDbContext<CustomDbContext>()
            .AddSingleton(
                new DbContextOptionsBuilder<CustomDbContext>()
                    .UseMySql(
                        AppConfig.ConnectionString,
                        AppConfig.ServerVersion,
                        options =>
                        {
                            options.UseNetTopologySuite();
                        })
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Debug)
                .Options);

        var provider = services.BuildServiceProvider();
        await using var context = provider.GetRequiredService<CustomDbContext>();

        // Validate `MySqlNetTopologySuiteTypeMappingSourcePlugin.FindMapping()` doesn't throw.
        await Should.NotThrowAsync(context.Database.EnsureDeletedAsync());
        await Should.NotThrowAsync(context.Database.EnsureCreatedAsync());
    }
}

public sealed class CustomDbContext(DbContextOptions<CustomDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var testClass = modelBuilder.Entity<TestClass>();
        testClass.Property(t => t.Vertices)
            .HasColumnType("GEOMETRY")
            .HasConversion(new MysqlGeometryWkbValueConverter());

        var testClass2 = modelBuilder.Entity<TestClass2>();
        testClass2.Property(t => t.Vertices)
            .HasColumnType("GEOMETRY");
    }
}

public class TestClass
{
    public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public byte[] Vertices { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public class TestClass2
{
    public int Id { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Geometry Vertices { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

/// <summary>
///   MySql's internal geometry format is WKB with an initial 4
///   bytes for the SRID:
///   https://dev.mysql.com/doc/refman/5.7/en/gis-data-formats.html
/// </summary>
public class MysqlGeometryWkbValueConverter : ValueConverter<byte[], byte[]>
{
    public MysqlGeometryWkbValueConverter()
        : base(
              clr => AddSRID(clr),
              col => StripSRID(col))
    {
    }

    private static byte[] AddSRID(byte[] wkb) =>
        new byte[] { 0, 0, 0, 0, }.Concat(wkb).ToArray();

    private static byte[] StripSRID(byte[] col) =>
        col.Skip(4).ToArray();
}
