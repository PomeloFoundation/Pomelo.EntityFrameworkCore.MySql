using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Update;

public class StoredProcedureUpdateMySqlTest : StoredProcedureUpdateTestBase
{
    public override async Task Insert_with_output_parameter(bool async)
    {
        await base.Insert_with_output_parameter(
            async,
"""
CREATE PROCEDURE Entity_Insert(pName varchar(255), OUT pId int)
BEGIN
    INSERT INTO `Entity` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='New' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Insert`(@p1, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Insert_twice_with_output_parameter(bool async)
    {
        await base.Insert_twice_with_output_parameter(
            async,
"""
CREATE PROCEDURE Entity_Insert(pName varchar(255), OUT pId int)
BEGIN
    INSERT INTO `Entity` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='New1' (Size = 4000)
@p3='New2' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Insert`(@p1, @_out_p0);
SELECT @_out_p0;
SET @_out_p2 = NULL;
CALL `Entity_Insert`(@p3, @_out_p2);
SELECT @_out_p2;
""");
    }

    public override async Task Insert_with_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Insert_with_result_column(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Insert"), exception.Message);
    }

    public override async Task Insert_with_two_result_columns(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Insert_with_two_result_columns(async, createSprocSql: ""));

        Assert.Equal(
            MySqlStrings.StoredProcedureResultColumnsNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Insert"), exception.Message);
    }

    public override async Task Insert_with_output_parameter_and_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Insert_with_output_parameter_and_result_column(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureResultColumnsNotSupported(nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Insert"), exception.Message);
    }

    public override async Task Update(bool async)
    {
        await base.Update(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pName varchar(255))
UPDATE `Entity` SET `Name` = pName WHERE `Id` = pid
""");

        AssertSql(
"""
@p0='1'
@p1='Updated' (Size = 4000)

CALL `Entity_Update`(@p0, @p1);
""");
    }

    public override async Task Update_partial(bool async)
    {
        await base.Update_partial(
            async,
"""
CREATE PROCEDURE EntityWithAdditionalProperty_Update(pId int, pName varchar(255), pAdditionalProperty int)
UPDATE `EntityWithAdditionalProperty` SET `Name` = pName, `AdditionalProperty` = pAdditionalProperty WHERE `Id` = pid
""");

        AssertSql(
"""
@p0='1'
@p1='Updated' (Size = 4000)
@p2='8'

CALL `EntityWithAdditionalProperty_Update`(@p0, @p1, @p2);
""");
    }

    public override async Task Update_with_output_parameter_and_rows_affected_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Update_with_output_parameter_and_rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(
            MySqlStrings.StoredProcedureResultColumnsNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Update"), exception.Message);
    }

    public override async Task Update_with_output_parameter_and_rows_affected_result_column_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Update_with_output_parameter_and_rows_affected_result_column_concurrency_failure(async, createSprocSql: ""));

        Assert.Equal(
            MySqlStrings.StoredProcedureResultColumnsNotSupported(
                nameof(EntityWithAdditionalProperty), nameof(EntityWithAdditionalProperty) + "_Update"), exception.Message);
    }

    public override async Task Delete(bool async)
    {
        await base.Delete(
            async,
"""
CREATE PROCEDURE Entity_Delete(pId int)
DELETE FROM `Entity` WHERE `Id` = pId
""");

        AssertSql(
"""
@p0='1'

CALL `Entity_Delete`(@p0);
""");
    }

    public override async Task Delete_and_insert(bool async)
    {
        await base.Delete_and_insert(
            async,
"""
CREATE PROCEDURE Entity_Insert(pName varchar(255), OUT pId int)
BEGIN
    INSERT INTO `Entity` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END;

GO;

CREATE PROCEDURE Entity_Delete(pId int)
DELETE FROM `Entity` WHERE `Id` = pId;
""");

        AssertSql(
"""
@p0='1'
@p2='Entity2' (Size = 4000)

CALL `Entity_Delete`(@p0);
SET @_out_p1 = NULL;
CALL `Entity_Insert`(@p2, @_out_p1);
SELECT @_out_p1;
""");
    }

    public override async Task Rows_affected_parameter(bool async)
    {
        await base.Rows_affected_parameter(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pName varchar(255), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
    SET pRowsAffected = ROW_COUNT();
END
""");

        AssertSql(
"""
@p1='1'
@p2='Updated' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Update`(@p1, @p2, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Rows_affected_parameter_and_concurrency_failure(bool async)
    {
        await base.Rows_affected_parameter_and_concurrency_failure(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pName varchar(255), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId;
    SET pRowsAffected = ROW_COUNT();
END
""");

        AssertSql(
"""
@p1='1'
@p2='Updated' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Entity_Update`(@p1, @p2, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Rows_affected_result_column(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_result_column_and_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column_and_concurrency_failure(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_return_value(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_return_value(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureReturnValueNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Rows_affected_return_value_and_concurrency_failure(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_return_value(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureReturnValueNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Store_generated_concurrency_token_as_in_out_parameter(bool async)
    {
        await base.Store_generated_concurrency_token_as_in_out_parameter(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, INOUT pConcurrencyToken timestamp(6), pName varchar(255), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId AND `ConcurrencyToken` = pConcurrencyToken;
    SET pRowsAffected = ROW_COUNT();
    SELECT `ConcurrencyToken` INTO pConcurrencyToken FROM `Entity` WHERE `Id` = pId;
END
""");

        Assert.StartsWith(
            """
@p2='1'
@p4=NULL (DbType = DateTime)
@p0='
""",
            TestSqlLoggerFactory.Sql);

        Assert.EndsWith(
"""
' (Nullable = true) (DbType = DateTime)
@p3='Updated' (Size = 4000)

SET @_out_p0 = @p0;
SET @_out_p1 = NULL;
CALL `Entity_Update`(@p2, @_out_p0, @p3, @_out_p1);
SELECT @_out_p0, @_out_p1;
""",
            TestSqlLoggerFactory.Sql);

//         AssertSql(
// """
// @p2='1'
// @p4=NULL (DbType = DateTime)
// @p0='2022-11-14T12:03:01.9884410' (Nullable = true) (DbType = DateTime)
// @p3='Updated' (Size = 4000)
//
// SET @_out_p0 = @p0;
// SET @_out_p1 = NULL;
// CALL `Entity_Update`(@p2, @_out_p0, @p3, @_out_p1);
// SELECT @_out_p0, @_out_p1;
// """);
    }

    public override async Task Store_generated_concurrency_token_as_two_parameters(bool async)
    {
        await base.Store_generated_concurrency_token_as_two_parameters(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pConcurrencyTokenIn timestamp(6), pName varchar(255), OUT pConcurrencyTokenOut timestamp(6), OUT pRowsAffected int)
BEGIN
    UPDATE `Entity` SET `Name` = pName WHERE `Id` = pId AND `ConcurrencyToken` = pConcurrencyTokenIn;
    SET pRowsAffected = ROW_COUNT();
    SELECT `ConcurrencyToken` INTO pConcurrencyTokenOut FROM `Entity` WHERE `Id` = pId;
END
""");

        Assert.StartsWith(
"""
@p2='1'
@p3='
""",
            TestSqlLoggerFactory.Sql);

        Assert.EndsWith(
            """
' (Nullable = true) (DbType = DateTime)
@p4='Updated' (Size = 4000)

SET @_out_p0 = NULL;
SET @_out_p1 = NULL;
CALL `Entity_Update`(@p2, @p3, @p4, @_out_p0, @_out_p1);
SELECT @_out_p0, @_out_p1;
""",
            TestSqlLoggerFactory.Sql);

//         AssertSql(
//             """
// @p2='1'
// @p3='2022-11-14T14:02:25.0912340' (Nullable = true) (DbType = DateTime)
// @p4='Updated' (Size = 4000)
//
// SET @_out_p0 = NULL;
// SET @_out_p1 = NULL;
// CALL `Entity_Update`(@p2, @p3, @p4, @_out_p0, @_out_p1);
// SELECT @_out_p0, @_out_p1;
// """);
    }

    public override async Task User_managed_concurrency_token(bool async)
    {
        await base.User_managed_concurrency_token(
            async,
"""
CREATE PROCEDURE EntityWithAdditionalProperty_Update(pId int, pConcurrencyTokenOriginal int, pName varchar(255), pConcurrencyTokenCurrent int, OUT pRowsAffected int)
BEGIN
    UPDATE `EntityWithAdditionalProperty` SET `Name` = pName, `AdditionalProperty` = pConcurrencyTokenCurrent WHERE `Id` = pId AND `AdditionalProperty` = pConcurrencyTokenOriginal;
    SET pRowsAffected = ROW_COUNT();
END
""");

        AssertSql(
"""
@p1='1'
@p2='8'
@p3='Updated' (Size = 4000)
@p4='9'

SET @_out_p0 = NULL;
CALL `EntityWithAdditionalProperty_Update`(@p1, @p2, @p3, @p4, @_out_p0);
SELECT @_out_p0;
""");
    }

    public override async Task Original_and_current_value_on_non_concurrency_token(bool async)
    {
        await base.Original_and_current_value_on_non_concurrency_token(
            async,
"""
CREATE PROCEDURE Entity_Update(pId int, pNameCurrent varchar(255), pNameOriginal varchar(255))
BEGIN
    IF pNameCurrent <> pNameOriginal THEN
        UPDATE `Entity` SET `Name` = pNameCurrent WHERE `Id` = pId;
    END IF;
END
""");

        AssertSql(
"""
@p0='1'
@p1='Updated' (Size = 4000)
@p2='Initial' (Size = 4000)

CALL `Entity_Update`(@p0, @p1, @p2);
""");
    }

    public override async Task Input_or_output_parameter_with_input(bool async)
    {
        await base.Input_or_output_parameter_with_input(
            async,
"""
CREATE PROCEDURE Entity_Insert(OUT pId int, INOUT pName varchar(255))
BEGIN
    IF pName IS NULL THEN
        INSERT INTO `Entity` (`Name`) VALUES ('Some default value');
        SET pName = 'Some default value';
    ELSE
        INSERT INTO `Entity` (`Name`) VALUES (pName);
        SET pName = NULL;
    END IF;

    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='Initial' (Nullable = false) (Size = 4000)

SET @_out_p0 = NULL;
SET @_out_p1 = @p1;
CALL `Entity_Insert`(@_out_p0, @_out_p1);
SELECT @_out_p0, @_out_p1;
""");
    }

    public override async Task Input_or_output_parameter_with_output(bool async)
    {
        await base.Input_or_output_parameter_with_output(
            async,
"""
CREATE PROCEDURE Entity_Insert(OUT pId int, INOUT pName varchar(255))
BEGIN
    IF pName IS NULL THEN
        INSERT INTO `Entity` (`Name`) VALUES ('Some default value');
        SET pName = 'Some default value';
    ELSE
        INSERT INTO `Entity` (`Name`) VALUES (pName);
        SET pName = NULL;
    END IF;

    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
SET @_out_p0 = NULL;
SET @_out_p1 = @p1;
CALL `Entity_Insert`(@_out_p0, @_out_p1);
SELECT @_out_p0, @_out_p1;
""");
    }

    public override async Task Tph(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Tpt(bool async)
    {
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Rows_affected_result_column(async, createSprocSql: ""));

        Assert.Equal(MySqlStrings.StoredProcedureResultColumnsNotSupported(nameof(Entity), nameof(Entity) + "_Update"), exception.Message);
    }

    public override async Task Tpt_mixed_sproc_and_non_sproc(bool async)
    {
        await base.Tpt_mixed_sproc_and_non_sproc(
            async,
"""
CREATE PROCEDURE Parent_Insert(OUT pId int, pName varchar(255))
BEGIN
    INSERT INTO `Parent` (`Name`) VALUES (pName);
    SET pId = LAST_INSERT_ID();
END
""");

        AssertSql(
"""
@p1='Child' (Size = 4000)

SET @_out_p0 = NULL;
CALL `Parent_Insert`(@_out_p0, @p1);
SELECT @_out_p0;
""",
                //
                """
@p2='1'
@p3='8'

INSERT INTO `Child1` (`Id`, `Child1Property`)
VALUES (@p2, @p3);
""");
    }

    public override async Task Tpc(bool async)
    {
        var createSprocSql =
"""
ALTER TABLE `Child1` MODIFY COLUMN `Id` INT AUTO_INCREMENT;
ALTER TABLE `Child1` AUTO_INCREMENT = 100000;

GO;

CREATE PROCEDURE Child1_Insert(OUT pId int, pName varchar(255), pChild1Property int)
BEGIN
    INSERT INTO `Child1` (`Name`, `Child1Property`) VALUES (pName, pChild1Property);
    SET pId = LAST_INSERT_ID();
END
""";

        var contextFactory = await InitializeAsync<DbContext>(
            modelBuilder =>
            {
                modelBuilder.Entity<Parent>().UseTpcMappingStrategy();

                modelBuilder.Entity<Child1>()
                    .UseTpcMappingStrategy()
                    .InsertUsingStoredProcedure(
                        nameof(Child1) + "_Insert",
                        spb => spb
                            .HasParameter(w => w.Id, pb => pb.IsOutput())
                            .HasParameter(w => w.Name)
                            .HasParameter(w => w.Child1Property))
                    .Property(e => e.Id).UseMySqlIdentityColumn(); // <--
            },
            seed: ctx => CreateStoredProcedures(ctx, createSprocSql),
            onConfiguring: optionsBuilder =>
            {
                optionsBuilder.ConfigureWarnings(builder =>
                    builder.Ignore(RelationalEventId.TpcStoreGeneratedIdentityWarning)); // <-- added
            });

        await using var context = contextFactory.CreateContext();

        var entity1 = new Child1 { Name = "Child", Child1Property = 8 };
        context.Set<Child1>().Add(entity1);
        await SaveChanges(context, async);

        context.ChangeTracker.Clear();

        using (TestSqlLoggerFactory.SuspendRecordingEvents())
        {
            var entity2 = context.Set<Child1>().Single(b => b.Id == entity1.Id);

            Assert.Equal("Child", entity2.Name);
            Assert.Equal(8, entity2.Child1Property);
        }

        AssertSql(
"""
@p1='Child' (Size = 4000)
@p2='8'

SET @_out_p0 = NULL;
CALL `Child1_Insert`(@_out_p0, @p1, @p2);
SELECT @_out_p0;
""");
    }

    private async Task SaveChanges(DbContext context, bool async)
    {
        if (async)
        {
            await context.SaveChangesAsync();
        }
        else
        {
            // ReSharper disable once MethodHasAsyncOverload
            context.SaveChanges();
        }
    }

    protected override void CreateStoredProcedures(DbContext context, string createSprocSql)
    {
        foreach (var batch in
                 new Regex(@"[\r\n\s]*(?:\r|\n)GO;?[\r\n\s]*", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000.0))
                     .Split(createSprocSql).Where(b => !string.IsNullOrEmpty(b)))
        {
            context.Database.ExecuteSqlRaw(batch);
        }
    }

    protected override void ConfigureStoreGeneratedConcurrencyToken(EntityTypeBuilder entityTypeBuilder, string propertyName)
        => entityTypeBuilder.Property<byte[]>(propertyName).IsRowVersion();

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
