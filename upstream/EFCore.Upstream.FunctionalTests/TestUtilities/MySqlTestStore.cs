// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;

#pragma warning disable IDE0022 // Use block body for methods
// ReSharper disable SuggestBaseTypeForParameter
namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class MySqlTestStore : RelationalTestStore
    {
        public const int CommandTimeout = 600;
        private static string CurrentDirectory => Environment.CurrentDirectory;

        public static MySqlTestStore GetNorthwindStore()
            => (MySqlTestStore)MySqlNorthwindTestStoreFactory.Instance
                .GetOrCreate(MySqlNorthwindTestStoreFactory.Name).Initialize(null, (Func<DbContext>)null, null, null);

        public static MySqlTestStore GetOrCreate(string name)
            => new MySqlTestStore(name);

        public static MySqlTestStore GetOrCreateInitialized(string name)
            => new MySqlTestStore(name).InitializeMySql(null, (Func<DbContext>)null, null);

        public static MySqlTestStore GetOrCreate(string name, string scriptPath)
            => new MySqlTestStore(name, scriptPath: scriptPath);

        public static MySqlTestStore Create(string name, bool useFileName = false)
            => new MySqlTestStore(name, useFileName, shared: false);

        public static MySqlTestStore CreateInitialized(string name, bool useFileName = false, bool? multipleActiveResultSets = null)
            => new MySqlTestStore(name, useFileName, shared: false, multipleActiveResultSets: multipleActiveResultSets)
                .InitializeMySql(null, (Func<DbContext>)null, null);

        private readonly string _fileName;
        private readonly string _scriptPath;

        private MySqlTestStore(
            string name,
            bool useFileName = false,
            bool? multipleActiveResultSets = null,
            string scriptPath = null,
            bool shared = true)
            : base(name, shared)
        {
            if (useFileName)
            {
                _fileName = Path.Combine(CurrentDirectory, name + ".mdf");
            }

            if (scriptPath != null)
            {
                _scriptPath = Path.Combine(Path.GetDirectoryName(typeof(MySqlTestStore).GetTypeInfo().Assembly.Location), scriptPath);
            }

            ConnectionString = CreateConnectionString(Name, _fileName, multipleActiveResultSets);
            Connection = new SqlConnection(ConnectionString);
        }

        public MySqlTestStore InitializeMySql(
            IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed)
            => (MySqlTestStore)Initialize(serviceProvider, createContext, seed, null);

        public MySqlTestStore InitializeMySql(
            IServiceProvider serviceProvider, Func<MySqlTestStore, DbContext> createContext, Action<DbContext> seed)
            => InitializeMySql(serviceProvider, () => createContext(this), seed);

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
        {
            if (CreateDatabase(clean))
            {
                if (_scriptPath != null)
                {
                    ExecuteScript(_scriptPath);
                }
                else
                {
                    using (var context = createContext())
                    {
                        context.Database.EnsureCreatedResiliently();
                        seed?.Invoke(context);
                    }
                }
            }
        }

        public virtual DbContextOptionsBuilder AddProviderOptions(
            DbContextOptionsBuilder builder,
            Action<MySqlDbContextOptionsBuilder> configureMySql)
            => builder.UseMySql(Connection, b => configureMySql?.Invoke(b));

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => AddProviderOptions(builder, configureMySql: null);

        private bool CreateDatabase(Action<DbContext> clean)
        {
            using (var master = new SqlConnection(CreateConnectionString("master", fileName: null, multipleActiveResultSets: false)))
            {
                if (ExecuteScalar<int>(master, $"SELECT COUNT(*) FROM sys.databases WHERE name = N'{Name}'") > 0)
                {
                    if (_scriptPath != null)
                    {
                        return false;
                    }

                    if (_fileName == null)
                    {
                        using (var context = new DbContext(
                            AddProviderOptions(
                                    new DbContextOptionsBuilder()
                                        .EnableServiceProviderCaching(false))
                                .Options))
                        {
                            clean?.Invoke(context);
                            Clean(context);
                            return true;
                        }
                    }

                    // Delete the database to ensure it's recreated with the correct file path
                    DeleteDatabase();
                }

                ExecuteNonQuery(master, GetCreateDatabaseStatement(Name, _fileName));
                WaitForExists((SqlConnection)Connection);
            }

            return true;
        }

        public override void Clean(DbContext context)
            => context.Database.EnsureClean();

        public void ExecuteScript(string scriptPath)
        {
            var script = File.ReadAllText(scriptPath);
            Execute(
                Connection, command =>
                {
                    foreach (var batch in
                        new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0))
                            .Split(script).Where(b => !string.IsNullOrEmpty(b)))
                    {
                        command.CommandText = batch;
                        command.ExecuteNonQuery();
                    }

                    return 0;
                }, "");
        }

        private static void WaitForExists(SqlConnection connection)
        {
            if (TestEnvironment.IsSqlAzure)
            {
                new TestMySqlRetryingExecutionStrategy().Execute(connection, WaitForExistsImplementation);
            }
            else
            {
                WaitForExistsImplementation(connection);
            }
        }

        private static void WaitForExistsImplementation(SqlConnection connection)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }

                    SqlConnection.ClearPool(connection);

                    connection.Open();
                    connection.Close();
                    return;
                }
                catch (SqlException e)
                {
                    if (++retryCount >= 30
                        || e.Number != 233 && e.Number != -2 && e.Number != 4060 && e.Number != 1832 && e.Number != 5120)
                    {
                        throw;
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private static string GetCreateDatabaseStatement(string name, string fileName)
        {
            var result = $"CREATE DATABASE [{name}]";

            if (TestEnvironment.IsSqlAzure)
            {
                var elasticGroupName = TestEnvironment.ElasticPoolName;
                result += Environment.NewLine +
                          (string.IsNullOrEmpty(elasticGroupName)
                              ? " ( Edition = 'basic' )"
                              : $" ( SERVICE_OBJECTIVE = ELASTIC_POOL ( name = {elasticGroupName} ) )");
            }
            else
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    var logFileName = Path.ChangeExtension(fileName, ".ldf");
                    result += Environment.NewLine +
                              $" ON (NAME = '{name}', FILENAME = '{fileName}')" +
                              $" LOG ON (NAME = '{name}_log', FILENAME = '{logFileName}')";
                }
            }

            return result;
        }

        public void DeleteDatabase()
        {
            using (var master = new SqlConnection(CreateConnectionString("master")))
            {
                ExecuteNonQuery(
                    master, string.Format(
                        @"IF EXISTS (SELECT * FROM sys.databases WHERE name = N'{0}')
                                          BEGIN
                                              ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                              DROP DATABASE [{0}];
                                          END", Name));

                SqlConnection.ClearAllPools();
            }
        }

        public override void OpenConnection()
        {
            if (TestEnvironment.IsSqlAzure)
            {
                new TestMySqlRetryingExecutionStrategy().Execute(Connection, connection => connection.Open());
            }
            else
            {
                Connection.Open();
            }
        }

        public override Task OpenConnectionAsync()
            => TestEnvironment.IsSqlAzure
                ? new TestMySqlRetryingExecutionStrategy().ExecuteAsync(Connection, connection => connection.OpenAsync())
                : Connection.OpenAsync();

        public T ExecuteScalar<T>(string sql, params object[] parameters)
            => ExecuteScalar<T>(Connection, sql, parameters);

        private static T ExecuteScalar<T>(DbConnection connection, string sql, params object[] parameters)
            => Execute(connection, command => (T)command.ExecuteScalar(), sql, false, parameters);

        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] parameters)
            => ExecuteScalarAsync<T>(Connection, sql, parameters);

        private static Task<T> ExecuteScalarAsync<T>(DbConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, async command => (T)await command.ExecuteScalarAsync(), sql, false, parameters);

        public int ExecuteNonQuery(string sql, params object[] parameters)
            => ExecuteNonQuery(Connection, sql, parameters);

        private static int ExecuteNonQuery(DbConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command => command.ExecuteNonQuery(), sql, false, parameters);

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
            => ExecuteNonQueryAsync(Connection, sql, parameters);

        private static Task<int> ExecuteNonQueryAsync(DbConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, command => command.ExecuteNonQueryAsync(), sql, false, parameters);

        public IEnumerable<T> Query<T>(string sql, params object[] parameters)
            => Query<T>(Connection, sql, parameters);

        private static IEnumerable<T> Query<T>(DbConnection connection, string sql, object[] parameters = null)
            => Execute(
                connection, command =>
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        var results = Enumerable.Empty<T>();
                        while (dataReader.Read())
                        {
                            results = results.Concat(new[] { dataReader.GetFieldValue<T>(0) });
                        }

                        return results;
                    }
                }, sql, false, parameters);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
            => QueryAsync<T>(Connection, sql, parameters);

        private static Task<IEnumerable<T>> QueryAsync<T>(DbConnection connection, string sql, object[] parameters = null)
            => ExecuteAsync(
                connection, async command =>
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        var results = Enumerable.Empty<T>();
                        while (await dataReader.ReadAsync())
                        {
                            results = results.Concat(new[] { await dataReader.GetFieldValueAsync<T>(0) });
                        }

                        return results;
                    }
                }, sql, false, parameters);

        private static T Execute<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql,
            bool useTransaction = false, object[] parameters = null)
            => new TestMySqlRetryingExecutionStrategy().Execute(
                    new
                    {
                        connection,
                        execute,
                        sql,
                        useTransaction,
                        parameters
                    },
                    state => ExecuteCommand(state.connection, state.execute, state.sql, state.useTransaction, state.parameters));

        private static T ExecuteCommand<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql, bool useTransaction, object[] parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }

            connection.Open();
            try
            {
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        command.Transaction = transaction;
                        result = execute(command);
                    }

                    transaction?.Commit();

                    return result;
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static Task<T> ExecuteAsync<T>(
            DbConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql,
            bool useTransaction = false, IReadOnlyList<object> parameters = null)
            => TestEnvironment.IsSqlAzure
                ? new TestMySqlRetryingExecutionStrategy().ExecuteAsync(
                    new
                    {
                        connection,
                        executeAsync,
                        sql,
                        useTransaction,
                        parameters
                    },
                    state => ExecuteCommandAsync(state.connection, state.executeAsync, state.sql, state.useTransaction, state.parameters))
                : ExecuteCommandAsync(connection, executeAsync, sql, useTransaction, parameters);

        private static async Task<T> ExecuteCommandAsync<T>(
            DbConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql, bool useTransaction,
            IReadOnlyList<object> parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }

            await connection.OpenAsync();
            try
            {
                using (var transaction = useTransaction ? await connection.BeginTransactionAsync() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        result = await executeAsync(command);
                    }

                    if (transaction != null)
                    {
                        await transaction.CommitAsync();
                    }

                    return result;
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private static DbCommand CreateCommand(
            DbConnection connection, string commandText, IReadOnlyList<object> parameters = null)
        {
            var command = (SqlCommand)connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    command.Parameters.AddWithValue("p" + i, parameters[i]);
                }
            }

            return command;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_fileName != null)
            {
                // Clean up the database using a local file, as it might get deleted later
                DeleteDatabase();
            }
        }

        public static string CreateConnectionString(string name, string fileName = null, bool? multipleActiveResultSets = null)
        {
            var builder = new SqlConnectionStringBuilder(TestEnvironment.DefaultConnection)
            {
                MultipleActiveResultSets = multipleActiveResultSets ?? new Random().Next(0, 2) == 1,
                InitialCatalog = name
            };
            if (fileName != null)
            {
                builder.AttachDBFilename = fileName;
            }

            return builder.ToString();
        }
    }
}
