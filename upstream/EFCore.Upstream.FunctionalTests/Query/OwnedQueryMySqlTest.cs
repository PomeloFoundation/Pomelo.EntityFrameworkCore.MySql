// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class OwnedQueryMySqlTest : RelationalOwnedQueryTestBase<OwnedQueryMySqlTest.OwnedQueryMySqlFixture>
    {
        public OwnedQueryMySqlTest(OwnedQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void Query_with_owned_entity_equality_operator()
        {
            base.Query_with_owned_entity_equality_operator();

            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafAAddress_Country_Name], [t13].[LeafAAddress_Country_PlanetId], [t14].[Id], [o16].[ClientId], [o16].[Id]
FROM [OwnedPerson] AS [o]
CROSS JOIN (
    SELECT [o15].[Id], [o15].[Discriminator]
    FROM [OwnedPerson] AS [o15]
    WHERE [o15].[Discriminator] = N'LeafB'
) AS [t14]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafA'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafAAddress_Country_Name], [o12].[LeafAAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafA'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN [Order] AS [o16] ON [o].[Id] = [o16].[ClientId]
WHERE CAST(0 AS bit) = CAST(1 AS bit)
ORDER BY [o].[Id], [t14].[Id], [o16].[ClientId], [o16].[Id]");
        }

        public override void Query_for_base_type_loads_all_owned_navs()
        {
            base.Query_for_base_type_loads_all_owned_navs();

            // See issue #10067
            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafBAddress_Country_Name], [t13].[LeafBAddress_Country_PlanetId], [t15].[Id], [t18].[Id], [t18].[LeafAAddress_Country_Name], [t18].[LeafAAddress_Country_PlanetId], [o20].[ClientId], [o20].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafB'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafBAddress_Country_Name], [o12].[LeafBAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafB'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafBAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN (
    SELECT [o15].[Id], [t14].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o15]
    INNER JOIN (
        SELECT [o16].[Id], [o16].[Discriminator]
        FROM [OwnedPerson] AS [o16]
        WHERE [o16].[Discriminator] = N'LeafA'
    ) AS [t14] ON [o15].[Id] = [t14].[Id]
) AS [t15] ON [o].[Id] = [t15].[Id]
LEFT JOIN (
    SELECT [o17].[Id], [o17].[LeafAAddress_Country_Name], [o17].[LeafAAddress_Country_PlanetId], [t17].[Id] AS [Id0], [t17].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o17]
    INNER JOIN (
        SELECT [o18].[Id], [t16].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o18]
        INNER JOIN (
            SELECT [o19].[Id], [o19].[Discriminator]
            FROM [OwnedPerson] AS [o19]
            WHERE [o19].[Discriminator] = N'LeafA'
        ) AS [t16] ON [o18].[Id] = [t16].[Id]
    ) AS [t17] ON [o17].[Id] = [t17].[Id]
    WHERE [o17].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t18] ON [t15].[Id] = [t18].[Id]
