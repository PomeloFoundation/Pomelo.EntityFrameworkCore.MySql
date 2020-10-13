using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class PropertyEntryMySqlTest : PropertyEntryTestBase<F1MySqlFixture>
    {
        public PropertyEntryMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        public override void Property_entry_original_value_is_set()
        {
            base.Property_entry_original_value_is_set();

            AssertSql(
                @"SELECT `e`.`Id`, `e`.`EngineSupplierId`, `e`.`Name`, `e`.`StorageLocation_Latitude`, `e`.`StorageLocation_Longitude`
FROM `Engines` AS `e`
ORDER BY `e`.`Id`
LIMIT 1",
                //
                @"@p1='1'
@p2='1'
@p0='FO 108X' (Size = 4000)
@p3='ChangedEngine' (Size = 4000)
@p4='47.64491' (Nullable = true)
@p5='-122.128101' (Nullable = true)

UPDATE `Engines` SET `Name` = @p0
WHERE `Id` = @p1 AND `EngineSupplierId` = @p2 AND `Name` = @p3 AND `StorageLocation_Latitude` = @p4 AND `StorageLocation_Longitude` = @p5;
SELECT ROW_COUNT();");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
