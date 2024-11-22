using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

#nullable enable

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.RenameColumn))]
    public class MigrationsInfrastructureMySqlTest : MigrationsInfrastructureTestBase<MigrationsInfrastructureMySqlTest.MigrationsInfrastructureMySqlFixture>
    {
        public MigrationsInfrastructureMySqlTest(MigrationsInfrastructureMySqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Can_generate_migration_from_initial_database_to_initial()
        {
            base.Can_generate_migration_from_initial_database_to_initial();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_generate_no_migration_script()
        {
            base.Can_generate_no_migration_script();

            Assert.Equal(
                @"CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_apply_one_migration()
        {
            base.Can_apply_one_migration();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_apply_one_migration_in_parallel()
        {
            base.Can_apply_one_migration_in_parallel();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_apply_second_migration_in_parallel()
        {
            base.Can_apply_second_migration_in_parallel();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_apply_one_migration_in_parallel_async()
        {
            await base.Can_apply_one_migration_in_parallel_async();

            Assert.Equal(
                """

                """,
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_apply_second_migration_in_parallel_async()
        {
            await base.Can_apply_second_migration_in_parallel_async();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_up_and_down_scripts()
        {
            await base.Can_generate_up_and_down_scripts();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_up_and_down_scripts_noTransactions()
        {
            await base.Can_generate_up_and_down_scripts_noTransactions();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_one_up_and_down_script()
        {
            await base.Can_generate_one_up_and_down_script();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_up_and_down_script_using_names()
        {
            await base.Can_generate_up_and_down_script_using_names();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_idempotent_up_and_down_scripts()
        {
            await base.Can_generate_idempotent_up_and_down_scripts();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task Can_generate_idempotent_up_and_down_scripts_noTransactions()
        {
            await base.Can_generate_idempotent_up_and_down_scripts_noTransactions();

            Assert.Equal(
"""

""",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void Can_get_active_provider()
        {
            base.Can_get_active_provider();

            Assert.Equal("Pomelo.EntityFrameworkCore.MySql", ActiveProvider);
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_2_2_model()
        {
            throw new NotImplementedException();
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_3_0_ASP_NET_Identity_model()
        {
            throw new NotImplementedException();
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_2_2_ASP_NET_Identity_model()
        {
            throw new NotImplementedException();
        }

        [ConditionalFact(Skip = "TODO: Implement")]
        public override void Can_diff_against_2_1_ASP_NET_Identity_model()
        {
            throw new NotImplementedException();
        }

        public override void Can_apply_all_migrations()
            => Assert.Throws<MySqlException>(() => base.Can_apply_all_migrations());

        public override void Can_apply_range_of_migrations()
            => Assert.Throws<MySqlException>(() => base.Can_apply_range_of_migrations());

        public override void Can_revert_all_migrations()
            => Assert.Throws<MySqlException>(() => base.Can_revert_all_migrations());

        public override void Can_revert_one_migrations()
            => Assert.Throws<MySqlException>(() => base.Can_revert_one_migrations());

        protected override Task ExecuteSqlAsync(string value)
            => ((MySqlTestStore)Fixture.TestStore).ExecuteNonQueryAsync(value);

        public override Task Can_apply_all_migrations_async()
            => Assert.ThrowsAsync<MySqlException>(() => base.Can_apply_all_migrations_async());

        public class MigrationsInfrastructureMySqlFixture : MigrationsInfrastructureFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlConnectionStringTestStoreFactory.Instance;
        }
    }
}
