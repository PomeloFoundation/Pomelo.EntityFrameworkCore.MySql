using System;
using System.Data.Common;
using System.Diagnostics;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Diagnostics.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.TestUtilities.FakeProvider;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql;

public class MySqlRelationalConnectionTest
{
    [Fact]
    public void Creates_MySql_Server_connection_string()
    {
        using var connection = CreateConnection();

        Assert.IsType<MySqlConnection>(connection.DbConnection);
    }

    [Fact]
    public void Uses_DbDataSource_from_DbContextOptions()
    {
        using var dataSource = new MySqlDataSourceBuilder("Server=FakeHost;AllowUserVariables=True;UseAffectedRows=False").Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddMySqlDataSource("Server=FakeHost")
            .AddDbContext<FakeDbContext>(o => o.UseMySql(dataSource, AppConfig.ServerVersion));

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        var context1 = scope1.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection1 = (MySqlRelationalConnection)context1.GetService<IRelationalConnection>()!;
        Assert.Same(dataSource, relationalConnection1.DbDataSource);

        var connection1 = context1.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Server=FakeHost;Allow User Variables=True;Use Affected Rows=False", connection1.ConnectionString);

        using var scope2 = serviceProvider.CreateScope();
        var context2 = scope2.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection2 = (MySqlRelationalConnection)context2.GetService<IRelationalConnection>()!;
        Assert.Same(dataSource, relationalConnection2.DbDataSource);

        var connection2 = context2.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Server=FakeHost;Allow User Variables=True;Use Affected Rows=False", connection2.ConnectionString);
    }

    [Fact]
    public void Uses_DbDataSource_from_DbContextOptions_without_mandatory_settings()
    {
        using var dataSource = new MySqlDataSourceBuilder("Server=FakeHost").Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddMySqlDataSource("Server=FakeHost;Allow User Variables=True;Use Affected Rows=False")
            .AddDbContext<FakeDbContext>(o => o.UseMySql(dataSource, AppConfig.ServerVersion));

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FakeDbContext>();

        Assert.Equal(
            "The connection string of a connection used by Pomelo.EntityFrameworkCore.MySql must contain \"AllowUserVariables=True;UseAffectedRows=False\".",
            Assert.Throws<InvalidOperationException>(
                    () => (MySqlRelationalConnection)context.GetService<IRelationalConnection>()!)
                .Message);
    }

    [Fact]
    public void Uses_DbDataSource_from_ApplicationServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddMySqlDataSource("Server=FakeHost;Allow User Variables=True;Use Affected Rows=False")
            .AddDbContext<FakeDbContext>(o => o.UseMySql(AppConfig.ServerVersion));

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var dataSource = serviceProvider.GetRequiredService<MySqlDataSource>();

        using var scope1 = serviceProvider.CreateScope();
        var context1 = scope1.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection1 = (MySqlRelationalConnection)context1.GetService<IRelationalConnection>()!;
        Assert.Same(dataSource, relationalConnection1.DbDataSource);

        var connection1 = context1.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Server=FakeHost;Allow User Variables=True;Use Affected Rows=False", connection1.ConnectionString);

        using var scope2 = serviceProvider.CreateScope();
        var context2 = scope2.ServiceProvider.GetRequiredService<FakeDbContext>();
        var relationalConnection2 = (MySqlRelationalConnection)context2.GetService<IRelationalConnection>()!;
        Assert.Same(dataSource, relationalConnection2.DbDataSource);

