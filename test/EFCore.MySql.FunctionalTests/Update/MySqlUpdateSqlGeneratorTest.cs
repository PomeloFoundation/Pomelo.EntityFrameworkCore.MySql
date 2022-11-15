using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Update
{
    public class MySqlUpdateSqlGeneratorTest : UpdateSqlGeneratorTestBase
    {
        protected override IUpdateSqlGenerator CreateSqlGenerator()
        {
            var options = GetOptions();

            return new MySqlUpdateSqlGenerator(
                new UpdateSqlGeneratorDependencies(
                    new MySqlSqlGenerationHelper(
                        new RelationalSqlGenerationHelperDependencies(),
                        options),
                    new MySqlTypeMappingSource(
                        TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                        TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>(),
                        options)),
                options);
        }

        private static MySqlOptions GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            var extension = new MySqlOptionsExtension()
                .WithServerVersion(AppConfig.ServerVersion);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            var options = new MySqlOptions();
            options.Initialize(optionsBuilder.Options);
            return options;
        }

        protected override TestHelpers TestHelpers
            => MySqlTestHelpers.Instance;

        #region Methods, that depend on RETURNING clause or OnAdd

        protected new IModificationCommand CreateInsertCommand(bool identityKey = true, bool isComputed = true, bool defaultsOnly = false)
        {
            var model = GetDuckModel();
            var stateManager = TestHelpers.CreateContextServices(model).GetRequiredService<IStateManager>();
            var entry = stateManager.GetOrCreateEntry(new Duck());
            entry.SetEntityState(EntityState.Added);
            var generator = new ParameterNameGenerator();

            var duckType = model.FindEntityType(typeof(Duck));
            var idProperty = duckType.FindProperty(nameof(Duck.Id));
            var nameProperty = duckType.FindProperty(nameof(Duck.Name));
            var quacksProperty = duckType.FindProperty(nameof(Duck.Quacks));
            var computedProperty = duckType.FindProperty(nameof(Duck.Computed));
            var concurrencyProperty = duckType.FindProperty(nameof(Duck.ConcurrencyToken));

            var columnModifications = new[]
            {
                new ColumnModificationParameters(
                    entry, idProperty, idProperty.GetTableColumnMappings().Single().Column, generator.GenerateNext,
                    idProperty.GetTableColumnMappings().Single().TypeMapping, identityKey, !identityKey, true, false, true),
                new ColumnModificationParameters(
                    entry, nameProperty, nameProperty.GetTableColumnMappings().Single().Column, generator.GenerateNext,
                    nameProperty.GetTableColumnMappings().Single().TypeMapping, false, true, false, false, true),
                new ColumnModificationParameters(
                    entry, quacksProperty, quacksProperty.GetTableColumnMappings().Single().Column, generator.GenerateNext,
                    quacksProperty.GetTableColumnMappings().Single().TypeMapping, false, true, false, false, true),
                new ColumnModificationParameters(
                    entry, computedProperty, computedProperty.GetTableColumnMappings().Single().Column, generator.GenerateNext,
                    computedProperty.GetTableColumnMappings().Single().TypeMapping, isComputed, false, false, false, true),
                new ColumnModificationParameters(
                    entry, concurrencyProperty, concurrencyProperty.GetTableColumnMappings().Single().Column, generator.GenerateNext,
                    concurrencyProperty.GetTableColumnMappings().Single().TypeMapping, false, true, false, false, true)
            };

            if (defaultsOnly)
            {
                columnModifications = columnModifications.Where(c => !c.IsWrite).ToArray();
            }

            return CreateModificationCommand(entry, columnModifications, false);
        }

        private IModificationCommand CreateModificationCommand(
            InternalEntityEntry entry,
            IReadOnlyList<ColumnModificationParameters> columnModifications,
            bool sensitiveLoggingEnabled)
        {
            var modificationCommandParameters = new ModificationCommandParameters(
                entry.EntityType.GetTableMappings().Single().Table, sensitiveLoggingEnabled);
            var modificationCommand = CreateMutableModificationCommandFactory().CreateModificationCommand(
                modificationCommandParameters);

            modificationCommand.AddEntry(entry, mainEntry: true);

            foreach (var columnModification in columnModifications)
            {
                ((INonTrackedModificationCommand)modificationCommand).AddColumnModification(columnModification);
            }

            return modificationCommand;
        }

        private IModel GetDuckModel()
        {
            var modelBuilder = TestHelpers.CreateConventionBuilder();
            modelBuilder.Entity<Duck>().ToTable("Ducks", Schema).Property(e => e.Id)/*.ValueGeneratedNever()*/;
            return modelBuilder.Model.FinalizeModel();
        }

        public override void AppendInsertOperation_for_only_identity()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(true, false);

            CreateSqlGenerator().AppendInsertOperation(stringBuilder, command, 0);

            AppendInsertOperation_for_only_identity_verification(stringBuilder);
        }

        public override void AppendInsertOperation_for_all_store_generated_columns()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(true, true, true);

            CreateSqlGenerator().AppendInsertOperation(stringBuilder, command, 0);

            AppendInsertOperation_for_all_store_generated_columns_verification(stringBuilder);
        }

        public override void AppendInsertOperation_for_only_single_identity_columns()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: true, isComputed: false, defaultsOnly: true);

            CreateSqlGenerator().AppendInsertOperation(stringBuilder, command, 0);

            AppendInsertOperation_for_only_single_identity_columns_verification(stringBuilder);
        }

        public override void AppendInsertOperation_insert_if_store_generated_columns_exist()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand();

            CreateSqlGenerator().AppendInsertOperation(stringBuilder, command, 0);

            AppendInsertOperation_insert_if_store_generated_columns_exist_verification(stringBuilder);
        }

        #endregion

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_store_generated_columns_exist()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand();

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0, out _);

            if (AppConfig.ServerVersion.Supports.Returning)
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2)
RETURNING `Id`, `Computed`;
INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2)
RETURNING `Id`, `Computed`;
",
                    stringBuilder.ToString());
            }
            else
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2);
SELECT `Id`, `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2);
SELECT `Id`, `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

