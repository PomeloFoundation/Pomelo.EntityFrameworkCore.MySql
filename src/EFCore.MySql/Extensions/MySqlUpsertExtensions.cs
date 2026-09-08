// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using MySqlConnector;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// UPSERT extensions using MySQL's INSERT ... ON DUPLICATE KEY UPDATE.
    /// </summary>
    public static class MySqlUpsertExtensions
    {
        // ──────────────────────────────────────────────────────────────
        // Single-entity UPSERT
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Inserts an entity, or updates if a duplicate key conflict occurs.
        /// </summary>
        /// <param name="context">The DbContext.</param>
        /// <param name="entity">The entity to upsert.</param>
        /// <param name="keyColumnNames">
        /// Property names that form the unique key. These are excluded from the
        /// UPDATE clause on conflict. MySQL detects duplicates automatically
        /// based on the table's unique indexes — this parameter only controls
        /// which columns are NOT updated.
        /// </param>
        /// <param name="updateColumns">
        /// Property names to update on conflict. If null, all non-key columns.
        /// </param>
        /// <returns>Affected rows (1=insert, 2=update).</returns>
        public static Task<int> UpsertAsync<TEntity>(
            this DbContext context,
            TEntity entity,
            string[] keyColumnNames,
            string[] updateColumns = null)
            where TEntity : class
            => context.UpsertAsync(entity, keyColumnNames, updateColumns, CancellationToken.None);

        /// <summary>
        /// Inserts an entity, or updates if a duplicate key conflict occurs.
        /// </summary>
        public static async Task<int> UpsertAsync<TEntity>(
            this DbContext context,
            TEntity entity,
            string[] keyColumnNames,
            string[] updateColumns,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var (sql, parameters) = BuildUpsertSql(
                context, entity, keyColumnNames, updateColumns, prefix: "p");
            return await context.Database.ExecuteSqlRawAsync(
                sql, parameters, cancellationToken);
        }

        // ──────────────────────────────────────────────────────────────
        // Bulk UPSERT (single SQL with multiple VALUES rows)
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Bulk upsert. All entities are upserted in a single SQL statement
        /// using multi-row VALUES.
        /// </summary>
        public static Task<int> UpsertRangeAsync<TEntity>(
            this DbContext context,
            IEnumerable<TEntity> entities,
            string[] keyColumnNames,
            string[] updateColumns = null)
            where TEntity : class
            => context.UpsertRangeAsync(entities, keyColumnNames, updateColumns, CancellationToken.None);

        /// <summary>
        /// Bulk upsert. All entities are upserted in a single SQL statement
        /// using multi-row VALUES.
        /// </summary>
        public static async Task<int> UpsertRangeAsync<TEntity>(
            this DbContext context,
            IEnumerable<TEntity> entities,
            string[] keyColumnNames,
            string[] updateColumns,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var entityList = entities as IList<TEntity> ?? entities.ToList();
            if (entityList.Count == 0)
                return 0;

            var (sql, parameters) = BuildBulkUpsertSql(
                context, entityList, keyColumnNames, updateColumns);
            return await context.Database.ExecuteSqlRawAsync(
                sql, parameters, cancellationToken);
        }

        // ──────────────────────────────────────────────────────────────
        // Internal helpers
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds UPSERT SQL for a single entity using MySQL 8.0+ alias syntax:
        ///   INSERT INTO t (cols) VALUES (@p0, @p1) AS new
        ///   ON DUPLICATE KEY UPDATE col = new.col
        /// </summary>
        private static (string Sql, IReadOnlyList<MySqlParameter> Parameters)
            BuildUpsertSql<TEntity>(
                DbContext context,
                TEntity entity,
                string[] keyColumnNames,
                string[] updateColumns,
                string prefix)
        {
            var (tableName, properties, updProps) =
                ResolveMetadata(context, entity, keyColumnNames, updateColumns);

            var colNames = properties
                .Select(p => $"`{Escape(p.GetColumnName())}`")
                .ToArray();
            var paramNames = properties
                .Select((_, i) => $"@{prefix}{i}")
                .ToArray();

            var updateClauses = updProps.Select(p =>
            {
                var col = Escape(p.GetColumnName());
                return $"`{col}` = new.`{col}`";
            });

            var parameters = new List<MySqlParameter>();
            for (var i = 0; i < properties.Count; i++)
            {
                var value = GetProviderValue(properties[i], entity);
                parameters.Add(new MySqlParameter($"@{prefix}{i}", value ?? DBNull.Value));
            }

            var sql =
                $"INSERT INTO `{Escape(tableName)}` ({string.Join(", ", colNames)}) " +
                $"VALUES ({string.Join(", ", paramNames)}) AS new " +
                $"ON DUPLICATE KEY UPDATE {string.Join(", ", updateClauses)}";

            return (sql, parameters);
        }

        /// <summary>
        /// Builds bulk UPSERT SQL for multiple entities in a single statement:
        ///   INSERT INTO t (cols) VALUES (@p0_0, @p0_1), (@p1_0, @p1_1) AS new
        ///   ON DUPLICATE KEY UPDATE col = new.col
        /// </summary>
        private static (string Sql, IReadOnlyList<MySqlParameter> Parameters)
            BuildBulkUpsertSql<TEntity>(
                DbContext context,
                IList<TEntity> entities,
                string[] keyColumnNames,
                string[] updateColumns)
        {
            var (tableName, properties, updProps) =
                ResolveMetadata(context, entities[0], keyColumnNames, updateColumns);

            var colNames = properties
                .Select(p => $"`{Escape(p.GetColumnName())}`")
                .ToArray();

            var parameters = new List<MySqlParameter>();
            var valueRows = new List<string>(entities.Count);

            for (var row = 0; row < entities.Count; row++)
            {
                var paramNames = new List<string>(properties.Count);
                for (var col = 0; col < properties.Count; col++)
                {
                    var paramName = $"@p{row}_{col}";
                    paramNames.Add(paramName);
                    var value = GetProviderValue(properties[col], entities[row]);
                    parameters.Add(new MySqlParameter(paramName, value ?? DBNull.Value));
                }
                valueRows.Add($"({string.Join(", ", paramNames)})");
            }

            var updateClauses = updProps.Select(p =>
            {
                var col = Escape(p.GetColumnName());
                return $"`{col}` = new.`{col}`";
            });

            var sql =
                $"INSERT INTO `{Escape(tableName)}` ({string.Join(", ", colNames)}) " +
                $"VALUES {string.Join(", ", valueRows)} AS new " +
                $"ON DUPLICATE KEY UPDATE {string.Join(", ", updateClauses)}";

            return (sql, parameters);
        }

        /// <summary>
        /// Resolves entity metadata: table name, non-shadow properties, and
        /// the subset of properties to update on conflict.
        /// </summary>
        private static (string TableName, List<IProperty> Properties, List<IProperty> UpdateProps)
            ResolveMetadata<TEntity>(
                DbContext context,
                TEntity entity,
                string[] keyColumnNames,
                string[] updateColumns)
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
                throw new InvalidOperationException(
                    $"Entity type {typeof(TEntity).Name} not found.");

            var tableName = entityType.GetTableName();
            var properties = entityType.GetProperties()
                .Where(p => !p.IsShadowProperty())
                .ToList();

            var keySet = new HashSet<string>(keyColumnNames);

            // updateColumns are property names; resolve to IProperty so we
            // can call GetColumnName() when building SQL (fixes P0-2).
            var updProps = updateColumns != null
                ? properties.Where(p => updateColumns.Contains(p.Name)).ToList()
                : properties.Where(p => !keySet.Contains(p.Name)).ToList();

            return (tableName, properties, updProps);
        }

        /// <summary>
        /// Gets the provider value for a property, applying any configured
        /// ValueConverter so the value matches what the database expects
        /// (fixes P2-7). Checks both property-level and type-mapping-level
        /// converters (e.g. enum → string via HasConversion&lt;string&gt;()).
        /// </summary>
        private static object GetProviderValue(IProperty property, object entity)
        {
            var clrValue = property.PropertyInfo?.GetValue(entity);
            if (clrValue == null)
                return null;

            var converter = property.GetValueConverter() ??
                            property.FindTypeMapping()?.Converter;
            if (converter != null)
                return converter.ConvertToProvider(clrValue);

            return clrValue;
        }

        /// <summary>
        /// Escapes a backtick for use in a MySQL identifier by doubling it.
        /// </summary>
        private static string Escape(string identifier)
            => identifier.Replace("`", "``");
    }
}
