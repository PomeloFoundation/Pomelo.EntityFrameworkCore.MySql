using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class InheritanceRelationshipsQueryMySqlTest : InheritanceRelationshipsQueryTestBase<InheritanceRelationshipsQueryMySqlFixture>
    {
        public InheritanceRelationshipsQueryMySqlTest(InheritanceRelationshipsQueryMySqlFixture fixture)
            : base(fixture)
        {
        }

        [SupportedServerVersionLessThanFact("8.0.22-mysql", Skip = "https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1210")]
        public override void Nested_include_with_inheritance_reference_collection1()
        {
            base.Nested_include_with_inheritance_reference_collection1();
        }

        [SupportedServerVersionLessThanFact("8.0.22-mysql", Skip = "https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1210")]
        public override void Nested_include_with_inheritance_reference_collection3()
        {
            base.Nested_include_with_inheritance_reference_collection3();
        }
    }
}