",
                    stringBuilder.ToString());
            }

            Assert.Equal(ResultSetMapping.LastInResultSet, grouping);
        }

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_no_store_generated_columns_exist()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: false, isComputed: false);

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0, out _);

            AssertBaseline(
                @"INSERT INTO `Ducks` (`Id`, `Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2, @p3),
(@p0, @p1, @p2, @p3);
",
                stringBuilder.ToString());
            Assert.Equal(ResultSetMapping.NoResults, grouping);
        }

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_store_generated_columns_exist_default_values_only()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: true, isComputed: true, defaultsOnly: true);

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0, out _);

            if (AppConfig.ServerVersion.Supports.Returning)
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` ()
VALUES ()
RETURNING `Id`, `Computed`;
INSERT INTO `Ducks` ()
VALUES ()
RETURNING `Id`, `Computed`;
",
                    stringBuilder.ToString());
            }
            else
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` ()
VALUES ();
SELECT `Id`, `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

INSERT INTO `Ducks` ()
VALUES ();
SELECT `Id`, `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

",
                    stringBuilder.ToString());
            }
            Assert.Equal(ResultSetMapping.LastInResultSet, grouping);
        }

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_no_store_generated_columns_exist_default_values_only()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: false, isComputed: false, defaultsOnly: true);

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0, out _);

            var expectedText = @"INSERT INTO `Ducks` ()
VALUES (),
();
";
            AssertBaseline(
                expectedText,
                stringBuilder.ToString());
            Assert.Equal(ResultSetMapping.NoResults, grouping);
        }

        protected override void AppendDeleteOperation_creates_full_delete_command_text_verification(StringBuilder stringBuilder)
            => AssertBaseline(
                @"DELETE FROM `Ducks`
WHERE `Id` = @p0;
SELECT ROW_COUNT();

",
                stringBuilder.ToString());

        protected override void AppendDeleteOperation_creates_full_delete_command_text_with_concurrency_check_verification(
            StringBuilder stringBuilder)
            => AssertBaseline(
                @"DELETE FROM `Ducks`
WHERE `Id` = @p0 AND `ConcurrencyToken` IS NULL;
SELECT ROW_COUNT();

",
                stringBuilder.ToString());

        protected override void AppendInsertOperation_insert_if_store_generated_columns_exist_verification(StringBuilder stringBuilder)
        {
            if (AppConfig.ServerVersion.Supports.Returning)
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2)
RETURNING `Id`, `Computed`;
",
                    stringBuilder.ToString());
            }
            else
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2);
SELECT `Id`, `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

",
                    stringBuilder.ToString());
            }
        }

        protected override void AppendInsertOperation_for_store_generated_columns_but_no_identity_verification(
            StringBuilder stringBuilder)
        {
            if (AppConfig.ServerVersion.Supports.Returning)
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Id`, `Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2, @p3)
RETURNING `Computed`;
",
                    stringBuilder.ToString());
            }
            else
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Id`, `Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2, @p3);
SELECT `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = @p0;

