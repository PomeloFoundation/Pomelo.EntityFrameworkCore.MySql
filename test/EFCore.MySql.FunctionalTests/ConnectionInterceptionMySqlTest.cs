using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class ConnectionInterceptionMySqlTestBase : ConnectionInterceptionTestBase
    {
        protected ConnectionInterceptionMySqlTestBase(InterceptionMySqlFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionMySqlFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "ConnectionInterception";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkMySql(), injectedInterceptors);
        }

        protected override BadUniverseContext CreateBadUniverse(DbContextOptionsBuilder optionsBuilder)
            => new BadUniverseContext(optionsBuilder.UseMySql(new FakeDbConnection()).Options);

        public class FakeDbConnection : DbConnection
        {
            public override string ConnectionString { get; set; }
            public override string Database => "Database";
            public override string DataSource => "DataSource";
            public override string ServerVersion => throw new NotImplementedException();
            public override ConnectionState State => ConnectionState.Closed;
            public override void ChangeDatabase(string databaseName) => throw new NotImplementedException();
            public override void Close() => throw new NotImplementedException();
            public override void Open() => throw new NotImplementedException();
            protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel) => throw new NotImplementedException();
            protected override DbCommand CreateDbCommand() => throw new NotImplementedException();
        }

        public class ConnectionInterceptionMySqlTest
            : ConnectionInterceptionMySqlTestBase, IClassFixture<ConnectionInterceptionMySqlTest.InterceptionMySqlFixture>
        {
            public ConnectionInterceptionMySqlTest(InterceptionMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionMySqlFixture : InterceptionMySqlFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;
            }
        }

        public class ConnectionInterceptionWithDiagnosticsMySqlTest
            : ConnectionInterceptionMySqlTestBase, IClassFixture<ConnectionInterceptionWithDiagnosticsMySqlTest.InterceptionMySqlFixture>
        {
            public ConnectionInterceptionWithDiagnosticsMySqlTest(InterceptionMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionMySqlFixture : InterceptionMySqlFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => true;
            }
        }
    }
}