LEFT JOIN [Order] AS [o20] ON [o].[Id] = [o20].[ClientId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
ORDER BY [o].[Id], [o20].[ClientId], [o20].[Id]");
        }

        public override void No_ignored_include_warning_when_implicit_load()
        {
            base.No_ignored_include_warning_when_implicit_load();

            AssertSql(
                @"SELECT COUNT(*)
FROM [OwnedPerson] AS [o]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Query_for_branch_type_loads_all_owned_navs()
        {
            base.Query_for_branch_type_loads_all_owned_navs();

            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafAAddress_Country_Name], [t13].[LeafAAddress_Country_PlanetId], [o15].[ClientId], [o15].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafA'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafAAddress_Country_Name], [o12].[LeafAAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafA'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN [Order] AS [o15] ON [o].[Id] = [o15].[ClientId]
WHERE [o].[Discriminator] IN (N'Branch', N'LeafA')
ORDER BY [o].[Id], [o15].[ClientId], [o15].[Id]");
        }

        public override void Query_for_leaf_type_loads_all_owned_navs()
        {
            base.Query_for_leaf_type_loads_all_owned_navs();

            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafAAddress_Country_Name], [t13].[LeafAAddress_Country_PlanetId], [o15].[ClientId], [o15].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafA'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafAAddress_Country_Name], [o12].[LeafAAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafA'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN [Order] AS [o15] ON [o].[Id] = [o15].[ClientId]
WHERE [o].[Discriminator] = N'LeafA'
ORDER BY [o].[Id], [o15].[ClientId], [o15].[Id]");
        }

        public override void Query_when_group_by()
        {
            base.Query_when_group_by();

            AssertSql(
                @"SELECT [op].[Id], [op].[Discriminator], [t].[Id], [t0].[Id], [t0].[LeafBAddress_Country_Name], [t0].[LeafBAddress_Country_PlanetId], [t1].[Id], [t2].[Id], [t2].[LeafAAddress_Country_Name], [t2].[LeafAAddress_Country_PlanetId], [t3].[Id], [t4].[Id], [t4].[BranchAddress_Country_Name], [t4].[BranchAddress_Country_PlanetId], [t5].[Id], [t6].[Id], [t6].[PersonAddress_Country_Name], [t6].[PersonAddress_Country_PlanetId]
FROM [OwnedPerson] AS [op]
LEFT JOIN (
    SELECT [op.LeafBAddress].*
    FROM [OwnedPerson] AS [op.LeafBAddress]
    WHERE [op.LeafBAddress].[Discriminator] = N'LeafB'
) AS [t] ON [op].[Id] = [t].[Id]
LEFT JOIN (
    SELECT [op.LeafBAddress.Country].*
    FROM [OwnedPerson] AS [op.LeafBAddress.Country]
    WHERE [op.LeafBAddress.Country].[Discriminator] = N'LeafB'
) AS [t0] ON [t].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [op.LeafAAddress].*
    FROM [OwnedPerson] AS [op.LeafAAddress]
    WHERE [op.LeafAAddress].[Discriminator] = N'LeafA'
) AS [t1] ON [op].[Id] = [t1].[Id]
LEFT JOIN (
    SELECT [op.LeafAAddress.Country].*
    FROM [OwnedPerson] AS [op.LeafAAddress.Country]
    WHERE [op.LeafAAddress.Country].[Discriminator] = N'LeafA'
) AS [t2] ON [t1].[Id] = [t2].[Id]
LEFT JOIN (
    SELECT [op.BranchAddress].*
    FROM [OwnedPerson] AS [op.BranchAddress]
    WHERE [op.BranchAddress].[Discriminator] IN (N'LeafA', N'Branch')
) AS [t3] ON [op].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [op.BranchAddress.Country].*
    FROM [OwnedPerson] AS [op.BranchAddress.Country]
    WHERE [op.BranchAddress.Country].[Discriminator] IN (N'LeafA', N'Branch')
) AS [t4] ON [t3].[Id] = [t4].[Id]
LEFT JOIN (
    SELECT [op.PersonAddress].*
    FROM [OwnedPerson] AS [op.PersonAddress]
    WHERE [op.PersonAddress].[Discriminator] IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
) AS [t5] ON [op].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [op.PersonAddress.Country].*
    FROM [OwnedPerson] AS [op.PersonAddress.Country]
    WHERE [op.PersonAddress.Country].[Discriminator] IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
) AS [t6] ON [t5].[Id] = [t6].[Id]
WHERE [op].[Discriminator] IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
ORDER BY [op].[Id]",
                //
                @"SELECT [op.Orders].[Id], [op.Orders].[ClientId]