",
                    stringBuilder.ToString());

            }
        }

        protected override void AppendInsertOperation_for_only_identity_verification(StringBuilder stringBuilder)
        {
            if (AppConfig.ServerVersion.Supports.Returning)
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2)
RETURNING `Id`;",
                    stringBuilder.ToString());
            }
            else
            {
                AssertBaseline(
                    @"INSERT INTO `Ducks` (`Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2);
SELECT `Id`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = LAST_INSERT_ID();

",
                    stringBuilder.ToString());
            }
        }

        protected override void AppendInsertOperation_for_all_store_generated_columns_verification(
            StringBuilder stringBuilder)
        {
            Assert.Equal(
                "INSERT INTO "
                + SchemaPrefix
                + OpenDelimiter
                + "Ducks"
                + CloseDelimiter
                + NoColumns      // added
                + Environment.NewLine
                + DefaultValues  // changed from "DEFAULT VALUES;"
                + (AppConfig.ServerVersion.Supports.Returning
                    ? Environment.NewLine
                      + "RETURNING `Id`, `Computed`;"
                      + Environment.NewLine
                    :  ";"
                      + Environment.NewLine
                      + "SELECT "
                      + OpenDelimiter
                      + "Id"
                      + CloseDelimiter
                      + ", "
                      + OpenDelimiter
                      + "Computed"
                      + CloseDelimiter
                      + ""
                      + Environment.NewLine
                      + "FROM "
                      + SchemaPrefix
                      + OpenDelimiter
                      + "Ducks"
                      + CloseDelimiter
                      + ""
                      + Environment.NewLine
                      + "WHERE "
                      + RowsAffected
                      + " = 1 AND "
                      + GetIdentityWhereCondition("Id")
                      + ";"
                      + Environment.NewLine
                      + Environment.NewLine),
                stringBuilder.ToString());
        }

        protected override void AppendInsertOperation_for_only_single_identity_columns_verification(
            StringBuilder stringBuilder)
        {
            Assert.Equal(
                "INSERT INTO "
                + SchemaPrefix
                + OpenDelimiter
                + "Ducks"
                + CloseDelimiter
                + NoColumns      // added
                + Environment.NewLine
                + DefaultValues  // changed from "DEFAULT VALUES;"
                + (AppConfig.ServerVersion.Supports.Returning
                    ?   Environment.NewLine
                      + "RETURNING `Id`;"
                      + Environment.NewLine
                    :   ";"
                      + Environment.NewLine
                      + "SELECT "
                      + OpenDelimiter
                      + "Id"
                      + CloseDelimiter
                      + ""
                      + Environment.NewLine
                      + "FROM "
                      + SchemaPrefix
                      + OpenDelimiter
                      + "Ducks"
                      + CloseDelimiter
                      + ""
                      + Environment.NewLine
                      + "WHERE "
                      + RowsAffected
                      + " = 1 AND "
                      + GetIdentityWhereCondition("Id")
                      + ";"
                      + Environment.NewLine
                      + Environment.NewLine),
                stringBuilder.ToString());
        }

        protected override void AppendUpdateOperation_if_store_generated_columns_exist_verification(
            StringBuilder stringBuilder)
            => AssertBaseline(
                @"UPDATE `Ducks` SET `Name` = @p0, `Quacks` = @p1, `ConcurrencyToken` = @p2
WHERE `Id` = @p3 AND `ConcurrencyToken` IS NULL;
SELECT `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = @p3;

",
                stringBuilder.ToString());

        protected override void AppendUpdateOperation_if_store_generated_columns_dont_exist_verification(
            StringBuilder stringBuilder)
            => AssertBaseline(
                @"UPDATE `Ducks` SET `Name` = @p0, `Quacks` = @p1, `ConcurrencyToken` = @p2
WHERE `Id` = @p3;
SELECT ROW_COUNT();

",
                stringBuilder.ToString());

        protected override void AppendUpdateOperation_appends_where_for_concurrency_token_verification(StringBuilder stringBuilder)
            => AssertBaseline(
                @"UPDATE `Ducks` SET `Name` = @p0, `Quacks` = @p1, `ConcurrencyToken` = @p2
WHERE `Id` = @p3 AND `ConcurrencyToken` IS NULL;
SELECT ROW_COUNT();

",
                stringBuilder.ToString());

        protected override void AppendUpdateOperation_for_computed_property_verification(StringBuilder stringBuilder)
            => AssertBaseline(
                @"UPDATE `Ducks` SET `Name` = @p0, `Quacks` = @p1, `ConcurrencyToken` = @p2
WHERE `Id` = @p3;
SELECT `Computed`
FROM `Ducks`
WHERE ROW_COUNT() = 1 AND `Id` = @p3;

",
                stringBuilder.ToString());

        protected override string RowsAffected
            => "ROW_COUNT()";

        protected override string Identity
            => "LAST_INSERT_ID()";

        protected virtual string NoColumns
            => " ()"; // null

        protected virtual string DefaultValues
            => "VALUES ()"; // "DEFAULT VALUES"

        protected override string Schema
            => null;

        protected override string OpenDelimiter
            => "`";

        protected override string CloseDelimiter
            => "`";

        private void AssertBaseline(string expected, string actual)
            => Assert.Equal(expected.TrimEnd(), actual.TrimEnd(), ignoreLineEndingDifferences: true);
    }
}
