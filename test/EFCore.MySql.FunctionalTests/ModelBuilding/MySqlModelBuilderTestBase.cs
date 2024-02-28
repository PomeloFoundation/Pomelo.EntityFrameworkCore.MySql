using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.ModelBuilding;

public class MySqlModelBuilderTestBase : RelationalModelBuilderTest
{
    public abstract class MySqlNonRelationship(MySqlModelBuilderFixture fixture)
        : RelationalNonRelationshipTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlComplexType(MySqlModelBuilderFixture fixture)
        : RelationalComplexTypeTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlInheritance(MySqlModelBuilderFixture fixture)
        : RelationalInheritanceTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlOneToMany(MySqlModelBuilderFixture fixture)
        : RelationalOneToManyTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlManyToOne(MySqlModelBuilderFixture fixture)
        : RelationalManyToOneTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlOneToOne(MySqlModelBuilderFixture fixture)
        : RelationalOneToOneTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlManyToMany(MySqlModelBuilderFixture fixture)
        : RelationalManyToManyTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public abstract class MySqlOwnedTypes(MySqlModelBuilderFixture fixture)
        : RelationalOwnedTypesTestBase(fixture), IClassFixture<MySqlModelBuilderFixture>;

    public class MySqlModelBuilderFixture : RelationalModelBuilderFixture
    {
        public override TestHelpers TestHelpers
            => MySqlTestHelpers.Instance;
    }
}
