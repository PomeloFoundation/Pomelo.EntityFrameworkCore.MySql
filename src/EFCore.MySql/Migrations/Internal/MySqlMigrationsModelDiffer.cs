// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
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

        protected override IEnumerable<MigrationOperation> Add(IRelationalModel target, DiffContext diffContext)
            => ApplyMySqlSpecificRelationalCollationAnnotations(base.Add(target, diffContext));

        protected override IEnumerable<MigrationOperation> Diff(IRelationalModel source, IRelationalModel target, DiffContext diffContext)
            => ApplyMySqlSpecificRelationalCollationAnnotations(base.Diff(source, target, diffContext));

        // CHECK: Can we somehow get rid of this, by moving it somewhere else (e.g. to MySqlMigrationsSqlGenerator)?
        protected override IEnumerable<MigrationOperation> Add(IColumn target, DiffContext diffContext, bool inline = false)
        {
            if (!inline)
            {
                foreach (var propertyMapping in target.PropertyMappings)
                {
                    if (propertyMapping.Property.FindTypeMapping() is RelationalTypeMapping storeType)
                    {
                        var valueGenerationStrategy = MySqlValueGenerationStrategyCompatibility.GetValueGenerationStrategy(
                            target.GetAnnotations()
                                .ToArray());

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

                        if (inline)
                        {
                            break;
                        }
                    }
                }
            }

            return ApplyMySqlSpecificRelationalCollationAnnotations(base.Add(target, diffContext, inline));
        }

        protected override IEnumerable<MigrationOperation> Diff(IColumn source, IColumn target, DiffContext diffContext)
            => ApplyMySqlSpecificRelationalCollationAnnotations(base.Diff(source, target, diffContext));

        protected virtual IEnumerable<MigrationOperation> ApplyMySqlSpecificRelationalCollationAnnotations(
            IEnumerable<MigrationOperation> migrationOperations)
        {
            // This is not strictly necessary. Without applying the (delegated) collations, that were generated in MySqlAnnotationProvider,
            // the MySqlMigrationsSqlGenerator implementation for column and database operations would need to not only use the `Collation`
            // property, but also explicitly check the `Relational:Collation` annotation.
            foreach (var migrationOperation in migrationOperations)
            {
                if (migrationOperation is ColumnOperation columnOperation &&
                    columnOperation[RelationalAnnotationNames.Collation] is string columnCollation)
                {
                    columnOperation.Collation ??= columnCollation;
                    columnOperation.RemoveAnnotation(RelationalAnnotationNames.Collation);

                    // CHECK: Can this condition be true?
                    if (migrationOperation is AlterColumnOperation alterColumnOperation &&
                        alterColumnOperation.OldColumn[RelationalAnnotationNames.Collation] is string oldColumnCollation)
                    {
                        alterColumnOperation.OldColumn.Collation ??= oldColumnCollation;
                        alterColumnOperation.OldColumn.RemoveAnnotation(RelationalAnnotationNames.Collation);
                    }
                }
                else if (migrationOperation is DatabaseOperation databaseOperation &&
                         databaseOperation[RelationalAnnotationNames.Collation] is string databaseCollation)
                {
                    databaseOperation.Collation ??= databaseCollation;
                    databaseOperation.RemoveAnnotation(RelationalAnnotationNames.Collation);

                    // CHECK: Can this condition be true?
                    if (migrationOperation is AlterDatabaseOperation alterDatabaseOperation &&
                        alterDatabaseOperation.OldDatabase[RelationalAnnotationNames.Collation] is string oldColumnCollation)
                    {
                        alterDatabaseOperation.OldDatabase.Collation ??= oldColumnCollation;
                        alterDatabaseOperation.OldDatabase.RemoveAnnotation(RelationalAnnotationNames.Collation);
                    }
                }

                yield return migrationOperation;
            }
        }
    }
}