        var connection2 = context2.GetService<FakeDbContext>().Database.GetDbConnection();
        Assert.Equal("Server=FakeHost;Allow User Variables=True;Use Affected Rows=False", connection2.ConnectionString);
    }

    [Fact]
    public void Uses_DbDataSource_from_ApplicationServiceProvider_without_mandatory_settings()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddMySqlDataSource("Server=FakeHost")
            .AddDbContext<FakeDbContext>(o => o.UseMySql(AppConfig.ServerVersion));

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var dataSource = serviceProvider.GetRequiredService<MySqlDataSource>();

        Assert.Equal("Server=FakeHost", dataSource.ConnectionString);

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FakeDbContext>();

        Assert.Equal(
            "The connection string of a connection used by Pomelo.EntityFrameworkCore.MySql must contain \"AllowUserVariables=True;UseAffectedRows=False\".",
            Assert.Throws<InvalidOperationException>(
                    () => (MySqlRelationalConnection)context.GetService<IRelationalConnection>()!)
                .Message);
    }

    [Fact]
    public void Can_create_master_connection_with_connection_string()
    {
        using var connection = CreateConnection();
        using var master = connection.CreateMasterConnection();

        Assert.Equal(
            @"Server=localhost;User ID=some_user;Password=some_password;Database=;Pooling=False;Allow User Variables=True;Use Affected Rows=False",
            master.ConnectionString);
    }

    // [Fact]
    // public void Can_create_master_connection_with_connection_string_and_alternate_admin_db()
    // {
    //     var options = new DbContextOptionsBuilder()
    //         .UseMySql(
    //             @"Server=localhost;Database=MySqlConnectionTest;User ID=some_user;Password=some_password",
    //             b => b.UseMasterDatabase("template0"))
    //         .Options;
    //
    //     using var connection = CreateConnection(options);
    //     using var master = connection.CreateMasterConnection();
    //
    //     Assert.Equal(
    //         @"Server=localhost;Database=template0;User ID=some_user;Password=some_password;Pooling=False;Multiplexing=False",
    //         master.ConnectionString);
    // }

    // CHECK: Do we need to fix this test?
    //
    // [Theory]
    // [InlineData("false")]
    // [InlineData("False")]
    // [InlineData("FALSE")]
    // public void CurrentAmbientTransaction_returns_null_with_AutoEnlist_set_to_false(string falseValue)
    // {
    //     var options = new DbContextOptionsBuilder()
    //         .UseMySql(
    //             @"Server=localhost;Database=MySqlConnectionTest;User ID=some_user;Password=some_password;Auto Enlist=" + falseValue,
    //             AppConfig.ServerVersion)
    //         .Options;
    //
    //     Transaction.Current = new CommittableTransaction();
    //
    //     using var connection = CreateConnection(options);
    //     Assert.Null(connection.CurrentAmbientTransaction);
    //
    //     Transaction.Current = null;
    // }

    [Theory]
    [InlineData(";Auto Enlist=true")]
    [InlineData("")] // Auto Enlist is true by default
    public void CurrentAmbientTransaction_returns_transaction_with_AutoEnlist_enabled(string autoEnlist)
    {
        var options = new DbContextOptionsBuilder()
            .UseMySql(
                @"Server=localhost;Database=MySqlConnectionTest;User ID=some_user;Password=some_password" + autoEnlist,
                AppConfig.ServerVersion)
            .Options;

        var transaction = new CommittableTransaction();
        Transaction.Current = transaction;

        using var connection = CreateConnection(options);
        Assert.Equal(transaction, connection.CurrentAmbientTransaction);

        Transaction.Current = null;
    }

    // INFO: We currently don't implement IMySqlRelationalConnection.CloneWith.
    //
    // [ConditionalFact]
    // public void CloneWith_with_connection_and_connection_string()
    // {
    //     var services = MySqlTestHelpers.Instance.CreateContextServices(
    //         new DbContextOptionsBuilder()
    //             .UseMySql("Server=localhost;Database=DummyDatabase", AppConfig.ServerVersion)
    //             .Options);
    //
    //     var relationalConnection = (IMySqlRelationalConnection)services.GetRequiredService<IRelationalConnection>();
    //
    //     var clone = relationalConnection.CloneWith("Server=localhost;Database=DummyDatabase;Application Name=foo");
    //
    //     Assert.Equal("Server=localhost;Database=DummyDatabase;Application Name=foo", clone.ConnectionString);
    // }

    private static MySqlRelationalConnection CreateConnection(DbContextOptions options = null, DbDataSource dataSource = null)
    {
        options ??= new DbContextOptionsBuilder()
            .UseMySql(@"Server=localhost;User ID=some_user;Password=some_password;Database=MySqlConnectionTest", AppConfig.ServerVersion)
            .Options;

        foreach (var extension in options.Extensions)
        {
            extension.Validate(options);
        }

        var singletonOptions = new MySqlOptions();
        singletonOptions.Initialize(options);

        return new MySqlRelationalConnection(
            new RelationalConnectionDependencies(
                options,
                new DiagnosticsLogger<DbLoggerCategory.Database.Transaction>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new MySqlLoggingDefinitions(),
                    new NullDbContextLogger()),
                new RelationalConnectionDiagnosticsLogger(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener"),
                    new MySqlLoggingDefinitions(),
                    new NullDbContextLogger(),
                    options),
                new NamedConnectionStringResolver(options),
                new RelationalTransactionFactory(
                    new RelationalTransactionFactoryDependencies(
                        new RelationalSqlGenerationHelper(
                            new RelationalSqlGenerationHelperDependencies()))),
                new CurrentDbContext(new FakeDbContext()),
                new RelationalCommandBuilderFactory(
                    new RelationalCommandBuilderDependencies(
                        new MySqlTypeMappingSource(
                            TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                            TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>(),
                            singletonOptions),
                        new ExceptionDetector()))),
            new MySqlConnectionStringOptionsValidator(),
            singletonOptions);
    }

    private const string ConnectionString = "Fake Connection String";

    private static IDbContextOptions CreateOptions(
        RelationalOptionsExtension optionsExtension = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder();

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(
                optionsExtension
                ?? new FakeRelationalOptionsExtension().WithConnectionString(ConnectionString));

        return optionsBuilder.Options;
    }

    private class FakeDbContext : DbContext
    {
        public FakeDbContext()
        {
        }

        public FakeDbContext(DbContextOptions<FakeDbContext> options)
            : base(options)
        {
        }
    }
}
