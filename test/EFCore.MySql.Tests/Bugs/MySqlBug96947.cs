﻿using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.Bugs
{
    public class MySqlBug96947 : RawSqlTestWithFixture<MySqlBug96947.MySqlBug96947Fixture>
    {
        public MySqlBug96947(MySqlBug96947Fixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
        }

        [Fact]
        public void Bug_exists_in_MySql57_or_higher()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = @"
SELECT `od1`.`Constant`, `od1`.`OrderID`
FROM `Orders` AS `o`
LEFT JOIN (
    SELECT 'MyConstantValue' AS `Constant`, `od`.`OrderID`
    FROM `Order Details` AS `od`
) AS `od1` ON `o`.`OrderID` = `od1`.`OrderID`
ORDER BY `od1`.`OrderID`;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var constant = reader["Constant"];
                var expected = ServerVersion.FromString(Connection.ServerVersion).Supports.MySqlBug96947Workaround
                    ? (object)DBNull.Value
                    : "MyConstantValue";

                Assert.Equal(expected, constant);
            }
        }

        public class MySqlBug96947Fixture : MySqlTestFixtureBase<ContextBase>
        {
            protected override string SetupDatabaseScript => @"
CREATE TABLE `Orders` (
  `OrderID` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`OrderID`)
) ENGINE=InnoDB;

CREATE TABLE `Order Details` (
  `OrderID` int(11) NOT NULL,
  PRIMARY KEY (`OrderID`)
) ENGINE=InnoDB;

INSERT INTO `Orders` (`OrderID`) VALUES (1);
INSERT INTO `Orders` (`OrderID`) VALUES (2);

INSERT INTO `Order Details` (`OrderID`) VALUES (1);
INSERT INTO `Order Details` (`OrderID`) VALUES (2);";
        }
    }
}
