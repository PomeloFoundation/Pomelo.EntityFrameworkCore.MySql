using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MigrationsMySqlTest : MigrationsTestBase<MigrationsMySqlTest.MigrationsMySqlFixture>
    {
        public MigrationsMySqlTest(MigrationsMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "TODO: Syntax issue in MySQL 7 only.")]
        public override Task Alter_check_constraint()
        {
            return base.Alter_check_constraint();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_make_computed(bool? stored)
        {
            return base.Alter_column_make_computed(stored);
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_computed_with_collation()
        {
            return base.Add_column_computed_with_collation();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_with_collation()
        {
            return base.Add_column_with_collation();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_with_defaultValue_string()
        {
            return base.Add_column_with_defaultValue_string();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_column_with_defaultValueSql()
        {
            return base.Add_column_with_defaultValueSql();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_primary_key()
        {
            return base.Add_primary_key();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_primary_key_composite_with_name()
        {
            return base.Add_primary_key_composite_with_name();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_primary_key_with_name()
        {
            return base.Add_primary_key_with_name();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_unique_constraint()
        {
            return base.Add_unique_constraint();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Add_unique_constraint_composite_with_name()
        {
            return base.Add_unique_constraint_composite_with_name();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_change_computed_type()
        {
            return base.Alter_column_change_computed_type();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_change_type()
        {
            return base.Alter_column_change_type();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_column_set_collation()
        {
            return base.Alter_column_set_collation();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_sequence_all_settings()
        {
            return base.Alter_sequence_all_settings();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_sequence_increment_by()
        {
            return base.Alter_sequence_increment_by();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Alter_table_add_comment_non_default_schema()
        {
            return base.Alter_table_add_comment_non_default_schema();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_index_with_filter()
        {
            return base.Create_index_with_filter();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_schema()
        {
            return base.Create_schema();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_sequence()
        {
            return base.Create_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_sequence_all_settings()
        {
            return base.Create_sequence_all_settings();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_table_all_settings()
        {
            return base.Create_table_all_settings();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_table_with_multiline_comments()
        {
            return base.Create_table_with_multiline_comments();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Create_unique_index_with_filter()
        {
            return base.Create_unique_index_with_filter();
        }

        [ConditionalTheory(Skip = "TODO: Syntax issue in MySQL 7 only.")]
        public override Task Drop_check_constraint()
        {
            return base.Drop_check_constraint();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Drop_column_primary_key()
        {
            return base.Drop_column_primary_key();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Drop_primary_key()
        {
            return base.Drop_primary_key();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Drop_sequence()
        {
            return base.Drop_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Move_sequence()
        {
            return base.Move_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Move_table()
        {
            return base.Move_table();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Rename_sequence()
        {
            return base.Rename_sequence();
        }

        [ConditionalTheory(Skip = "TODO")]
        public override Task Rename_table_with_primary_key()
        {
            return base.Rename_table_with_primary_key();
        }

        protected override string NonDefaultCollation => ((MySqlTestStore)Fixture.TestStore).GetCaseSensitiveUtf8Mb4Collation();

        public class MigrationsMySqlFixture : MigrationsFixtureBase
        {
            protected override string StoreName { get; } = nameof(MigrationsMySqlTest);
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public override TestHelpers TestHelpers => MySqlTestHelpers.Instance;

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
                => base.AddServices(serviceCollection)
                    .AddScoped<IDatabaseModelFactory, MySqlDatabaseModelFactory>();

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new MySqlDbContextOptionsBuilder(base.AddOptions(builder))
                    .CharSetBehavior(CharSetBehavior.NeverAppend);

                return builder;
            }
        }
    }
}
