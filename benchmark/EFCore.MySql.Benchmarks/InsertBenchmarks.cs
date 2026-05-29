using System;
using BenchmarkDotNet.Attributes;

namespace Microting.EntityFrameworkCore.MySql.Benchmarks;

/// <summary>
/// Benchmarks for insert operations
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 1, iterationCount: 5)]
public class InsertBenchmarks : BenchmarkBase
{
    [GlobalSetup]
    public override void GlobalSetup()
    {
        base.GlobalSetup();
    }

    [GlobalCleanup]
    public override void GlobalCleanup()
    {
        base.GlobalCleanup();
    }

    [Benchmark]
    public void InsertSingleSimpleEntity()
    {
        using var context = CreateContext();
        
        var entity = new SimpleEntity
        {
            Name = "Benchmark Entity",
            Value = 42,
            CreatedAt = DateTime.UtcNow
        };

        context.SimpleEntities.Add(entity);
        context.SaveChanges();
    }

    [Benchmark]
    public void InsertBatch10SimpleEntities()
    {
        using var context = CreateContext();
        
        for (int i = 0; i < 10; i++)
        {
            context.SimpleEntities.Add(new SimpleEntity
            {
                Name = $"Batch Entity {i}",
                Value = i,
                CreatedAt = DateTime.UtcNow
            });
        }

        context.SaveChanges();
    }

    [Benchmark]
    public void InsertBatch100SimpleEntities()
    {
        using var context = CreateContext();
        
        for (int i = 0; i < 100; i++)
        {
            context.SimpleEntities.Add(new SimpleEntity
            {
                Name = $"Batch Entity {i}",
                Value = i,
                CreatedAt = DateTime.UtcNow
            });
        }

        context.SaveChanges();
    }

    [Benchmark]
    public void InsertSingleComplexEntity()
    {
        using var context = CreateContext();
        
        var entity = new ComplexEntity
        {
            Name = "Complex Benchmark Entity",
            Description = "This is a complex entity with more fields for realistic benchmarking",
            Price = 99.99m,
            Quantity = 10,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Category = "Benchmark",
            Metadata = "{\"benchmark\": true}"
        };

        context.ComplexEntities.Add(entity);
        context.SaveChanges();
    }

    [Benchmark]
    public void InsertComplexEntityWithRelations()
    {
        using var context = CreateContext();
        
        var entity = new ComplexEntity
        {
            Name = "Complex Entity with Relations",
            Description = "Entity with child relationships",
            Price = 149.99m,
            Quantity = 5,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Category = "Benchmark"
        };

        for (int i = 0; i < 5; i++)
        {
            entity.RelatedEntities.Add(new RelatedEntity
            {
                Data = $"Related data {i}",
                Order = i
            });
        }

        context.ComplexEntities.Add(entity);
        context.SaveChanges();
    }

    [Benchmark]
    public void InsertBatch10ComplexEntities()
    {
        using var context = CreateContext();
        
        for (int i = 0; i < 10; i++)
        {
            context.ComplexEntities.Add(new ComplexEntity
            {
                Name = $"Complex Batch Entity {i}",
                Description = $"Description for entity {i}",
                Price = 10.0m + i,
                Quantity = i + 1,
                CreatedAt = DateTime.UtcNow,
                IsActive = i % 2 == 0,
                Category = i % 3 == 0 ? "CategoryA" : "CategoryB"
            });
        }

        context.SaveChanges();
    }
}
