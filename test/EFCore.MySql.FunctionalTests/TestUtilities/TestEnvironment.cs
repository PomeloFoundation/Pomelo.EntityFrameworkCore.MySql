using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities
{
    public static class TestEnvironment
    {
        private const string DefaultConnectionString = "Server=localhost;Username=mysql_tests;Password=mysql_tests";

        private static Version _mySqlVersion;

        public static IConfiguration Config { get; }
        public static string DefaultConnection => Config["DefaultConnection"] ?? DefaultConnectionString;

        public static bool IsCI { get; } = Environment.GetEnvironmentVariable("PIPELINE_WORKSPACE") != null
                                           || Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;

        static TestEnvironment()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true)
                .AddJsonFile("config.test.json", optional: true)
                .AddEnvironmentVariables();

            Config = configBuilder.Build()
                .GetSection("Test:MySql");

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }

        public static Version MySqlVersion
        {
            get
            {
                if (_mySqlVersion != null)
                {
                    return _mySqlVersion;
                }

                using (var connection = new MySqlConnection(
                    new MySqlConnectionStringBuilder(TestEnvironment.DefaultConnection) { Database = "mysql" }.ConnectionString))
                {
                    connection.Open();
                    return _mySqlVersion = new Version(connection.ServerVersion);
                }
            }
        }
    }
}
