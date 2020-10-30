using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindSplitIncludeQueryMySqlTest : NorthwindSplitIncludeQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindSplitIncludeQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //TestSqlLoggerFactory.CaptureOutput(testOutputHelper);
        }

        public override Task Include_collection_with_last_no_orderby(bool async)
            => AssertTranslationFailedWithDetails(
                () => AssertLast(
                    async,
                    ss => ss.Set<Customer>()
                        .Include(c => c.Orders),
                    entryCount: 8),
                RelationalStrings.MissingOrderingInSqlExpression);

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/21202")]
        public override Task Include_collection_skip_no_order_by(bool async)
        {
            return base.Include_collection_skip_no_order_by(async);
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/21202")]
        public override Task Include_collection_skip_take_no_order_by(bool async)
        {
            return base.Include_collection_skip_take_no_order_by(async);
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/issues/21202")]
        public override Task Include_collection_take_no_order_by(bool async)
        {
            return base.Include_collection_take_no_order_by(async);
        }
    }
}
