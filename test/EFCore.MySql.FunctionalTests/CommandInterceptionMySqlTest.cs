using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class CommandInterceptionMySqlTestBase : CommandInterceptionTestBase
    {
        protected CommandInterceptionMySqlTestBase(InterceptionMySqlFixtureBase fixture)
            : base(fixture)
        {
        }

        public abstract class InterceptionMySqlFixtureBase : InterceptionFixtureBase
        {
            protected override string StoreName => "CommandInterception";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override IServiceCollection InjectInterceptors(
                IServiceCollection serviceCollection,
                IEnumerable<IInterceptor> injectedInterceptors)
                => base.InjectInterceptors(serviceCollection.AddEntityFrameworkMySql(), injectedInterceptors);
        }

        // Made internal to skip all tests.
        internal class CommandInterceptionMySqlTest
            : CommandInterceptionMySqlTestBase, IClassFixture<CommandInterceptionMySqlTest.InterceptionMySqlFixture>
        {
            public CommandInterceptionMySqlTest(InterceptionMySqlFixture fixture)
                : base(fixture)
            {
            }

            public class InterceptionMySqlFixture : InterceptionMySqlFixtureBase
            {
                protected override bool ShouldSubscribeToDiagnosticListener => false;
            }
        }

        // Made internal to skip all tests.
        internal class CommandInterceptionWithDiagnosticsMySqlTest
            : CommandInterceptionMySqlTestBase, IClassFixture<CommandInterceptionWithDiagnosticsMySqlTest.InterceptionMySqlFixture>
        {
            public CommandInterceptionWithDiagnosticsMySqlTest(InterceptionMySqlFixture fixture)
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
