using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public class MySqlTestStore : RelationalTestStore
    {
        public const int CommandTimeout = 30;
        private readonly bool _useConnectionString;
        private readonly bool _noBackslashEscapes;

        public static MySqlTestStore GetOrCreate(string name, bool useConnectionString = false, bool noBackslashEscapes = false)
            => new MySqlTestStore(name, useConnectionString: useConnectionString, noBackslashEscapes: noBackslashEscapes);

        public static MySqlTestStore GetOrCreateInitialized(string name)
            => new MySqlTestStore(name).InitializeMySql(null, (Func<DbContext>)null, null);

        public static MySqlTestStore Create(string name, bool useConnectionString = false, bool noBackslashEscapes = false)
            => new MySqlTestStore(name, useConnectionString: useConnectionString, shared: false, noBackslashEscapes: noBackslashEscapes);

        public static MySqlTestStore CreateInitialized(string name)
            => new MySqlTestStore(name, shared: false).InitializeMySql(null, (Func<DbContext>)null, null);

        private MySqlTestStore(string name, bool useConnectionString = false, bool shared = true, bool noBackslashEscapes = false)
            : base(name, shared)
        {
            _useConnectionString = useConnectionString;
            _noBackslashEscapes = noBackslashEscapes;

            ConnectionString = new MySqlConnectionStringBuilder(LazyConfig.Value["Data:ConnectionString"])
            {
                Database = name
            }.ToString();

            Connection = new MySqlConnection(ConnectionString);
        }

        private static readonly Lazy<IConfigurationRoot> LazyConfig = new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("config.json")
            .Build());

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => _useConnectionString
                ? builder.UseMySql(ConnectionString,x => AddOptions(x, _noBackslashEscapes))
                : builder.UseMySql(Connection, x => AddOptions(x, _noBackslashEscapes));

        public static void AddOptions(MySqlDbContextOptionsBuilder builder)
        {
            builder.CommandTimeout(CommandTimeout).ServerVersion(LazyConfig.Value["Data:ServerVersion"]);
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

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed)
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

        private DbCommand CreateCommand(MySqlConnection connection, string commandText, object[] parameters)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            for (var i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue("@p" + i, parameters[i]);
            }

            return command;
        }
    }
}