FROM [Order] AS [op.Orders]
INNER JOIN (
    SELECT DISTINCT [op0].[Id]
    FROM [OwnedPerson] AS [op0]
    LEFT JOIN (
        SELECT [op.LeafBAddress0].*
        FROM [OwnedPerson] AS [op.LeafBAddress0]
        WHERE [op.LeafBAddress0].[Discriminator] = N'LeafB'
    ) AS [t7] ON [op0].[Id] = [t7].[Id]
    LEFT JOIN (
        SELECT [op.LeafBAddress.Country0].*
        FROM [OwnedPerson] AS [op.LeafBAddress.Country0]
        WHERE [op.LeafBAddress.Country0].[Discriminator] = N'LeafB'
    ) AS [t8] ON [t7].[Id] = [t8].[Id]
    LEFT JOIN (
        SELECT [op.LeafAAddress0].*
        FROM [OwnedPerson] AS [op.LeafAAddress0]
        WHERE [op.LeafAAddress0].[Discriminator] = N'LeafA'
    ) AS [t9] ON [op0].[Id] = [t9].[Id]
    LEFT JOIN (
        SELECT [op.LeafAAddress.Country0].*
        FROM [OwnedPerson] AS [op.LeafAAddress.Country0]
        WHERE [op.LeafAAddress.Country0].[Discriminator] = N'LeafA'
    ) AS [t10] ON [t9].[Id] = [t10].[Id]
    LEFT JOIN (
        SELECT [op.BranchAddress0].*
        FROM [OwnedPerson] AS [op.BranchAddress0]
        WHERE [op.BranchAddress0].[Discriminator] IN (N'LeafA', N'Branch')
    ) AS [t11] ON [op0].[Id] = [t11].[Id]
    LEFT JOIN (
        SELECT [op.BranchAddress.Country0].*
        FROM [OwnedPerson] AS [op.BranchAddress.Country0]
        WHERE [op.BranchAddress.Country0].[Discriminator] IN (N'LeafA', N'Branch')
    ) AS [t12] ON [t11].[Id] = [t12].[Id]
    LEFT JOIN (
        SELECT [op.PersonAddress0].*
        FROM [OwnedPerson] AS [op.PersonAddress0]
        WHERE [op.PersonAddress0].[Discriminator] IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
    ) AS [t13] ON [op0].[Id] = [t13].[Id]
    LEFT JOIN (
        SELECT [op.PersonAddress.Country0].*
        FROM [OwnedPerson] AS [op.PersonAddress.Country0]
        WHERE [op.PersonAddress.Country0].[Discriminator] IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
    ) AS [t14] ON [t13].[Id] = [t14].[Id]
    WHERE [op0].[Discriminator] IN (N'LeafB', N'LeafA', N'Branch', N'OwnedPerson')
) AS [t15] ON [op.Orders].[ClientId] = [t15].[Id]
ORDER BY [t15].[Id]");
        }

        public override void Query_when_subquery()
        {
            base.Query_when_subquery();

            AssertSql(
                @"@__p_0='5'

SELECT [t0].[Id], [t0].[Discriminator], [t2].[Id], [t5].[Id], [t5].[PersonAddress_Country_Name], [t5].[PersonAddress_Country_PlanetId], [t7].[Id], [t10].[Id], [t10].[BranchAddress_Country_Name], [t10].[BranchAddress_Country_PlanetId], [t12].[Id], [t15].[Id], [t15].[LeafBAddress_Country_Name], [t15].[LeafBAddress_Country_PlanetId], [t17].[Id], [t20].[Id], [t20].[LeafAAddress_Country_Name], [t20].[LeafAAddress_Country_PlanetId], [o20].[ClientId], [o20].[Id]
FROM (
    SELECT TOP(@__p_0) [t].[Id], [t].[Discriminator]
    FROM (
        SELECT DISTINCT [o].[Id], [o].[Discriminator]
        FROM [OwnedPerson] AS [o]
        WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t]
    ORDER BY [t].[Id]
) AS [t0]
LEFT JOIN (
    SELECT [o21].[Id], [t21].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o21]
    INNER JOIN (
        SELECT [o22].[Id], [o22].[Discriminator]
        FROM [OwnedPerson] AS [o22]
        WHERE [o22].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t21] ON [o21].[Id] = [t21].[Id]
) AS [t22] ON [t0].[Id] = [t22].[Id]
LEFT JOIN (
    SELECT [o0].[Id], [t1].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t1] ON [o0].[Id] = [t1].[Id]
) AS [t2] ON [t0].[Id] = [t2].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t4].[Id] AS [Id0], [t4].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t3].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t3] ON [o3].[Id] = [t3].[Id]
    ) AS [t4] ON [o2].[Id] = [t4].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t5] ON [t2].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t6].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t6] ON [o5].[Id] = [t6].[Id]
) AS [t7] ON [t0].[Id] = [t7].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t9].[Id] AS [Id0], [t9].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t8].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t8] ON [o8].[Id] = [t8].[Id]
    ) AS [t9] ON [o7].[Id] = [t9].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t10] ON [t7].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t11].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafB'
    ) AS [t11] ON [o10].[Id] = [t11].[Id]
) AS [t12] ON [t0].[Id] = [t12].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafBAddress_Country_Name], [o12].[LeafBAddress_Country_PlanetId], [t14].[Id] AS [Id0], [t14].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t13].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafB'
        ) AS [t13] ON [o13].[Id] = [t13].[Id]
    ) AS [t14] ON [o12].[Id] = [t14].[Id]
    WHERE [o12].[LeafBAddress_Country_PlanetId] IS NOT NULL
) AS [t15] ON [t12].[Id] = [t15].[Id]
LEFT JOIN (
    SELECT [o15].[Id], [t16].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o15]
    INNER JOIN (
        SELECT [o16].[Id], [o16].[Discriminator]
        FROM [OwnedPerson] AS [o16]
        WHERE [o16].[Discriminator] = N'LeafA'
    ) AS [t16] ON [o15].[Id] = [t16].[Id]
) AS [t17] ON [t0].[Id] = [t17].[Id]
LEFT JOIN (
    SELECT [o17].[Id], [o17].[LeafAAddress_Country_Name], [o17].[LeafAAddress_Country_PlanetId], [t19].[Id] AS [Id0], [t19].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o17]
    INNER JOIN (
        SELECT [o18].[Id], [t18].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o18]
        INNER JOIN (
            SELECT [o19].[Id], [o19].[Discriminator]
            FROM [OwnedPerson] AS [o19]
            WHERE [o19].[Discriminator] = N'LeafA'
        ) AS [t18] ON [o18].[Id] = [t18].[Id]
    ) AS [t19] ON [o17].[Id] = [t19].[Id]
    WHERE [o17].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t20] ON [t17].[Id] = [t20].[Id]
