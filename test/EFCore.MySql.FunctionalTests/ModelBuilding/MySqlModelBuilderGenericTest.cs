using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.ModelBuilding;

public class MySqlModelBuilderGenericTest : MySqlModelBuilderTestBase
{
    public class MySqlGenericNonRelationship(MySqlModelBuilderFixture fixture) : MySqlNonRelationship(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericComplexType(MySqlModelBuilderFixture fixture) : MySqlComplexType(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericInheritance(MySqlModelBuilderFixture fixture) : MySqlInheritance(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericOneToMany(MySqlModelBuilderFixture fixture) : MySqlOneToMany(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericManyToOne(MySqlModelBuilderFixture fixture) : MySqlManyToOne(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericOneToOne(MySqlModelBuilderFixture fixture) : MySqlOneToOne(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericManyToMany(MySqlModelBuilderFixture fixture) : MySqlManyToMany(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }

    public class MySqlGenericOwnedTypes(MySqlModelBuilderFixture fixture) : MySqlOwnedTypes(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }
}
