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

            AssertContainsSql(
                @"SELECT `e`.`Id`, `e`.`EngineSupplierId`, `e`.`Name`, `t`.`Id`, `t`.`StorageLocation_Latitude`, `t`.`StorageLocation_Longitude`
FROM `Engines` AS `e`
LEFT JOIN (
    SELECT `e0`.`Id`, `e0`.`StorageLocation_Latitude`, `e0`.`StorageLocation_Longitude`, `e1`.`Id` AS `Id0`
    FROM `Engines` AS `e0`
    INNER JOIN `Engines` AS `e1` ON `e0`.`Id` = `e1`.`Id`
    WHERE `e0`.`StorageLocation_Longitude` IS NOT NULL AND `e0`.`StorageLocation_Latitude` IS NOT NULL
) AS `t` ON `e`.`Id` = `t`.`Id`
ORDER BY `e`.`Id`
LIMIT 1",
                //
                @"@p1='1'
@p2='1'
@p0='FO 108X' (Size = 4000)
@p3='ChangedEngine' (Size = 4000)
@p4='47.64491'
@p5='-122.128101'

UPDATE `Engines` SET `Name` = @p0
WHERE `Id` = @p1 AND `EngineSupplierId` = @p2 AND `Name` = @p3 AND `StorageLocation_Latitude` = @p4 AND `StorageLocation_Longitude` = @p5;
SELECT ROW_COUNT();");
        }

        private void AssertContainsSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
