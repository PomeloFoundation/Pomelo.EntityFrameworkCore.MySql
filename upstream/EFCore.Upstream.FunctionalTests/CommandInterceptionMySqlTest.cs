// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class CommandInterceptionMySqlTestBase : CommandInterceptionTestBase
    {
        protected CommandInterceptionMySqlTestBase(InterceptionMySqlFixtureBase fixture)
            : base(fixture)
        {
        }

        public override async Task<string> Intercept_query_passively(bool async, bool inject)
        {
            AssertSql(
                @"SELECT [s].[Id], [s].[Type] FROM [Singularity] AS [s]",
                await base.Intercept_query_passively(async, inject));

            return null;
        }

        public override async Task<string> Intercept_query_to_mutate_command(bool async, bool inject)
        {
            AssertSql(
                @"SELECT [s].[Id], [s].[Type] FROM [Brane] AS [s]",
                await base.Intercept_query_to_mutate_command(async, inject));

            return null;
        }

        public override async Task<string> Intercept_query_to_replace_execution(bool async, bool inject)
        {
            AssertSql(
                @"SELECT [s].[Id], [s].[Type] FROM [Singularity] AS [s]",
                await base.Intercept_query_to_replace_execution(async, inject));

            return null;
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

        public class CommandInterceptionMySqlTest
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

        public class CommandInterceptionWithDiagnosticsMySqlTest
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
