using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

// Disabled via internal access. Complex collections require JSON mapping which is not currently supported.
internal class ComplexPropertiesProjectionMySqlTest : ComplexPropertiesProjectionTestBase<ComplexPropertiesProjectionMySqlTest.ComplexPropertiesProjectionMySqlFixture>
{
    public ComplexPropertiesProjectionMySqlTest(ComplexPropertiesProjectionMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesProjectionMySqlFixture : ComplexPropertiesFixtureBase, ITestSqlLoggerFactory
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
