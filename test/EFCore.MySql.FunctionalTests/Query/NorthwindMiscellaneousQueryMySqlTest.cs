﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindMiscellaneousQueryMySqlTest : NorthwindMiscellaneousQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindMiscellaneousQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void Select_bitwise_or()
        {
            base.Select_bitwise_or();

            AssertSql(
                @"SELECT `c`.`CustomerID`, (`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override void Select_bitwise_or_multiple()
        {
            base.Select_bitwise_or_multiple();

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) | (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override void Select_bitwise_and()
        {
            base.Select_bitwise_and();

            AssertSql(
                @"SELECT `c`.`CustomerID`, (`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override void Select_bitwise_and_or()
        {
            base.Select_bitwise_and_or();

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) | (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override async Task Where_bitwise_or_with_logical_or(bool async)
        {
            await base.Where_bitwise_or_with_logical_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) OR (`c`.`CustomerID` = 'ANTON')");
        }

        public override async Task Where_bitwise_and_with_logical_and(bool async)
        {
            await base.Where_bitwise_and_with_logical_and(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) AND (`c`.`CustomerID` = 'ANTON')");
        }

        public override async Task Where_bitwise_or_with_logical_and(bool async)
        {
            await base.Where_bitwise_or_with_logical_and(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) AND (`c`.`Country` = 'Germany')");
        }

        public override async Task Where_bitwise_and_with_logical_or(bool async)
        {
            await base.Where_bitwise_and_with_logical_or(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) OR (`c`.`CustomerID` = 'ANTON')");
        }

        public override async Task Where_bitwise_binary_not(bool async)
        {
            await base.Where_bitwise_binary_not(async);

            AssertSql(
                @"@__negatedId_0='-10249'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE CAST(~`o`.`OrderID` AS signed) = @__negatedId_0");
        }

        public override async Task Where_bitwise_binary_and(bool async)
        {
            await base.Where_bitwise_binary_and(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (`o`.`OrderID` & 10248) = 10248");
        }

        public override async Task Where_bitwise_binary_or(bool async)
        {
            await base.Where_bitwise_binary_or(async);

            AssertSql(
                @"SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (`o`.`OrderID` | 10248) = 10248");
        }

        public override void Select_bitwise_or_with_logical_or()
        {
            base.Select_bitwise_or_with_logical_or();

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') | (`c`.`CustomerID` = 'ANATR')) OR (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        public override void Select_bitwise_and_with_logical_and()
        {
            base.Select_bitwise_and_with_logical_and();

            AssertSql(
                @"SELECT `c`.`CustomerID`, ((`c`.`CustomerID` = 'ALFKI') & (`c`.`CustomerID` = 'ANATR')) AND (`c`.`CustomerID` = 'ANTON') AS `Value`
FROM `Customers` AS `c`
ORDER BY `c`.`CustomerID`");
        }

        [ConditionalTheory]
        public override async Task Take_Skip(bool async)
        {
            await base.Take_Skip(async);

            AssertSql(
                @"@__p_0='10'
@__p_1='5'

SELECT `t`.`CustomerID`, `t`.`Address`, `t`.`City`, `t`.`CompanyName`, `t`.`ContactName`, `t`.`ContactTitle`, `t`.`Country`, `t`.`Fax`, `t`.`Phone`, `t`.`PostalCode`, `t`.`Region`
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactName`
    LIMIT @__p_0
) AS `t`
ORDER BY `t`.`ContactName`
LIMIT 18446744073709551610 OFFSET @__p_1");
        }

        [ConditionalTheory]
        public override async Task Select_expression_references_are_updated_correctly_with_subquery(bool async)
        {
            await base.Select_expression_references_are_updated_correctly_with_subquery(async);

            AssertSql(
                @"@__nextYear_0='2017'

SELECT DISTINCT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` IS NOT NULL AND (EXTRACT(year FROM `o`.`OrderDate`) < @__nextYear_0)");
        }

        public override Task Entity_equality_orderby_subquery(bool async)
        {
            // Ordering in the base test is arbitrary.
            return AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.OrderBy(o => o.OrderID).FirstOrDefault()).ThenBy(c => c.CustomerID),
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.FirstOrDefault() == null ? (int?)null : c.Orders.OrderBy(o => o.OrderID).FirstOrDefault().OrderID).ThenBy(c => c.CustomerID),
                entryCount: 91,
                assertOrder: true);
        }

        [SupportedServerVersionTheory(ServerVersion.WindowFunctionsSupportKey)]
        public override Task SelectMany_Joined_Take(bool async)
        {
            return base.SelectMany_Joined_Take(async);
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        public override Task Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(bool async)
        {
            // MySql.Data.MySqlClient.MySqlException: Reference 'CustomerID' not supported (forward reference in item list)
            return Assert.ThrowsAsync<MySqlException>(() => base.Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(async));
        }

        [SupportedServerVersionTheory(ServerVersion.OuterApplySupportKey)]
        public override Task AsQueryable_in_query_server_evals(bool async)
        {
            return base.AsQueryable_in_query_server_evals(async);
        }

        [SupportedServerVersionTheory(ServerVersion.CrossApplySupportKey)]
        public override Task SelectMany_correlated_subquery_hard(bool async)
        {
            return base.SelectMany_correlated_subquery_hard(async);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}