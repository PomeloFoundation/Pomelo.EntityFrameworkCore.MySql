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
                @"SELECT `e`.`Id`, `e`.`EngineSupplierId`, `e`.`Name`, `e`.`Id`, `e`.`StorageLocation_Latitude`, `e`.`StorageLocation_Longitude`
FROM `Engines` AS `e`
ORDER BY `e`.`Id`
LIMIT 1",
                //
                @"@p1='1'
@p2='1'
@p0='FO 108X' (Size = 4000)
@p3='ChangedEngine' (Size = 4000)

UPDATE `Engines` SET `Name` = @p0
WHERE `Id` = @p1 AND `EngineSupplierId` = @p2 AND `Name` = @p3;
SELECT ROW_COUNT();");
        }

        private void AssertContainsSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
