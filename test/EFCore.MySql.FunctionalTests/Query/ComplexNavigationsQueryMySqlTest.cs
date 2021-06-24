using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsQueryMySqlTest : ComplexNavigationsQueryRelationalTestBase<ComplexNavigationsQueryMySqlFixture>
    {
        public ComplexNavigationsQueryMySqlTest(ComplexNavigationsQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override bool CanExecuteQueryString
            => true;

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(bool async)
        {
            return base.SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Including_reference_navigation_and_projecting_collection_navigation_2(bool async)
        {
            return base.Including_reference_navigation_and_projecting_collection_navigation_2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override void Member_pushdown_chain_3_levels_deep_entity()
        {
            base.Member_pushdown_chain_3_levels_deep_entity();
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Collection_FirstOrDefault_entity_reference_accesses_in_projection(bool async)
        {
            return base.Collection_FirstOrDefault_entity_reference_accesses_in_projection(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task SelectMany_with_outside_reference_to_joined_table_correctly_translated_to_apply(bool async)
        {
            return base.SelectMany_with_outside_reference_to_joined_table_correctly_translated_to_apply(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Let_let_contains_from_outer_let(bool async)
        {
            return base.Let_let_contains_from_outer_let(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task Contains_with_subquery_optional_navigation_and_constant_item(bool async)
        {
            return base.Contains_with_subquery_optional_navigation_and_constant_item(async);
        }
    }
}