LEFT JOIN [Order] AS [o20] ON [t0].[Id] = [o20].[ClientId]
ORDER BY [t0].[Id], [o20].[ClientId], [o20].[Id]");
        }

        public override void Navigation_rewrite_on_owned_reference_projecting_scalar()
        {
            base.Navigation_rewrite_on_owned_reference_projecting_scalar();

            AssertSql(
                @"SELECT [t1].[PersonAddress_Country_Name]
FROM [OwnedPerson] AS [o2]
LEFT JOIN (
    SELECT [o3].[Id], [t2].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o3]
    INNER JOIN (
        SELECT [o4].[Id], [o4].[Discriminator]
        FROM [OwnedPerson] AS [o4]
        WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t2] ON [o3].[Id] = [t2].[Id]
) AS [t3] ON [o2].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o].[Id], [o].[PersonAddress_Country_Name], [o].[PersonAddress_Country_PlanetId], [t0].[Id] AS [Id0], [t0].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o]
    INNER JOIN (
        SELECT [o0].[Id], [t].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o0]
        INNER JOIN (
            SELECT [o1].[Id], [o1].[Discriminator]
            FROM [OwnedPerson] AS [o1]
            WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t] ON [o0].[Id] = [t].[Id]
    ) AS [t0] ON [o].[Id] = [t0].[Id]
    WHERE [o].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t1] ON [t3].[Id] = [t1].[Id]
WHERE [o2].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (([t1].[PersonAddress_Country_Name] = N'USA') AND [t1].[PersonAddress_Country_Name] IS NOT NULL)");
        }

        public override void Navigation_rewrite_on_owned_reference_projecting_entity()
        {
            base.Navigation_rewrite_on_owned_reference_projecting_entity();

            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafBAddress_Country_Name], [t13].[LeafBAddress_Country_PlanetId], [t15].[Id], [t18].[Id], [t18].[LeafAAddress_Country_Name], [t18].[LeafAAddress_Country_PlanetId], [o20].[ClientId], [o20].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafB'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafBAddress_Country_Name], [o12].[LeafBAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafB'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafBAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN (
    SELECT [o15].[Id], [t14].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o15]
    INNER JOIN (
        SELECT [o16].[Id], [o16].[Discriminator]
        FROM [OwnedPerson] AS [o16]
        WHERE [o16].[Discriminator] = N'LeafA'
    ) AS [t14] ON [o15].[Id] = [t14].[Id]
) AS [t15] ON [o].[Id] = [t15].[Id]
LEFT JOIN (
    SELECT [o17].[Id], [o17].[LeafAAddress_Country_Name], [o17].[LeafAAddress_Country_PlanetId], [t17].[Id] AS [Id0], [t17].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o17]
    INNER JOIN (
        SELECT [o18].[Id], [t16].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o18]
        INNER JOIN (
            SELECT [o19].[Id], [o19].[Discriminator]
            FROM [OwnedPerson] AS [o19]
            WHERE [o19].[Discriminator] = N'LeafA'
        ) AS [t16] ON [o18].[Id] = [t16].[Id]
    ) AS [t17] ON [o17].[Id] = [t17].[Id]
    WHERE [o17].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t18] ON [t15].[Id] = [t18].[Id]
LEFT JOIN [Order] AS [o20] ON [o].[Id] = [o20].[ClientId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (([t3].[PersonAddress_Country_Name] = N'USA') AND [t3].[PersonAddress_Country_Name] IS NOT NULL)
ORDER BY [o].[Id], [o20].[ClientId], [o20].[Id]");
        }

        public override void Navigation_rewrite_on_owned_collection()
        {
            base.Navigation_rewrite_on_owned_collection();

            AssertSql(
                @"SELECT [o].[Id], [o0].[ClientId], [o0].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN [Order] AS [o0] ON [o].[Id] = [o0].[ClientId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND ((
    SELECT COUNT(*)
    FROM [Order] AS [o1]
    WHERE [o].[Id] = [o1].[ClientId]) > 0)
