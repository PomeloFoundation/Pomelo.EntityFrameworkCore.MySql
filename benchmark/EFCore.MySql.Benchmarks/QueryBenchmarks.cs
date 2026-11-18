using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Microting.EntityFrameworkCore.MySql.Benchmarks;

/// <summary>
/// Benchmarks for query and retrieval operations
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 1, iterationCount: 5)]
public class QueryBenchmarks : BenchmarkBase
{
    private const int SeedCount = 1000;

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
                Name = $"Entity {i}",
                Value = i,
                CreatedAt = DateTime.UtcNow.AddDays(-i % 365)
            });
        }

        for (int i = 0; i < SeedCount / 10; i++)
        {
            var complex = new ComplexEntity
            {
                Name = $"Complex Entity {i}",
                Description = $"This is a longer description for entity {i} with more text content",
                Price = 10.0m + (i * 0.5m),
                Quantity = i % 50,
                CreatedAt = DateTime.UtcNow.AddDays(-i % 365),
                IsActive = i % 2 == 0,
                Category = i % 5 == 0 ? "CategoryA" : i % 3 == 0 ? "CategoryB" : "CategoryC"
            };

            for (int j = 0; j < 5; j++)
            {
                complex.RelatedEntities.Add(new RelatedEntity
                {
                    Data = $"Related data for {i}-{j}",
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
    public void QuerySingleById()
    {
        using var context = CreateContext();
        
        var entity = context.SimpleEntities.Find(50);
    }

    [Benchmark]
    public void QueryFirstWithFilter()
    {
        using var context = CreateContext();
        
        var entity = context.SimpleEntities
            .Where(e => e.Value > 100)
            .First();
    }

    [Benchmark]
    public void QueryTop10()
    {
        using var context = CreateContext();
        
        var entities = context.SimpleEntities
            .Take(10)
            .ToList();
    }

    [Benchmark]
    public void QueryWithFilterAndOrder()
    {
        using var context = CreateContext();
        
        var entities = context.SimpleEntities
            .Where(e => e.Value > 100 && e.Value < 200)
            .OrderBy(e => e.CreatedAt)
            .Take(20)
            .ToList();
    }

    [Benchmark]
    public void QueryComplexWithFilter()
    {
        using var context = CreateContext();
        
        var entities = context.ComplexEntities
            .Where(e => e.IsActive && e.Price > 15.0m)
            .OrderByDescending(e => e.Price)
            .Take(10)
            .ToList();
    }

    [Benchmark]
    public void QueryComplexWithJoin()
    {
        using var context = CreateContext();
        
        var entities = context.ComplexEntities
            .Include(e => e.RelatedEntities)
            .Where(e => e.Category == "CategoryA")
            .Take(5)
            .ToList();
    }

    [Benchmark]
    public void QueryComplexGroupBy()
    {
        using var context = CreateContext();
        
        var results = context.ComplexEntities
            .GroupBy(e => e.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                AvgPrice = g.Average(e => e.Price)
            })
            .ToList();
    }

    [Benchmark]
    public void QueryWithMultipleFilters()
    {
        using var context = CreateContext();
        
        var entities = context.ComplexEntities
            .Where(e => e.IsActive)
            .Where(e => e.Price > 10.0m && e.Price < 50.0m)
            .Where(e => e.Quantity > 0)
            .OrderBy(e => e.Category)
            .ThenByDescending(e => e.Price)
            .Take(20)
            .ToList();
    }

    [Benchmark]
    public void QueryCount()
    {
        using var context = CreateContext();
        
        var count = context.SimpleEntities
            .Where(e => e.Value > 500)
            .Count();
    }

    [Benchmark]
    public void QueryAny()
    {
        using var context = CreateContext();
        
        var exists = context.ComplexEntities
            .Where(e => e.Category == "CategoryB" && e.IsActive)
            .Any();
    }

    [Benchmark]
    public void QueryComplexAggregation()
    {
        using var context = CreateContext();
        
        var result = context.ComplexEntities
            .Where(e => e.IsActive)
            .GroupBy(e => e.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalQuantity = g.Sum(e => e.Quantity),
                AvgPrice = g.Average(e => e.Price),
                MaxPrice = g.Max(e => e.Price),
                Count = g.Count()
            })
            .OrderByDescending(r => r.TotalQuantity)
            .ToList();
    }

    [Benchmark]
    public void QueryWithNavigationProperty()
    {
        using var context = CreateContext();
        
        var results = context.ComplexEntities
            .Include(e => e.RelatedEntities.OrderBy(r => r.Order))
            .Where(e => e.RelatedEntities.Any(r => r.Order < 3))
            .Take(10)
            .ToList();
    }
}
