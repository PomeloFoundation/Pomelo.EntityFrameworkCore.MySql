using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations.Internal
{
    public class MySqlMigrationsModelDiffer : MigrationsModelDiffer
    {
        public MySqlMigrationsModelDiffer(
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] IMigrationsAnnotationProvider migrationsAnnotations,
            [NotNull] IChangeDetector changeDetector,
            [NotNull] IUpdateAdapterFactory updateAdapterFactory,
            [NotNull] CommandBatchPreparerDependencies commandBatchPreparerDependencies)
            : base(
                typeMappingSource,
                migrationsAnnotations,
                changeDetector,
                updateAdapterFactory,
                commandBatchPreparerDependencies)
        {
        }

        protected override IEnumerable<MigrationOperation> Add(IProperty target, DiffContext diffContext, bool inline = false)
        {
            if (target.FindTypeMapping() is RelationalTypeMapping storeType)
            {
                var valueGenerationStrategy = MySqlValueGenerationStrategyCompatibility.GetValueGenerationStrategy(MigrationsAnnotations.For(target).ToArray());

                // Ensure that null will be set for the columns default value, if CURRENT_TIMESTAMP has been required,
                // or when the store type of the column does not support default values at all.
                inline = inline ||
                         (storeType.StoreTypeNameBase == "datetime" ||
                          storeType.StoreTypeNameBase == "timestamp") &&
                         (valueGenerationStrategy == MySqlValueGenerationStrategy.IdentityColumn ||
                          valueGenerationStrategy == MySqlValueGenerationStrategy.ComputedColumn) ||
                         storeType.StoreTypeNameBase.Contains("text") ||
                         storeType.StoreTypeNameBase.Contains("blob") ||
                         storeType.StoreTypeNameBase == "geometry" ||
                         storeType.StoreTypeNameBase == "json";
            }

            return base.Add(target, diffContext, inline);
        }
    }
}