ORDER BY [o].[Id], [o0].[ClientId], [o0].[Id]");
        }

        public override void Navigation_rewrite_on_owned_collection_with_composition()
        {
            base.Navigation_rewrite_on_owned_collection_with_composition();

            AssertSql(
                @"SELECT (
    SELECT TOP(1) CASE
        WHEN [o].[Id] <> 42 THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END
    FROM [Order] AS [o]
    WHERE [o0].[Id] = [o].[ClientId])
FROM [OwnedPerson] AS [o0]
WHERE [o0].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Navigation_rewrite_on_owned_collection_with_composition_complex()
        {
            base.Navigation_rewrite_on_owned_collection_with_composition_complex();

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [t1].[PersonAddress_Country_Name]
    FROM [Order] AS [o2]
    LEFT JOIN (
        SELECT [o3].[Id], [o3].[Discriminator]
        FROM [OwnedPerson] AS [o3]
        WHERE [o3].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t2] ON [o2].[ClientId] = [t2].[Id]
    LEFT JOIN (
        SELECT [o4].[Id], [t3].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o4]
        INNER JOIN (
            SELECT [o5].[Id], [o5].[Discriminator]
            FROM [OwnedPerson] AS [o5]
            WHERE [o5].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t3] ON [o4].[Id] = [t3].[Id]
    ) AS [t4] ON [t2].[Id] = [t4].[Id]
    LEFT JOIN (
        SELECT [o].[Id], [o].[PersonAddress_Country_Name], [o].[PersonAddress_Country_PlanetId], [t0].[Id] AS [Id0], [t0].[Id0] AS [Id00]
        FROM [OwnedPerson] AS [o]
        INNER JOIN (
            SELECT [o0].[Id], [t].[Id] AS [Id0]
            FROM [OwnedPerson] AS [o0]
            INNER JOIN (
                SELECT [o1].[Id], [o1].[Discriminator]
                FROM [OwnedPerson] AS [o1]
                WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ) AS [t] ON [o0].[Id] = [t].[Id]
        ) AS [t0] ON [o].[Id] = [t0].[Id]
        WHERE [o].[PersonAddress_Country_PlanetId] IS NOT NULL
    ) AS [t1] ON [t4].[Id] = [t1].[Id]
    WHERE [o6].[Id] = [o2].[ClientId])
FROM [OwnedPerson] AS [o6]
WHERE [o6].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void SelectMany_on_owned_collection()
        {
            base.SelectMany_on_owned_collection();

            AssertSql(
                @"SELECT [o].[ClientId], [o].[Id]
FROM [OwnedPerson] AS [o0]
INNER JOIN [Order] AS [o] ON [o0].[Id] = [o].[ClientId]
WHERE [o0].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Navigation_rewrite_on_owned_reference_followed_by_regular_entity()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity();

            AssertSql(
               @"SELECT [p].[Id], [p].[StarId]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Filter_owned_entity_chained_with_regular_entity_followed_by_projecting_owned_collection()
        {
            base.Filter_owned_entity_chained_with_regular_entity_followed_by_projecting_owned_collection();

            AssertSql(
                @"SELECT [o].[Id], [o0].[ClientId], [o0].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o1].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o1]
    INNER JOIN (
        SELECT [o2].[Id], [o2].[Discriminator]
        FROM [OwnedPerson] AS [o2]
        WHERE [o2].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o1].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o3].[Id], [o3].[PersonAddress_Country_Name], [o3].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o3]
    INNER JOIN (
        SELECT [o4].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o4]
        INNER JOIN (
            SELECT [o5].[Id], [o5].[Discriminator]
            FROM [OwnedPerson] AS [o5]
            WHERE [o5].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o4].[Id] = [t1].[Id]
    ) AS [t2] ON [o3].[Id] = [t2].[Id]
    WHERE [o3].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Order] AS [o0] ON [o].[Id] = [o0].[ClientId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (([p].[Id] <> 42) OR [p].[Id] IS NULL)
ORDER BY [o].[Id], [o0].[ClientId], [o0].[Id]");
        }

        public override void Project_multiple_owned_navigations()
        {
            base.Project_multiple_owned_navigations();

            AssertSql(
                @"SELECT [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [p].[Id], [p].[StarId], [o4].[Id], [o5].[ClientId], [o5].[Id]
FROM [OwnedPerson] AS [o4]
LEFT JOIN (
    SELECT [o].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o]
    INNER JOIN (
        SELECT [o0].[Id], [o0].[Discriminator]
        FROM [OwnedPerson] AS [o0]
        WHERE [o0].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o].[Id] = [t].[Id]
) AS [t0] ON [o4].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o1].[Id], [o1].[PersonAddress_Country_Name], [o1].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o1]
    INNER JOIN (
        SELECT [o2].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o2]
        INNER JOIN (
            SELECT [o3].[Id], [o3].[Discriminator]
            FROM [OwnedPerson] AS [o3]
            WHERE [o3].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o2].[Id] = [t1].[Id]
    ) AS [t2] ON [o1].[Id] = [t2].[Id]
    WHERE [o1].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Order] AS [o5] ON [o4].[Id] = [o5].[ClientId]
WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
ORDER BY [o4].[Id], [o5].[ClientId], [o5].[Id]");
        }

        public override void Project_multiple_owned_navigations_with_expansion_on_owned_collections()
        {
            base.Project_multiple_owned_navigations_with_expansion_on_owned_collections();

            AssertSql(
                @"SELECT (
    SELECT COUNT(*)
    FROM [Order] AS [o]
    LEFT JOIN (
        SELECT [o0].[Id], [o0].[Discriminator]
        FROM [OwnedPerson] AS [o0]
        WHERE [o0].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o].[ClientId] = [t].[Id]
    LEFT JOIN (
        SELECT [o1].[Id], [t0].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o1]
        INNER JOIN (
            SELECT [o2].[Id], [o2].[Discriminator]
            FROM [OwnedPerson] AS [o2]
            WHERE [o2].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t0] ON [o1].[Id] = [t0].[Id]
    ) AS [t1] ON [t].[Id] = [t1].[Id]
    LEFT JOIN (
        SELECT [o3].[Id], [o3].[PersonAddress_Country_Name], [o3].[PersonAddress_Country_PlanetId], [t3].[Id] AS [Id0], [t3].[Id0] AS [Id00]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [t2].[Id] AS [Id0]
            FROM [OwnedPerson] AS [o4]
            INNER JOIN (
                SELECT [o5].[Id], [o5].[Discriminator]
                FROM [OwnedPerson] AS [o5]
                WHERE [o5].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
            ) AS [t2] ON [o4].[Id] = [t2].[Id]
        ) AS [t3] ON [o3].[Id] = [t3].[Id]
        WHERE [o3].[PersonAddress_Country_PlanetId] IS NOT NULL
    ) AS [t4] ON [t1].[Id] = [t4].[Id]
    LEFT JOIN [Planet] AS [p] ON [t4].[PersonAddress_Country_PlanetId] = [p].[Id]
    LEFT JOIN [Star] AS [s] ON [p].[StarId] = [s].[Id]
    WHERE ([o6].[Id] = [o].[ClientId]) AND (([s].[Id] <> 42) OR [s].[Id] IS NULL)) AS [Count], [p0].[Id], [p0].[StarId]
FROM [OwnedPerson] AS [o6]
LEFT JOIN (
    SELECT [o7].[Id], [t5].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [o8].[Discriminator]
        FROM [OwnedPerson] AS [o8]
        WHERE [o8].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t5] ON [o7].[Id] = [t5].[Id]
) AS [t6] ON [o6].[Id] = [t6].[Id]
LEFT JOIN (
    SELECT [o9].[Id], [o9].[PersonAddress_Country_Name], [o9].[PersonAddress_Country_PlanetId], [t8].[Id] AS [Id0], [t8].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o9]
    INNER JOIN (
        SELECT [o10].[Id], [t7].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o10]
        INNER JOIN (
            SELECT [o11].[Id], [o11].[Discriminator]
            FROM [OwnedPerson] AS [o11]
            WHERE [o11].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t7] ON [o10].[Id] = [t7].[Id]
    ) AS [t8] ON [o9].[Id] = [t8].[Id]
    WHERE [o9].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t9] ON [t6].[Id] = [t9].[Id]
