using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindDbFunctionsQueryMySqlTest
    {
        [ConditionalFact]
        public virtual void DateDiff_Year()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffYear(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(YEAR, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Quarter()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffQuarter(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(QUARTER, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Week()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffWeek(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(WEEK, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Month()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffMonth(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(MONTH, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Day()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffDay(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(DAY, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Hour()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.DateDiffHour(o.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(HOUR, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Minute()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffMinute(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(MINUTE, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Second()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.DateDiffSecond(o.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(SECOND, `o`.`OrderDate`, CURRENT_TIMESTAMP(6)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Millisecond()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.DateDiffMillisecond(DateTime.Now, DateTime.Now.AddSeconds(1)) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE (TIMESTAMPDIFF(MICROSECOND, CURRENT_TIMESTAMP(6), DATE_ADD(CURRENT_TIMESTAMP(6), INTERVAL CAST(1.0 AS signed) second))) DIV (1000) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Microsecond()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.DateDiffMicrosecond(DateTime.Now, DateTime.Now.AddSeconds(1)) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE TIMESTAMPDIFF(MICROSECOND, CURRENT_TIMESTAMP(6), DATE_ADD(CURRENT_TIMESTAMP(6), INTERVAL CAST(1.0 AS signed) second)) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Tick()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.DateDiffTick(DateTime.Now, DateTime.Now.AddSeconds(1)) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE (TIMESTAMPDIFF(MICROSECOND, CURRENT_TIMESTAMP(6), DATE_ADD(CURRENT_TIMESTAMP(6), INTERVAL CAST(1.0 AS signed) second)) * 10) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Nanosecond()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.DateDiffNanosecond(DateTime.Now, DateTime.Now.AddSeconds(1)) == 0);

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE (TIMESTAMPDIFF(MICROSECOND, CURRENT_TIMESTAMP(6), DATE_ADD(CURRENT_TIMESTAMP(6), INTERVAL CAST(1.0 AS signed) second)) * 1000) = 0");
            }
        }

        [ConditionalFact]
        public virtual void Like_Int_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(o => EF.Functions.Like(o.OrderID, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE `o`.`OrderID` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_DateTime_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(o => EF.Functions.Like(o.OrderDate, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Uint_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(o => EF.Functions.Like(o.EmployeeID, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE `o`.`EmployeeID` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Short_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.OrderDetails.Count(o => EF.Functions.Like(o.Quantity, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Order Details` AS `o`
WHERE `o`.`Quantity` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Int_literal_with_escape()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(o => EF.Functions.Like(o.OrderID, "!%", "!"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE `o`.`OrderID` LIKE '!%' ESCAPE '!'");
            }
        }

        [ConditionalFact]
        public virtual void Hex()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.Hex(o.CustomerID) == "56494E4554");

                Assert.Equal(5, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE HEX(`o`.`CustomerID`) = '56494E4554'");
            }
        }

        [ConditionalFact]
        public virtual void Unhex()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(o => EF.Functions.Unhex(EF.Functions.Hex(o.CustomerID)) == "VINET");

                Assert.Equal(5, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `o`
WHERE UNHEX(HEX(`o`.`CustomerID`)) = 'VINET'");
            }
        }

        [ConditionalFact]
        public virtual void Degrees()
        {
            var degrees = 90.0;
            var radians = Math.PI / 180.0 * degrees;

            using var context = CreateContext();

            var office = context.Customers
                .Select(c => new { c.CustomerID, OfficeRoofAngleDegrees = EF.Functions.Degrees(radians) })
                .First(c => c.CustomerID == "VINET");

            Assert.Equal(degrees, office.OfficeRoofAngleDegrees);

            AssertSql(
                @"@__radians_1='1.5707963267948966'

SELECT `c`.`CustomerID`, DEGREES(@__radians_1) AS `OfficeRoofAngleDegrees`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'VINET'
LIMIT 1");
        }

        [ConditionalFact]
        public virtual void Radians()
        {
            var degrees = 90.0;
            var radians = Math.PI / 180.0 * degrees;

            using var context = CreateContext();

            var office = context.Customers
                .Select(c => new { c.CustomerID, OfficeRoofAngleRadians = EF.Functions.Radians(degrees) })
                .First(c => c.CustomerID == "VINET");

            Assert.Equal(radians, office.OfficeRoofAngleRadians);

            AssertSql(
                @"@__degrees_1='90'

SELECT `c`.`CustomerID`, RADIANS(@__degrees_1) AS `OfficeRoofAngleRadians`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'VINET'
LIMIT 1");
        }

        [ConditionalFact]
        public virtual void Contains_with_escape_char()
        {
            using var context = CreateContext();
            var count = context.Customers.Count(c => c.CompanyName.Replace("/", @"\").Contains(@"\"));

            Assert.Equal(1, count);

            AssertSql(
"""
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`CompanyName`, '/', '\\') LIKE '%\\\\%'
""");
        }

        [ConditionalFact]
        public virtual void Contains_with_wild_char()
        {
            using var context = CreateContext();
            var count = context.Customers.Count(c => c.CompanyName.Replace("/", "%").Contains("%"));

            Assert.Equal(1, count);

            AssertSql(
"""
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`CompanyName`, '/', '%') LIKE '%\\%%'
""");
        }

        [ConditionalFact]
        public virtual void StartsWith_with_escape_char()
        {
            using var context = CreateContext();
            var count = context.Customers.Count(c => c.CompanyName.Replace("A", @"\").StartsWith(@"\"));

            Assert.Equal(4, count);

            AssertSql(
"""
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`CompanyName`, 'A', '\\') LIKE '\\\\%'
""");
        }

        [ConditionalFact]
        public virtual void StartsWith_with_wild_char()
        {
            using var context = CreateContext();
            var count = context.Customers.Count(c => c.CompanyName.Replace("A", @"%").StartsWith(@"%"));

            Assert.Equal(4, count);

            AssertSql(
"""
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`CompanyName`, 'A', '%') LIKE '\\%%'
""");
        }

        [ConditionalFact]
        public virtual void EndsWith_with_escape_char()
        {
            using var context = CreateContext();
            var count = context.Customers.Count(c => c.CompanyName.Replace("a", @"\").EndsWith(@"\"));

            Assert.Equal(7, count);

            AssertSql(
"""
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`CompanyName`, 'a', '\\') LIKE '%\\\\'
""");
        }

        [ConditionalFact]
        public virtual void EndsWith_with_wild_char()
        {
            using var context = CreateContext();
            var count = context.Customers.Count(c => c.CompanyName.Replace("a", @"%").EndsWith(@"%"));

            Assert.Equal(7, count);

            AssertSql(
"""
SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE REPLACE(`c`.`CompanyName`, 'a', '%') LIKE '%\\%'
""");
        }
    }
}
