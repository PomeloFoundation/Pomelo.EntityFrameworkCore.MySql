using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStore : RelationalTestStore
    {
        private const int DefaultCommandTimeout = 600;
        private const string NoBackslashEscapes = "NO_BACKSLASH_ESCAPES";

        private readonly bool _useConnectionString;
        private readonly bool _noBackslashEscapes;

        protected override string OpenDelimiter => "`";
        protected override string CloseDelimiter => "`";

        public static MySqlTestStore GetOrCreate(string name, bool useConnectionString = false, bool noBackslashEscapes = false)
            => new MySqlTestStore(name, useConnectionString: useConnectionString, shared: true, noBackslashEscapes: noBackslashEscapes);

        public static MySqlTestStore GetOrCreateInitialized(string name)
            => new MySqlTestStore(name, shared: true).InitializeMySql(null, (Func<DbContext>)null, null);

        public static MySqlTestStore Create(string name, bool useConnectionString = false, bool noBackslashEscapes = false)
            => new MySqlTestStore(name, useConnectionString: useConnectionString, shared: false, noBackslashEscapes: noBackslashEscapes);

        public static MySqlTestStore CreateInitialized(string name)
            => new MySqlTestStore(name, shared: false).InitializeMySql(null, (Func<DbContext>)null, null);

        // ReSharper disable VirtualMemberCallInConstructor
        private MySqlTestStore(string name, bool useConnectionString = false, bool shared = true, bool noBackslashEscapes = false)
            : base(name, shared)
        {
            _useConnectionString = useConnectionString;
            _noBackslashEscapes = noBackslashEscapes;

            ConnectionString = CreateConnectionString(name, _noBackslashEscapes);
            Connection = new MySqlConnection(ConnectionString);
        }

        public static string CreateConnectionString(string name, bool noBackslashEscapes, MySqlGuidFormat guidFormat = MySqlGuidFormat.Default)
            => new MySqlConnectionStringBuilder(AppConfig.Config["Data:ConnectionString"])
            {
                Database = name,
                DefaultCommandTimeout = (uint)GetCommandTimeout(),
                NoBackslashEscapes = noBackslashEscapes,
                PersistSecurityInfo = true, // needed by some tests to not leak a broken connection into the following tests
                GuidFormat = guidFormat,
            }.ConnectionString;

        private static int GetCommandTimeout() => AppConfig.Config.GetValue<int>("Data:CommandTimeout", DefaultCommandTimeout);

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => _useConnectionString
                ? builder.UseMySql(ConnectionString, x => AddOptions(x, _noBackslashEscapes))
                : builder.UseMySql(Connection, x => AddOptions(x, _noBackslashEscapes));

        public static void AddOptions(MySqlDbContextOptionsBuilder builder)
        {
            builder
                .CommandTimeout(GetCommandTimeout())
                .ServerVersion(AppConfig.ServerVersion.Version, AppConfig.ServerVersion.Type)
                .CharSetBehavior(CharSetBehavior.AppendToAllColumns)
                .CharSet(CharSet.Utf8Mb4);
        }

        public static void AddOptions(MySqlDbContextOptionsBuilder builder, bool noBackslashEscapes)
        {
            AddOptions(builder);
            if (noBackslashEscapes)
            {
                builder.DisableBackslashEscaping();
            }
        }

        public MySqlTestStore InitializeMySql(IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed)
            => (MySqlTestStore)Initialize(serviceProvider, createContext, seed);

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
        {
            using (var context = createContext())
            {
                if (!context.Database.EnsureCreated())
                {
                    Clean(context);
                }
                seed(context);
            }
        }

        public override void Clean(DbContext context)
            => context.Database.EnsureClean();

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            var connection = _useConnectionString
                ? new MySqlConnection(ConnectionString)
                : (MySqlConnection)Connection;
            try
            {
                using (var command = CreateCommand(connection, sql, parameters))
                {
                    return command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (_useConnectionString)
                {
                    connection.Dispose();
                }
            }
        }

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

        private DbCommand CreateCommand(MySqlConnection connection, string commandText, object[] parameters)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = GetCommandTimeout();

            for (var i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue("@p" + i, parameters[i]);
            }

            return command;
        }

        public virtual void AppendToSqlMode(string mode, MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)";
            command.Parameters.Add(new MySqlParameter("@p0", mode));

            command.ExecuteNonQuery();
        }

        public virtual Task AppendToSqlModeAsync(string mode, MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @"SET SESSION sql_mode = CONCAT(@@sql_mode, ',', @p0)";
            command.Parameters.Add(new MySqlParameter("@p0", mode));

            return command.ExecuteNonQueryAsync();
        }
    }
}
