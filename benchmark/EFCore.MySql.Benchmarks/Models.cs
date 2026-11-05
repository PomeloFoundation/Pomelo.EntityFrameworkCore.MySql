using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Microting.EntityFrameworkCore.MySql.Benchmarks;

/// <summary>
/// Simple entity for basic CRUD benchmarks
/// </summary>
public class SimpleEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Complex entity with relationships for more realistic scenarios
/// </summary>
public class ComplexEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    
    public ICollection<RelatedEntity> RelatedEntities { get; set; } = new List<RelatedEntity>();
}

/// <summary>
/// Related entity for testing joins and navigation properties
/// </summary>
public class RelatedEntity
{
    public int Id { get; set; }
    public int ComplexEntityId { get; set; }
    public string Data { get; set; } = string.Empty;
    public int Order { get; set; }
    
    public ComplexEntity ComplexEntity { get; set; } = null!;
}

/// <summary>
/// Benchmark DbContext
/// </summary>
public class BenchmarkDbContext : DbContext
{
    public BenchmarkDbContext(DbContextOptions<BenchmarkDbContext> options)
        : base(options)
    {
    }

    public DbSet<SimpleEntity> SimpleEntities { get; set; } = null!;
    public DbSet<ComplexEntity> ComplexEntities { get; set; } = null!;
    public DbSet<RelatedEntity> RelatedEntities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SimpleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Name);
        });

        modelBuilder.Entity<ComplexEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<RelatedEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Data).HasMaxLength(500);
            entity.HasIndex(e => e.ComplexEntityId);
            
            entity.HasOne(e => e.ComplexEntity)
                .WithMany(e => e.RelatedEntities)
                .HasForeignKey(e => e.ComplexEntityId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
