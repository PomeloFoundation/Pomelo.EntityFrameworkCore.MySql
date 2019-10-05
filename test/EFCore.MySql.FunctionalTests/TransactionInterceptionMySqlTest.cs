using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class TransactionInterceptionMySqlTestBase : TransactionInterceptionTestBase
    {
        protected TransactionInterceptionMySqlTestBase(InterceptionMySqlFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionMySqlFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "TransactionInterception";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkMySql(), injectedInterceptors);
        }

        // Made internal to skip all tests.
        internal class TransactionInterceptionMySqlTest
            : TransactionInterceptionMySqlTestBase, IClassFixture<TransactionInterceptionMySqlTest.InterceptionMySqlFixture>
        {
            public TransactionInterceptionMySqlTest(InterceptionMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionMySqlFixture : InterceptionMySqlFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;
            }
        }

        // Made internal to skip all tests.
        internal class TransactionInterceptionWithDiagnosticsMySqlTest
            : TransactionInterceptionMySqlTestBase, IClassFixture<TransactionInterceptionWithDiagnosticsMySqlTest.InterceptionMySqlFixture>
        {
            public TransactionInterceptionWithDiagnosticsMySqlTest(InterceptionMySqlFixture fixture)
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
