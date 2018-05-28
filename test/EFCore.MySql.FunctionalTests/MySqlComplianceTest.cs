using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class MySqlComplianceTest : RelationalComplianceTestBase
    {
        protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>();

        protected override Assembly TargetAssembly { get; } = typeof(MySqlComplianceTest).Assembly;
    }
}
