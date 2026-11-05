using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Microting.EntityFrameworkCore.MySql.Benchmarks;

/// <summary>
/// Benchmarks for update operations
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 1, iterationCount: 5)]
public class UpdateBenchmarks : BenchmarkBase
{
    private const int SeedCount = 100;

    [GlobalSetup]
    public override void GlobalSetup()
    {
        base.GlobalSetup();
        
        // Seed database with test data
        using var context = CreateContext();
        
        for (int i = 0; i < SeedCount; i++)
        {
            context.SimpleEntities.Add(new SimpleEntity
            {
                Name = $"Seed Entity {i}",
                Value = i,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });

            var complex = new ComplexEntity
            {
                Name = $"Complex Seed Entity {i}",
                Description = $"Description {i}",
                Price = 10.0m + i,
                Quantity = i + 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                IsActive = true,
                Category = i % 3 == 0 ? "CategoryA" : "CategoryB"
            };

            for (int j = 0; j < 3; j++)
            {
                complex.RelatedEntities.Add(new RelatedEntity
                {
                    Data = $"Related {i}-{j}",
                    Order = j
                });
            }

            context.ComplexEntities.Add(complex);
        }

        context.SaveChanges();
    }

    [GlobalCleanup]
    public override void GlobalCleanup()
    {
        base.GlobalCleanup();
    }

    [Benchmark]
    public void UpdateSingleSimpleEntity()
    {
        using var context = CreateContext();
        
        var entity = context.SimpleEntities.First();
        entity.Value += 1;
        entity.Name = $"Updated {entity.Name}";
        
        context.SaveChanges();
    }

    [Benchmark]
    public void UpdateBatch10SimpleEntities()
    {
        using var context = CreateContext();
        
        var entities = context.SimpleEntities.Take(10).ToList();
        
        foreach (var entity in entities)
        {
            entity.Value += 1;
            entity.Name = $"Updated {entity.Name}";
        }
        
        context.SaveChanges();
    }

    [Benchmark]
    public void UpdateSingleComplexEntity()
    {
        using var context = CreateContext();
        
        var entity = context.ComplexEntities.First();
        entity.Price += 1.0m;
        entity.Quantity += 1;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Description = $"Updated: {entity.Description}";
        
        context.SaveChanges();
    }

    [Benchmark]
    public void UpdateComplexEntityWithQuery()
    {
        using var context = CreateContext();
        
        var entity = context.ComplexEntities
            .Where(e => e.IsActive)
            .OrderBy(e => e.CreatedAt)
            .First();
        
        entity.Price *= 1.1m;
        entity.UpdatedAt = DateTime.UtcNow;
        
        context.SaveChanges();
    }

    [Benchmark]
    public void UpdateBatch10ComplexEntities()
    {
        using var context = CreateContext();
        
        var entities = context.ComplexEntities
            .Where(e => e.Category == "CategoryA")
            .Take(10)
            .ToList();
        
        foreach (var entity in entities)
        {
            entity.Price += 5.0m;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        
        context.SaveChanges();
    }
}
