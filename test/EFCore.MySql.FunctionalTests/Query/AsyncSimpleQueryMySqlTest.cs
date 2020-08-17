using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class AsyncSimpleQueryMySqlTest : AsyncSimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncSimpleQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [SupportedServerVersionFact(ServerVersion.ExceptInterceptSupportKey)]
        public override async Task Intersect_non_entity()
            => await base.Intersect_non_entity();

        [SupportedServerVersionFact(ServerVersion.ExceptInterceptSupportKey)]
        public override async Task Except_non_entity()
            => await base.Except_non_entity();
    }
}