LEFT JOIN [Planet] AS [p0] ON [t9].[PersonAddress_Country_PlanetId] = [p0].[Id]
WHERE [o6].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Navigation_rewrite_on_owned_reference_followed_by_regular_entity_filter()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_filter();

            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafBAddress_Country_Name], [t13].[LeafBAddress_Country_PlanetId], [t15].[Id], [t18].[Id], [t18].[LeafAAddress_Country_Name], [t18].[LeafAAddress_Country_PlanetId], [o20].[ClientId], [o20].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafB'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafBAddress_Country_Name], [o12].[LeafBAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafB'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafBAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN (
    SELECT [o15].[Id], [t14].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o15]
    INNER JOIN (
        SELECT [o16].[Id], [o16].[Discriminator]
        FROM [OwnedPerson] AS [o16]
        WHERE [o16].[Discriminator] = N'LeafA'
    ) AS [t14] ON [o15].[Id] = [t14].[Id]
) AS [t15] ON [o].[Id] = [t15].[Id]
LEFT JOIN (
    SELECT [o17].[Id], [o17].[LeafAAddress_Country_Name], [o17].[LeafAAddress_Country_PlanetId], [t17].[Id] AS [Id0], [t17].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o17]
    INNER JOIN (
        SELECT [o18].[Id], [t16].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o18]
        INNER JOIN (
            SELECT [o19].[Id], [o19].[Discriminator]
            FROM [OwnedPerson] AS [o19]
            WHERE [o19].[Discriminator] = N'LeafA'
        ) AS [t16] ON [o18].[Id] = [t16].[Id]
    ) AS [t17] ON [o17].[Id] = [t17].[Id]
    WHERE [o17].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t18] ON [t15].[Id] = [t18].[Id]
