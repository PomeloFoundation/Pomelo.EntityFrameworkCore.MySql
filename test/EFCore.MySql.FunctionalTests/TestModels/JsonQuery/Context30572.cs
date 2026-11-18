// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore.Query;

public class Context30572 : DbContext
{
    public Context30572(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }

    public void Seed()
    {
        var entity1 = new TestEntity { Id = 1 };
        var entity2 = new TestEntity { Id = 2 };
        var entity3 = new TestEntity { Id = 3 };

        TestEntities.AddRange(entity1, entity2, entity3);
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>(b =>
        {
            b.ToTable("TestEntity");
            b.HasKey(e => e.Id);
        });
    }

    public class TestEntity
    {
        public int Id { get; set; }
    }
}
