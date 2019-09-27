using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.SpatialModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SpatialQueryMySqlFixture : SpatialQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            => base.AddServices(serviceCollection)
                //.AddEntityFrameworkMySqlNetTopologySuite()
                .AddSingleton<IRelationalTypeMappingSource, ReplacementTypeMappingSource>();

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new MySqlDbContextOptionsBuilder(optionsBuilder)/*.UseNetTopologySuite()*/;

            return optionsBuilder;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.HasDbFunction(
                typeof(GeoExtensions).GetMethod(nameof(GeoExtensions.Distance)),
                b => b.HasTranslation(
                    e => SqlFunctionExpression.Create(
                        "Distance",
                        e,
                        typeof(double),
                        null)));
        }

        private class ReplacementTypeMappingSource : MySqlTypeMappingSource
        {
            public ReplacementTypeMappingSource(
                TypeMappingSourceDependencies dependencies,
                RelationalTypeMappingSourceDependencies relationalDependencies,
                IMySqlOptions options)
                : base(dependencies, relationalDependencies, options)
            {
            }

            protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
                => mappingInfo.ClrType == typeof(GeoPoint)
                    ? ((RelationalTypeMapping)base.FindMapping(typeof(Point))
                        .Clone(new GeoPointConverter()))
                    .Clone("geometry", null)
                    : base.FindMapping(mappingInfo);
        }
    }
}
