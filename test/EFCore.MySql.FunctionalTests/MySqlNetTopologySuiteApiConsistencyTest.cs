using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlNetTopologySuiteApiConsistencyTest : ApiConsistencyTestBase<MySqlNetTopologySuiteApiConsistencyTest.MySqlNetTopologySuiteApiConsistencyFixture>
    {
        public MySqlNetTopologySuiteApiConsistencyTest(MySqlNetTopologySuiteApiConsistencyFixture fixture)
            : base(fixture)
        {
        }

        protected override void AddServices(ServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkMySqlNetTopologySuite();

        protected override Assembly TargetAssembly
            => typeof(MySqlNetTopologySuiteServiceCollectionExtensions).Assembly;

        public class MySqlNetTopologySuiteApiConsistencyFixture : ApiConsistencyFixtureBase
        {
            public override HashSet<Type> FluentApiTypes { get; } = new()
            {
                typeof(MySqlNetTopologySuiteDbContextOptionsBuilderExtensions),
                typeof(MySqlNetTopologySuiteServiceCollectionExtensions)
            };
        }
    }
}
