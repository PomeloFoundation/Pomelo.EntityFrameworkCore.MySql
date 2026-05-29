using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Microting.EntityFrameworkCore.MySql.Benchmarks;

/// <summary>
/// Configuration for benchmarks
/// </summary>
public static class BenchmarkConfig
{
    public static IConfig GetConfig()
    {
        return DefaultConfig.Instance
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance));
    }

    public static string GetConnectionString()
    {
        var server = Environment.GetEnvironmentVariable("BENCHMARK_DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("BENCHMARK_DB_PORT") ?? "3306";
        var database = Environment.GetEnvironmentVariable("BENCHMARK_DB_NAME") ?? "pomelo_benchmark";
        var user = Environment.GetEnvironmentVariable("BENCHMARK_DB_USER") ?? "root";
        var password = Environment.GetEnvironmentVariable("BENCHMARK_DB_PASSWORD") ?? "Password12!";

        return new MySqlConnectionStringBuilder
        {
            Server = server,
            Port = uint.Parse(port),
            Database = database,
            UserID = user,
            Password = password,
            AllowUserVariables = true,
            UseAffectedRows = false
        }.ConnectionString;
    }

    public static DbContextOptions<BenchmarkDbContext> CreateDbContextOptions()
    {
        var connectionString = GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<BenchmarkDbContext>();
        
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return optionsBuilder.Options;
    }
}

/// <summary>
/// Base class for benchmarks with setup and cleanup
/// </summary>
public abstract class BenchmarkBase
{
    protected DbContextOptions<BenchmarkDbContext>? _options;

    public virtual void GlobalSetup()
    {
        _options = BenchmarkConfig.CreateDbContextOptions();
        
        // Ensure database exists and is clean
        using var context = new BenchmarkDbContext(_options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public virtual void GlobalCleanup()
    {
        // Clean up database after benchmarks
        if (_options != null)
        {
            using var context = new BenchmarkDbContext(_options);
            context.Database.EnsureDeleted();
        }
    }

    protected BenchmarkDbContext CreateContext()
    {
        if (_options == null)
            throw new InvalidOperationException("Options not initialized. Call GlobalSetup first.");
        
        return new BenchmarkDbContext(_options);
    }
}
