using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Xunit;

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
        // MySQL stored procedures do not support result columns.
        public override void Can_use_sproc_mapping_with_owned_reference()
            => Assert.Throws<InvalidOperationException>(() => base.Can_use_sproc_mapping_with_owned_reference());

        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder> configure)
            => new ModelBuilderTest.GenericTestModelBuilder(Fixture, configure);
    }
}
