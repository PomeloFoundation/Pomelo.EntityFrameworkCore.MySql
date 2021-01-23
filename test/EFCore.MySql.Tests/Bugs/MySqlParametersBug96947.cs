using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.Bugs
{
    public class MySqlParametersBug96947 : RawSqlTestWithFixture<MySqlParametersBug96947.MySqlBug96947Fixture>
    {
        public MySqlParametersBug96947(MySqlBug96947Fixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
        }

        [Fact]
        public void Bug_exists_in_MySql57_or_higher()
        {
            var paramValue = Guid.NewGuid();
            var paramName = "testParam";
            var sqlParamName = $"@{paramName}";

            using var command = Connection.CreateCommand();
            command.Parameters.Add(new MySqlParameter(paramName, paramValue));
            command.CommandText = @$"
                SELECT `od1`.`Constant`, `od1`.`OrderID`
                FROM `Orders` AS `o`
                LEFT JOIN (
                    SELECT {sqlParamName} AS `Constant`, `od`.`OrderID`
                    FROM `Order Details` AS `od`
                ) AS `od1` ON `o`.`OrderID` = `od1`.`OrderID`
                ORDER BY `od1`.`OrderID`;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var constant = reader["Constant"];
                var expected = ServerVersion.FromString(Connection.ServerVersion).Supports.MySqlBug96947Workaround
                    ? (object)DBNull.Value
                    : paramValue.ToString();

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
