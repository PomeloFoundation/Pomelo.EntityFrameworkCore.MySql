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

        [ConditionalFact(Skip = "issue #552")]
        public override void Enum_ToString_is_client_eval()
        {
            base.Enum_ToString_is_client_eval();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Optional_Navigation_Null_Coalesce_To_Clr_Type()
        {
            base.Optional_Navigation_Null_Coalesce_To_Clr_Type();
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

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_subquery_boolean()
        {
            base.Select_subquery_boolean();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_subquery_boolean_with_pushdown()
        {
            base.Select_subquery_boolean_with_pushdown();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_subquery_boolean_empty()
        {
            base.Select_subquery_boolean_empty();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_subquery_boolean_empty_with_pushdown()
        {
            base.Select_subquery_boolean_empty_with_pushdown();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override void Select_subquery_projecting_single_constant_bool()
        {
            base.Select_subquery_projecting_single_constant_bool();
        }

        [ConditionalFact(Skip = "DateTimeOffset is mapped to DateTime, which gives different results than Linq To Objects if Offset term is non-zero")]
        public override void Where_datetimeoffset_hour_component()
        {
            base.Where_datetimeoffset_hour_component();
        }

        [ConditionalFact(Skip = "DateTimeOffset is mapped to DateTime, which gives different results than Linq To Objects if Offset term is non-zero")]
        public override void Where_datetimeoffset_minute_component()
        {
            base.Where_datetimeoffset_minute_component();
        }
    }
}
