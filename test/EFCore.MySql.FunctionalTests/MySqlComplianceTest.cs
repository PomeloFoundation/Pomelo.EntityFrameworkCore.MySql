using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlComplianceTest : RelationalComplianceTestBase
    {
        // TODO: Implement remaining 3.x tests.
        protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>
        {
            typeof(UdfDbFunctionTestBase<>),
            typeof(TransactionInterceptionTestBase),
            typeof(CommandInterceptionTestBase),
            typeof(NorthwindQueryTaggingQueryTestBase<>),

            // TODO: Reenable LoggingMySqlTest once its issue has been fixed in EF Core upstream.
            typeof(LoggingTestBase),
            typeof(LoggingRelationalTestBase<,>)
        };

        protected override Assembly TargetAssembly { get; } = typeof(MySqlComplianceTest).Assembly;
    }
}
