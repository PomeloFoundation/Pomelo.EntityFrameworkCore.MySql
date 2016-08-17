// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Migrations.Internal
{
    public class MySqlMigrationsModelDiffer : MigrationsModelDiffer
    {
        private static readonly MethodInfo GetForeignKeysInHierarchyMi = typeof(MigrationsModelDiffer).GetTypeInfo().DeclaredMethods.Single(x => x.Name == "GetForeignKeysInHierarchy");

        private static readonly Type[] _dropOperationTypes =
        {
            typeof(DropIndexOperation),
            typeof(DropPrimaryKeyOperation),
            typeof(DropSequenceOperation),
            typeof(DropUniqueConstraintOperation)
        };

        private static readonly Type[] _alterOperationTypes =
        {
            typeof(AddPrimaryKeyOperation),
            typeof(AddUniqueConstraintOperation),
            typeof(AlterColumnOperation),
            typeof(AlterSequenceOperation),
            typeof(CreateIndexOperation),
            typeof(RestartSequenceOperation)
        };

        private static readonly Type[] _renameOperationTypes =
        {
            typeof(RenameIndexOperation),
            typeof(RenameSequenceOperation)
        };

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlMigrationsModelDiffer(
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] IRelationalAnnotationProvider annotations,
            [NotNull] IMigrationsAnnotationProvider migrationsAnnotations)
            :base(typeMapper, annotations, migrationsAnnotations)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override IReadOnlyList<MigrationOperation> Sort([NotNull] IEnumerable<MigrationOperation> operations, [NotNull] MigrationsModelDiffer.DiffContext diffContext)
        {
            Check.NotNull(operations, nameof(operations));

            var dropForeignKeyOperations = new List<MigrationOperation>();
            var dropOperations = new List<MigrationOperation>();
            var dropColumnOperations = new List<MigrationOperation>();
            var dropTableOperations = new List<DropTableOperation>();
            var ensureSchemaOperations = new List<MigrationOperation>();
            var createSequenceOperations = new List<MigrationOperation>();
            var createTableOperations = new List<CreateTableOperation>();
            var addColumnOperations = new List<MigrationOperation>();
            var alterOperations = new List<MigrationOperation>();
            var addForeignKeyOperations = new List<MigrationOperation>();
            var renameColumnOperations = new List<MigrationOperation>();
            var renameOperations = new List<MigrationOperation>();
            var renameTableOperations = new List<MigrationOperation>();
            var leftovers = new List<MigrationOperation>();

            foreach (var operation in operations)
            {
                var type = operation.GetType();
                if (type == typeof(DropForeignKeyOperation))
                {
                    dropForeignKeyOperations.Add(operation);
                }
                else if (_dropOperationTypes.Contains(type))
                {
                    dropOperations.Add(operation);
                }
                else if (type == typeof(DropColumnOperation))
                {
                    dropColumnOperations.Add(operation);
                }
                else if (type == typeof(DropTableOperation))
                {
                    dropTableOperations.Add((DropTableOperation)operation);
                }
                else if (type == typeof(EnsureSchemaOperation))
                {
                    ensureSchemaOperations.Add(operation);
                }
                else if (type == typeof(CreateSequenceOperation))
                {
                    createSequenceOperations.Add(operation);
                }
                else if (type == typeof(CreateTableOperation))
                {
                    createTableOperations.Add((CreateTableOperation)operation);
                }
                else if (type == typeof(AddColumnOperation))
                {
                    addColumnOperations.Add(operation);
                }
                else if (_alterOperationTypes.Contains(type))
                {
                    alterOperations.Add(operation);
                }
                else if (type == typeof(AddForeignKeyOperation))
                {
                    addForeignKeyOperations.Add(operation);
                }
                else if (type == typeof(RenameColumnOperation))
                {
                    renameColumnOperations.Add(operation);
                }
                else if (_renameOperationTypes.Contains(type))
                {
                    renameOperations.Add(operation);
                }
                else if (type == typeof(RenameTableOperation))
                {
                    renameTableOperations.Add(operation);
                }
                else
                {
                    Debug.Assert(false, "Unexpected operation type: " + operation.GetType());
                    leftovers.Add(operation);
                }
            }

            var createTableGraph = new Multigraph<CreateTableOperation, AddForeignKeyOperation>();
            createTableGraph.AddVertices(createTableOperations);
            foreach (var createTableOperation in createTableOperations)
            {
                foreach (var addForeignKeyOperation in createTableOperation.ForeignKeys)
                {
                    if ((addForeignKeyOperation.Table == addForeignKeyOperation.PrincipalTable)
                        && (addForeignKeyOperation.Schema == addForeignKeyOperation.PrincipalSchema))
                    {
                        continue;
                    }

                    var principalCreateTableOperation = createTableOperations.FirstOrDefault(
                        o => (o.Name == addForeignKeyOperation.PrincipalTable)
                             && (o.Schema == addForeignKeyOperation.PrincipalSchema));
                    if (principalCreateTableOperation != null)
                    {
                        createTableGraph.AddEdge(principalCreateTableOperation, createTableOperation, addForeignKeyOperation);
                    }
                }
            }
            createTableOperations = createTableGraph.TopologicalSort(
                (principalCreateTableOperation, createTableOperation, cyclicAddForeignKeyOperations) =>
                    {
                        foreach (var cyclicAddForeignKeyOperation in cyclicAddForeignKeyOperations)
                        {
                            createTableOperation.ForeignKeys.Remove(cyclicAddForeignKeyOperation);
                            addForeignKeyOperations.Add(cyclicAddForeignKeyOperation);
                        }

                        return true;
                    }).ToList();

            var dropTableGraph = new Multigraph<DropTableOperation, IForeignKey>();
            dropTableGraph.AddVertices(dropTableOperations);
            foreach (var dropTableOperation in dropTableOperations)
            {
                var entityType = diffContext.GetMetadata(dropTableOperation);
                foreach (var foreignKey in (IEnumerable<IForeignKey>)GetForeignKeysInHierarchyMi.Invoke(this, new object[] { entityType }))
                {
                    var principalRootEntityType = foreignKey.PrincipalEntityType.RootType();
                    if (entityType == principalRootEntityType)
                    {
                        continue;
                    }

                    var principalDropTableOperation = diffContext.FindDrop(principalRootEntityType);
                    if (principalDropTableOperation != null)
                    {
                        dropTableGraph.AddEdge(dropTableOperation, principalDropTableOperation, foreignKey);
                    }
                }
            }
            var newDiffContext = new DiffContext();
            dropTableOperations = dropTableGraph.TopologicalSort(
                (dropTableOperation, principalDropTableOperation, foreignKeys) =>
                    {
                        dropForeignKeyOperations.AddRange(foreignKeys.SelectMany(c => Remove(c, newDiffContext)));

                        return true;
                    }).ToList();

            var ret = dropForeignKeyOperations
                .Concat(renameTableOperations)
                .Concat(renameOperations)
                .Concat(renameColumnOperations)
                .Concat(dropOperations)
                .Concat(dropColumnOperations)
                .Concat(dropTableOperations)
                .Concat(ensureSchemaOperations)
                .Concat(createTableOperations)
                .Concat(addColumnOperations)
                .Concat(alterOperations)
                .Concat(addForeignKeyOperations)
                .Concat(leftovers)
                .Concat(createSequenceOperations)
                .ToArray();

            // Fixed #37 Renaming a table while adding a new field/column fails when updating the migration in the database.
            var rename = ret.Where(x => x is RenameTableOperation).ToList();
            foreach(RenameTableOperation rto in rename)
            {
                var fix = ret.Where(x => !(x is RenameTableOperation)).ToList();
                foreach (dynamic x in fix)
                {
                    try { x.Table = x.Table.Replace(rto.Name, rto.NewName); } catch { }
                }
            }
            
            // Fixed Renaming a table will drop and re-create PK issue.
            ret = ret.Where(x => !(x is AddPrimaryKeyOperation && ret.Any(y => y is DropPrimaryKeyOperation && ((DropPrimaryKeyOperation)y).Table == ((AddPrimaryKeyOperation)x).Table)) && !(x is DropPrimaryKeyOperation && ret.Any(y => y is AddPrimaryKeyOperation && ((AddPrimaryKeyOperation)y).Table == ((DropPrimaryKeyOperation)x).Table))).ToArray();

            return ret; 
        }
    }
}
