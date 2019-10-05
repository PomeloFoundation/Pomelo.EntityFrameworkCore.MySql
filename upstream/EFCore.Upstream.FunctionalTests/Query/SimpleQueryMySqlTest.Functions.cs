// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.Query
{
    public partial class SimpleQueryMySqlTest
    {
        public override async Task String_StartsWith_Literal(bool isAsync)
        {
            await base.String_StartsWith_Literal(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[ContactName] IS NOT NULL AND ([c].[ContactName] LIKE N'M%')");
        }

        public override async Task String_StartsWith_Identity(bool isAsync)
        {
            await base.String_StartsWith_Identity(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (([c].[ContactName] = N'') AND [c].[ContactName] IS NOT NULL) OR ([c].[ContactName] IS NOT NULL AND ([c].[ContactName] IS NOT NULL AND (([c].[ContactName] LIKE [c].[ContactName] + N'%') AND (((LEFT([c].[ContactName], LEN([c].[ContactName])) = [c].[ContactName]) AND (LEFT([c].[ContactName], LEN([c].[ContactName])) IS NOT NULL AND [c].[ContactName] IS NOT NULL)) OR (LEFT([c].[ContactName], LEN([c].[ContactName])) IS NULL AND [c].[ContactName] IS NULL)))))");
        }

        public override async Task String_StartsWith_Column(bool isAsync)
        {
            await base.String_StartsWith_Column(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (([c].[ContactName] = N'') AND [c].[ContactName] IS NOT NULL) OR ([c].[ContactName] IS NOT NULL AND ([c].[ContactName] IS NOT NULL AND (([c].[ContactName] LIKE [c].[ContactName] + N'%') AND (((LEFT([c].[ContactName], LEN([c].[ContactName])) = [c].[ContactName]) AND (LEFT([c].[ContactName], LEN([c].[ContactName])) IS NOT NULL AND [c].[ContactName] IS NOT NULL)) OR (LEFT([c].[ContactName], LEN([c].[ContactName])) IS NULL AND [c].[ContactName] IS NULL)))))");
        }

        public override async Task String_StartsWith_MethodCall(bool isAsync)
        {
            await base.String_StartsWith_MethodCall(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[ContactName] IS NOT NULL AND ([c].[ContactName] LIKE N'M%')");
        }

        public override async Task String_EndsWith_Literal(bool isAsync)
        {
            await base.String_EndsWith_Literal(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[ContactName] IS NOT NULL AND ([c].[ContactName] LIKE N'%b')");
        }

        public override async Task String_EndsWith_Identity(bool isAsync)
        {
            await base.String_EndsWith_Identity(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (([c].[ContactName] = N'') AND [c].[ContactName] IS NOT NULL) OR ([c].[ContactName] IS NOT NULL AND ([c].[ContactName] IS NOT NULL AND (((RIGHT([c].[ContactName], LEN([c].[ContactName])) = [c].[ContactName]) AND (RIGHT([c].[ContactName], LEN([c].[ContactName])) IS NOT NULL AND [c].[ContactName] IS NOT NULL)) OR (RIGHT([c].[ContactName], LEN([c].[ContactName])) IS NULL AND [c].[ContactName] IS NULL))))");
        }

        public override async Task String_EndsWith_Column(bool isAsync)
        {
            await base.String_EndsWith_Column(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (([c].[ContactName] = N'') AND [c].[ContactName] IS NOT NULL) OR ([c].[ContactName] IS NOT NULL AND ([c].[ContactName] IS NOT NULL AND (((RIGHT([c].[ContactName], LEN([c].[ContactName])) = [c].[ContactName]) AND (RIGHT([c].[ContactName], LEN([c].[ContactName])) IS NOT NULL AND [c].[ContactName] IS NOT NULL)) OR (RIGHT([c].[ContactName], LEN([c].[ContactName])) IS NULL AND [c].[ContactName] IS NULL))))");
        }

        public override async Task String_EndsWith_MethodCall(bool isAsync)
        {
            await base.String_EndsWith_MethodCall(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[ContactName] IS NOT NULL AND ([c].[ContactName] LIKE N'%m')");
        }

        public override async Task String_Contains_Literal(bool isAsync)
        {
            await AssertQuery<Customer>(
                isAsync,
                cs => cs.Where(c => c.ContactName.Contains("M")), // case-insensitive
                cs => cs.Where(c => c.ContactName.Contains("M") || c.ContactName.Contains("m")), // case-sensitive
                entryCount: 34);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE CHARINDEX(N'M', [c].[ContactName]) > 0");
        }

        public override async Task String_Contains_Identity(bool isAsync)
        {
            await base.String_Contains_Identity(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (([c].[ContactName] = N'') AND [c].[ContactName] IS NOT NULL) OR (CHARINDEX([c].[ContactName], [c].[ContactName]) > 0)");
        }

        public override async Task String_Contains_Column(bool isAsync)
        {
            await base.String_Contains_Column(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (([c].[ContactName] = N'') AND [c].[ContactName] IS NOT NULL) OR (CHARINDEX([c].[ContactName], [c].[ContactName]) > 0)");
        }

        public override async Task String_Contains_MethodCall(bool isAsync)
        {
            await AssertQuery<Customer>(
                isAsync,
                cs => cs.Where(c => c.ContactName.Contains(LocalMethod1())), // case-insensitive
                cs => cs.Where(
                    c => c.ContactName.Contains(LocalMethod1().ToLower())
                         || c.ContactName.Contains(LocalMethod1().ToUpper())), // case-sensitive
                entryCount: 34);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE CHARINDEX(N'M', [c].[ContactName]) > 0");
        }

        public override async Task String_Compare_simple_zero(bool isAsync)
        {
            await base.String_Compare_simple_zero(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <> N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'");
        }

        public override async Task String_Compare_simple_one(bool isAsync)
        {
            await base.String_Compare_simple_one(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] < N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= N'ALFKI'");
        }

        public override async Task String_compare_with_parameter(bool isAsync)
        {
            await base.String_compare_with_parameter(isAsync);

            // issue #15994
//            AssertSql(
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] < @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= @__customer_CustomerID_0");
        }

        public override async Task String_Compare_simple_more_than_one(bool isAsync)
        {
            await base.String_Compare_simple_more_than_one(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]");
        }

        public override async Task String_Compare_nested(bool isAsync)
        {
            await base.String_Compare_nested(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'M' + [c].[CustomerID]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <> UPPER([c].[CustomerID])",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > REPLACE(N'ALFKI', N'ALF', [c].[CustomerID])",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'M' + [c].[CustomerID]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > UPPER([c].[CustomerID])",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] < REPLACE(N'ALFKI', N'ALF', [c].[CustomerID])");
        }

        public override async Task String_Compare_multi_predicate(bool isAsync)
        {
            await base.String_Compare_multi_predicate(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= N'ALFKI' AND [c].[CustomerID] < N'CACTU'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[ContactTitle] = N'Owner' AND [c].[Country] <> N'USA'");
        }

        public override async Task String_Compare_to_simple_zero(bool isAsync)
        {
            await base.String_Compare_to_simple_zero(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <> N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'");
        }

        public override async Task String_Compare_to_simple_one(bool isAsync)
        {
            await base.String_Compare_to_simple_one(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] < N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= N'ALFKI'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= N'ALFKI'");
        }

        public override async Task String_compare_to_with_parameter(bool isAsync)
        {
            await base.String_compare_to_with_parameter(isAsync);

            // issue #15994
//            AssertSql(
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] < @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= @__customer_CustomerID_0",
//                //
//                @"@__customer_CustomerID_0='ALFKI' (Size = 4000)

//SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= @__customer_CustomerID_0");
        }

        public override async Task String_Compare_to_simple_more_than_one(bool isAsync)
        {
            await base.String_Compare_to_simple_more_than_one(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]");
        }

        public override async Task String_Compare_to_nested(bool isAsync)
        {
            await base.String_Compare_to_nested(isAsync);

            //issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'M' + [c].[CustomerID]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <> UPPER([c].[CustomerID])",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > REPLACE(N'ALFKI', N'ALF', [c].[CustomerID])",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] <= N'M' + [c].[CustomerID]",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] > UPPER([c].[CustomerID])",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] < REPLACE(N'ALFKI', N'ALF', [c].[CustomerID])");
        }

        public override async Task String_Compare_to_multi_predicate(bool isAsync)
        {
            await base.String_Compare_to_multi_predicate(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] >= N'ALFKI' AND [c].[CustomerID] < N'CACTU'",
//                //
//                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
//FROM [Customers] AS [c]
//WHERE [c].[ContactTitle] = N'Owner' AND [c].[Country] <> N'USA'");
        }

        public override async Task DateTime_Compare_to_simple_zero(bool isAsync, bool compareTo)
        {
            await base.DateTime_Compare_to_simple_zero(isAsync, compareTo);

            // issue #15994
//            AssertSql(
//                @"@__myDatetime_0='1998-05-04T00:00:00'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderDate] = @__myDatetime_0",
//                //
//                @"@__myDatetime_0='1998-05-04T00:00:00'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderDate] <> @__myDatetime_0",
//                //
//                @"@__myDatetime_0='1998-05-04T00:00:00'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderDate] > @__myDatetime_0",
//                //
//                @"@__myDatetime_0='1998-05-04T00:00:00'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderDate] <= @__myDatetime_0",
//                //
//                @"@__myDatetime_0='1998-05-04T00:00:00'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderDate] > @__myDatetime_0",
//                //
//                @"@__myDatetime_0='1998-05-04T00:00:00'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderDate] <= @__myDatetime_0");
        }

        public override async Task Int_Compare_to_simple_zero(bool isAsync)
        {
            await base.Int_Compare_to_simple_zero(isAsync);

            // issue #15994
//            AssertSql(
//                @"@__orderId_0='10250'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderID] = @__orderId_0",
//                //
//                @"@__orderId_0='10250'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderID] <> @__orderId_0",
//                //
//                @"@__orderId_0='10250'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderID] > @__orderId_0",
//                //
//                @"@__orderId_0='10250'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderID] <= @__orderId_0",
//                //
//                @"@__orderId_0='10250'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderID] > @__orderId_0",
//                //
//                @"@__orderId_0='10250'

//SELECT [c].[OrderID], [c].[CustomerID], [c].[EmployeeID], [c].[OrderDate]
//FROM [Orders] AS [c]
//WHERE [c].[OrderID] <= @__orderId_0");
        }

        public override async Task Where_math_abs1(bool isAsync)
        {
            await base.Where_math_abs1(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ABS([o].[ProductID]) > 10");
        }

        public override async Task Where_math_abs2(bool isAsync)
        {
            await base.Where_math_abs2(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ABS([o].[Quantity]) > CAST(10 AS smallint)");
        }

        public override async Task Where_math_abs3(bool isAsync)
        {
            await base.Where_math_abs3(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ABS([o].[UnitPrice]) > 10.0");
        }

        public override async Task Where_math_abs_uncorrelated(bool isAsync)
        {
            await base.Where_math_abs_uncorrelated(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE 10 < [o].[ProductID]");
        }

        public override async Task Where_math_ceiling1(bool isAsync)
        {
            await base.Where_math_ceiling1(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE CEILING(CAST([o].[Discount] AS float)) > 0.0E0");
        }

        public override async Task Where_math_ceiling2(bool isAsync)
        {
            await base.Where_math_ceiling2(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE CEILING([o].[UnitPrice]) > 10.0");
        }

        public override async Task Where_math_floor(bool isAsync)
        {
            await base.Where_math_floor(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE FLOOR([o].[UnitPrice]) > 10.0");
        }

        public override async Task Where_math_power(bool isAsync)
        {
            await base.Where_math_power(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE POWER(CAST([o].[Discount] AS float), 2.0E0) > 0.05000000074505806E0");
        }

        public override async Task Where_math_round(bool isAsync)
        {
            await base.Where_math_round(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ROUND([o].[UnitPrice], 0) > 10.0");
        }

        public override async Task Select_math_round_int(bool isAsync)
        {
            await base.Select_math_round_int(isAsync);

            AssertSql(
                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0) AS [A]
FROM [Orders] AS [o]
WHERE [o].[OrderID] < 10250");
        }

        public override async Task Select_math_truncate_int(bool isAsync)
        {
            await base.Select_math_truncate_int(isAsync);

            AssertSql(
                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0, 1) AS [A]
FROM [Orders] AS [o]
WHERE [o].[OrderID] < 10250");
        }

        public override async Task Where_math_round2(bool isAsync)
        {
            await base.Where_math_round2(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ROUND([o].[UnitPrice], 2) > 100.0");
        }

        public override async Task Where_math_truncate(bool isAsync)
        {
            await base.Where_math_truncate(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ROUND([o].[UnitPrice], 0, 1) > 10.0");
        }

        public override async Task Where_math_exp(bool isAsync)
        {
            await base.Where_math_exp(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (EXP(CAST([o].[Discount] AS float)) > 1.0E0)");
        }

        public override async Task Where_math_log10(bool isAsync)
        {
            await base.Where_math_log10(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE (([o].[OrderID] = 11077) AND ([o].[Discount] > CAST(0 AS real))) AND (LOG10(CAST([o].[Discount] AS float)) < 0.0E0)");
        }

        public override async Task Where_math_log(bool isAsync)
        {
            await base.Where_math_log(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE (([o].[OrderID] = 11077) AND ([o].[Discount] > CAST(0 AS real))) AND (LOG(CAST([o].[Discount] AS float)) < 0.0E0)");
        }

        public override async Task Where_math_log_new_base(bool isAsync)
        {
            await base.Where_math_log_new_base(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE (([o].[OrderID] = 11077) AND ([o].[Discount] > CAST(0 AS real))) AND (LOG(CAST([o].[Discount] AS float), 7.0E0) < 0.0E0)");
        }

        public override async Task Where_math_sqrt(bool isAsync)
        {
            await base.Where_math_sqrt(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (SQRT(CAST([o].[Discount] AS float)) > 0.0E0)");
        }

        public override async Task Where_math_acos(bool isAsync)
        {
            await base.Where_math_acos(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (ACOS(CAST([o].[Discount] AS float)) > 1.0E0)");
        }

        public override async Task Where_math_asin(bool isAsync)
        {
            await base.Where_math_asin(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (ASIN(CAST([o].[Discount] AS float)) > 0.0E0)");
        }

        public override async Task Where_math_atan(bool isAsync)
        {
            await base.Where_math_atan(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (ATAN(CAST([o].[Discount] AS float)) > 0.0E0)");
        }

        public override async Task Where_math_atan2(bool isAsync)
        {
            await base.Where_math_atan2(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (ATN2(CAST([o].[Discount] AS float), 1.0E0) > 0.0E0)");
        }

        public override async Task Where_math_cos(bool isAsync)
        {
            await base.Where_math_cos(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (COS(CAST([o].[Discount] AS float)) > 0.0E0)");
        }

        public override async Task Where_math_sin(bool isAsync)
        {
            await base.Where_math_sin(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (SIN(CAST([o].[Discount] AS float)) > 0.0E0)");
        }

        public override async Task Where_math_tan(bool isAsync)
        {
            await base.Where_math_tan(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (TAN(CAST([o].[Discount] AS float)) > 0.0E0)");
        }

        public override async Task Where_math_sign(bool isAsync)
        {
            await base.Where_math_sign(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE ([o].[OrderID] = 11077) AND (SIGN([o].[Discount]) > 0)");
        }

        // TODO: Client Eval
        public override Task Where_math_min(bool isAsync) => null;

        // TODO: Client Eval
        public override Task Where_math_max(bool isAsync) => null;

        public override async Task Where_guid_newguid(bool isAsync)
        {
            await base.Where_guid_newguid(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[ProductID], [o].[Discount], [o].[Quantity], [o].[UnitPrice]
FROM [Order Details] AS [o]
WHERE NEWID() <> '00000000-0000-0000-0000-000000000000'");
        }

        public override async Task Where_string_to_upper(bool isAsync)
        {
            await base.Where_string_to_upper(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE UPPER([c].[CustomerID]) = N'ALFKI'");
        }

        public override async Task Where_string_to_lower(bool isAsync)
        {
            await base.Where_string_to_lower(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE LOWER([c].[CustomerID]) = N'alfki'");
        }

        public override async Task Where_functions_nested(bool isAsync)
        {
            await base.Where_functions_nested(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE POWER(CAST(CAST(LEN([c].[CustomerID]) AS int) AS float), 2.0E0) = 25.0E0");
        }

        public override async Task Convert_ToByte(bool isAsync)
        {
            await base.Convert_ToByte(isAsync);


            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(tinyint, [o].[OrderID] % 1)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(decimal(18, 2), [o].[OrderID] % 1)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(float, [o].[OrderID] % 1)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(smallint, [o].[OrderID] % 1)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(int, [o].[OrderID] % 1)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(bigint, [o].[OrderID] % 1)) >= CAST(0 AS tinyint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(tinyint, CONVERT(nvarchar(max), [o].[OrderID] % 1)) >= CAST(0 AS tinyint))");
        }

        public override async Task Convert_ToDecimal(bool isAsync)
        {
            await base.Convert_ToDecimal(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(tinyint, [o].[OrderID] % 1)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(decimal(18, 2), [o].[OrderID] % 1)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(float, [o].[OrderID] % 1)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(smallint, [o].[OrderID] % 1)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(int, [o].[OrderID] % 1)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(bigint, [o].[OrderID] % 1)) >= 0.0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(decimal(18, 2), CONVERT(nvarchar(max), [o].[OrderID] % 1)) >= 0.0)");
        }

        public override async Task Convert_ToDouble(bool isAsync)
        {
            await base.Convert_ToDouble(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(tinyint, [o].[OrderID] % 1)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(decimal(18, 2), [o].[OrderID] % 1)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(float, [o].[OrderID] % 1)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(smallint, [o].[OrderID] % 1)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(int, [o].[OrderID] % 1)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(bigint, [o].[OrderID] % 1)) >= 0.0E0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(float, CONVERT(nvarchar(max), [o].[OrderID] % 1)) >= 0.0E0)");
        }

        public override async Task Convert_ToInt16(bool isAsync)
        {
            await base.Convert_ToInt16(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(tinyint, [o].[OrderID] % 1)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(decimal(18, 2), [o].[OrderID] % 1)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(float, [o].[OrderID] % 1)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(smallint, [o].[OrderID] % 1)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(int, [o].[OrderID] % 1)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(bigint, [o].[OrderID] % 1)) >= CAST(0 AS smallint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(smallint, CONVERT(nvarchar(max), [o].[OrderID] % 1)) >= CAST(0 AS smallint))");
        }

        public override async Task Convert_ToInt32(bool isAsync)
        {
            await base.Convert_ToInt32(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(tinyint, [o].[OrderID] % 1)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(decimal(18, 2), [o].[OrderID] % 1)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(float, [o].[OrderID] % 1)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(smallint, [o].[OrderID] % 1)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(int, [o].[OrderID] % 1)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(bigint, [o].[OrderID] % 1)) >= 0)",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(int, CONVERT(nvarchar(max), [o].[OrderID] % 1)) >= 0)");
        }

        public override async Task Convert_ToInt64(bool isAsync)
        {
            await base.Convert_ToInt64(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(tinyint, [o].[OrderID] % 1)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(decimal(18, 2), [o].[OrderID] % 1)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(float, [o].[OrderID] % 1)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(smallint, [o].[OrderID] % 1)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(int, [o].[OrderID] % 1)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(bigint, [o].[OrderID] % 1)) >= CAST(0 AS bigint))",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(bigint, CONVERT(nvarchar(max), [o].[OrderID] % 1)) >= CAST(0 AS bigint))");
        }

        public override async Task Convert_ToString(bool isAsync)
        {
            await base.Convert_ToString(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(tinyint, [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(decimal(18, 2), [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(float, [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CAST(CONVERT(float, [o].[OrderID] % 1) AS real)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(smallint, [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(int, [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(bigint, [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND (CONVERT(nvarchar(max), CONVERT(nvarchar(max), [o].[OrderID] % 1)) <> N'10')",
                //
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[CustomerID] = N'ALFKI') AND [o].[CustomerID] IS NOT NULL) AND ((CHARINDEX(N'1997', CONVERT(nvarchar(max), [o].[OrderDate])) > 0) OR (CHARINDEX(N'1998', CONVERT(nvarchar(max), [o].[OrderDate])) > 0))");

        }

        public override async Task Indexof_with_emptystring(bool isAsync)
        {
            await base.Indexof_with_emptystring(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN N'' = N'' THEN 0
    ELSE CHARINDEX(N'', [c].[ContactName]) - 1
END
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task Replace_with_emptystring(bool isAsync)
        {
            await base.Replace_with_emptystring(isAsync);

            AssertSql(
                @"SELECT REPLACE([c].[ContactName], N'ari', N'')
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task Substring_with_zero_startindex(bool isAsync)
        {
            await base.Substring_with_zero_startindex(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT SUBSTRING([c].[ContactName], 1, 3)
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task Substring_with_zero_length(bool isAsync)
        {
            await base.Substring_with_zero_length(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT SUBSTRING([c].[ContactName], 3, 0)
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task Substring_with_constant(bool isAsync)
        {
            await base.Substring_with_constant(isAsync);

            // issue #15994
//            AssertSql(
//                @"SELECT SUBSTRING([c].[ContactName], 2, 3)
//FROM [Customers] AS [c]
//WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task Substring_with_closure(bool isAsync)
        {
            await base.Substring_with_closure(isAsync);

            AssertSql(
                @"@__start_0='2'

SELECT SUBSTRING([c].[ContactName], @__start_0 + 1, 3)
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task Substring_with_Index_of(bool isAsync)
        {
            await base.Substring_with_Index_of(isAsync);

            AssertSql(
                @"SELECT SUBSTRING([c].[ContactName], CASE
    WHEN N'a' = N'' THEN 0
    ELSE CHARINDEX(N'a', [c].[ContactName]) - 1
END + 1, 3)
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = N'ALFKI'");
        }

        public override async Task IsNullOrEmpty_in_predicate(bool isAsync)
        {
            await base.IsNullOrEmpty_in_predicate(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[Region] IS NULL OR (([c].[Region] = N'') AND [c].[Region] IS NOT NULL)");
        }

        public override void IsNullOrEmpty_in_projection()
        {
            base.IsNullOrEmpty_in_projection();

            AssertSql(
                @"SELECT [c].[CustomerID] AS [Id], CASE
    WHEN [c].[Region] IS NULL OR (([c].[Region] = N'') AND [c].[Region] IS NOT NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Value]
FROM [Customers] AS [c]");
        }

        public override void IsNullOrEmpty_negated_in_projection()
        {
            base.IsNullOrEmpty_negated_in_projection();

            AssertSql(
                @"SELECT [c].[CustomerID] AS [Id], CASE
    WHEN [c].[Region] IS NOT NULL AND (([c].[Region] <> N'') OR [c].[Region] IS NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Value]
FROM [Customers] AS [c]");
        }

        public override async Task IsNullOrWhiteSpace_in_predicate(bool isAsync)
        {
            await base.IsNullOrWhiteSpace_in_predicate(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[Region] IS NULL OR ((LTRIM(RTRIM([c].[Region])) = N'') AND LTRIM(RTRIM([c].[Region])) IS NOT NULL)");
        }

        public override async Task TrimStart_without_arguments_in_predicate(bool isAsync)
        {
            await base.TrimStart_without_arguments_in_predicate(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (LTRIM([c].[ContactTitle]) = N'Owner') AND LTRIM([c].[ContactTitle]) IS NOT NULL");
        }

        // TODO: Client Eval
        public override Task TrimStart_with_char_argument_in_predicate(bool isAsync) => null;

        // TODO: Client Eval
        public override Task TrimStart_with_char_array_argument_in_predicate(bool isAsync) => null;

        public override async Task TrimEnd_without_arguments_in_predicate(bool isAsync)
        {
            await base.TrimEnd_without_arguments_in_predicate(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (RTRIM([c].[ContactTitle]) = N'Owner') AND RTRIM([c].[ContactTitle]) IS NOT NULL");
        }

        // TODO: Client Eval
        public override Task TrimEnd_with_char_argument_in_predicate(bool isAsync) => null;

        // TODO: Client Eval
        public override Task TrimEnd_with_char_array_argument_in_predicate(bool isAsync) => null;

        public override async Task Trim_without_argument_in_predicate(bool isAsync)
        {
            await base.Trim_without_argument_in_predicate(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE (LTRIM(RTRIM([c].[ContactTitle])) = N'Owner') AND LTRIM(RTRIM([c].[ContactTitle])) IS NOT NULL");
        }

        // TODO: Client Eval
        public override Task Trim_with_char_argument_in_predicate(bool isAsync) => null;

        // TODO: Client Eval
        public override Task Trim_with_char_array_argument_in_predicate(bool isAsync) => null;

        public override async Task Order_by_length_twice(bool isAsync)
        {
            await base.Order_by_length_twice(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
ORDER BY CAST(LEN([c].[CustomerID]) AS int), [c].[CustomerID]");
        }

        public override async Task Order_by_length_twice_followed_by_projection_of_naked_collection_navigation(bool isAsync)
        {
            await base.Order_by_length_twice_followed_by_projection_of_naked_collection_navigation(isAsync);

            AssertSql(
                "");
        }

        public override async Task Static_string_equals_in_predicate(bool isAsync)
        {
            await base.Static_string_equals_in_predicate(isAsync);

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region]
FROM [Customers] AS [c]
WHERE [c].[CustomerID] = N'ANATR'");
        }

        public override async Task Static_equals_nullable_datetime_compared_to_non_nullable(bool isAsync)
        {
            await base.Static_equals_nullable_datetime_compared_to_non_nullable(isAsync);

            AssertSql(
                @"@__arg_0='1996-07-04T00:00:00'

SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE (([o].[OrderDate] = @__arg_0) AND ([o].[OrderDate] IS NOT NULL AND @__arg_0 IS NOT NULL)) OR ([o].[OrderDate] IS NULL AND @__arg_0 IS NULL)");
        }

        public override async Task Static_equals_int_compared_to_long(bool isAsync)
        {
            await base.Static_equals_int_compared_to_long(isAsync);

            AssertSql(
                @"SELECT [o].[OrderID], [o].[CustomerID], [o].[EmployeeID], [o].[OrderDate]
FROM [Orders] AS [o]
WHERE CAST(0 AS bit) = CAST(1 AS bit)");
        }

        public override async Task Projecting_Math_Truncate_and_ordering_by_it_twice(bool isAsync)
        {
            await base.Projecting_Math_Truncate_and_ordering_by_it_twice(isAsync);

            // issue #16038
//            AssertSql(
//                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0, 1) AS [A]
//FROM [Orders] AS [o]
//WHERE [o].[OrderID] < 10250
//ORDER BY [A]");
        }

        public override async Task Projecting_Math_Truncate_and_ordering_by_it_twice2(bool isAsync)
        {
            await base.Projecting_Math_Truncate_and_ordering_by_it_twice2(isAsync);

            // issue #16038
//            AssertSql(
//                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0, 1) AS [A]
//FROM [Orders] AS [o]
//WHERE [o].[OrderID] < 10250
//ORDER BY [A] DESC");
        }

        public override async Task Projecting_Math_Truncate_and_ordering_by_it_twice3(bool isAsync)
        {
            await base.Projecting_Math_Truncate_and_ordering_by_it_twice3(isAsync);

            // issue #16038
//            AssertSql(
//                @"SELECT ROUND(CAST([o].[OrderID] AS float), 0, 1) AS [A]
//FROM [Orders] AS [o]
//WHERE [o].[OrderID] < 10250
//ORDER BY [A] DESC");
        }
    }
}
