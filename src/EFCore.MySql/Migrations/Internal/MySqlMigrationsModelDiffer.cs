// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using Pomelo.EntityFrameworkCore.MySql.Metadata.Internal;

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
            => PostFilterOperations(base.Add(target, diffContext));

        protected override IEnumerable<MigrationOperation> Diff(IRelationalModel source, IRelationalModel target, DiffContext diffContext)
            => PostFilterOperations(base.Diff(source, target, diffContext));

        protected override IEnumerable<MigrationOperation> Add(ITable target, DiffContext diffContext)
            => PostFilterOperations(base.Add(target, diffContext));

        protected override IEnumerable<MigrationOperation> Diff(ITable source, ITable target, DiffContext diffContext)
            => PostFilterOperations(base.Diff(source, target, diffContext));

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

            return PostFilterOperations(base.Add(target, diffContext, inline));
        }

        protected override IEnumerable<MigrationOperation> Diff(IColumn source, IColumn target, DiffContext diffContext)
            => PostFilterOperations(base.Diff(source, target, diffContext));

        protected virtual IEnumerable<MigrationOperation> PostFilterOperations(IEnumerable<MigrationOperation> migrationOperations)
        {
            foreach (var migrationOperation in migrationOperations)
            {
                var resultOperation = migrationOperation switch
                {
                    AlterDatabaseOperation operation => PostFilterOperation(operation),
                    CreateTableOperation operation => PostFilterOperation(operation),
                    AlterTableOperation operation => PostFilterOperation(operation),
                    AddColumnOperation operation => PostFilterOperation(operation),
                    AlterColumnOperation operation => PostFilterOperation(operation),
                    _ => migrationOperation
                };

                if (resultOperation != null)
                {
                    yield return resultOperation;
                }
            }
        }

        protected virtual AlterDatabaseOperation PostFilterOperation(AlterDatabaseOperation operation)
        {
            HandleCharSetDelegation(operation, DelegationModes.ApplyToDatabases);

            ApplyCollationAnnotation(operation, (operation, collation) => operation.Collation ??= collation);
            ApplyCollationAnnotation(operation.OldDatabase, (operation, collation) => operation.Collation ??= collation);
            HandleCollationDelegation(operation, DelegationModes.ApplyToDatabases, o => o.Collation = null);

            // Ensure, that no properties have been added by the EF Core team in the meantime.
            // If they have, they need to be checked below.
            AssertMigrationOperationProperties(
                operation,
                new[]
                {
                    nameof(AlterDatabaseOperation.OldDatabase),
                    nameof(AlterDatabaseOperation.Collation),
                });

            // Ensure, that this hasn't become an empty operation.
            return operation.Collation != operation.OldDatabase.Collation ||
                   operation.IsReadOnly != operation.OldDatabase.IsReadOnly ||
                   HasDifferences(operation.GetAnnotations(), operation.OldDatabase.GetAnnotations())
                ? operation
                : null;
        }

        protected virtual CreateTableOperation PostFilterOperation(CreateTableOperation operation)
        {
            HandleCharSetDelegation(operation, DelegationModes.ApplyToTables);
            HandleCollationDelegation(operation, DelegationModes.ApplyToTables);

            for (var i = 0; i < operation.Columns.Count; i++)
            {
                var oldColumn = operation.Columns[i];
                var newColumn = PostFilterOperation(oldColumn);

                if (newColumn != oldColumn)
                {
                    operation.Columns[i] = newColumn;
                }
            }

            return operation;
        }

        protected virtual AlterTableOperation PostFilterOperation(AlterTableOperation operation)
        {
            HandleCharSetDelegation(operation, DelegationModes.ApplyToTables);
            HandleCollationDelegation(operation, DelegationModes.ApplyToTables);

            // Ensure, that no properties have been added by the EF Core team in the meantime.
            // If they have, they need to be checked below.
            AssertMigrationOperationProperties(
                operation,
                new[]
                {
                    nameof(AlterTableOperation.OldTable),
                    nameof(AlterTableOperation.Name),
                    nameof(AlterTableOperation.Schema),
                    nameof(AlterTableOperation.Comment),
                });

            // Ensure, that this hasn't become an empty operation.
            // We do not check Name and Schema, because changes would have resulted in a RenameTableOperation already.
            return operation.Comment != operation.OldTable.Comment ||
                   HasDifferences(operation.GetAnnotations(), operation.OldTable.GetAnnotations())
                ? operation
                : null;
        }

        protected virtual AddColumnOperation PostFilterOperation(AddColumnOperation operation)
        {
            ApplyCollationAnnotation(operation, (operation, collation) => operation.Collation ??= collation);

            return operation;
        }

        protected virtual AlterColumnOperation PostFilterOperation(AlterColumnOperation operation)
        {
            ApplyCollationAnnotation(operation, (operation, collation) => operation.Collation ??= collation);

            return operation;
        }

        private static void ApplyCollationAnnotation<TOperation>(TOperation operation, Action<TOperation, string> applyCollation)
            where TOperation : MigrationOperation
        {
            if (operation[RelationalAnnotationNames.Collation] is string collation)
            {
                operation.RemoveAnnotation(RelationalAnnotationNames.Collation);
                applyCollation(operation, collation);
            }
        }

        private static void HandleCollationDelegation<TOperation>(
            TOperation operation,
            DelegationModes delegationModes,
            Action<TOperation> resetCollationProperty = null)
            where TOperation : MigrationOperation
        {
            // If the database collation should not be applied to the database itself, we need to reset the Collation property.
            // If the CollationDelegation annotation does not exist, it is ApplyToAll implicitly.
            if (operation[MySqlAnnotationNames.CollationDelegation] is DelegationModes databaseCollationDelegation)
            {
                // Don't leak the CollationDelegation annotation to the MigrationOperation.
                operation[MySqlAnnotationNames.CollationDelegation] = null;

                if (!databaseCollationDelegation.HasFlag(delegationModes))
                {
                    if (resetCollationProperty == null)
                    {
                        operation[RelationalAnnotationNames.Collation] = null;
                    }
                    else
                    {
                        resetCollationProperty(operation);
                    }
                }
            }
        }

        private static void HandleCharSetDelegation(MigrationOperation operation, DelegationModes delegationModes)
        {
            // If the character set should not be applied to the database itself, we need to remove the CharSet annotation.
            // If the CharSetDelegation annotation does not exist, it is ApplyToAll implicitly.
            if (operation[MySqlAnnotationNames.CharSetDelegation] is DelegationModes charSetDelegation)
            {
                // Don't leak the CharSetDelegation annotation to the MigrationOperation.
                operation[MySqlAnnotationNames.CharSetDelegation] = null;

                if (!charSetDelegation.HasFlag(delegationModes))
                {
                    operation[MySqlAnnotationNames.CharSet] = null;
                }
            }
        }

        [Conditional("DEBUG")]
        private static void AssertMigrationOperationProperties(MigrationOperation operation, IEnumerable<string> propertyNames)
        {
            if (operation.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => p.Name)
                .Except(
                    propertyNames.Concat(
                        new[]
                        {
                            "Item",
                            nameof(AlterDatabaseOperation.IsReadOnly),
                            nameof(MigrationOperation.IsDestructiveChange)
                        }))
                .FirstOrDefault() is string unexpectedProperty)
            {
                throw new InvalidOperationException(
                    $"The 'MigrationOperation' of type '{operation.GetType().Name}' contains an unexpected property '{unexpectedProperty}'.");
            }
        }
    }
}
