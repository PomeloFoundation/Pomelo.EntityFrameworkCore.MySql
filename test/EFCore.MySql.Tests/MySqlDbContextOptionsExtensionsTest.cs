using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public class MySqlDbContextOptionsBuilderExtensionsTest
    {
        [Fact]
        public void Multiple_UseMySql_calls_each_get_fully_applied()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=first;",
                AppConfig.ServerVersion,
                options =>
                    options.DefaultDataTypeMappings(
                        mappings =>
                            mappings.WithClrBoolean(MySqlBooleanType.Bit1)));

            builder.UseMySql(
                "Server=second;",
                AppConfig.ServerVersion,
                options =>
                    options.DefaultDataTypeMappings(
                        mappings =>
                            mappings.WithClrBoolean(MySqlBooleanType.TinyInt1)));

            var mySqlOptionsExtension = builder.Options.GetExtension<MySqlOptionsExtension>();
            Assert.StartsWith("Server=second;", mySqlOptionsExtension.ConnectionString, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(MySqlBooleanType.TinyInt1, mySqlOptionsExtension.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_true()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql("TreatTinyAsBoolean=True", AppConfig.ServerVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.TinyInt1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_false()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql("TreatTinyAsBoolean=False", AppConfig.ServerVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_unspecified()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql("Server=foo", AppConfig.ServerVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Default, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void Explicit_DefaultDataTypeMappings_take_precedence_over_TreatTinyAsBoolean_true()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "TreatTinyAsBoolean=True",
                AppConfig.ServerVersion,
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void Explicit_DefaultDataTypeMappings_take_precedence_over_TreatTinyAsBoolean_false()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "TreatTinyAsBoolean=False",
                AppConfig.ServerVersion,
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.TinyInt1)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.TinyInt1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void UseMySql_with_MySqlServerVersion_Version()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MySqlServerVersion(new Version(8, 0, 21)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 21), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MySql, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mysql", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MySqlServerVersion_string_version_only()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MySqlServerVersion("8.0.21"));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 21), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MySql, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mysql", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MySqlServerVersion_string_version_full()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MySqlServerVersion("8.0.21-mysql"));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 21), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MySql, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mysql", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MySqlServerVersion_ServerVersion()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MySqlServerVersion(new MySqlServerVersion(new Version(8, 0, 21))));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 21), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MySql, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mysql", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MySqlServerVersion_incorrect_ServerVersion_throws()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    var builder = new DbContextOptionsBuilder();

                    builder.UseMySql(
                        "Server=foo",
                        new MySqlServerVersion(new MariaDbServerVersion("10.5.5-mariadb")));
                });
        }

        [Fact]
        public void UseMySql_with_MySqlServerVersion_LatestSupportedServerVersion()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                MySqlServerVersion.LatestSupportedServerVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlServerVersion.LatestSupportedServerVersion.Version, mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MySql, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mysql", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MariaDbServerVersion_Version()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MariaDbServerVersion(new Version(10, 5, 5)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(10, 5, 5), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MariaDb, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mariadb", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MariaDbServerVersion_string_version_only()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MariaDbServerVersion("10.5.5"));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(10, 5, 5), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MariaDb, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mariadb", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MariaDbServerVersion_string_version_full()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MariaDbServerVersion("10.5.5-mariadb"));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(10, 5, 5), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MariaDb, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mariadb", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MariaDbServerVersion_ServerVersion()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                new MariaDbServerVersion(new MariaDbServerVersion(new Version(10, 5, 5))));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(10, 5, 5), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MariaDb, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mariadb", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_MariaDbServerVersion_incorrect_ServerVersion_throws()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    var builder = new DbContextOptionsBuilder();

                    builder.UseMySql(
                        "Server=foo",
                        new MariaDbServerVersion(new MySqlServerVersion("8.0.21-mysql")));
                });
        }

        [Fact]
        public void UseMySql_with_MariaDbServerVersion_LatestSupportedServerVersion()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=foo",
                MariaDbServerVersion.LatestSupportedServerVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MariaDbServerVersion.LatestSupportedServerVersion.Version, mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MariaDb, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mariadb", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_ServerVersion_FromString()
        {
            var builder = new DbContextOptionsBuilder();
            var serverVersion = ServerVersion.Parse("8.0.21-mysql");

            builder.UseMySql(
                "Server=foo",
                serverVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(new Version(8, 0, 21), mySqlOptions.ServerVersion.Version);
            Assert.Equal(ServerType.MySql, mySqlOptions.ServerVersion.Type);
            Assert.Equal("mysql", mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_with_ServerVersion_AutoDetect()
        {
            var builder = new DbContextOptionsBuilder();
            var serverVersion = ServerVersion.AutoDetect(AppConfig.ConnectionString);

            builder.UseMySql(
                "Server=foo",
                serverVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(serverVersion.Version, mySqlOptions.ServerVersion.Version);
            Assert.Equal(serverVersion.Type, mySqlOptions.ServerVersion.Type);
            Assert.Equal(serverVersion.TypeIdentifier, mySqlOptions.ServerVersion.TypeIdentifier);
        }

        [Fact]
        public void UseMySql_without_connection_string()
        {
            var builder = new DbContextOptionsBuilder();
            var serverVersion = ServerVersion.AutoDetect(AppConfig.ConnectionString);

            builder.UseMySql(serverVersion);

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);
        }

        [Fact]
        public void UseMySql_without_connection_explicit_DefaultDataTypeMappings_is_applied()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                AppConfig.ServerVersion,
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }
    }
}