LEFT JOIN [Order] AS [o20] ON [o].[Id] = [o20].[ClientId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (([p].[Id] <> 7) OR [p].[Id] IS NULL)
ORDER BY [o].[Id], [o20].[ClientId], [o20].[Id]");
        }

        public override void Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_property()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_property();

            AssertSql(
                @"SELECT [p].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_collection();

            AssertSql(
                @"SELECT [o].[Id], [m].[Id], [m].[Diameter], [m].[PlanetId]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Moon] AS [m] ON [p].[Id] = [m].[PlanetId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
ORDER BY [o].[Id], [m].[Id]");
        }

        public override void SelectMany_on_owned_reference_followed_by_regular_entity_and_collection()
        {
            base.SelectMany_on_owned_reference_followed_by_regular_entity_and_collection();

            AssertSql(
                @"SELECT [m].[Id], [m].[Diameter], [m].[PlanetId]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
INNER JOIN [Moon] AS [m] ON [p].[Id] = [m].[PlanetId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void SelectMany_on_owned_reference_with_entity_in_between_ending_in_owned_collection()
        {
            base.SelectMany_on_owned_reference_with_entity_in_between_ending_in_owned_collection();

            AssertSql(
                @"SELECT [e].[Id], [e].[Name], [e].[StarId]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Star] AS [s] ON [p].[StarId] = [s].[Id]
INNER JOIN [Element] AS [e] ON [s].[Id] = [e].[StarId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference();

            AssertSql(
                @"SELECT [s].[Id], [s].[Name], [o].[Id], [e].[Id], [e].[Name], [e].[StarId]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Star] AS [s] ON [p].[StarId] = [s].[Id]
LEFT JOIN [Element] AS [e] ON [s].[Id] = [e].[StarId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
ORDER BY [o].[Id], [e].[Id]");
        }

        public override void Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_and_scalar()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_and_scalar();

            AssertSql(
                @"SELECT [s].[Name]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Star] AS [s] ON [p].[StarId] = [s].[Id]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')");
        }

        public override void
            Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_in_predicate_and_projection()
        {
            base.Navigation_rewrite_on_owned_reference_followed_by_regular_entity_and_another_reference_in_predicate_and_projection();

            AssertSql(
                @"SELECT [s].[Id], [s].[Name], [o].[Id], [e].[Id], [e].[Name], [e].[StarId]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN [Planet] AS [p] ON [t3].[PersonAddress_Country_PlanetId] = [p].[Id]
LEFT JOIN [Star] AS [s] ON [p].[StarId] = [s].[Id]
LEFT JOIN [Element] AS [e] ON [s].[Id] = [e].[StarId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND (([s].[Name] = N'Sol') AND [s].[Name] IS NOT NULL)
ORDER BY [o].[Id], [e].[Id]");
        }

        public override void Query_with_OfType_eagerly_loads_correct_owned_navigations()
        {
            base.Query_with_OfType_eagerly_loads_correct_owned_navigations();

            AssertSql(
                @"SELECT [o].[Id], [o].[Discriminator], [t0].[Id], [t3].[Id], [t3].[PersonAddress_Country_Name], [t3].[PersonAddress_Country_PlanetId], [t5].[Id], [t8].[Id], [t8].[BranchAddress_Country_Name], [t8].[BranchAddress_Country_PlanetId], [t10].[Id], [t13].[Id], [t13].[LeafAAddress_Country_Name], [t13].[LeafAAddress_Country_PlanetId], [o15].[ClientId], [o15].[Id]
FROM [OwnedPerson] AS [o]
LEFT JOIN (
    SELECT [o0].[Id], [t].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o0]
    INNER JOIN (
        SELECT [o1].[Id], [o1].[Discriminator]
        FROM [OwnedPerson] AS [o1]
        WHERE [o1].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
    ) AS [t] ON [o0].[Id] = [t].[Id]
) AS [t0] ON [o].[Id] = [t0].[Id]
LEFT JOIN (
    SELECT [o2].[Id], [o2].[PersonAddress_Country_Name], [o2].[PersonAddress_Country_PlanetId], [t2].[Id] AS [Id0], [t2].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o2]
    INNER JOIN (
        SELECT [o3].[Id], [t1].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o3]
        INNER JOIN (
            SELECT [o4].[Id], [o4].[Discriminator]
            FROM [OwnedPerson] AS [o4]
            WHERE [o4].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA')
        ) AS [t1] ON [o3].[Id] = [t1].[Id]
    ) AS [t2] ON [o2].[Id] = [t2].[Id]
    WHERE [o2].[PersonAddress_Country_PlanetId] IS NOT NULL
) AS [t3] ON [t0].[Id] = [t3].[Id]
LEFT JOIN (
    SELECT [o5].[Id], [t4].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o5]
    INNER JOIN (
        SELECT [o6].[Id], [o6].[Discriminator]
        FROM [OwnedPerson] AS [o6]
        WHERE [o6].[Discriminator] IN (N'Branch', N'LeafA')
    ) AS [t4] ON [o5].[Id] = [t4].[Id]
) AS [t5] ON [o].[Id] = [t5].[Id]
LEFT JOIN (
    SELECT [o7].[Id], [o7].[BranchAddress_Country_Name], [o7].[BranchAddress_Country_PlanetId], [t7].[Id] AS [Id0], [t7].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o7]
    INNER JOIN (
        SELECT [o8].[Id], [t6].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o8]
        INNER JOIN (
            SELECT [o9].[Id], [o9].[Discriminator]
            FROM [OwnedPerson] AS [o9]
            WHERE [o9].[Discriminator] IN (N'Branch', N'LeafA')
        ) AS [t6] ON [o8].[Id] = [t6].[Id]
    ) AS [t7] ON [o7].[Id] = [t7].[Id]
    WHERE [o7].[BranchAddress_Country_PlanetId] IS NOT NULL
) AS [t8] ON [t5].[Id] = [t8].[Id]
LEFT JOIN (
    SELECT [o10].[Id], [t9].[Id] AS [Id0]
    FROM [OwnedPerson] AS [o10]
    INNER JOIN (
        SELECT [o11].[Id], [o11].[Discriminator]
        FROM [OwnedPerson] AS [o11]
        WHERE [o11].[Discriminator] = N'LeafA'
    ) AS [t9] ON [o10].[Id] = [t9].[Id]
) AS [t10] ON [o].[Id] = [t10].[Id]
LEFT JOIN (
    SELECT [o12].[Id], [o12].[LeafAAddress_Country_Name], [o12].[LeafAAddress_Country_PlanetId], [t12].[Id] AS [Id0], [t12].[Id0] AS [Id00]
    FROM [OwnedPerson] AS [o12]
    INNER JOIN (
        SELECT [o13].[Id], [t11].[Id] AS [Id0]
        FROM [OwnedPerson] AS [o13]
        INNER JOIN (
            SELECT [o14].[Id], [o14].[Discriminator]
            FROM [OwnedPerson] AS [o14]
            WHERE [o14].[Discriminator] = N'LeafA'
        ) AS [t11] ON [o13].[Id] = [t11].[Id]
    ) AS [t12] ON [o12].[Id] = [t12].[Id]
    WHERE [o12].[LeafAAddress_Country_PlanetId] IS NOT NULL
) AS [t13] ON [t10].[Id] = [t13].[Id]
LEFT JOIN [Order] AS [o15] ON [o].[Id] = [o15].[ClientId]
WHERE [o].[Discriminator] IN (N'OwnedPerson', N'Branch', N'LeafB', N'LeafA') AND ([o].[Discriminator] = N'LeafA')
ORDER BY [o].[Id], [o15].[ClientId], [o15].[Id]");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class OwnedQueryMySqlFixture : RelationalOwnedQueryFixture
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
