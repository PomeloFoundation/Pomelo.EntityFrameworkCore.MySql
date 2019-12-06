using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsQueryMySqlTest : ComplexNavigationsQueryTestBase<ComplexNavigationsQueryMySqlFixture>
    {
        public ComplexNavigationsQueryMySqlTest(ComplexNavigationsQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Include_inside_subquery(bool isAsync)
        {
            return base.Include_inside_subquery(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Project_collection_navigation_nested_with_take(bool isAsync)
        {
            return base.Project_collection_navigation_nested_with_take(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(bool isAsync)
        {
            return base.SelectMany_with_navigation_filter_paging_and_explicit_DefaultIfEmpty(isAsync);
        }
        
        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Including_reference_navigation_and_projecting_collection_navigation_2(bool isAsync)
        {
            return base.Including_reference_navigation_and_projecting_collection_navigation_2(isAsync);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        [MemberData(nameof(IsAsyncData))]
        public override Task Lift_projection_mapping_when_pushing_down_subquery(bool isAsync)
        {
            return base.Lift_projection_mapping_when_pushing_down_subquery(isAsync);
        }

        [SupportedServerVersionFact(ServerVersion.WindowFunctionsSupportKey)]
        public override void Member_pushdown_chain_3_levels_deep_entity()
        {
            base.Member_pushdown_chain_3_levels_deep_entity();
        }
    }
}
