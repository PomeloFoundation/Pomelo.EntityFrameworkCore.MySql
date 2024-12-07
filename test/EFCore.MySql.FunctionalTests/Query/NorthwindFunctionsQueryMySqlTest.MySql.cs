using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindFunctionsQueryMySqlTest
    {
        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadLeft_without_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadLeft(8) == "   ALFKI"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 8, ' ') = '   ALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadLeft_with_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadLeft(8, 'x') == "xxxALFKI"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE LPAD(`c`.`CustomerID`, 8, 'x') = 'xxxALFKI'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadRight_without_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadRight(8) == "ALFKI   "));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 8, ' ') = 'ALFKI   '");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public async Task PadRight_with_second_arg(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(r => r.CustomerID.PadRight(8, 'c') == "ALFKIccc"));

            AssertSql(@"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE RPAD(`c`.`CustomerID`, 8, 'c') = 'ALFKIccc'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_math_log_new_base2(bool async)
        {
            // The original `Where_math_log_new_base` test will succeed even if the number and base are swapped by accident.
            await AssertQueryScalar(
                async,
                ss => ss.Set<OrderDetail>().Where(od => od.OrderID == 11077 && od.Discount > 0).Where(od => Math.Log(od.Discount, 7) < -1).Select(od => Math.Log(od.Discount, 7)));

            AssertSql(
$"""
 SELECT LOG(7.0, {MySqlTestHelpers.CastAsDouble("`o`.`Discount`")})
 FROM `Order Details` AS `o`
 WHERE ((`o`.`OrderID` = 11077) AND (`o`.`Discount` > 0)) AND (LOG(7.0, {MySqlTestHelpers.CastAsDouble("`o`.`Discount`")}) < -1.0)
 """);
        }
    }
}
