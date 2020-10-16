using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class DbFunctionsMySqlTest
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
WHERE TIMESTAMPDIFF(YEAR, `o`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
WHERE TIMESTAMPDIFF(MONTH, `o`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
WHERE TIMESTAMPDIFF(DAY, `o`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
WHERE TIMESTAMPDIFF(HOUR, `o`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
WHERE TIMESTAMPDIFF(MINUTE, `o`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
WHERE TIMESTAMPDIFF(SECOND, `o`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
WHERE TIMESTAMPDIFF(MICROSECOND, CURRENT_TIMESTAMP(), DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL CAST(1.0 AS signed) second)) = 0");
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
        public virtual void Like_Client_InvariantCulture_nullable()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");
            DateTime? exampleDate = new DateTime(2019, 8, 1, 18, 32, 6);

            Assert.True(EF.Functions.Like(exampleDate, "08/01/2019%"));

            double? d = 12.34D;
            Assert.True(EF.Functions.Like(d, "12.3%"));

            byte[] b = {0x30, 0x31};
            Assert.True(EF.Functions.Like(b, "30%"));
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
        public virtual void Hex_client_eval()
        {
            var hex = EF.Functions.Hex("VINET");

            Assert.Equal("56494E4554", hex);
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
        public virtual void Unhex_client_eval()
        {
            var unhex = EF.Functions.Unhex("56494E4554");

            Assert.Equal("VINET", unhex);
        }

        [Fact]
        public void JsonQuote_client_eval()
        {
            var quoted = EF.Functions.JsonQuote("first\\\"second\"\tthird");

            Assert.Equal(@"""first\\\""second\""\tthird""", quoted);
        }

        [Fact]
        public void JsonUnquote_client_eval()
        {
            var unquoted = EF.Functions.JsonUnquote(@"""first\\\""second\""\tthird""");

            Assert.Equal("first\\\"second\"\tthird", unquoted);
        }

        [Fact]
        public void JsonUnquote_client_eval_unicode()
        {
            var unquoted = EF.Functions.JsonUnquote(@"""a\u003da""");

            Assert.Equal("a=a", unquoted);
        }

        [Fact]
        public void JsonUnquote_client_eval_throws()
        {
            Assert.Equal(
                "The JSON string is not well formed.",
                Assert.Throws<ArgumentException>(
                        () => EF.Functions.JsonUnquote(@"""a\u3da"""))
                    .Message);
        }
    }
}
