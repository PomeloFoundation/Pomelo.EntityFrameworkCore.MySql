using System;
using BenchmarkDotNet.Running;
using Microting.EntityFrameworkCore.MySql.Benchmarks;

namespace Microting.EntityFrameworkCore.MySql.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Microting.EntityFrameworkCore.MySql Benchmarks");
        Console.WriteLine("===========================================");
        Console.WriteLine();
        Console.WriteLine("Database Connection:");
        Console.WriteLine($"  Server: {Environment.GetEnvironmentVariable("BENCHMARK_DB_HOST") ?? "localhost"}");
        Console.WriteLine($"  Port: {Environment.GetEnvironmentVariable("BENCHMARK_DB_PORT") ?? "3306"}");
        Console.WriteLine($"  Database: {Environment.GetEnvironmentVariable("BENCHMARK_DB_NAME") ?? "pomelo_benchmark"}");
        Console.WriteLine($"  User: {Environment.GetEnvironmentVariable("BENCHMARK_DB_USER") ?? "root"}");
        Console.WriteLine();

        // If specific benchmark class is requested
        if (args.Length > 0)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "insert":
                    BenchmarkRunner.Run<InsertBenchmarks>(BenchmarkConfig.GetConfig());
                    break;
                case "update":
                    BenchmarkRunner.Run<UpdateBenchmarks>(BenchmarkConfig.GetConfig());
                    break;
                case "query":
                    BenchmarkRunner.Run<QueryBenchmarks>(BenchmarkConfig.GetConfig());
                    break;
                case "all":
                    RunAllBenchmarks();
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }
        else
        {
            ShowHelp();
        }
    }

    static void RunAllBenchmarks()
    {
        Console.WriteLine("Running all benchmarks...");
        Console.WriteLine();

        BenchmarkRunner.Run<InsertBenchmarks>(BenchmarkConfig.GetConfig());
        BenchmarkRunner.Run<UpdateBenchmarks>(BenchmarkConfig.GetConfig());
        BenchmarkRunner.Run<QueryBenchmarks>(BenchmarkConfig.GetConfig());
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage: dotnet run -- [benchmark-type]");
        Console.WriteLine();
        Console.WriteLine("Benchmark types:");
        Console.WriteLine("  insert  - Run insert operation benchmarks");
        Console.WriteLine("  update  - Run update operation benchmarks");
        Console.WriteLine("  query   - Run query/retrieve operation benchmarks");
        Console.WriteLine("  all     - Run all benchmarks");
        Console.WriteLine();
        Console.WriteLine("Environment variables (optional):");
        Console.WriteLine("  BENCHMARK_DB_HOST     - Database host (default: localhost)");
        Console.WriteLine("  BENCHMARK_DB_PORT     - Database port (default: 3306)");
        Console.WriteLine("  BENCHMARK_DB_NAME     - Database name (default: pomelo_benchmark)");
        Console.WriteLine("  BENCHMARK_DB_USER     - Database user (default: root)");
        Console.WriteLine("  BENCHMARK_DB_PASSWORD - Database password (default: Password12!)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run -- insert");
        Console.WriteLine("  dotnet run -- all");
        Console.WriteLine("  BENCHMARK_DB_HOST=127.0.0.1 dotnet run -- query");
    }
}
