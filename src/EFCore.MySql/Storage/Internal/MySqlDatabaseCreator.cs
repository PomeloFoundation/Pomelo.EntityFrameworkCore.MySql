// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly IMySqlRelationalConnection _connection;
	    private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;


	    /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
	    public MySqlDatabaseCreator(
            [NotNull] RelationalDatabaseCreatorDependencies dependencies,
            [NotNull] IMySqlRelationalConnection connection,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder)
            : base(dependencies)
        {
            _connection = connection;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
        }

        public virtual TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

        public virtual TimeSpan RetryTimeout { get; set; } = TimeSpan.FromMinutes(1);

	    public override void Create()
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                Dependencies.MigrationCommandExecutor
                    .ExecuteNonQuery(CreateCreateOperations(), masterConnection);

                ClearPool();
            }

            Exists(retryOnNotExists: true);
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await Dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(CreateCreateOperations(), masterConnection, cancellationToken).ConfigureAwait(false);

                ClearPool();
            }

            await ExistsAsync(retryOnNotExists: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        protected override bool HasTables()
            => Dependencies.ExecutionStrategyFactory.Create().Execute(_connection, connection => Convert.ToInt64(CreateHasTablesCommand().ExecuteScalar(connection)) != 0);

        protected override Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(_connection,
                async (connection, ct) => Convert.ToInt64(await CreateHasTablesCommand().ExecuteScalarAsync(connection, cancellationToken: ct).ConfigureAwait(false)) != 0, cancellationToken);

        private IRelationalCommand CreateHasTablesCommand()
            => _rawSqlCommandBuilder
                .Build(@"
                    SELECT CASE WHEN COUNT(*) = 0 THEN FALSE ELSE TRUE END
                    FROM information_schema.tables
                    WHERE table_type = 'BASE TABLE' AND table_schema = '" + _connection.DbConnection.Database + "'");

        private IReadOnlyList<MigrationCommand> CreateCreateOperations()
        {
            return Dependencies.MigrationsSqlGenerator.Generate((new[] { new MySqlCreateDatabaseOperation { Name = _connection.DbConnection.Database } }));
        }

        public override bool Exists()
            => Exists(retryOnNotExists: false);

        private bool Exists(bool retryOnNotExists)
            => Dependencies.ExecutionStrategyFactory.Create().Execute(DateTime.UtcNow + RetryTimeout, giveUp =>
            {
                while (true)
                {
                    try
                    {
                        try
                        {
                            using (var masterConnection = _connection.CreateMasterConnection())
                            {
                                masterConnection.Open();
                                using (var cmd = masterConnection.DbConnection.CreateCommand())
                                {
                                    cmd.CommandText = $"USE `{_connection.DbConnection.Database}`";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (MySqlException e)
                        {
                            if (e.Number != 1045) // Access denied because credentials were lost
                            {
                                throw;
                            }

                            _connection.Open(errorsExpected: true);

                            _connection.Close();
                        }
                        return true;
                    }
                    catch (MySqlException e)
                    {
                        if (!retryOnNotExists && IsDoesNotExist(e))
                            return false;

                        if (DateTime.UtcNow > giveUp || !RetryOnExistsFailure(e))
                            throw;

                        Thread.Sleep(RetryDelay);
                    }
                }
            });

        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => ExistsAsync(retryOnNotExists: false, cancellationToken: cancellationToken);

        private Task<bool> ExistsAsync(bool retryOnNotExists, CancellationToken cancellationToken)
            => Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(DateTime.UtcNow + RetryTimeout, async (giveUp, ct) =>
            {
                while (true)
                {
                    try
                    {
                        try
                        {
                            using (var masterConnection = _connection.CreateMasterConnection())
                            {
                                await masterConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
                                using (var cmd = masterConnection.DbConnection.CreateCommand())
                                {
                                    cmd.CommandText = $"USE `{_connection.DbConnection.Database}`";
                                    await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (MySqlException e)
                        {
                            if (e.Number != 1045) // Access denied because credentials were lost
                            {
                                throw;
                            }

                            await _connection.OpenAsync(ct, errorsExpected: true);

                            _connection.Close();
                        }
                        return true;
                    }
                    catch (MySqlException e)
                    {
                        if (!retryOnNotExists && IsDoesNotExist(e))
                            return false;

                        if (DateTime.UtcNow > giveUp || !RetryOnExistsFailure(e))
                            throw;

                        await Task.Delay(RetryDelay, ct).ConfigureAwait(false);
                    }
                }
            }, cancellationToken);

        private static bool IsDoesNotExist(MySqlException exception) => exception.Number == 1049;

        private bool RetryOnExistsFailure(MySqlException exception)
        {
            if (exception.Number == 1049)
            {
                ClearPool();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void Delete()
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                Dependencies.MigrationCommandExecutor
                    .ExecuteNonQuery(CreateDropCommands(), masterConnection);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await Dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(CreateDropCommands(), masterConnection, cancellationToken).ConfigureAwait(false);
            }
        }

        private IReadOnlyList<MigrationCommand> CreateDropCommands()
        {
            var operations = new MigrationOperation[]
            {
                // TODO Check DbConnection.Database always gives us what we want
                // Issue #775
                new MySqlDropDatabaseOperation { Name = _connection.DbConnection.Database }
            };

            var masterCommands = Dependencies.MigrationsSqlGenerator.Generate(operations);
            return masterCommands;
        }

        // Clear connection pools in case there are active connections that are pooled
        private static void ClearAllPools() => MySqlConnection.ClearAllPools();

        // Clear connection pool for the database connection since after the 'create database' call, a previously
        // invalid connection may now be valid.
        private void ClearPool() => MySqlConnection.ClearPool(_connection.DbConnection as MySqlConnection);
    }
}
