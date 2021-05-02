using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlApiConsistencyTest : ApiConsistencyTestBase<MySqlApiConsistencyTest.MySqlApiConsistencyFixture>
    {
        public MySqlApiConsistencyTest(MySqlApiConsistencyFixture fixture)
            : base(fixture)
        {
        }

        protected override void AddServices(ServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySql();

        protected override Assembly TargetAssembly => typeof(MySqlRelationalConnection).Assembly;

        public class MySqlApiConsistencyFixture : ApiConsistencyFixtureBase
        {
            public override HashSet<Type> FluentApiTypes { get; } = new()
            {
                typeof(MySqlDbContextOptionsBuilder),
                typeof(MySqlDbContextOptionsBuilderExtensions),
                typeof(MySqlMigrationBuilderExtensions),
                typeof(MySqlIndexBuilderExtensions),
                typeof(MySqlModelBuilderExtensions),
                typeof(MySqlPropertyBuilderExtensions),
                typeof(MySqlEntityTypeBuilderExtensions),
                typeof(MySqlServiceCollectionExtensions)
            };

            public override
                List<(Type Type,
                    Type ReadonlyExtensions,
                    Type MutableExtensions,
                    Type ConventionExtensions,
                    Type ConventionBuilderExtensions,
                    Type RuntimeExtensions)> MetadataExtensionTypes { get; }
                = new()
                {
                    (
                        typeof(IReadOnlyModel),
                        typeof(MySqlModelExtensions),
                        typeof(MySqlModelExtensions),
                        typeof(MySqlModelExtensions),
                        typeof(MySqlModelBuilderExtensions),
                        null
                    ),
                    (
                        typeof(IReadOnlyEntityType),
                        typeof(MySqlEntityTypeExtensions),
                        typeof(MySqlEntityTypeExtensions),
                        typeof(MySqlEntityTypeExtensions),
                        typeof(MySqlEntityTypeBuilderExtensions),
                        null
                    ),
                    (
                        typeof(IReadOnlyProperty),
                        typeof(MySqlPropertyExtensions),
                        typeof(MySqlPropertyExtensions),
                        typeof(MySqlPropertyExtensions),
                        typeof(MySqlPropertyBuilderExtensions),
                        null
                    ),
                    (
                        typeof(IReadOnlyIndex),
                        typeof(MySqlIndexExtensions),
                        typeof(MySqlIndexExtensions),
                        typeof(MySqlIndexExtensions),
                        typeof(MySqlIndexBuilderExtensions),
                        null
                    )
                };

            public override HashSet<MethodInfo> UnmatchedMetadataMethods { get; } = new()
            {
                typeof(MySqlModelBuilderExtensions).GetMethod(
                    nameof(MySqlModelBuilderExtensions.UseCollation),
                    new[] {typeof(IConventionModelBuilder), typeof(string), typeof(DelegationModes?), typeof(bool)}),
                typeof(MySqlModelBuilderExtensions).GetMethod(
                    nameof(MySqlModelBuilderExtensions.UseCollation),
                    new[] {typeof(IConventionModelBuilder), typeof(string), typeof(bool?), typeof(bool)}),
            };
        }
    }
}
