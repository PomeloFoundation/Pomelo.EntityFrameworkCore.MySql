using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class AsyncSimpleQueryMySqlTest : AsyncSimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncSimpleQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public override Task Intersect_non_entity()
        {
            // INTERSECT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Intersect_non_entity());
        }

        [ConditionalFact]
        public override Task Except_non_entity()
        {
            // EXCEPT is not natively supported by MySQL.
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.Except_non_entity());
        }
    }
}
