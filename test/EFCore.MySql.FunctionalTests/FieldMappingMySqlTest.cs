using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class FieldMappingMySqlTest
    {
        public abstract class FieldMappingMySqlTestBase<TFixture> : FieldMappingTestBase<TFixture>
            where TFixture : FieldMappingMySqlTestBase<TFixture>.FieldMappingMySqlFixtureBase, new()
        {
            protected FieldMappingMySqlTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class FieldMappingMySqlFixtureBase : FieldMappingFixtureBase
            {
                protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            }
        }

        public class DefaultMappingTest
            : FieldMappingMySqlTestBase<DefaultMappingTest.DefaultMappingFixture>
        {
            public DefaultMappingTest(DefaultMappingFixture fixture)
                : base(fixture)
            {
            }

            public class DefaultMappingFixture : FieldMappingMySqlFixtureBase
            {
            }
        }

        public class EnforceFieldTest
            : FieldMappingMySqlTestBase<EnforceFieldTest.EnforceFieldFixture>
        {
            public EnforceFieldTest(EnforceFieldFixture fixture)
                : base(fixture)
            {
            }

            public class EnforceFieldFixture : FieldMappingMySqlFixtureBase
            {
                protected override string StoreName { get; } = "FieldMappingEnforceFieldTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Field);
                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class EnforceFieldForQueryTest
            : FieldMappingMySqlTestBase<EnforceFieldForQueryTest.EnforceFieldForQueryFixture>
        {
            public EnforceFieldForQueryTest(EnforceFieldForQueryFixture fixture)
                : base(fixture)
            {
            }

            public class EnforceFieldForQueryFixture : FieldMappingMySqlFixtureBase
            {
                protected override string StoreName { get; } = "FieldMappingFieldQueryTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class EnforcePropertyTest
            : FieldMappingMySqlTestBase<EnforcePropertyTest.EnforcePropertyFixture>
        {
            public EnforcePropertyTest(EnforcePropertyFixture fixture)
                : base(fixture)
            {
            }

            // Cannot force property access when properties missing getter/setter

            public override void Include_collection_read_only_props(bool tracking)
            {
            }

            public override void Include_reference_read_only_props(bool tracking)
            {
            }

            public override void Load_collection_read_only_props()
            {
            }

            public override void Load_reference_read_only_props()
            {
            }

            public override void Query_with_conditional_constant_read_only_props(bool tracking)
            {
            }

            public override void Query_with_conditional_param_read_only_props(bool tracking)
            {
            }

            public override void Projection_read_only_props(bool tracking)
            {
            }

            public override void Update_read_only_props()
            {
            }

            public override void Include_collection_read_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Include_reference_read_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Load_collection_read_only_props_with_named_fields()
            {
            }

            public override void Load_reference_read_only_props_with_named_fields()
            {
            }

            public override void Query_with_conditional_constant_read_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Query_with_conditional_param_read_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Projection_read_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Update_read_only_props_with_named_fields()
            {
            }

            public override void Include_collection_write_only_props(bool tracking)
            {
            }

            public override void Include_reference_write_only_props(bool tracking)
            {
            }

            public override void Load_collection_write_only_props()
            {
            }

            public override void Load_reference_write_only_props()
            {
            }

            public override void Query_with_conditional_constant_write_only_props(bool tracking)
            {
            }

            public override void Query_with_conditional_param_write_only_props(bool tracking)
            {
            }

            public override void Projection_write_only_props(bool tracking)
            {
            }

            public override void Update_write_only_props()
            {
            }

            public override void Include_collection_write_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Include_reference_write_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Load_collection_write_only_props_with_named_fields()
            {
            }

            public override void Load_reference_write_only_props_with_named_fields()
            {
            }

            public override void Query_with_conditional_constant_write_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Query_with_conditional_param_write_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Projection_write_only_props_with_named_fields(bool tracking)
            {
            }

            public override void Update_write_only_props_with_named_fields()
            {
            }

            public override void Include_collection_fields_only(bool tracking)
            {
            }

            public override void Include_reference_fields_only(bool tracking)
            {
            }

            public override void Load_collection_fields_only()
            {
            }

            public override void Load_reference_fields_only()
            {
            }

            public override void Query_with_conditional_constant_fields_only(bool tracking)
            {
            }

            public override void Query_with_conditional_param_fields_only(bool tracking)
            {
            }

            public override void Projection_fields_only(bool tracking)
            {
            }

            public override void Update_fields_only()
            {
            }

            public override void Include_collection_fields_only_for_navs_too(bool tracking)
            {
            }

            public override void Include_reference_fields_only_only_for_navs_too(bool tracking)
            {
            }

            public override void Load_collection_fields_only_only_for_navs_too()
            {
            }

            public override void Load_reference_fields_only_only_for_navs_too()
            {
            }

            public override void Query_with_conditional_constant_fields_only_only_for_navs_too(bool tracking)
            {
            }

            public override void Query_with_conditional_param_fields_only_only_for_navs_too(bool tracking)
            {
            }

            public override void Projection_fields_only_only_for_navs_too(bool tracking)
            {
            }

            public override void Update_fields_only_only_for_navs_too()
            {
            }

            public class EnforcePropertyFixture : FieldMappingMySqlFixtureBase
            {
                protected override string StoreName { get; } = "FieldMappingEnforcePropertyTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Property);
                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }
    }
}
