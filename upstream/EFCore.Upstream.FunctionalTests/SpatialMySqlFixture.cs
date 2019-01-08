// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
#if !Test21
    public class SpatialMySqlFixture : SpatialFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            => base.AddServices(serviceCollection)
                .AddEntityFrameworkMySqlNetTopologySuite();

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new MySqlDbContextOptionsBuilder(optionsBuilder).UseNetTopologySuite();

            return optionsBuilder;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<LineStringEntity>().Property(e => e.LineString).HasColumnType("geometry");
            modelBuilder.Entity<MultiLineStringEntity>().Property(e => e.MultiLineString).HasColumnType("geometry");
            modelBuilder.Entity<PointEntity>(
                x =>
                {
                    x.Property(e => e.Geometry).HasColumnType("geometry");
                    x.Property(e => e.Point).HasColumnType("geometry");
                    x.Property(e => e.ConcretePoint).HasColumnType("geometry");
                });
            modelBuilder.Entity<PolygonEntity>().Property(e => e.Polygon).HasColumnType("geometry");
            modelBuilder.Entity<GeoPointEntity>().Property(e => e.Location).HasColumnType("geometry");
        }
    }
#endif
}
