using System;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Update.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Update
{
    public class MySqlUpdateSqlGeneratorTest : UpdateSqlGeneratorTestBase
    {
        protected override IUpdateSqlGenerator CreateSqlGenerator()
        {
            var options = new MySqlOptions();
            return new MySqlUpdateSqlGenerator(
                new UpdateSqlGeneratorDependencies(
                    new MySqlSqlGenerationHelper(
                        new RelationalSqlGenerationHelperDependencies(),
                        options),
                    new MySqlTypeMappingSource(
                        TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                        TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>(),
                        options)));
        }

        protected override TestHelpers TestHelpers
            => MySqlTestHelpers.Instance;

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_store_generated_columns_exist()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand();

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0);

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
            Assert.Equal(ResultSetMapping.LastInResultSet, grouping);
        }

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_no_store_generated_columns_exist()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: false, isComputed: false);

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0);

            AssertBaseline(
                @"INSERT INTO `Ducks` (`Id`, `Name`, `Quacks`, `ConcurrencyToken`)
VALUES (@p0, @p1, @p2, @p3),
(@p0, @p1, @p2, @p3);
",
                stringBuilder.ToString());
            Assert.Equal(ResultSetMapping.NoResultSet, grouping);
        }

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_store_generated_columns_exist_default_values_only()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: true, isComputed: true, defaultsOnly: true);

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0);

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
            Assert.Equal(ResultSetMapping.LastInResultSet, grouping);
        }

        [ConditionalFact]
        public void AppendBulkInsertOperation_appends_insert_if_no_store_generated_columns_exist_default_values_only()
        {
            var stringBuilder = new StringBuilder();
            var command = CreateInsertCommand(identityKey: false, isComputed: false, defaultsOnly: true);

            var sqlGenerator = (IMySqlUpdateSqlGenerator)CreateSqlGenerator();
            var grouping = sqlGenerator.AppendBulkInsertOperation(stringBuilder, new[] { command, command }, 0);

            var expectedText = @"INSERT INTO "
                               + SchemaPrefix
                               + OpenDelimiter
                               + "Ducks"
                               + CloseDelimiter
                               + NoColumns
                               + Environment.NewLine
                               + DefaultValues
                               + ";"
                               + Environment.NewLine;
            AssertBaseline(
                expectedText + expectedText,
                stringBuilder.ToString());
            Assert.Equal(ResultSetMapping.NoResultSet, grouping);
        }

        protected override void AppendInsertOperation_appends_insert_and_select_for_all_store_generated_columns_verification(
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
                + DefaultValues  // changed from
                + ";"            // "DEFAULT VALUES;"
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
                + Environment.NewLine,
                stringBuilder.ToString());
        }

        protected override void AppendInsertOperation_appends_insert_and_select_for_only_single_identity_columns_verification(
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
                + DefaultValues  // changed from
                + ";"            // "DEFAULT VALUES;"
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
                + Environment.NewLine,
                stringBuilder.ToString());
        }

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
        {
            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}
