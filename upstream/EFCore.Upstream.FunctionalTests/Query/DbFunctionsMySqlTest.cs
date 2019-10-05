// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Query
{
    public class DbFunctionsMySqlTest : DbFunctionsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public DbFunctionsMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void Like_literal()
        {
            base.Like_literal();

            AssertSql(
                @"SELECT COUNT(*)
FROM [Customers] AS [c]
WHERE [c].[ContactName] LIKE N'%M%'");
        }

        public override void Like_identity()
        {
            base.Like_identity();

            AssertSql(
                @"SELECT COUNT(*)
FROM [Customers] AS [c]
WHERE [c].[ContactName] LIKE [c].[ContactName]");
        }

        public override void Like_literal_with_escape()
        {
            base.Like_literal_with_escape();

            AssertSql(
                @"SELECT COUNT(*)
FROM [Customers] AS [c]
WHERE [c].[ContactName] LIKE N'!%' ESCAPE N'!'");
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task FreeText_literal()
        {
            using (var context = CreateContext())
            {
                var result = await context.Employees
                    .Where(c => EF.Functions.FreeText(c.Title, "Representative"))
                    .ToListAsync();

                Assert.Equal(result.First().EmployeeID, 1u);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE FREETEXT([c].[Title], N'Representative')");
            }
        }

        [ConditionalFact]
        public void FreeText_client_eval_throws()
        {
            Assert.Throws<InvalidOperationException>(() => EF.Functions.FreeText("teststring", "teststring"));
            Assert.Throws<InvalidOperationException>(() => EF.Functions.FreeText("teststring", "teststring", 1033));
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_multiple_words()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees
                    .Where(c => EF.Functions.FreeText(c.Title, "Representative Sales"))
                    .Count();

                Assert.Equal(result, 9);

                AssertSql(
                    @"SELECT COUNT(*)
FROM [Employees] AS [c]
WHERE FREETEXT([c].[Title], N'Representative Sales')");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_with_language_term()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees.SingleOrDefault(c => EF.Functions.FreeText(c.Title, "President", 1033));

                Assert.Equal(result.EmployeeID, 2u);

                AssertSql(
                    @"SELECT TOP(2) [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE FREETEXT([c].[Title], N'President', LANGUAGE 1033)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_with_multiple_words_and_language_term()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees
                    .Where(c => EF.Functions.FreeText(c.Title, "Representative President", 1033))
                    .ToList();

                Assert.Equal(result.First().EmployeeID, 1u);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE FREETEXT([c].[Title], N'Representative President', LANGUAGE 1033)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_multiple_predicates()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees
                    .Where(
                        c => EF.Functions.FreeText(c.City, "London")
                             && EF.Functions.FreeText(c.Title, "Manager", 1033))
                    .FirstOrDefault();

                Assert.Equal(result.EmployeeID, 5u);

                AssertSql(
                    @"SELECT TOP(1) [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE (FREETEXT([c].[City], N'London')) AND (FREETEXT([c].[Title], N'Manager', LANGUAGE 1033))");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_throws_for_no_FullText_index()
        {
            using (var context = CreateContext())
            {
                Assert.Throws<SqlException>(
                    () => context.Employees.Where(c => EF.Functions.FreeText(c.FirstName, "Fred")).ToArray());
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_through_navigation()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees
                    .Where(
                        c => EF.Functions.FreeText(c.Manager.Title, "President")
                             && EF.Functions.FreeText(c.Title, "Inside")
                             && c.FirstName.Contains("Lau"))
                    .LastOrDefault();

                Assert.Equal(result.EmployeeID, 8u);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
LEFT JOIN [Employees] AS [c.Manager] ON [c].[ReportsTo] = [c.Manager].[EmployeeID]
WHERE ((FREETEXT([c.Manager].[Title], N'President')) AND (FREETEXT([c].[Title], N'Inside'))) AND (CHARINDEX(N'Lau', [c].[FirstName]) > 0)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void FreeText_through_navigation_with_language_terms()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees
                    .Where(
                        c => EF.Functions.FreeText(c.Manager.Title, "President", 1033)
                             && EF.Functions.FreeText(c.Title, "Inside", 1031)
                             && c.FirstName.Contains("Lau"))
                    .LastOrDefault();

                Assert.Equal(result.EmployeeID, 8u);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
LEFT JOIN [Employees] AS [c.Manager] ON [c].[ReportsTo] = [c.Manager].[EmployeeID]
WHERE ((FREETEXT([c.Manager].[Title], N'President', LANGUAGE 1033)) AND (FREETEXT([c].[Title], N'Inside', LANGUAGE 1031))) AND (CHARINDEX(N'Lau', [c].[FirstName]) > 0)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task FreeText_throws_when_using_non_parameter_or_constant_for_freetext_string()
        {
            using (var context = CreateContext())
            {
                await Assert.ThrowsAsync<SqlException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.FreeText(e.City, e.FirstName)));

                await Assert.ThrowsAsync<SqlException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.FreeText(e.City, "")));

                await Assert.ThrowsAsync<SqlException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.FreeText(e.City, e.FirstName.ToUpper())));
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task FreeText_throws_when_using_non_column_for_property_reference()
        {
            using (var context = CreateContext())
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.FreeText(e.City + "1", "President")));

                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.FreeText(e.City.ToLower(), "President")));

                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await (from e1 in context.Employees
                                       join m1 in context.Employees.OrderBy(e => e.EmployeeID).Skip(0)
                                           on e1.ReportsTo equals m1.EmployeeID
                                       where EF.Functions.FreeText(m1.Title, "President")
                                       select e1).LastOrDefaultAsync());
            }
        }

        [ConditionalFact]
        public void Contains_should_throw_on_client_eval()
        {
            var exNoLang = Assert.Throws<InvalidOperationException>(() => EF.Functions.Contains("teststring", "teststring"));
            Assert.Equal(
                MySqlStrings.FunctionOnClient(nameof(MySqlDbFunctionsExtensions.Contains)),
                exNoLang.Message);

            var exLang = Assert.Throws<InvalidOperationException>(() => EF.Functions.Contains("teststring", "teststring", 1033));
            Assert.Equal(
                MySqlStrings.FunctionOnClient(nameof(MySqlDbFunctionsExtensions.Contains)),
                exLang.Message);
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task Contains_should_throw_when_using_non_parameter_or_constant_for_contains_string()
        {
            using (var context = CreateContext())
            {
                await Assert.ThrowsAsync<SqlException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.Contains(e.City, e.FirstName)));

                await Assert.ThrowsAsync<SqlException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.Contains(e.City, "")));

                await Assert.ThrowsAsync<SqlException>(
                    async () => await context.Employees.FirstOrDefaultAsync(
                        e => EF.Functions.Contains(e.City, e.FirstName.ToUpper())));
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void Contains_should_throw_for_no_FullText_index()
        {
            using (var context = CreateContext())
            {
                Assert.Throws<SqlException>(
                    () => context.Employees.Where(c => EF.Functions.Contains(c.FirstName, "Fred")).ToArray());
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task Contains_literal()
        {
            using (var context = CreateContext())
            {
                var result = await context.Employees
                    .Where(c => EF.Functions.Contains(c.Title, "Representative"))
                    .ToListAsync();

                Assert.Equal(result.First().EmployeeID, 1u);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE CONTAINS([c].[Title], N'Representative')");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void Contains_with_language_term()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees.SingleOrDefault(c => EF.Functions.Contains(c.Title, "President", 1033));

                Assert.Equal(result.EmployeeID, 2u);

                AssertSql(
                    @"SELECT TOP(2) [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE CONTAINS([c].[Title], N'President', LANGUAGE 1033)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task Contains_with_logical_operator()
        {
            using (var context = CreateContext())
            {
                var result = await context.Employees
                    .Where(c => EF.Functions.Contains(c.Title, "Vice OR Inside"))
                    .ToListAsync();

                Assert.Equal(2, result.Count);
                Assert.Equal(2u, result.First().EmployeeID);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE CONTAINS([c].[Title], N'Vice OR Inside')");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task Contains_with_prefix_term_and_language_term()
        {
            using (var context = CreateContext())
            {
                var result = await context.Employees
                    .SingleOrDefaultAsync(c => EF.Functions.Contains(c.Title, "\"Mana*\"", 1033));

                Assert.Equal(5u, result.EmployeeID);

                AssertSql(
                    @"SELECT TOP(2) [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE CONTAINS([c].[Title], N'""Mana*""', LANGUAGE 1033)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public async Task Contains_with_proximity_term_and_language_term()
        {
            using (var context = CreateContext())
            {
                var result = await context.Employees
                    .SingleOrDefaultAsync(c => EF.Functions.Contains(c.Title, "NEAR((Sales, President), 1)", 1033));

                Assert.Equal(2u, result.EmployeeID);

                AssertSql(
                    @"SELECT TOP(2) [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
WHERE CONTAINS([c].[Title], N'NEAR((Sales, President), 1)', LANGUAGE 1033)");
            }
        }

        [ConditionalFact]
        [MySqlCondition(MySqlCondition.SupportsFullTextSearch)]
        public void Contains_through_navigation()
        {
            using (var context = CreateContext())
            {
                var result = context.Employees
                    .Where(
                        c => EF.Functions.Contains(c.Manager.Title, "President")
                             && EF.Functions.Contains(c.Title, "\"Ins*\""))
                    .LastOrDefault();

                Assert.NotNull(result);
                Assert.Equal(result.EmployeeID, 8u);

                AssertSql(
                    @"SELECT [c].[EmployeeID], [c].[City], [c].[Country], [c].[FirstName], [c].[ReportsTo], [c].[Title]
FROM [Employees] AS [c]
LEFT JOIN [Employees] AS [c.Manager] ON [c].[ReportsTo] = [c.Manager].[EmployeeID]
WHERE (CONTAINS([c.Manager].[Title], N'President')) AND (CONTAINS([c].[Title], N'""Ins*""'))");
            }
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
FROM [Orders] AS [o]
WHERE (DATEDIFF(YEAR, [o].[OrderDate], GETDATE()) = 0) AND DATEDIFF(YEAR, [o].[OrderDate], GETDATE()) IS NOT NULL");
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
FROM [Orders] AS [o]
WHERE (DATEDIFF(MONTH, [o].[OrderDate], GETDATE()) = 0) AND DATEDIFF(MONTH, [o].[OrderDate], GETDATE()) IS NOT NULL");
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
FROM [Orders] AS [o]
WHERE (DATEDIFF(DAY, [o].[OrderDate], GETDATE()) = 0) AND DATEDIFF(DAY, [o].[OrderDate], GETDATE()) IS NOT NULL");
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
FROM [Orders] AS [o]
WHERE (DATEDIFF(HOUR, [o].[OrderDate], GETDATE()) = 0) AND DATEDIFF(HOUR, [o].[OrderDate], GETDATE()) IS NOT NULL");
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
FROM [Orders] AS [o]
WHERE (DATEDIFF(MINUTE, [o].[OrderDate], GETDATE()) = 0) AND DATEDIFF(MINUTE, [o].[OrderDate], GETDATE()) IS NOT NULL");
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
FROM [Orders] AS [o]
WHERE (DATEDIFF(SECOND, [o].[OrderDate], GETDATE()) = 0) AND DATEDIFF(SECOND, [o].[OrderDate], GETDATE()) IS NOT NULL");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Millisecond()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffMillisecond(DateTime.Now, DateTime.Now.AddDays(1)) == 0);

                Assert.Equal(0, count);
                AssertSql(
                    @"SELECT COUNT(*)
FROM [Orders] AS [o]
WHERE DATEDIFF(MILLISECOND, GETDATE(), DATEADD(day, CAST(1.0E0 AS int), GETDATE())) = 0");
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
FROM [Orders] AS [o]
WHERE DATEDIFF(MICROSECOND, GETDATE(), DATEADD(second, CAST(1.0E0 AS int), GETDATE())) = 0");
            }
        }

        [ConditionalFact]
        public virtual void DateDiff_Nanosecond()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders
                    .Count(c => EF.Functions.DateDiffNanosecond(DateTime.Now, DateTime.Now.AddSeconds(1)) == 0);

                Assert.Equal(0, count);
                AssertSql(
                    @"SELECT COUNT(*)
FROM [Orders] AS [o]
WHERE DATEDIFF(NANOSECOND, GETDATE(), DATEADD(second, CAST(1.0E0 AS int), GETDATE())) = 0");
            }
        }

        [ConditionalFact]
        public virtual void IsDate_not_valid()
        {
            using (var context = CreateContext())
            {
                var actual = context
                    .Orders
                    .Where(c => !EF.Functions.IsDate(c.CustomerID))
                    .Select(c => EF.Functions.IsDate(c.CustomerID))
                    .FirstOrDefault();

                Assert.Equal(actual, false);

                AssertSql(
                    @"SELECT TOP(1) CAST(ISDATE([o].[CustomerID]) AS bit)
FROM [Orders] AS [o]
WHERE CAST(ISDATE([o].[CustomerID]) AS bit) <> CAST(1 AS bit)");
            }
        }

        [ConditionalFact]
        public virtual void IsDate_valid()
        {
            using (var context = CreateContext())
            {
                var actual = context
                    .Orders
                    .Where(c => EF.Functions.IsDate(c.OrderDate.Value.ToString()))
                    .Select(c => EF.Functions.IsDate(c.OrderDate.Value.ToString()))
                    .FirstOrDefault();

                Assert.Equal(actual, true);

                AssertSql(
                    @"SELECT TOP(1) CAST(ISDATE(CONVERT(VARCHAR(100), [o].[OrderDate])) AS bit)
FROM [Orders] AS [o]
WHERE CAST(ISDATE(CONVERT(VARCHAR(100), [o].[OrderDate])) AS bit) = CAST(1 AS bit)");

            }
        }

        [ConditionalFact]
        public virtual void IsDate_join_fields()
        {
            using (var context = CreateContext())
            {
                var count = context.Orders.Count(c => EF.Functions.IsDate(c.CustomerID + c.OrderID));

                Assert.Equal(0, count);

                AssertSql(
                    @"SELECT COUNT(*)
FROM [Orders] AS [o]
WHERE CAST(ISDATE([o].[CustomerID] + CAST([o].[OrderID] AS nchar(5))) AS bit) = CAST(1 AS bit)");
            }
        }

        [ConditionalFact]
        public void IsDate_should_throw_on_client_eval()
        {
            var exIsDate = Assert.Throws<InvalidOperationException>(() => EF.Functions.IsDate("#ISDATE#"));

            Assert.Equal(
                MySqlStrings.FunctionOnClient(nameof(MySqlDbFunctionsExtensions.IsDate)),
                exIsDate.Message);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
