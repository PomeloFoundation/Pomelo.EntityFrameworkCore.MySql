using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GearsOfWarQueryMySqlTest : GearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Projecting_nullable_bool_in_conditional_works()
        {
            base.Projecting_nullable_bool_in_conditional_works();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Where_subquery_distinct_firstordefault_boolean()
        {
            base.Where_subquery_distinct_firstordefault_boolean();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Where_subquery_distinct_firstordefault_boolean_with_pushdown()
        {
            base.Where_subquery_distinct_firstordefault_boolean_with_pushdown();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Where_subquery_distinct_orderby_firstordefault_boolean()
        {
            base.Where_subquery_distinct_orderby_firstordefault_boolean();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown()
        {
            base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown();
        }

        [ConditionalFact(Skip = "issue #573")]
        public override void Select_subquery_distinct_firstordefault()
        {
            base.Select_subquery_distinct_firstordefault();
        }

        public override void Where_datetimeoffset_minute_component()
        {
            base.Where_datetimeoffset_minute_component();
        }
    }
}
