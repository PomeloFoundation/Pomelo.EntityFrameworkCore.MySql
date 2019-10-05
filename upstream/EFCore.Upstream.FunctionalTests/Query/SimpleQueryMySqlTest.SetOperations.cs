// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    public partial class SimpleQueryMySqlTest
    {
        public override async Task Union(bool isAsync)
        {
            await base.Union(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
UNION
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM [Customers] AS [c0]
WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL");
        }

        public override async Task Concat(bool isAsync)
        {
            await base.Concat(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
UNION ALL
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM [Customers] AS [c0]
WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL");
        }

        public override async Task Intersect(bool isAsync)
        {
            await base.Intersect(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'London') AND [c].[City] IS NOT NULL
INTERSECT
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM [Customers] AS [c0]
WHERE CHARINDEX(N'Thomas', [c0].[ContactName]) > 0");
        }

        public override async Task Except(bool isAsync)
        {
            await base.Except(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'London') AND [c].[City] IS NOT NULL
EXCEPT
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM [Customers] AS [c0]
WHERE CHARINDEX(N'Thomas', [c0].[ContactName]) > 0");        }

        public override async Task Union_OrderBy_Skip_Take(bool isAsync)
        {
            await base.Union_OrderBy_Skip_Take(isAsync);

            AssertSql(
                @"@__p_0='1'

SELECT [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region]
FROM (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
) AS [t]
ORDER BY [t].[ContactName]
OFFSET @__p_0 ROWS FETCH NEXT @__p_0 ROWS ONLY");
        }

        public override async Task Union_Where(bool isAsync)
        {
            await base.Union_Where(isAsync);

            AssertSql(
                @"SELECT [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region]
FROM (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
) AS [t]
WHERE CHARINDEX(N'Thomas', [t].[ContactName]) > 0");
        }

        public override async Task Union_Skip_Take_OrderBy_ThenBy_Where(bool isAsync)
        {
            await base.Union_Skip_Take_OrderBy_ThenBy_Where(isAsync);

            AssertSql(
                @"@__p_0='0'

SELECT [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region]
FROM (
    SELECT [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region]
    FROM (
        SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
        UNION
        SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
        FROM [Customers] AS [c0]
        WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
    ) AS [t]
    ORDER BY [t].[Region], [t].[City]
    OFFSET @__p_0 ROWS
) AS [t0]
WHERE CHARINDEX(N'Thomas', [t0].[ContactName]) > 0
ORDER BY [t0].[Region], [t0].[City]");
        }

        public override async Task Union_Union(bool isAsync)
        {
            await base.Union_Union(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
UNION
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM [Customers] AS [c0]
WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
UNION
SELECT [c1].[CustomerID], [c1].[Address], [c1].[City], [c1].[CompanyName], [c1].[ContactName], [c1].[ContactTitle], [c1].[Country], [c1].[Fax], [c1].[Phone], [c1].[PostalCode], [c1].[Region]
FROM [Customers] AS [c1]
WHERE ([c1].[City] = N'Mannheim') AND [c1].[City] IS NOT NULL");
        }

        public override async Task Union_Intersect(bool isAsync)
        {
            await base.Union_Intersect(isAsync);

            AssertSql(
                @"(
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
)
INTERSECT
SELECT [c1].[CustomerID], [c1].[Address], [c1].[City], [c1].[CompanyName], [c1].[ContactName], [c1].[ContactTitle], [c1].[Country], [c1].[Fax], [c1].[Phone], [c1].[PostalCode], [c1].[Region]
FROM [Customers] AS [c1]
WHERE CHARINDEX(N'Thomas', [c1].[ContactName]) > 0");
        }

        [ConditionalTheory]
        public override async Task Union_Take_Union_Take(bool isAsync)
        {
            await base.Union_Take_Union_Take(isAsync);

            AssertSql(
                @"@__p_0='1'

SELECT [t1].[CustomerID], [t1].[Address], [t1].[City], [t1].[CompanyName], [t1].[ContactName], [t1].[ContactTitle], [t1].[Country], [t1].[Fax], [t1].[Phone], [t1].[PostalCode], [t1].[Region]
FROM (
    SELECT TOP(@__p_0) [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region]
    FROM (
        SELECT TOP(@__p_0) [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region]
        FROM (
            SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
            FROM [Customers] AS [c]
            WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
            UNION
            SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
            FROM [Customers] AS [c0]
            WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
        ) AS [t]
        ORDER BY [t].[CustomerID]
        UNION
        SELECT [c1].[CustomerID], [c1].[Address], [c1].[City], [c1].[CompanyName], [c1].[ContactName], [c1].[ContactTitle], [c1].[Country], [c1].[Fax], [c1].[Phone], [c1].[PostalCode], [c1].[Region]
        FROM [Customers] AS [c1]
        WHERE ([c1].[City] = N'Mannheim') AND [c1].[City] IS NOT NULL
    ) AS [t0]
) AS [t1]
ORDER BY [t1].[CustomerID]");
        }

        public override async Task Select_Union(bool isAsync)
        {
            await base.Select_Union(isAsync);

            AssertSql(@"SELECT [c].[Address]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
UNION
SELECT [c0].[Address]
FROM [Customers] AS [c0]
WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL");
        }

        public override async Task Union_Select(bool isAsync)
        {
            await base.Union_Select(isAsync);

            AssertSql(@"SELECT [t].[Address]
FROM (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
) AS [t]
WHERE CHARINDEX(N'Hanover', [t].[Address]) > 0");
        }

        public override async Task Union_with_anonymous_type_projection(bool isAsync)
        {
            await base.Union_with_anonymous_type_projection(isAsync);

            AssertSql(
                @"SELECT [t].[CustomerID] AS [Id]
FROM (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE [c].[CompanyName] IS NOT NULL AND ([c].[CompanyName] LIKE N'A%')
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE [c0].[CompanyName] IS NOT NULL AND ([c0].[CompanyName] LIKE N'B%')
) AS [t]");
        }

        public override async Task Select_Union_unrelated(bool isAsync)
        {
            await base.Select_Union_unrelated(isAsync);

            AssertSql(
                @"SELECT [t].[ContactName]
FROM (
    SELECT [c].[ContactName]
    FROM [Customers] AS [c]
    UNION
    SELECT [p].[ProductName] AS [ContactName]
    FROM [Products] AS [p]
) AS [t]
WHERE [t].[ContactName] IS NOT NULL AND ([t].[ContactName] LIKE N'C%')
ORDER BY [t].[ContactName]");
        }

        public override async Task Select_Union_different_fields_in_anonymous_with_subquery(bool isAsync)
        {
            await base.Select_Union_different_fields_in_anonymous_with_subquery(isAsync);

            AssertSql(
                @"@__p_0='1'
@__p_1='10'

SELECT [t0].[Foo], [t0].[CustomerID], [t0].[Address], [t0].[City], [t0].[CompanyName], [t0].[ContactName], [t0].[ContactTitle], [t0].[Country], [t0].[Fax], [t0].[Phone], [t0].[PostalCode], [t0].[Region]
FROM (
    SELECT [t].[Foo], [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region]
    FROM (
        SELECT [c].[City] AS [Foo], [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
        FROM [Customers] AS [c]
        WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
        UNION
        SELECT [c0].[Region] AS [Foo], [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
        FROM [Customers] AS [c0]
        WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
    ) AS [t]
    ORDER BY [t].[Foo]
    OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
) AS [t0]
WHERE ([t0].[Foo] = N'Berlin') AND [t0].[Foo] IS NOT NULL
ORDER BY [t0].[Foo]");
        }

        public override async Task Union_Include(bool isAsync)
        {
            await base.Union_Include(isAsync);

            AssertSql(
                @"SELECT [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region], [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
) AS [t]
LEFT JOIN [Orders] AS [o] ON [t].[CustomerID] = [o].[CustomerID]
ORDER BY [t].[CustomerID], [o].[OrderID]");
        }

        public override async Task Include_Union(bool isAsync)
        {
            await base.Include_Union(isAsync);

            AssertSql(
                @"SELECT [t].[CustomerID], [t].[Address], [t].[City], [t].[CompanyName], [t].[ContactName], [t].[ContactTitle], [t].[Country], [t].[Fax], [t].[Phone], [t].[PostalCode], [t].[Region], [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM (
    SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
    FROM [Customers] AS [c]
    WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
    UNION
    SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
    FROM [Customers] AS [c0]
    WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
) AS [t]
LEFT JOIN [Orders] AS [o] ON [t].[CustomerID] = [o].[CustomerID]
ORDER BY [t].[CustomerID], [o].[OrderID]");
        }

        public override async Task Select_Except_reference_projection(bool isAsync)
        {
            await base.Select_Except_reference_projection(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [c] ON [o].[CustomerID] = [c].[CustomerID]
EXCEPT
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region]
FROM [Orders] AS [o0]
LEFT JOIN [Customers] AS [c0] ON [o0].[CustomerID] = [c0].[CustomerID]
WHERE ([o0].[CustomerID] = N'ALFKI') AND [o0].[CustomerID] IS NOT NULL");
        }

        public override async Task SubSelect_Union(bool isAsync)
        {
            await base.SubSelect_Union(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region], (
    SELECT COUNT(*)
    FROM [Orders] AS [o]
    WHERE ([c].[CustomerID] = [o].[CustomerID]) AND [o].[CustomerID] IS NOT NULL) AS [Orders]
FROM [Customers] AS [c]
UNION
SELECT [c0].[CustomerID], [c0].[Address], [c0].[City], [c0].[CompanyName], [c0].[ContactName], [c0].[ContactTitle], [c0].[Country], [c0].[Fax], [c0].[Phone], [c0].[PostalCode], [c0].[Region], (
    SELECT COUNT(*)
    FROM [Orders] AS [o0]
    WHERE ([c0].[CustomerID] = [o0].[CustomerID]) AND [o0].[CustomerID] IS NOT NULL) AS [Orders]
FROM [Customers] AS [c0]");
        }

        public override async Task GroupBy_Select_Union(bool isAsync)
        {
            await base.GroupBy_Select_Union(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], COUNT(*) AS [Count]
FROM [Customers] AS [c]
WHERE ([c].[City] = N'Berlin') AND [c].[City] IS NOT NULL
GROUP BY [c].[CustomerID]
UNION
SELECT [c0].[CustomerID], COUNT(*) AS [Count]
FROM [Customers] AS [c0]
WHERE ([c0].[City] = N'London') AND [c0].[City] IS NOT NULL
GROUP BY [c0].[CustomerID]");
        }

        public override async Task Union_over_different_projection_types(bool isAsync, string leftType, string rightType)
        {
            await base.Union_over_different_projection_types(isAsync, leftType, rightType);

            var leftSql = GenerateSql(leftType);
            var rightSql = GenerateSql(rightType);

            // Fix up right-side SQL as table aliases shift
            rightSql = leftType == "ScalarSubquery"
                ? rightSql.Replace("[o]", "[o1]").Replace("[o0]", "[o2]")
                : rightSql.Replace("[o0]", "[o1]").Replace("[o]", "[o0]");

            AssertSql(leftSql + Environment.NewLine + "UNION" + Environment.NewLine + rightSql);

            static string GenerateSql(string expressionType)
            {
                switch (expressionType)
                {
                    case "Column":
                        return @"SELECT [o].[OrderID]
FROM [Orders] AS [o]";
                    case "Function":
                        return @"SELECT COUNT(*)
FROM [Orders] AS [o]
GROUP BY [o].[OrderID]";
                    case "Constant":
                        return @"SELECT 8
FROM [Orders] AS [o]";
                    case "Unary":
                        return @"SELECT -[o].[OrderID]
FROM [Orders] AS [o]";
                    case "Binary":
                        return @"SELECT [o].[OrderID] + 1
FROM [Orders] AS [o]";
                    case "ScalarSubquery":
                        return @"SELECT (
    SELECT COUNT(*)
    FROM [Order Details] AS [o]
    WHERE [o0].[OrderID] = [o].[OrderID])
FROM [Orders] AS [o0]";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
