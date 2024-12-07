using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStore : RelationalTestStore
    {
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        public const int DefaultCommandTimeout = 600;

        private readonly string _connectionString;
        private readonly string _scriptPath;
        private readonly bool _useConnectionString;
        private readonly bool _noBackslashEscapes;

        protected override string OpenDelimiter => "`";
        protected override string CloseDelimiter => "`";

        public static MySqlTestStore GetOrCreate(string name, bool useConnectionString = false, bool noBackslashEscapes = false, string databaseCollation = null, MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
            => new MySqlTestStore(name, useConnectionString: useConnectionString, shared: true, noBackslashEscapes: noBackslashEscapes, databaseCollation: databaseCollation, guidFormat: guidFormat);

        public static MySqlTestStore GetOrCreate(string name, string scriptPath, bool noBackslashEscapes = false, string databaseCollation = null, MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
            => new MySqlTestStore(name, scriptPath: scriptPath, noBackslashEscapes: noBackslashEscapes, databaseCollation: databaseCollation, guidFormat: guidFormat);

        public static Task<MySqlTestStore> GetOrCreateInitializedAsync(string name)
            => new MySqlTestStore(name, shared: true).InitializeMySqlAsync(null, (Func<DbContext>)null, null);

        public static MySqlTestStore Create(string name, bool useConnectionString = false, bool noBackslashEscapes = false, string databaseCollation = null, MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
            => new MySqlTestStore(name, useConnectionString: useConnectionString, shared: false, noBackslashEscapes: noBackslashEscapes, databaseCollation: databaseCollation, guidFormat: guidFormat);

        public static Task<MySqlTestStore> CreateInitializedAsync(string name)
            => new MySqlTestStore(name, shared: false).InitializeMySqlAsync(null, null, null);

        public static Task<MySqlTestStore> RecreateInitializedAsync(string name)
            => new MySqlTestStore(name, shared: false).InitializeMySqlAsync(null, null, null, async c =>
            {
                await c.Database.EnsureDeletedAsync();
                await c.Database.EnsureCreatedAsync();
            });

        public Lazy<ServerVersion> ServerVersion { get; }
        public string DatabaseCharSet { get; }
        public string DatabaseCollation { get; set; }

        // ReSharper disable VirtualMemberCallInConstructor
        private MySqlTestStore(
            string name,
            string databaseCharSet = null,
            string databaseCollation = null,
            bool useConnectionString = false,
            string scriptPath = null,
            bool shared = true,
            bool noBackslashEscapes = false,
            MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
            : base(name, shared, new MySqlConnection(CreateConnectionString(name, noBackslashEscapes, guidFormat)))
        {
            _useConnectionString = useConnectionString;
            _noBackslashEscapes = noBackslashEscapes;

            if (useConnectionString)
            {
                _connectionString = CreateConnectionString(name, _noBackslashEscapes, guidFormat);
            }

            ServerVersion = new Lazy<ServerVersion>(() => Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect((MySqlConnection)Connection));
            DatabaseCharSet = databaseCharSet ?? "utf8mb4";
            DatabaseCollation = databaseCollation ?? ServerVersion.Value.DefaultUtf8CsCollation;

            if (scriptPath != null)
            {
                _scriptPath = Path.Combine(
                    Path.GetDirectoryName(typeof(MySqlTestStore).GetTypeInfo().Assembly.Location) ?? string.Empty,
                    scriptPath);
            }
        }

        public static string CreateConnectionString(string name, bool noBackslashEscapes = false, MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
            => new MySqlConnectionStringBuilder(AppConfig.ConnectionString)
            {
                Database = name,
                DefaultCommandTimeout = (uint)GetCommandTimeout(),
                NoBackslashEscapes = noBackslashEscapes,
                PersistSecurityInfo = true, // needed by some tests to not leak a broken connection into the following tests
                GuidFormat = guidFormat,
                AllowUserVariables = true,
                UseAffectedRows = false,
            }.ConnectionString;

        private static int GetCommandTimeout() => AppConfig.Config.GetValue("Data:CommandTimeout", DefaultCommandTimeout);

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => _useConnectionString
                ? builder.UseMySql(_connectionString, AppConfig.ServerVersion, x => AddOptions(x, _noBackslashEscapes))
                : builder.UseMySql(Connection, AppConfig.ServerVersion, x => AddOptions(x, _noBackslashEscapes));

        public static MySqlDbContextOptionsBuilder AddOptions(MySqlDbContextOptionsBuilder builder)
        {
            return builder
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                .CommandTimeout(GetCommandTimeout())
                .ExecutionStrategy(d => new TestMySqlRetryingExecutionStrategy(d));
            // .EnableIndexOptimizedBooleanColumns(); // TODO: Activate for all test for .NET 5. Tests should use
            //       `ONLY_FULL_GROUP_BY` to ensure correct working of the
            //       expression visitor in all cases, which is blocked by
            //       #1167 for MariaDB.
        }

        public static void AddOptions(MySqlDbContextOptionsBuilder builder, bool noBackslashEscapes)
        {
            AddOptions(builder);
            if (noBackslashEscapes)
            {
                builder.DisableBackslashEscaping();
            }
        }

        public async Task<MySqlTestStore> InitializeMySqlAsync(IServiceProvider serviceProvider, Func<DbContext> createContext, Func<DbContext, Task> seed, Func<DbContext, Task> clean = null)
            => (MySqlTestStore)await InitializeAsync(serviceProvider, createContext, seed, clean);

        protected override async Task InitializeAsync(
            Func<DbContext> createContext,
            Func<DbContext, Task> seed,
            Func<DbContext, Task> clean)
        {
            if (await CreateDatabaseAsync(clean))
            {
                if (_scriptPath != null)
                {
                    ExecuteScript(new FileInfo(_scriptPath));
                }
                else
                {
                    await using (var context = createContext())
                    {
                        await context.Database.EnsureCreatedResilientlyAsync();

                        if (seed != null)
                        {
                            await seed(context);
                        }
                    }
                }
            }
        }

        private async Task<bool> CreateDatabaseAsync(Func<DbContext, Task> clean)
        {
            await using var master = new MySqlConnection(CreateAdminConnectionString());
            await master.OpenAsync();

            string databaseSetupSql;
            if (await DatabaseExistsAsync(Name))
            {
                // if (_scriptPath != null
                //     && !TestEnvironment.IsCI)
                // {
                //     return false;
                // }

                await using (var context = new DbContext(
                                 AddProviderOptions(
                                         new DbContextOptionsBuilder()
                                             .EnableServiceProviderCaching(false))
                                     .Options))
                {
                    if (clean != null)
                    {
                        await clean(context);
                    }

                    await CleanAsync(context);
                }

                databaseSetupSql = GetAlterDatabaseStatement(Name, DatabaseCharSet, DatabaseCollation);

                // databaseSetupSql = GetCreateDatabaseStatement(Name, DatabaseCharSet, DatabaseCollation);
                // DeleteDatabase();
            }
            else
            {
                databaseSetupSql = GetCreateDatabaseStatement(Name, DatabaseCharSet, DatabaseCollation);
            }

            await ExecuteNonQueryAsync(master, databaseSetupSql);

            return true;
        }

        private void DeleteDatabase()
        {
            using var master = new MySqlConnection(CreateAdminConnectionString());
            ExecuteNonQuery(master, $@"DROP DATABASE IF EXISTS `{Name}`;");
        }

        private static string GetCreateDatabaseStatement(string name, string charset = null, string collation = null)
            => $@"CREATE DATABASE `{name}`{(string.IsNullOrEmpty(charset) ? null : $" CHARACTER SET {charset}")}{(string.IsNullOrEmpty(collation) ? null : $" COLLATE {collation}")};";

        private static string GetAlterDatabaseStatement(string name, string charset = null, string collation = null)
            => $@"ALTER DATABASE `{name}`{(string.IsNullOrEmpty(charset) ? null : $" CHARACTER SET {charset}")}{(string.IsNullOrEmpty(collation) ? null : $" COLLATE {collation}")};";

        private static async Task<bool> DatabaseExistsAsync(string name)
        {
            await using var master = new MySqlConnection(CreateAdminConnectionString());
            return await ExecuteScalarAsync<long>(master, $@"SELECT COUNT(*) FROM `INFORMATION_SCHEMA`.`SCHEMATA` WHERE `SCHEMA_NAME` = '{name}';") > 0;
        }

        private static string CreateAdminConnectionString()
            => CreateConnectionString(null);

        public void ExecuteScript(FileInfo scriptFile)
            => ExecuteScript(File.ReadAllText(scriptFile.FullName));

        public void ExecuteScript(string script)
            => Execute(
                Connection, command =>
                {
                    foreach (var batch in
                        new Regex(@"^/\*\s*GO\s*\*/", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0))
                            .Split(script)
                            .Where(b => !string.IsNullOrEmpty(b)))
                    {
                        command.CommandText = batch;
                        command.ExecuteNonQuery();
                    }

                    return 0;
                }, string.Empty);

        public override Task CleanAsync(DbContext context)
        {
            context.Database.EnsureClean();
            return Task.CompletedTask;
        }

        private static T ExecuteScalar<T>(DbConnection connection, string sql, params object[] parameters)
            => Execute(connection, command => (T)command.ExecuteScalar(), sql, false, parameters);

        private static Task<T> ExecuteScalarAsync<T>(DbConnection connection, string sql, params object[] parameters)
            => ExecuteAsync(connection, async command => (T)(await command.ExecuteScalarAsync()), sql, false, parameters);

        private static T Execute<T>(
            DbConnection connection, Func<DbCommand, T> execute, string sql,
            bool useTransaction = false, object[] parameters = null)
            => ExecuteCommand(connection, execute, sql, useTransaction, parameters);

        private static Task<T> ExecuteAsync<T>(
            DbConnection connection,
            Func<DbCommand, Task<T>> execute,
            string sql,
            bool useTransaction = false,
            object[] parameters = null)
            => ExecuteCommandAsync(connection, execute, sql, useTransaction, parameters);

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
                using (var transaction = useTransaction
                    ? connection.BeginTransaction()
                    : null)
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

        private static async Task<T> ExecuteCommandAsync<T>(
            DbConnection connection,
            Func<DbCommand, Task<T>> execute,
            string sql,
            bool useTransaction,
            object[] parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }

            await connection.OpenAsync();

            try
            {
                await using (var transaction = useTransaction
                                 ? await connection.BeginTransactionAsync()
                                 : null)
                {
                    T result;
                    await using (var command = CreateCommand(connection, sql, parameters))
                    {
                        command.Transaction = transaction;
                        result = await execute(command);
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

        public int ExecuteNonQuery(string sql, params object[] parameters)
            => ExecuteNonQuery(Connection, sql, parameters);

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
            => ExecuteNonQueryAsync(Connection, sql, parameters);

        private static int ExecuteNonQuery(DbConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command => command.ExecuteNonQuery(), sql, false, parameters);

        private static Task<int> ExecuteNonQueryAsync(DbConnection connection, string sql, object[] parameters = null)
            => ExecuteAsync(connection, command => command.ExecuteNonQueryAsync(), sql, false, parameters);

        public override void OpenConnection()
        {
            base.OpenConnection();

            if (_noBackslashEscapes)
            {
                AppendToSqlMode(NoBackslashEscapes, (MySqlConnection)Connection);
            }
        }

        public override async Task OpenConnectionAsync()
        {
            await base.OpenConnectionAsync();

            if (_noBackslashEscapes)
            {
                await AppendToSqlModeAsync(NoBackslashEscapes, (MySqlConnection)Connection);
            }
        }

        private static DbCommand CreateCommand(DbConnection connection, string commandText, object[] parameters)
        {
            var command = (MySqlCommand)connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = GetCommandTimeout();

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue("@p" + i, parameters[i]);
                }
            }

            return command;
        }

        public virtual void AppendToSqlMode(string mode, MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0);";
            command.Parameters.Add(new MySqlParameter("@p0", mode));

            command.ExecuteNonQuery();
        }

        public virtual Task AppendToSqlModeAsync(string mode, MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0);";
            command.Parameters.Add(new MySqlParameter("@p0", mode));

            return command.ExecuteNonQueryAsync();
        }
    }
}
