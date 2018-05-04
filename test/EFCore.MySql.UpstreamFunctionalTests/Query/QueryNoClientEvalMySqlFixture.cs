using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class QueryNoClientEvalMySqlFixture : NorthwindQueryMySqlFixture<NoopModelCustomizer>
    {
        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder).ConfigureWarnings(c => c.Throw(RelationalEventId.QueryClientEvaluationWarning));
    }
}
