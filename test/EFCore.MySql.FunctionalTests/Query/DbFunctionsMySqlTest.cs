using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Pomelo.EntityFrameworkCore.MySql.Extensions;
using Xunit;
using Xunit.Abstractions;
using Pomelo.EntityFrameworkCore.MySql.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class DbFunctionsMySqlTest : DbFunctionsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public DbFunctionsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

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
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(YEAR, `c`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(MONTH, `c`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(DAY, `c`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Hour()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffHour(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);
                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(HOUR, `c`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
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
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(MINUTE, `c`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Second()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffSecond(c.OrderDate, DateTime.Now) == 0);

                Assert.Equal(0, count);
                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(SECOND, `c`.`OrderDate`, CURRENT_TIMESTAMP()) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Microsecond()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffMicrosecond(DateTime.Now, DateTime.Now.AddSeconds(1)) == 0);

                Assert.Equal(0, count);
                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE TIMESTAMPDIFF(MICROSECOND, CURRENT_TIMESTAMP(), DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL 1.0 second)) = 0");
            }
        }

        public override void Like_literal()
        {
            base.Like_literal();

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '%M%'");
        }

        public override void Like_identity()
        {
            base.Like_identity();

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE `c`.`ContactName`");
        }

        public override void Like_literal_with_escape()
        {
            base.Like_literal_with_escape();

            AssertSql(
                @"SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`ContactName` LIKE '!%' ESCAPE '!'");
        }

        [ConditionalFact]
        public virtual void Like_Int_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(c => EF.Functions.Like(c.OrderID, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE `c`.`OrderID` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_DateTime_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(c => EF.Functions.Like(c.OrderDate, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE `c`.`OrderDate` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Uint_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(c => EF.Functions.Like(c.EmployeeID, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE `c`.`EmployeeID` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Short_literal()
        {
            using (var context = CreateContext())
            {
                var count = context.OrderDetails.Count(c => EF.Functions.Like(c.Quantity, "%M%"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Order Details` AS `c`
WHERE `c`.`Quantity` LIKE '%M%'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Int_literal_with_escape()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(c => EF.Functions.Like(c.OrderID, "!%", "!"));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM `Orders` AS `c`
WHERE `c`.`OrderID` LIKE '!%' ESCAPE '!'");
            }
        }

        [ConditionalFact]
        public virtual void Like_Client_InvariantCulture()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");
            DateTime? exampleDate = new DateTime(2019, 8, 1, 18, 32, 6);

            Assert.True(EF.Functions.Like(exampleDate, "08/01/2019%"));

            double? d = 12.34D;
            Assert.True(EF.Functions.Like(d, "12.3%"));

            byte[] b = { 0x30, 0x31 };
            Assert.True(EF.Functions.Like(b, "30%"));
        }

        private void AssertSql(params string[] expected) => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}