// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class GearsOfWarQueryMySqlTest : GearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        private static readonly string _eol = Environment.NewLine;

        // ReSharper disable once UnusedParameter.Local
#pragma warning disable IDE0060 // Remove unused parameter
        public GearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
#pragma warning restore IDE0060 // Remove unused parameter
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Entity_equality_empty(bool isAsync)
        {
            await base.Entity_equality_empty(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE CAST(0 AS bit) = CAST(1 AS bit)");
        }

        public override async Task Include_multiple_one_to_one_and_one_to_many(bool isAsync)
        {
            await base.Include_multiple_one_to_one_and_one_to_many(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [t0].[FullName] = [w].[OwnerFullName]
ORDER BY [t].[Id], [w].[Id]");
        }

        public override async Task Include_multiple_one_to_one_optional_and_one_to_one_required(bool isAsync)
        {
            await base.Include_multiple_one_to_one_optional_and_one_to_one_required(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [s].[Id], [s].[InternalNumber], [s].[Name]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]");
        }

        public override async Task Include_multiple_circular(bool isAsync)
        {
            await base.Include_multiple_circular(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [c].[Name], [c].[Location], [c].[Nation], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [c].[Name] = [t].[AssignedCityName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [c].[Name], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_multiple_circular_with_filter(bool isAsync)
        {
            await base.Include_multiple_circular_with_filter(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [c].[Name], [c].[Location], [c].[Nation], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [c].[Name] = [t].[AssignedCityName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] = N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [c].[Name], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_using_alternate_key(bool isAsync)
        {
            await base.Include_using_alternate_key(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] = N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [w].[Id]");
        }


        public override async Task Include_navigation_on_derived_type(bool isAsync)
        {
            await base.Include_navigation_on_derived_type(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task String_based_Include_navigation_on_derived_type(bool isAsync)
        {
            await base.String_based_Include_navigation_on_derived_type(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Select_Where_Navigation_Included(bool isAsync)
        {
            await base.Select_Where_Navigation_Included(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Nickname] = N'Marcus') AND [t0].[Nickname] IS NOT NULL");
        }

        public override async Task Include_with_join_reference1(bool isAsync)
        {
            await base.Include_with_join_reference1(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [c].[Name], [c].[Location], [c].[Nation]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t] ON (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL) AND (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL)
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Include_with_join_reference2(bool isAsync)
        {
            await base.Include_with_join_reference2(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [c].[Name], [c].[Location], [c].[Nation]
FROM [Tags] AS [t0]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL) AND (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL)
INNER JOIN [Cities] AS [c] ON [t].[CityOrBirthName] = [c].[Name]");
        }

        public override async Task Include_with_join_collection1(bool isAsync)
        {
            await base.Include_with_join_collection1(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t] ON (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL) AND (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id], [w].[Id]");
        }

        public override async Task Include_with_join_collection2(bool isAsync)
        {
            await base.Include_with_join_collection2(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [t0].[Id], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Tags] AS [t0]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL) AND (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
ORDER BY [t0].[Id], [t].[Nickname], [t].[SquadId], [w].[Id]");
        }

        public override void Include_where_list_contains_navigation(bool isAsync)
        {
            base.Include_where_list_contains_navigation(isAsync);

            AssertSql(
                @"SELECT [t].[Id]
FROM [Tags] AS [t]",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([t].[Id] IS NOT NULL AND [t].[Id] IN ('34c8d86e-a4ac-4be5-827f-584dda348a07', 'df36f493-463f-4123-83f9-6b135deeb7ba', 'a8ad98f9-e023-4e2a-9a70-c2728455bd34', '70534e05-782c-4052-8720-c2c54481ce5f', 'a7be028a-0cf2-448f-ab55-ce8bc5d8cf69', 'b39a6fba-9026-4d69-828e-fd7068673e57'))");
        }

        public override void Include_where_list_contains_navigation2(bool isAsync)
        {
            base.Include_where_list_contains_navigation2(isAsync);

            AssertSql(
                @"SELECT [t].[Id]
FROM [Tags] AS [t]",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([c].[Location] IS NOT NULL AND [t].[Id] IN ('34c8d86e-a4ac-4be5-827f-584dda348a07', 'df36f493-463f-4123-83f9-6b135deeb7ba', 'a8ad98f9-e023-4e2a-9a70-c2728455bd34', '70534e05-782c-4052-8720-c2c54481ce5f', 'a7be028a-0cf2-448f-ab55-ce8bc5d8cf69', 'b39a6fba-9026-4d69-828e-fd7068673e57'))");
        }

        public override void Navigation_accessed_twice_outside_and_inside_subquery(bool isAsync)
        {
            base.Navigation_accessed_twice_outside_and_inside_subquery(isAsync);

            AssertSql(
                @"SELECT [t].[Id]
FROM [Tags] AS [t]",
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([t].[Id] IS NOT NULL AND [t].[Id] IN ('34c8d86e-a4ac-4be5-827f-584dda348a07', 'df36f493-463f-4123-83f9-6b135deeb7ba', 'a8ad98f9-e023-4e2a-9a70-c2728455bd34', '70534e05-782c-4052-8720-c2c54481ce5f', 'a7be028a-0cf2-448f-ab55-ce8bc5d8cf69', 'b39a6fba-9026-4d69-828e-fd7068673e57'))");
        }

        public override async Task Include_with_join_multi_level(bool isAsync)
        {
            await base.Include_with_join_multi_level(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [c].[Name], [c].[Location], [c].[Nation], [t].[Id], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t] ON (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL) AND (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL)
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON [c].[Name] = [t0].[AssignedCityName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id], [c].[Name], [t0].[Nickname], [t0].[SquadId]");
        }

        public override async Task Include_with_join_and_inheritance1(bool isAsync)
        {
            await base.Include_with_join_and_inheritance1(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [c].[Name], [c].[Location], [c].[Nation]
FROM [Tags] AS [t0]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
) AS [t] ON (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL) AND (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL)
INNER JOIN [Cities] AS [c] ON [t].[CityOrBirthName] = [c].[Name]");
        }

        public override async Task Include_with_join_and_inheritance_with_orderby_before_and_after_include(bool isAsync)
        {
            await base.Include_with_join_and_inheritance_with_orderby_before_and_after_include(isAsync);

            AssertSql(
                "");
        }

        public override async Task Include_with_join_and_inheritance2(bool isAsync)
        {
            await base.Include_with_join_and_inheritance2(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t] ON (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL) AND (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id], [w].[Id]");
        }

        public override async Task Include_with_join_and_inheritance3(bool isAsync)
        {
            await base.Include_with_join_and_inheritance3(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [t0].[Id], [t1].[Nickname], [t1].[SquadId], [t1].[AssignedCityName], [t1].[CityOrBirthName], [t1].[Discriminator], [t1].[FullName], [t1].[HasSoulPatch], [t1].[LeaderNickname], [t1].[LeaderSquadId], [t1].[Rank]
FROM [Tags] AS [t0]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
) AS [t] ON (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL) AND (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t].[Nickname] = [t1].[LeaderNickname]) AND [t1].[LeaderNickname] IS NOT NULL) AND ([t].[SquadId] = [t1].[LeaderSquadId])
ORDER BY [t0].[Id], [t].[Nickname], [t].[SquadId], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task Include_with_nested_navigation_in_order_by(bool isAsync)
        {
            await base.Include_with_nested_navigation_in_order_by(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Weapons] AS [w]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
LEFT JOIN [Cities] AS [c] ON [t].[CityOrBirthName] = [c].[Name]
WHERE ([t].[Nickname] <> N'Paduk') OR [t].[Nickname] IS NULL
ORDER BY [c].[Name], [w].[Id]");
        }

        public override async Task Where_enum(bool isAsync)
        {
            await base.Where_enum(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Rank] = 2)");
        }

        public override async Task Where_nullable_enum_with_constant(bool isAsync)
        {
            await base.Where_nullable_enum_with_constant(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE ([w].[AmmunitionType] = 1) AND [w].[AmmunitionType] IS NOT NULL");
        }

        public override async Task Where_nullable_enum_with_null_constant(bool isAsync)
        {
            await base.Where_nullable_enum_with_null_constant(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE [w].[AmmunitionType] IS NULL");
        }

        public override async Task Where_nullable_enum_with_non_nullable_parameter(bool isAsync)
        {
            await base.Where_nullable_enum_with_non_nullable_parameter(isAsync);

            AssertSql(
                @"@__ammunitionType_0='1'

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL)");
        }

        public override async Task Where_nullable_enum_with_nullable_parameter(bool isAsync)
        {
            await base.Where_nullable_enum_with_nullable_parameter(isAsync);

            AssertSql(
                @"@__ammunitionType_0='1' (Nullable = true)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL)",
                //
                @"@__ammunitionType_0='' (DbType = Int32)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL)");
        }

        public override async Task Where_bitwise_and_enum(bool isAsync)
        {
            await base.Where_bitwise_and_enum(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) > 0)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)");
        }

        public override async Task Where_bitwise_and_integral(bool isAsync)
        {
            await base.Where_bitwise_and_integral(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ((CAST([g].[Rank] AS bigint) & CAST(1 AS bigint)) = CAST(1 AS bigint))",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ((CAST([g].[Rank] AS smallint) & CAST(1 AS smallint)) = CAST(1 AS smallint))");
        }

        public override async Task Where_bitwise_and_nullable_enum_with_constant(bool isAsync)
        {
            await base.Where_bitwise_and_nullable_enum_with_constant(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE ([w].[AmmunitionType] & 1) > 0");
        }

        public override async Task Where_bitwise_and_nullable_enum_with_null_constant(bool isAsync)
        {
            await base.Where_bitwise_and_nullable_enum_with_null_constant(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE ([w].[AmmunitionType] & NULL) > 0");
        }

        public override async Task Where_bitwise_and_nullable_enum_with_non_nullable_parameter(bool isAsync)
        {
            await base.Where_bitwise_and_nullable_enum_with_non_nullable_parameter(isAsync);

            AssertSql(
                @"@__ammunitionType_0='1'

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE ([w].[AmmunitionType] & @__ammunitionType_0) > 0");
        }

        public override async Task Where_bitwise_and_nullable_enum_with_nullable_parameter(bool isAsync)
        {
            await base.Where_bitwise_and_nullable_enum_with_nullable_parameter(isAsync);

            AssertSql(
                @"@__ammunitionType_0='1' (Nullable = true)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE ([w].[AmmunitionType] & @__ammunitionType_0) > 0",
                //
                @"@__ammunitionType_0='' (DbType = Int32)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE ([w].[AmmunitionType] & @__ammunitionType_0) > 0");
        }

        public override async Task Where_bitwise_or_enum(bool isAsync)
        {
            await base.Where_bitwise_or_enum(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] | 1) > 0)");
        }

        public override async Task Bitwise_projects_values_in_select(bool isAsync)
        {
            await base.Bitwise_projects_values_in_select(isAsync);

            AssertSql(
                @"SELECT TOP(1) CASE
    WHEN ([g].[Rank] & 1) = 1 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, CASE
    WHEN ([g].[Rank] & 1) = 2 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [g].[Rank] & 1
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)");
        }

        public override async Task Where_enum_has_flag(bool isAsync)
        {
            await base.Where_enum_has_flag(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 9) = 9)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ((1 & [g].[Rank]) = [g].[Rank])");
        }

        public override async Task Where_enum_has_flag_subquery(bool isAsync)
        {
            await base.Where_enum_has_flag_subquery(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (((([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) = (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) AND ([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL)) OR ([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL))",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ((((1 & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) = (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) AND (1 & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL)) OR (1 & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL))");
        }

        public override async Task Where_enum_has_flag_subquery_with_pushdown(bool isAsync)
        {
            await base.Where_enum_has_flag_subquery_with_pushdown(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (((([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) = (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) AND ([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL)) OR ([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL))",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ((((1 & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) = (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) AND (1 & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL)) OR (1 & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL))");
        }

        public override async Task Where_enum_has_flag_subquery_client_eval(bool isAsync)
        {
            await base.Where_enum_has_flag_subquery_client_eval(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (((([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) = (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId])) AND ([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NOT NULL)) OR ([g].[Rank] & (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL AND (
    SELECT TOP(1) [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g0].[Nickname], [g0].[SquadId]) IS NULL))");
        }

        public override async Task Where_enum_has_flag_with_non_nullable_parameter(bool isAsync)
        {
            await base.Where_enum_has_flag_with_non_nullable_parameter(isAsync);

            AssertSql(
                @"@__parameter_0='1'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (((([g].[Rank] & @__parameter_0) = @__parameter_0) AND ([g].[Rank] & @__parameter_0 IS NOT NULL AND @__parameter_0 IS NOT NULL)) OR ([g].[Rank] & @__parameter_0 IS NULL AND @__parameter_0 IS NULL))");
        }

        public override async Task Where_has_flag_with_nullable_parameter(bool isAsync)
        {
            await base.Where_has_flag_with_nullable_parameter(isAsync);

            AssertSql(
                @"@__parameter_0='1' (Nullable = true)

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (((([g].[Rank] & @__parameter_0) = @__parameter_0) AND ([g].[Rank] & @__parameter_0 IS NOT NULL AND @__parameter_0 IS NOT NULL)) OR ([g].[Rank] & @__parameter_0 IS NULL AND @__parameter_0 IS NULL))");
        }

        public override async Task Select_enum_has_flag(bool isAsync)
        {
            await base.Select_enum_has_flag(isAsync);

            AssertSql(
                @"SELECT TOP(1) CASE
    WHEN ([g].[Rank] & 1) = 1 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, CASE
    WHEN ([g].[Rank] & 2) = 2 THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Rank] & 1) = 1)");
        }

        public override async Task Where_count_subquery_without_collision(bool isAsync)
        {
            await base.Where_count_subquery_without_collision(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (((
    SELECT COUNT(*)
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) = 2) AND (
    SELECT COUNT(*)
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) IS NOT NULL)");
        }

        public override async Task Where_any_subquery_without_collision(bool isAsync)
        {
            await base.Where_any_subquery_without_collision(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND EXISTS (
    SELECT 1
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL)");
        }

        public override async Task Select_inverted_boolean(bool isAsync)
        {
            await base.Select_inverted_boolean(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN [w].[IsAutomatic] <> CAST(1 AS bit) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Manual]
FROM [Weapons] AS [w]
WHERE [w].[IsAutomatic] = CAST(1 AS bit)");
        }

        public override async Task Select_comparison_with_null(bool isAsync)
        {
            await base.Select_comparison_with_null(isAsync);

            AssertSql(
                @"@__ammunitionType_0='1' (Nullable = true)

SELECT [w].[Id], CASE
    WHEN (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Cartridge]
FROM [Weapons] AS [w]
WHERE (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL)",
                //
                @"@__ammunitionType_0='' (DbType = Int32)

SELECT [w].[Id], CASE
    WHEN (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [Cartridge]
FROM [Weapons] AS [w]
WHERE (([w].[AmmunitionType] = @__ammunitionType_0) AND ([w].[AmmunitionType] IS NOT NULL AND @__ammunitionType_0 IS NOT NULL)) OR ([w].[AmmunitionType] IS NULL AND @__ammunitionType_0 IS NULL)");
        }

        public override async Task Select_ternary_operation_with_boolean(bool isAsync)
        {
            await base.Select_ternary_operation_with_boolean(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN [w].[IsAutomatic] = CAST(1 AS bit) THEN 1
    ELSE 0
END AS [Num]
FROM [Weapons] AS [w]");
        }

        public override async Task Select_ternary_operation_with_inverted_boolean(bool isAsync)
        {
            await base.Select_ternary_operation_with_inverted_boolean(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN [w].[IsAutomatic] <> CAST(1 AS bit) THEN 1
    ELSE 0
END AS [Num]
FROM [Weapons] AS [w]");
        }

        public override async Task Select_ternary_operation_with_has_value_not_null(bool isAsync)
        {
            await base.Select_ternary_operation_with_has_value_not_null(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN [w].[AmmunitionType] IS NOT NULL AND (([w].[AmmunitionType] = 1) AND [w].[AmmunitionType] IS NOT NULL) THEN N'Yes'
    ELSE N'No'
END AS [IsCartridge]
FROM [Weapons] AS [w]
WHERE [w].[AmmunitionType] IS NOT NULL AND (([w].[AmmunitionType] = 1) AND [w].[AmmunitionType] IS NOT NULL)");
        }

        public override async Task Select_ternary_operation_multiple_conditions(bool isAsync)
        {
            await base.Select_ternary_operation_multiple_conditions(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN (([w].[AmmunitionType] = 2) AND [w].[AmmunitionType] IS NOT NULL) AND (([w].[SynergyWithId] = 1) AND [w].[SynergyWithId] IS NOT NULL) THEN N'Yes'
    ELSE N'No'
END AS [IsCartridge]
FROM [Weapons] AS [w]");
        }

        public override async Task Select_ternary_operation_multiple_conditions_2(bool isAsync)
        {
            await base.Select_ternary_operation_multiple_conditions_2(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN ([w].[IsAutomatic] <> CAST(1 AS bit)) AND (([w].[SynergyWithId] = 1) AND [w].[SynergyWithId] IS NOT NULL) THEN N'Yes'
    ELSE N'No'
END AS [IsCartridge]
FROM [Weapons] AS [w]");
        }

        public override async Task Select_multiple_conditions(bool isAsync)
        {
            await base.Select_multiple_conditions(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN ([w].[IsAutomatic] <> CAST(1 AS bit)) AND (([w].[SynergyWithId] = 1) AND [w].[SynergyWithId] IS NOT NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [IsCartridge]
FROM [Weapons] AS [w]");
        }

        public override async Task Select_nested_ternary_operations(bool isAsync)
        {
            await base.Select_nested_ternary_operations(isAsync);

            AssertSql(
                @"SELECT [w].[Id], CASE
    WHEN [w].[IsAutomatic] <> CAST(1 AS bit) THEN CASE
        WHEN ([w].[AmmunitionType] = 1) AND [w].[AmmunitionType] IS NOT NULL THEN N'ManualCartridge'
        ELSE N'Manual'
    END
    ELSE N'Auto'
END AS [IsManualCartridge]
FROM [Weapons] AS [w]");
        }

        public override async Task Null_propagation_optimization1(bool isAsync)
        {
            await base.Null_propagation_optimization1(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[LeaderNickname] = N'Marcus') AND [g].[LeaderNickname] IS NOT NULL)");
        }

        public override async Task Null_propagation_optimization2(bool isAsync)
        {
            await base.Null_propagation_optimization2(isAsync);

            // issue #16050
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND [g].[LeaderNickname] LIKE N'%us'");
        }

        public override async Task Null_propagation_optimization3(bool isAsync)
        {
            await base.Null_propagation_optimization3(isAsync);

            // issue #16050
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND [g].[LeaderNickname] LIKE N'%us'");
        }

        public override async Task Null_propagation_optimization4(bool isAsync)
        {
            await base.Null_propagation_optimization4(isAsync);

            // issue #16050
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (CAST(LEN([g].[LeaderNickname]) AS int) = 5)");
        }

        public override async Task Null_propagation_optimization5(bool isAsync)
        {
            await base.Null_propagation_optimization5(isAsync);

            // issue #16050
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (CAST(LEN([g].[LeaderNickname]) AS int) = 5)");
        }

        public override async Task Null_propagation_optimization6(bool isAsync)
        {
            await base.Null_propagation_optimization6(isAsync);

            // issue #16050
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (CAST(LEN([g].[LeaderNickname]) AS int) = 5)");
        }

        public override async Task Select_null_propagation_optimization7(bool isAsync)
        {
            await base.Select_null_propagation_optimization7(isAsync);

            // issue #16050
            //            AssertSql(
            //                @"SELECT [g].[LeaderNickname] + [g].[LeaderNickname]
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Select_null_propagation_optimization8(bool isAsync)
        {
            await base.Select_null_propagation_optimization8(isAsync);

            AssertSql(
                @"SELECT [g].[LeaderNickname] + [g].[LeaderNickname]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Select_null_propagation_optimization9(bool isAsync)
        {
            await base.Select_null_propagation_optimization9(isAsync);

            AssertSql(
                @"SELECT CAST(LEN([g].[FullName]) AS int)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_null_propagation_negative1(bool isAsync)
        {
            await base.Select_null_propagation_negative1(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[LeaderNickname] IS NOT NULL THEN CASE
        WHEN CAST(LEN([g].[Nickname]) AS int) = 5 THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END
    ELSE NULL
END
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_null_propagation_negative2(bool isAsync)
        {
            await base.Select_null_propagation_negative2(isAsync);

            // issue #16081
            //            AssertSql(
            //                @"SELECT CASE
            //    WHEN [g1].[LeaderNickname] IS NOT NULL
            //    THEN [g2].[LeaderNickname] ELSE NULL
            //END
            //FROM [Gears] AS [g1]
            //CROSS JOIN [Gears] AS [g2]
            //WHERE [g1].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Select_null_propagation_negative3(bool isAsync)
        {
            await base.Select_null_propagation_negative3(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], CASE
    WHEN [t].[Nickname] IS NOT NULL THEN CASE
        WHEN [t].[LeaderNickname] IS NOT NULL THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END
    ELSE NULL
END AS [Condition]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[HasSoulPatch] = CAST(1 AS bit)
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [t].[Nickname]");
        }

        public override async Task Select_null_propagation_negative4(bool isAsync)
        {
            await base.Select_null_propagation_negative4(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [t].[Nickname] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [t].[Nickname]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[HasSoulPatch] = CAST(1 AS bit)
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [t].[Nickname]");
        }

        public override async Task Select_null_propagation_negative5(bool isAsync)
        {
            await base.Select_null_propagation_negative5(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [t].[Nickname] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [t].[Nickname]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[HasSoulPatch] = CAST(1 AS bit)
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [t].[Nickname]");
        }

        public override async Task Select_null_propagation_negative6(bool isAsync)
        {
            await base.Select_null_propagation_negative6(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[LeaderNickname] IS NOT NULL THEN CASE
        WHEN ((CAST(LEN([g].[LeaderNickname]) AS int) <> CAST(LEN([g].[LeaderNickname]) AS int)) OR (CAST(LEN([g].[LeaderNickname]) AS int) IS NULL OR CAST(LEN([g].[LeaderNickname]) AS int) IS NULL)) AND (CAST(LEN([g].[LeaderNickname]) AS int) IS NOT NULL OR CAST(LEN([g].[LeaderNickname]) AS int) IS NOT NULL) THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END
    ELSE NULL
END
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_null_propagation_negative7(bool isAsync)
        {
            await base.Select_null_propagation_negative7(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[LeaderNickname] IS NOT NULL THEN CASE
        WHEN (([g].[LeaderNickname] = [g].[LeaderNickname]) AND ([g].[LeaderNickname] IS NOT NULL AND [g].[LeaderNickname] IS NOT NULL)) OR ([g].[LeaderNickname] IS NULL AND [g].[LeaderNickname] IS NULL) THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END
    ELSE NULL
END
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_null_propagation_negative8(bool isAsync)
        {
            await base.Select_null_propagation_negative8(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [s].[Id] IS NOT NULL THEN [c].[Name]
    ELSE NULL
END
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
LEFT JOIN [Cities] AS [c] ON [t0].[AssignedCityName] = [c].[Name]");
        }

        public override async Task Select_null_propagation_works_for_navigations_with_composite_keys(bool isAsync)
        {
            await base.Select_null_propagation_works_for_navigations_with_composite_keys(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)");
        }

        public override async Task Select_null_propagation_works_for_multiple_navigations_with_composite_keys(bool isAsync)
        {
            await base.Select_null_propagation_works_for_multiple_navigations_with_composite_keys(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [c].[Name] IS NOT NULL THEN [c].[Name]
    ELSE NULL
END
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Tags] AS [t1] ON ((([t0].[Nickname] = [t1].[GearNickName]) AND ([t0].[Nickname] IS NOT NULL AND [t1].[GearNickName] IS NOT NULL)) OR ([t0].[Nickname] IS NULL AND [t1].[GearNickName] IS NULL)) AND ((([t0].[SquadId] = [t1].[GearSquadId]) AND ([t0].[SquadId] IS NOT NULL AND [t1].[GearSquadId] IS NOT NULL)) OR ([t0].[SquadId] IS NULL AND [t1].[GearSquadId] IS NULL))
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t1].[GearNickName] = [t2].[Nickname]) AND [t1].[GearNickName] IS NOT NULL) AND (([t1].[GearSquadId] = [t2].[SquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN [Cities] AS [c] ON [t2].[AssignedCityName] = [c].[Name]");
        }

        public override async Task Select_conditional_with_anonymous_type_and_null_constant(bool isAsync)
        {
            await base.Select_conditional_with_anonymous_type_and_null_constant(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[LeaderNickname] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [g].[HasSoulPatch]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname]");
        }

        public override async Task Select_conditional_with_anonymous_types(bool isAsync)
        {
            await base.Select_conditional_with_anonymous_types(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[LeaderNickname] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [g].[Nickname], [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname]");
        }

        public override async Task Select_coalesce_with_anonymous_types(bool isAsync)
        {
            await base.Select_coalesce_with_anonymous_types(isAsync);

            AssertSql(
                @"SELECT [g].[LeaderNickname], [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname]");
        }

        public override void Where_compare_anonymous_types()
        {
            base.Where_compare_anonymous_types();

            AssertSql(
                "");
        }

        public override async Task Where_member_access_on_anonymous_type(bool isAsync)
        {
            await base.Where_member_access_on_anonymous_type(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[LeaderNickname] = N'Marcus') AND [g].[LeaderNickname] IS NOT NULL)");
        }

        public override async Task Where_compare_anonymous_types_with_uncorrelated_members(bool isAsync)
        {
            await base.Where_compare_anonymous_types_with_uncorrelated_members(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname]
FROM [Gears] AS [g]
WHERE CAST(0 AS bit) = CAST(1 AS bit)");
        }

        public override async Task Select_Where_Navigation_Scalar_Equals_Navigation_Scalar(bool isAsync)
        {
            await base.Select_Where_Navigation_Scalar_Equals_Navigation_Scalar(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Id], [t0].[GearNickName], [t0].[GearSquadId], [t0].[Note]
FROM [Tags] AS [t]
CROSS JOIN [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t].[GearNickName] = [t1].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t1].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t0].[GearNickName] = [t2].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t2].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE (([t1].[Nickname] = [t2].[Nickname]) AND ([t1].[Nickname] IS NOT NULL AND [t2].[Nickname] IS NOT NULL)) OR ([t1].[Nickname] IS NULL AND [t2].[Nickname] IS NULL)");
        }

        public override async Task Select_Singleton_Navigation_With_Member_Access(bool isAsync)
        {
            await base.Select_Singleton_Navigation_With_Member_Access(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE (([t].[Nickname] = N'Marcus') AND [t].[Nickname] IS NOT NULL) AND (([t].[CityOrBirthName] <> N'Ephyra') OR [t].[CityOrBirthName] IS NULL)");
        }

        public override async Task Select_Where_Navigation(bool isAsync)
        {
            await base.Select_Where_Navigation(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Nickname] = N'Marcus') AND [t0].[Nickname] IS NOT NULL");
        }

        public override async Task Select_Where_Navigation_Equals_Navigation(bool isAsync)
        {
            await base.Select_Where_Navigation_Equals_Navigation(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Id], [t0].[GearNickName], [t0].[GearSquadId], [t0].[Note]
FROM [Tags] AS [t]
CROSS JOIN [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t].[GearNickName] = [t1].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t1].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t0].[GearNickName] = [t2].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t2].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE ((([t1].[Nickname] = [t2].[Nickname]) AND ([t1].[Nickname] IS NOT NULL AND [t2].[Nickname] IS NOT NULL)) OR ([t1].[Nickname] IS NULL AND [t2].[Nickname] IS NULL)) AND ((([t1].[SquadId] = [t2].[SquadId]) AND ([t1].[SquadId] IS NOT NULL AND [t2].[SquadId] IS NOT NULL)) OR ([t1].[SquadId] IS NULL AND [t2].[SquadId] IS NULL))");
        }

        public override async Task Select_Where_Navigation_Null(bool isAsync)
        {
            await base.Select_Where_Navigation_Null(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [t0].[Nickname] IS NULL");
        }

        public override async Task Select_Where_Navigation_Null_Reverse(bool isAsync)
        {
            await base.Select_Where_Navigation_Null_Reverse(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [t0].[Nickname] IS NULL");
        }

        public override async Task Select_Where_Navigation_Scalar_Equals_Navigation_Scalar_Projected(bool isAsync)
        {
            await base.Select_Where_Navigation_Scalar_Equals_Navigation_Scalar_Projected(isAsync);

            AssertSql(
                @"SELECT [t].[Id] AS [Id1], [t0].[Id] AS [Id2]
FROM [Tags] AS [t]
CROSS JOIN [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t].[GearNickName] = [t1].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t1].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t0].[GearNickName] = [t2].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t2].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE (([t1].[Nickname] = [t2].[Nickname]) AND ([t1].[Nickname] IS NOT NULL AND [t2].[Nickname] IS NOT NULL)) OR ([t1].[Nickname] IS NULL AND [t2].[Nickname] IS NULL)");
        }

        public override async Task Optional_Navigation_Null_Coalesce_To_Clr_Type(bool isAsync)
        {
            await base.Optional_Navigation_Null_Coalesce_To_Clr_Type(isAsync);

            AssertSql(
                @"SELECT TOP(1) COALESCE([w].[IsAutomatic], CAST(0 AS bit))
FROM [Weapons] AS [w0]
LEFT JOIN [Weapons] AS [w] ON [w0].[SynergyWithId] = [w].[Id]
ORDER BY [w0].[Id]");
        }

        public override async Task Where_subquery_boolean(bool isAsync)
        {
            await base.Where_subquery_boolean(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (COALESCE((
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE [g].[FullName] = [w].[OwnerFullName]
    ORDER BY [w].[Id]
), CAST(0 AS bit)) = CAST(1 AS bit))");
        }

        public override async Task Where_subquery_boolean_with_pushdown(bool isAsync)
        {
            await base.Where_subquery_boolean_with_pushdown(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ((
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE [g].[FullName] = [w].[OwnerFullName]
    ORDER BY [w].[Id]
) = CAST(1 AS bit))");
        }

        public override async Task Where_subquery_distinct_firstordefault_boolean(bool isAsync)
        {
            await base.Where_subquery_distinct_firstordefault_boolean(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND (COALESCE((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].*
        FROM [Weapons] AS [w]
        WHERE [g].[FullName] = [w].[OwnerFullName]
    ) AS [t]
    ORDER BY [t].[Id]
), CAST(0 AS bit)) = CAST(1 AS bit)))");
        }

        public override async Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool isAsync)
        {
            await base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].*
        FROM [Weapons] AS [w]
        WHERE [g].[FullName] = [w].[OwnerFullName]
    ) AS [t]
    ORDER BY [t].[Id]
) = CAST(1 AS bit)))");
        }

        public override async Task Where_subquery_distinct_first_boolean(bool isAsync)
        {
            await base.Where_subquery_distinct_first_boolean(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ) AS [t]
    ORDER BY [t].[Id]) = CAST(1 AS bit)))
ORDER BY [g].[Nickname]");
        }

        public override async Task Where_subquery_distinct_singleordefault_boolean1(bool isAsync)
        {
            await base.Where_subquery_distinct_singleordefault_boolean1(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Lancer', [w].[Name]) > 0)
    ) AS [t]) = CAST(1 AS bit)))
ORDER BY [g].[Nickname]");
        }

        public override async Task Where_subquery_distinct_singleordefault_boolean2(bool isAsync)
        {
            await base.Where_subquery_distinct_singleordefault_boolean2(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ((
    SELECT DISTINCT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Lancer', [w].[Name]) > 0)) = CAST(1 AS bit)))
ORDER BY [g].[Nickname]");
        }

        public override async Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool isAsync)
        {
            await base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Lancer', [w].[Name]) > 0)
    ) AS [t]) = CAST(1 AS bit)))
ORDER BY [g].[Nickname]");
        }

        public override void Where_subquery_distinct_lastordefault_boolean()
        {
            base.Where_subquery_distinct_lastordefault_boolean();

            AssertSql(
                "");
        }

        public override void Where_subquery_distinct_last_boolean()
        {
            base.Where_subquery_distinct_last_boolean();

            AssertSql(
                "");
        }

        public override async Task Where_subquery_distinct_orderby_firstordefault_boolean(bool isAsync)
        {
            await base.Where_subquery_distinct_orderby_firstordefault_boolean(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND (COALESCE((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].*
        FROM [Weapons] AS [w]
        WHERE [g].[FullName] = [w].[OwnerFullName]
    ) AS [t]
    ORDER BY [t].[Id]
), CAST(0 AS bit)) = CAST(1 AS bit)))");
        }

        public override async Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool isAsync)
        {
            await base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ((
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].*
        FROM [Weapons] AS [w]
        WHERE [g].[FullName] = [w].[OwnerFullName]
    ) AS [t]
    ORDER BY [t].[Id]
) = CAST(1 AS bit)))");
        }

        public override async Task Where_subquery_union_firstordefault_boolean(bool isAsync)
        {
            await base.Where_subquery_union_firstordefault_boolean(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[HasSoulPatch] = 1)",
                //
                @"@_outer_FullName6='Damon Baird' (Size = 450)

SELECT [w6].[Id], [w6].[AmmunitionType], [w6].[IsAutomatic], [w6].[Name], [w6].[OwnerFullName], [w6].[SynergyWithId]
FROM [Weapons] AS [w6]
WHERE @_outer_FullName6 = [w6].[OwnerFullName]",
                //
                @"@_outer_FullName5='Damon Baird' (Size = 450)

SELECT [w5].[Id], [w5].[AmmunitionType], [w5].[IsAutomatic], [w5].[Name], [w5].[OwnerFullName], [w5].[SynergyWithId]
FROM [Weapons] AS [w5]
WHERE @_outer_FullName5 = [w5].[OwnerFullName]",
                //
                @"@_outer_FullName6='Marcus Fenix' (Size = 450)

SELECT [w6].[Id], [w6].[AmmunitionType], [w6].[IsAutomatic], [w6].[Name], [w6].[OwnerFullName], [w6].[SynergyWithId]
FROM [Weapons] AS [w6]
WHERE @_outer_FullName6 = [w6].[OwnerFullName]",
                //
                @"@_outer_FullName5='Marcus Fenix' (Size = 450)

SELECT [w5].[Id], [w5].[AmmunitionType], [w5].[IsAutomatic], [w5].[Name], [w5].[OwnerFullName], [w5].[SynergyWithId]
FROM [Weapons] AS [w5]
WHERE @_outer_FullName5 = [w5].[OwnerFullName]");
        }

        public override async Task Concat_with_count(bool isAsync)
        {
            await base.Concat_with_count(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    UNION ALL
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t]");
        }

        public override async Task Concat_scalars_with_count(bool isAsync)
        {
            await base.Concat_scalars_with_count(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM (
    SELECT [g].[Nickname]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    UNION ALL
    SELECT [g0].[FullName] AS [Nickname]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t]");
        }

        public override async Task Concat_anonymous_with_count(bool isAsync)
        {
            await base.Concat_anonymous_with_count(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [g].[Nickname] AS [Name]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    UNION ALL
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [g0].[FullName] AS [Name]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t]");
        }

        public override void Concat_with_scalar_projection()
        {
            base.Concat_with_scalar_projection();

            AssertSql(
                "");
        }

        public override async Task Concat_with_groupings(bool isAsync)
        {
            await base.Concat_with_groupings(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[LeaderNickname]",
                //
                @"SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
FROM [Gears] AS [g0]
WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g0].[LeaderNickname]");
        }

        public override async Task Select_subquery_distinct_firstordefault(bool isAsync)
        {
            await base.Select_subquery_distinct_firstordefault(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [t].[Name]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ) AS [t]
    ORDER BY [t].[Id])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Singleton_Navigation_With_Member_Access(bool isAsync)
        {
            await base.Singleton_Navigation_With_Member_Access(isAsync);

            AssertSql(
                @"SELECT [t].[CityOrBirthName] AS [B]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE (([t].[Nickname] = N'Marcus') AND [t].[Nickname] IS NOT NULL) AND (([t].[CityOrBirthName] <> N'Ephyra') OR [t].[CityOrBirthName] IS NULL)");
        }

        public override async Task GroupJoin_Composite_Key(bool isAsync)
        {
            await base.GroupJoin_Composite_Key(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Tags] AS [t0]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)");
        }

        public override async Task Join_navigation_translated_to_subquery_composite_key(bool isAsync)
        {
            await base.Join_navigation_translated_to_subquery_composite_key(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [t1].[Note]
FROM [Gears] AS [g]
INNER JOIN (
    SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
    FROM [Tags] AS [t]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
) AS [t1] ON [g].[FullName] = [t1].[FullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Join_with_order_by_on_inner_sequence_navigation_translated_to_subquery_composite_key(bool isAsync)
        {
            await base.Join_with_order_by_on_inner_sequence_navigation_translated_to_subquery_composite_key(isAsync);

            AssertSql(
                "");
        }

        public override async Task Join_with_order_by_without_skip_or_take(bool isAsync)
        {
            await base.Join_with_order_by_without_skip_or_take(isAsync);

            // issue #16086
            //            AssertSql(
            //                @"SELECT [t].[Name], [g].[FullName]
            //FROM [Gears] AS [g]
            //INNER JOIN (
            //    SELECT [ww].*
            //    FROM [Weapons] AS [ww]
            //    ORDER BY [ww].[Name]
            //    OFFSET 0 ROWS
            //) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Collection_with_inheritance_and_join_include_joined(bool isAsync)
        {
            await base.Collection_with_inheritance_and_join_include_joined(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [t0].[Id], [t0].[GearNickName], [t0].[GearSquadId], [t0].[Note]
FROM [Tags] AS [t1]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
) AS [t] ON (([t1].[GearSquadId] = [t].[SquadId]) AND [t1].[GearSquadId] IS NOT NULL) AND (([t1].[GearNickName] = [t].[Nickname]) AND [t1].[GearNickName] IS NOT NULL)
LEFT JOIN [Tags] AS [t0] ON (([t].[Nickname] = [t0].[GearNickName]) AND [t0].[GearNickName] IS NOT NULL) AND (([t].[SquadId] = [t0].[GearSquadId]) AND [t0].[GearSquadId] IS NOT NULL)");
        }

        public override async Task Collection_with_inheritance_and_join_include_source(bool isAsync)
        {
            await base.Collection_with_inheritance_and_join_include_source(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t0] ON (([g].[SquadId] = [t0].[GearSquadId]) AND [t0].[GearSquadId] IS NOT NULL) AND (([g].[Nickname] = [t0].[GearNickName]) AND [t0].[GearNickName] IS NOT NULL)
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')");
        }

        public override async Task Non_unicode_string_literal_is_used_for_non_unicode_column(bool isAsync)
        {
            await base.Non_unicode_string_literal_is_used_for_non_unicode_column(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE ([c].[Location] = 'Unknown') AND [c].[Location] IS NOT NULL");
        }

        public override async Task Non_unicode_string_literal_is_used_for_non_unicode_column_right(bool isAsync)
        {
            await base.Non_unicode_string_literal_is_used_for_non_unicode_column_right(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE ('Unknown' = [c].[Location]) AND [c].[Location] IS NOT NULL");
        }

        public override async Task Non_unicode_parameter_is_used_for_non_unicode_column(bool isAsync)
        {
            await base.Non_unicode_parameter_is_used_for_non_unicode_column(isAsync);

            AssertSql(
                @"@__value_0='Unknown' (Size = 100) (DbType = AnsiString)

SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE (([c].[Location] = @__value_0) AND ([c].[Location] IS NOT NULL AND @__value_0 IS NOT NULL)) OR ([c].[Location] IS NULL AND @__value_0 IS NULL)");
        }

        public override async Task Non_unicode_string_literals_in_contains_is_used_for_non_unicode_column(bool isAsync)
        {
            await base.Non_unicode_string_literals_in_contains_is_used_for_non_unicode_column(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE [c].[Location] IN ('Unknown', 'Jacinto''s location', 'Ephyra''s location')");
        }

        public override async Task Non_unicode_string_literals_is_used_for_non_unicode_column_with_subquery(bool isAsync)
        {
            await base.Non_unicode_string_literals_is_used_for_non_unicode_column_with_subquery(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE (([c].[Location] = 'Unknown') AND [c].[Location] IS NOT NULL) AND (((
    SELECT COUNT(*)
    FROM [Gears] AS [g]
    WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([c].[Name] = [g].[CityOrBirthName])) AND ([g].[Nickname] = N'Paduk')) = 1) AND (
    SELECT COUNT(*)
    FROM [Gears] AS [g]
    WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([c].[Name] = [g].[CityOrBirthName])) AND ([g].[Nickname] = N'Paduk')) IS NOT NULL)");
        }

        public override async Task Non_unicode_string_literals_is_used_for_non_unicode_column_in_subquery(bool isAsync)
        {
            await base.Non_unicode_string_literals_is_used_for_non_unicode_column_in_subquery(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Nickname] = N'Marcus') AND (([c].[Location] = 'Jacinto''s location') AND [c].[Location] IS NOT NULL))");
        }

        public override async Task Non_unicode_string_literals_is_used_for_non_unicode_column_with_contains(bool isAsync)
        {
            await base.Non_unicode_string_literals_is_used_for_non_unicode_column_with_contains(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE CHARINDEX('Jacinto', [c].[Location]) > 0");
        }

        public override void Non_unicode_string_literals_is_used_for_non_unicode_column_with_concat()
        {
            base.Non_unicode_string_literals_is_used_for_non_unicode_column_with_concat();

            AssertSql(
                @"SELECT [c].[Name], [c].[Location]
FROM [Cities] AS [c]
WHERE CHARINDEX('Add', [c].[Location] + 'Added') > 0");
        }

        public override void Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result1()
        {
            base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result1();

            // Issue#16897
            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[LeaderNickname] = [t].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [g0].[FullName] = [w].[OwnerFullName]
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g0].[Nickname], [g0].[SquadId], [w].[Id]");
        }

        public override void Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result2()
        {
            base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result2();

            // Issue#16897
            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[LeaderNickname] = [t].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g0].[Nickname], [g0].[SquadId], [w].[Id]");
        }

        public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result3(bool isAsync)
        {
            await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result3(isAsync);

            // Issue#16897
            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[LeaderNickname] = [t].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
LEFT JOIN [Weapons] AS [w0] ON [g0].[FullName] = [w0].[OwnerFullName]
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g0].[Nickname], [g0].[SquadId], [w].[Id], [w0].[Id]");
        }

        public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result4(bool isAsync)
        {
            await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result4(isAsync);

            // Issue#16897
            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g2].*
    FROM [Gears] AS [g2]
    WHERE [g2].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g].[LeaderNickname] = [t].[Nickname]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[FullName], [t].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT DISTINCT [g0].[FullName]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [g20].*
        FROM [Gears] AS [g20]
        WHERE [g20].[Discriminator] IN (N'Officer', N'Gear')
    ) AS [t0] ON [g0].[LeaderNickname] = [t0].[Nickname]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t1] ON [g.Weapons].[OwnerFullName] = [t1].[FullName]
ORDER BY [t1].[FullName]",
                //
                @"SELECT [g2.Weapons].[Id], [g2.Weapons].[AmmunitionType], [g2.Weapons].[IsAutomatic], [g2.Weapons].[Name], [g2.Weapons].[OwnerFullName], [g2.Weapons].[SynergyWithId]
FROM [Weapons] AS [g2.Weapons]
INNER JOIN (
    SELECT DISTINCT [t2].[FullName], [g1].[FullName] AS [FullName0]
    FROM [Gears] AS [g1]
    LEFT JOIN (
        SELECT [g21].*
        FROM [Gears] AS [g21]
        WHERE [g21].[Discriminator] IN (N'Officer', N'Gear')
    ) AS [t2] ON [g1].[LeaderNickname] = [t2].[Nickname]
    WHERE [g1].[Discriminator] IN (N'Officer', N'Gear')
) AS [t3] ON [g2.Weapons].[OwnerFullName] = [t3].[FullName]
ORDER BY [t3].[FullName0], [t3].[FullName]");
        }

        public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_inheritance_and_coalesce_result(bool isAsync)
        {
            await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_inheritance_and_coalesce_result(isAsync);

            // Issue#16897
            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
) AS [t] ON [g0].[LeaderNickname] = [t].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
LEFT JOIN [Weapons] AS [w0] ON [g0].[FullName] = [w0].[OwnerFullName]
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g0].[Nickname], [g0].[SquadId], [w].[Id], [w0].[Id]");
        }

        public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_conditional_result(bool isAsync)
        {
            await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_conditional_result(isAsync);

            // Issue#16897
            AssertSql(
                @"SELECT CASE
    WHEN [t].[Nickname] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Gears] AS [g0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [g0].[LeaderNickname] = [t].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
LEFT JOIN [Weapons] AS [w0] ON [g0].[FullName] = [w0].[OwnerFullName]
WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g0].[Nickname], [g0].[SquadId], [w].[Id], [w0].[Id]");
        }

        public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_complex_projection_result(bool isAsync)
        {
            await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_complex_projection_result(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g2].*
    FROM [Gears] AS [g2]
    WHERE [g2].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g].[LeaderNickname] = [t].[Nickname]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[FullName], [t].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT DISTINCT [g0].[FullName]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [g20].*
        FROM [Gears] AS [g20]
        WHERE [g20].[Discriminator] IN (N'Officer', N'Gear')
    ) AS [t0] ON [g0].[LeaderNickname] = [t0].[Nickname]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear') AND ([g0].[Nickname] IS NOT NULL AND [t0].[Nickname] IS NULL)
) AS [t1] ON [g.Weapons].[OwnerFullName] = [t1].[FullName]
ORDER BY [t1].[FullName]",
                //
                @"SELECT [g2.Weapons].[Id], [g2.Weapons].[AmmunitionType], [g2.Weapons].[IsAutomatic], [g2.Weapons].[Name], [g2.Weapons].[OwnerFullName], [g2.Weapons].[SynergyWithId]
FROM [Weapons] AS [g2.Weapons]
INNER JOIN (
    SELECT DISTINCT [t2].[FullName], [g1].[FullName] AS [FullName0]
    FROM [Gears] AS [g1]
    LEFT JOIN (
        SELECT [g21].*
        FROM [Gears] AS [g21]
        WHERE [g21].[Discriminator] IN (N'Officer', N'Gear')
    ) AS [t2] ON [g1].[LeaderNickname] = [t2].[Nickname]
    WHERE [g1].[Discriminator] IN (N'Officer', N'Gear') AND [t2].[Nickname] IS NOT NULL
) AS [t3] ON [g2.Weapons].[OwnerFullName] = [t3].[FullName]
ORDER BY [t3].[FullName0], [t3].[FullName]");
        }

        public override async Task Coalesce_operator_in_predicate(bool isAsync)
        {
            await base.Coalesce_operator_in_predicate(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE COALESCE([w].[IsAutomatic], CAST(0 AS bit)) = CAST(1 AS bit)");
        }

        public override async Task Coalesce_operator_in_predicate_with_other_conditions(bool isAsync)
        {
            await base.Coalesce_operator_in_predicate_with_other_conditions(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE (([w].[AmmunitionType] = 1) AND [w].[AmmunitionType] IS NOT NULL) AND (COALESCE([w].[IsAutomatic], CAST(0 AS bit)) = CAST(1 AS bit))");
        }

        public override async Task Coalesce_operator_in_projection_with_other_conditions(bool isAsync)
        {
            await base.Coalesce_operator_in_projection_with_other_conditions(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN (([w].[AmmunitionType] = 1) AND [w].[AmmunitionType] IS NOT NULL) AND (COALESCE([w].[IsAutomatic], CAST(0 AS bit)) = CAST(1 AS bit)) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [Weapons] AS [w]");
        }

        public override async Task Optional_navigation_type_compensation_works_with_predicate(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_predicate(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE (([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL) AND ([t0].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Optional_navigation_type_compensation_works_with_predicate2(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_predicate2(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [t0].[HasSoulPatch] = CAST(1 AS bit)");
        }

        public override async Task Optional_navigation_type_compensation_works_with_predicate_negated(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_predicate_negated(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [t0].[HasSoulPatch] <> CAST(1 AS bit)");
        }

        public override async Task Optional_navigation_type_compensation_works_with_predicate_negated_complex1(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_predicate_negated_complex1(isAsync);

            AssertSql(
                "");
        }

        public override async Task Optional_navigation_type_compensation_works_with_predicate_negated_complex2(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_predicate_negated_complex2(isAsync);

            AssertSql(
                "");
        }

        public override async Task Optional_navigation_type_compensation_works_with_conditional_expression(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_conditional_expression(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE CASE
    WHEN [t0].[HasSoulPatch] = CAST(1 AS bit) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END = CAST(1 AS bit)");
        }

        public override async Task Optional_navigation_type_compensation_works_with_binary_expression(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_binary_expression(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE ([t0].[HasSoulPatch] = CAST(1 AS bit)) OR (CHARINDEX(N'Cole', [t].[Note]) > 0)");
        }

        public override async Task Optional_navigation_type_compensation_works_with_binary_and_expression(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_binary_and_expression(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN ([t].[HasSoulPatch] = CAST(1 AS bit)) AND (CHARINDEX(N'Cole', [t0].[Note]) > 0) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)");
        }

        public override async Task Optional_navigation_type_compensation_works_with_projection(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_projection(isAsync);

            AssertSql(
                @"SELECT [t].[SquadId]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Note] <> N'K.I.A.') OR [t0].[Note] IS NULL");
        }

        public override async Task Optional_navigation_type_compensation_works_with_projection_into_anonymous_type(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_projection_into_anonymous_type(isAsync);

            AssertSql(
                @"SELECT [t].[SquadId]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Note] <> N'K.I.A.') OR [t0].[Note] IS NULL");
        }

        public override async Task Optional_navigation_type_compensation_works_with_DTOs(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_DTOs(isAsync);

            AssertSql(
                @"SELECT [t].[SquadId] AS [Id]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Note] <> N'K.I.A.') OR [t0].[Note] IS NULL");
        }

        public override async Task Optional_navigation_type_compensation_works_with_list_initializers(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_list_initializers(isAsync);

            AssertSql(
                @"SELECT [t].[SquadId], [t].[SquadId] + 1
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Note] <> N'K.I.A.') OR [t0].[Note] IS NULL
ORDER BY [t0].[Note]");
        }

        public override async Task Optional_navigation_type_compensation_works_with_array_initializers(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_array_initializers(isAsync);

            AssertSql(
                @"SELECT [t].[SquadId]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)
WHERE ([t0].[Note] <> N'K.I.A.') OR [t0].[Note] IS NULL");
        }

        public override async Task Optional_navigation_type_compensation_works_with_orderby(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_orderby(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE ([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL
ORDER BY [t0].[SquadId]");
        }

        public override async Task Optional_navigation_type_compensation_works_with_groupby(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_groupby(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[SquadId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [t.Gear].*
    FROM [Gears] AS [t.Gear]
    WHERE [t.Gear].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON ([t].[GearNickName] = [t0].[Nickname]) AND ([t].[GearSquadId] = [t0].[SquadId])
WHERE ([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL
ORDER BY [t0].[SquadId]");
        }

        public override async Task Optional_navigation_type_compensation_works_with_all(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_all(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN NOT EXISTS (
        SELECT 1
        FROM [Tags] AS [t]
        LEFT JOIN (
            SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            FROM [Gears] AS [g]
            WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
        WHERE (([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL) AND ([t0].[HasSoulPatch] <> CAST(1 AS bit))) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END");
        }

        public override async Task Optional_navigation_type_compensation_works_with_negated_predicate(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_negated_predicate(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE (([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL) AND ([t0].[HasSoulPatch] <> CAST(1 AS bit))");
        }

        public override async Task Optional_navigation_type_compensation_works_with_contains(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_contains(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE (([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL) AND [t0].[SquadId] IN (
    SELECT [g0].[SquadId]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
)");
        }

        public override async Task Optional_navigation_type_compensation_works_with_skip(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_skip(isAsync);

            AssertSql(
                @"SELECT [t0].[SquadId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [t.Gear].*
    FROM [Gears] AS [t.Gear]
    WHERE [t.Gear].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON ([t].[GearNickName] = [t0].[Nickname]) AND ([t].[GearSquadId] = [t0].[SquadId])
WHERE ([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL
ORDER BY [t].[Note]",
                //
                @"@_outer_SquadId='1'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]
OFFSET @_outer_SquadId ROWS",
                //
                @"@_outer_SquadId='1'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]
OFFSET @_outer_SquadId ROWS",
                //
                @"@_outer_SquadId='1'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]
OFFSET @_outer_SquadId ROWS",
                //
                @"@_outer_SquadId='1'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]
OFFSET @_outer_SquadId ROWS",
                //
                @"@_outer_SquadId='2'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]
OFFSET @_outer_SquadId ROWS");
        }

        public override async Task Optional_navigation_type_compensation_works_with_take(bool isAsync)
        {
            await base.Optional_navigation_type_compensation_works_with_take(isAsync);

            AssertSql(
                @"SELECT [t0].[SquadId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [t.Gear].*
    FROM [Gears] AS [t.Gear]
    WHERE [t.Gear].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON ([t].[GearNickName] = [t0].[Nickname]) AND ([t].[GearSquadId] = [t0].[SquadId])
WHERE ([t].[Note] <> N'K.I.A.') OR [t].[Note] IS NULL
ORDER BY [t].[Note]",
                //
                @"@_outer_SquadId='1'

SELECT TOP(@_outer_SquadId) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]",
                //
                @"@_outer_SquadId='1'

SELECT TOP(@_outer_SquadId) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]",
                //
                @"@_outer_SquadId='1'

SELECT TOP(@_outer_SquadId) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]",
                //
                @"@_outer_SquadId='1'

SELECT TOP(@_outer_SquadId) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]",
                //
                @"@_outer_SquadId='2'

SELECT TOP(@_outer_SquadId) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]");
        }

        public override async Task Select_correlated_filtered_collection(bool isAsync)
        {
            await base.Select_correlated_filtered_collection(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [c].[Name], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE ([w].[Name] <> N'Lancer') OR [w].[Name] IS NULL
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([c].[Name] = N'Ephyra') OR ([c].[Name] = N'Hanover'))
ORDER BY [g].[Nickname], [g].[SquadId], [c].[Name], [t].[Id]");
        }

        public override async Task Select_correlated_filtered_collection_with_composite_key(bool isAsync)
        {
            await base.Select_correlated_filtered_collection_with_composite_key(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[Nickname] <> N'Dom')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Select_correlated_filtered_collection_works_with_caching(bool isAsync)
        {
            await base.Select_correlated_filtered_collection_works_with_caching(isAsync);

            AssertSql(
                @"SELECT [t].[GearNickName]
FROM [Tags] AS [t]
ORDER BY [t].[Note]",
                //
                @"@_outer_GearNickName='Baird' (Size = 450)

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[Nickname] = @_outer_GearNickName)",
                //
                @"@_outer_GearNickName='Cole Train' (Size = 450)

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[Nickname] = @_outer_GearNickName)",
                //
                @"@_outer_GearNickName='Dom' (Size = 450)

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[Nickname] = @_outer_GearNickName)",
                //
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND [g].[Nickname] IS NULL",
                //
                @"@_outer_GearNickName='Marcus' (Size = 450)

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[Nickname] = @_outer_GearNickName)",
                //
                @"@_outer_GearNickName='Paduk' (Size = 450)

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[Nickname] = @_outer_GearNickName)");
        }

        public override async Task Join_predicate_value_equals_condition(bool isAsync)
        {
            await base.Join_predicate_value_equals_condition(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Weapons] AS [w] ON [w].[SynergyWithId] IS NOT NULL
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");


        }

        public override async Task Join_predicate_value(bool isAsync)
        {
            await base.Join_predicate_value(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Weapons] AS [w] ON [g].[HasSoulPatch] = CAST(1 AS bit)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Join_predicate_condition_equals_condition(bool isAsync)
        {
            await base.Join_predicate_condition_equals_condition(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
INNER JOIN [Weapons] AS [w] ON CAST(1 AS bit) = CASE
    WHEN [w].[SynergyWithId] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Left_join_predicate_value_equals_condition(bool isAsync)
        {
            await base.Left_join_predicate_value_equals_condition(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [w].[SynergyWithId] IS NOT NULL
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Left_join_predicate_value(bool isAsync)
        {
            await base.Left_join_predicate_value(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[HasSoulPatch] = CAST(1 AS bit)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Left_join_predicate_condition_equals_condition(bool isAsync)
        {
            await base.Left_join_predicate_condition_equals_condition(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON CAST(1 AS bit) = CASE
    WHEN [w].[SynergyWithId] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Where_datetimeoffset_now(bool isAsync)
        {
            await base.Where_datetimeoffset_now(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE [m].[Timeline] <> SYSDATETIMEOFFSET()");
        }

        public override async Task Where_datetimeoffset_utcnow(bool isAsync)
        {
            await base.Where_datetimeoffset_utcnow(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE [m].[Timeline] <> CAST(SYSUTCDATETIME() AS datetimeoffset)");
        }

        public override async Task Where_datetimeoffset_date_component(bool isAsync)
        {
            await base.Where_datetimeoffset_date_component(isAsync);

            // issue #16057
            //            AssertSql(
            //                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
            //FROM [Missions] AS [m]
            //WHERE CONVERT(date, [m].[Timeline]) > '0001-01-01T00:00:00.0000000-08:00'");
        }

        public override async Task Where_datetimeoffset_year_component(bool isAsync)
        {
            await base.Where_datetimeoffset_year_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(year, [m].[Timeline]) = 2");
        }

        public override async Task Where_datetimeoffset_month_component(bool isAsync)
        {
            await base.Where_datetimeoffset_month_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(month, [m].[Timeline]) = 1");
        }

        public override async Task Where_datetimeoffset_dayofyear_component(bool isAsync)
        {
            await base.Where_datetimeoffset_dayofyear_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(dayofyear, [m].[Timeline]) = 2");
        }

        public override async Task Where_datetimeoffset_day_component(bool isAsync)
        {
            await base.Where_datetimeoffset_day_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(day, [m].[Timeline]) = 2");
        }

        public override async Task Where_datetimeoffset_hour_component(bool isAsync)
        {
            await base.Where_datetimeoffset_hour_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(hour, [m].[Timeline]) = 10");
        }

        public override async Task Where_datetimeoffset_minute_component(bool isAsync)
        {
            await base.Where_datetimeoffset_minute_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(minute, [m].[Timeline]) = 0");
        }

        public override async Task Where_datetimeoffset_second_component(bool isAsync)
        {
            await base.Where_datetimeoffset_second_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(second, [m].[Timeline]) = 0");
        }

        public override async Task Where_datetimeoffset_millisecond_component(bool isAsync)
        {
            await base.Where_datetimeoffset_millisecond_component(isAsync);

            AssertSql(
                @"SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE DATEPART(millisecond, [m].[Timeline]) = 0");
        }

        public override async Task DateTimeOffset_DateAdd_AddMonths(bool isAsync)
        {
            await base.DateTimeOffset_DateAdd_AddMonths(isAsync);

            AssertSql(
                @"SELECT DATEADD(month, CAST(1 AS int), [m].[Timeline])
FROM [Missions] AS [m]");
        }

        public override async Task DateTimeOffset_DateAdd_AddDays(bool isAsync)
        {
            await base.DateTimeOffset_DateAdd_AddDays(isAsync);

            AssertSql(
                @"SELECT DATEADD(day, CAST(1.0E0 AS int), [m].[Timeline])
FROM [Missions] AS [m]");
        }

        public override async Task DateTimeOffset_DateAdd_AddHours(bool isAsync)
        {
            await base.DateTimeOffset_DateAdd_AddHours(isAsync);

            AssertSql(
                @"SELECT DATEADD(hour, CAST(1.0E0 AS int), [m].[Timeline])
FROM [Missions] AS [m]");
        }

        public override async Task DateTimeOffset_DateAdd_AddMinutes(bool isAsync)
        {
            await base.DateTimeOffset_DateAdd_AddMinutes(isAsync);

            AssertSql(
                @"SELECT DATEADD(minute, CAST(1.0E0 AS int), [m].[Timeline])
FROM [Missions] AS [m]");
        }

        public override async Task DateTimeOffset_DateAdd_AddSeconds(bool isAsync)
        {
            await base.DateTimeOffset_DateAdd_AddSeconds(isAsync);

            AssertSql(
                @"SELECT DATEADD(second, CAST(1.0E0 AS int), [m].[Timeline])
FROM [Missions] AS [m]");
        }

        public override async Task DateTimeOffset_DateAdd_AddMilliseconds(bool isAsync)
        {
            await base.DateTimeOffset_DateAdd_AddMilliseconds(isAsync);

            AssertSql(
                @"SELECT DATEADD(millisecond, CAST(300.0E0 AS int), [m].[Timeline])
FROM [Missions] AS [m]");
        }

        public override void Where_datetimeoffset_milliseconds_parameter_and_constant()
        {
            base.Where_datetimeoffset_milliseconds_parameter_and_constant();

            AssertSql(
                @"@__dateTimeOffset_0='1902-01-02T10:00:00.1234567+01:30'

SELECT COUNT(*)
FROM [Missions] AS [m]
WHERE ([m].[Timeline] = @__dateTimeOffset_0) AND @__dateTimeOffset_0 IS NOT NULL",
                //
                @"SELECT COUNT(*)
FROM [Missions] AS [m]
WHERE [m].[Timeline] = '1902-01-02T10:00:00.1234567+01:30'");
        }

        public override async Task Orderby_added_for_client_side_GroupJoin_composite_dependent_to_principal_LOJ_when_incomplete_key_is_used(
            bool isAsync)
        {
            await base.Orderby_added_for_client_side_GroupJoin_composite_dependent_to_principal_LOJ_when_incomplete_key_is_used(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].*
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON [t].[GearNickName] = [t0].[Nickname]
ORDER BY [t].[GearNickName]");
        }

        public override async Task Complex_predicate_with_AndAlso_and_nullable_bool_property(bool isAsync)
        {
            await base.Complex_predicate_with_AndAlso_and_nullable_bool_property(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
WHERE ([w].[Id] <> 50) AND ([t].[HasSoulPatch] <> CAST(1 AS bit))");
        }

        public override async Task Distinct_with_optional_navigation_is_translated_to_sql(bool isAsync)
        {
            await base.Distinct_with_optional_navigation_is_translated_to_sql(isAsync);

            AssertSql(
                @"SELECT DISTINCT [g].[HasSoulPatch]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([t].[Note] <> N'Foo') OR [t].[Note] IS NULL)");
        }

        public override async Task Sum_with_optional_navigation_is_translated_to_sql(bool isAsync)
        {
            await base.Sum_with_optional_navigation_is_translated_to_sql(isAsync);

            AssertSql(
                @"SELECT SUM([g].[SquadId])
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([t].[Note] <> N'Foo') OR [t].[Note] IS NULL)");
        }

        public override async Task Count_with_optional_navigation_is_translated_to_sql(bool isAsync)
        {
            await base.Count_with_optional_navigation_is_translated_to_sql(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([t].[Note] <> N'Foo') OR [t].[Note] IS NULL)");
        }

        public override async Task Count_with_unflattened_groupjoin_is_evaluated_on_client(bool isAsync)
        {
            await base.Count_with_unflattened_groupjoin_is_evaluated_on_client(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON ([g].[Nickname] = [t].[GearNickName]) AND ([g].[SquadId] = [t].[GearSquadId])
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname], [g].[SquadId]");
        }

        public override async Task Distinct_with_unflattened_groupjoin_is_evaluated_on_client(bool isAsync)
        {
            await base.Distinct_with_unflattened_groupjoin_is_evaluated_on_client(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON ([g].[Nickname] = [t].[GearNickName]) AND ([g].[SquadId] = [t].[GearSquadId])
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname], [g].[SquadId]");
        }

        public override async Task FirstOrDefault_with_manually_created_groupjoin_is_translated_to_sql(bool isAsync)
        {
            await base.FirstOrDefault_with_manually_created_groupjoin_is_translated_to_sql(isAsync);

            AssertSql(
                @"SELECT TOP(1) [s].[Id], [s].[InternalNumber], [s].[Name]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [s].[Id] = [t].[SquadId]
WHERE ([s].[Name] = N'Kilo') AND [s].[Name] IS NOT NULL");
        }

        public override async Task Any_with_optional_navigation_as_subquery_predicate_is_translated_to_sql(bool isAsync)
        {
            await base.Any_with_optional_navigation_as_subquery_predicate_is_translated_to_sql(isAsync);

            AssertSql(
                @"SELECT [s].[Name]
FROM [Squads] AS [s]
WHERE NOT (EXISTS (
    SELECT 1
    FROM [Gears] AS [g]
    LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
    WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([s].[Id] = [g].[SquadId])) AND (([t].[Note] = N'Dom''s Tag') AND [t].[Note] IS NOT NULL)))");
        }

        public override async Task All_with_optional_navigation_is_translated_to_sql(bool isAsync)
        {
            await base.All_with_optional_navigation_is_translated_to_sql(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN NOT EXISTS (
        SELECT 1
        FROM [Gears] AS [g]
        LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
        WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([t].[Note] = N'Foo') AND [t].[Note] IS NOT NULL)) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END");
        }

        public override async Task Non_flattened_GroupJoin_with_result_operator_evaluates_on_the_client(bool isAsync)
        {
            await base.Non_flattened_GroupJoin_with_result_operator_evaluates_on_the_client(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].*
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON ([t].[GearNickName] = [t0].[Nickname]) AND ([t].[GearSquadId] = [t0].[SquadId])
ORDER BY [t].[GearNickName], [t].[GearSquadId]");
        }

        public override async Task Contains_with_local_nullable_guid_list_closure(bool isAsync)
        {
            await base.Contains_with_local_nullable_guid_list_closure(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
WHERE [t].[Id] IN ('d2c26679-562b-44d1-ab96-23d1775e0926', '23cbcf9b-ce14-45cf-aafa-2c2667ebfdd3', 'ab1b82d7-88db-42bd-a132-7eef9aa68af4')");
        }

        public override void Unnecessary_include_doesnt_get_added_complex_when_projecting_EF_Property()
        {
            base.Unnecessary_include_doesnt_get_added_complex_when_projecting_EF_Property();

            AssertSql(
                @"SELECT [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))
ORDER BY [g].[Rank]");
        }

        public override void Multiple_order_bys_are_properly_lifted_from_subquery_created_by_include()
        {
            base.Multiple_order_bys_are_properly_lifted_from_subquery_created_by_include();

            AssertSql(
                @"SELECT [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
ORDER BY [g].[FullName]");
        }

        public override void Order_by_is_properly_lifted_from_subquery_with_same_order_by_in_the_outer_query()
        {
            base.Order_by_is_properly_lifted_from_subquery_with_same_order_by_in_the_outer_query();

            AssertSql(
                @"SELECT [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
ORDER BY [g].[FullName]");
        }

        public override void Where_is_properly_lifted_from_subquery_created_by_include()
        {
            base.Where_is_properly_lifted_from_subquery_created_by_include();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[FullName] <> N'Augustus Cole')) AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
ORDER BY [g].[FullName]");
        }

        public override void Subquery_is_lifted_from_main_from_clause_of_SelectMany()
        {
            base.Subquery_is_lifted_from_main_from_clause_of_SelectMany();

            // issue #16081
            //            AssertSql(
            //                @"SELECT [g].[FullName] AS [Name1], [g2].[FullName] AS [Name2]
            //FROM [Gears] AS [g]
            //CROSS JOIN [Gears] AS [g2]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (([g].[HasSoulPatch] = CAST(1 AS bit)) AND ([g2].[HasSoulPatch] = CAST(0 AS bit)))
            //ORDER BY [Name1], [g].[Rank]");
        }

        public override async Task Subquery_containing_SelectMany_projecting_main_from_clause_gets_lifted(bool isAsync)
        {
            await base.Subquery_containing_SelectMany_projecting_main_from_clause_gets_lifted(isAsync);

            // issue #16081
            //            AssertSql(
            //                @"SELECT [gear].[FullName]
            //FROM [Gears] AS [gear]
            //CROSS JOIN [Tags] AS [tag]
            //WHERE [gear].[Discriminator] IN (N'Officer', N'Gear') AND ([gear].[HasSoulPatch] = CAST(1 AS bit))
            //ORDER BY [gear].[FullName], [tag].[Note]");
        }

        public override async Task Subquery_containing_join_projecting_main_from_clause_gets_lifted(bool isAsync)
        {
            await base.Subquery_containing_join_projecting_main_from_clause_gets_lifted(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t] ON [g].[Nickname] = [t].[GearNickName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname]");
        }

        public override async Task Subquery_containing_left_join_projecting_main_from_clause_gets_lifted(bool isAsync)
        {
            await base.Subquery_containing_left_join_projecting_main_from_clause_gets_lifted(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON [g].[Nickname] = [t].[GearNickName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname]");
        }

        public override async Task Subquery_containing_join_gets_lifted_clashing_names(bool isAsync)
        {
            await base.Subquery_containing_join_gets_lifted_clashing_names(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname]
FROM [Gears] AS [g]
INNER JOIN [Tags] AS [t] ON [g].[Nickname] = [t].[GearNickName]
INNER JOIN [Tags] AS [t0] ON [g].[Nickname] = [t0].[GearNickName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([t].[GearNickName] <> N'Cole Train') OR [t].[GearNickName] IS NULL)
ORDER BY [g].[Nickname], [t0].[Id]");
        }

        public override void Subquery_created_by_include_gets_lifted_nested()
        {
            base.Subquery_created_by_include_gets_lifted_nested();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [c].[Name], [c].[Location], [c].[Nation]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND EXISTS (
    SELECT 1
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL)) AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
ORDER BY [g].[Nickname]");
        }

        public override async Task Subquery_is_lifted_from_additional_from_clause(bool isAsync)
        {
            await base.Subquery_is_lifted_from_additional_from_clause(isAsync);

            // issue #16081
            //            AssertSql(
            //                @"SELECT [g1].[FullName] AS [Name1], [t].[FullName] AS [Name2]
            //FROM [Gears] AS [g1]
            //CROSS JOIN (
            //    SELECT [g].*
            //    FROM [Gears] AS [g]
            //    WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
            //    ORDER BY [g].[Rank]
            //    OFFSET 0 ROWS
            //) AS [t]
            //WHERE [g1].[Discriminator] IN (N'Officer', N'Gear') AND (([g1].[HasSoulPatch] = CAST(1 AS bit)) AND ([t].[HasSoulPatch] = CAST(0 AS bit)))
            //ORDER BY [Name1]");
        }

        public override async Task Subquery_with_result_operator_is_not_lifted(bool isAsync)
        {
            await base.Subquery_with_result_operator_is_not_lifted(isAsync);

            AssertSql(
                @"@__p_0='2'

SELECT [t].[FullName]
FROM (
    SELECT TOP(@__p_0) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
    ORDER BY [g].[FullName]
) AS [t]
ORDER BY [t].[Rank]");
        }

        public override async Task Skip_with_orderby_followed_by_orderBy_is_pushed_down(bool isAsync)
        {
            await base.Skip_with_orderby_followed_by_orderBy_is_pushed_down(isAsync);

            AssertSql(
                @"@__p_0='1'

SELECT [t].[FullName]
FROM (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
    ORDER BY [g].[FullName]
    OFFSET @__p_0 ROWS
) AS [t]
ORDER BY [t].[Rank]");
        }

        public override async Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool isAsync)
        {
            await base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(isAsync);

            AssertSql(
                @"@__p_0='999'

SELECT [t].[FullName]
FROM (
    SELECT TOP(@__p_0) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t]
ORDER BY [t].[Rank]");
        }

        public override async Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool isAsync)
        {
            await base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(isAsync);

            AssertSql(
                @"@__p_0='999'

SELECT [t].[FullName]
FROM (
    SELECT TOP(@__p_0) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t]
ORDER BY [t].[Rank]");
        }

        public override async Task Take_without_orderby_followed_by_orderBy_is_pushed_down3(bool isAsync)
        {
            await base.Take_without_orderby_followed_by_orderBy_is_pushed_down3(isAsync);

            AssertSql(
                @"@__p_0='999'

SELECT [t].[FullName]
FROM (
    SELECT TOP(@__p_0) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t]
ORDER BY [t].[FullName], [t].[Rank]");
        }

        public override async Task Select_length_of_string_property(bool isAsync)
        {
            await base.Select_length_of_string_property(isAsync);

            AssertSql(
                @"SELECT [w].[Name], CAST(LEN([w].[Name]) AS int) AS [Length]
FROM [Weapons] AS [w]");
        }

        public override async Task Client_method_on_collection_navigation_in_outer_join_key(bool isAsync)
        {
            await base.Client_method_on_collection_navigation_in_outer_join_key(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')",
                //
                @"@_outer_FullName1='Damon Baird' (Size = 450)

SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Weapons] AS [w0]
WHERE @_outer_FullName1 = [w0].[OwnerFullName]",
                //
                @"@_outer_FullName1='Augustus Cole' (Size = 450)

SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Weapons] AS [w0]
WHERE @_outer_FullName1 = [w0].[OwnerFullName]",
                //
                @"@_outer_FullName1='Dominic Santiago' (Size = 450)

SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Weapons] AS [w0]
WHERE @_outer_FullName1 = [w0].[OwnerFullName]",
                //
                @"@_outer_FullName1='Marcus Fenix' (Size = 450)

SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Weapons] AS [w0]
WHERE @_outer_FullName1 = [w0].[OwnerFullName]",
                //
                @"@_outer_FullName1='Garron Paduk' (Size = 450)

SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
FROM [Weapons] AS [w0]
WHERE @_outer_FullName1 = [w0].[OwnerFullName]",
                //
                @"SELECT [o].[FullName], [o].[Nickname] AS [o]
FROM [Gears] AS [o]
WHERE ([o].[Discriminator] = N'Officer') AND ([o].[HasSoulPatch] = 1)",
                //
                @"@_outer_FullName='Damon Baird' (Size = 450)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE @_outer_FullName = [w].[OwnerFullName]",
                //
                @"@_outer_FullName='Marcus Fenix' (Size = 450)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE @_outer_FullName = [w].[OwnerFullName]");
        }

        public override void Member_access_on_derived_entity_using_cast()
        {
            base.Member_access_on_derived_entity_using_cast();

            AssertSql(
                @"SELECT [f].[Name], [f].[Eradicated]
FROM [Factions] AS [f]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Member_access_on_derived_materialized_entity_using_cast()
        {
            base.Member_access_on_derived_materialized_entity_using_cast();

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated]
FROM [Factions] AS [f]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Member_access_on_derived_entity_using_cast_and_let()
        {
            base.Member_access_on_derived_entity_using_cast_and_let();

            AssertSql(
                @"SELECT [f].[Name], [f].[Eradicated]
FROM [Factions] AS [f]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Property_access_on_derived_entity_using_cast()
        {
            base.Property_access_on_derived_entity_using_cast();

            AssertSql(
                @"SELECT [f].[Name], [f].[Eradicated]
FROM [Factions] AS [f]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Navigation_access_on_derived_entity_using_cast()
        {
            base.Navigation_access_on_derived_entity_using_cast();

            // issue #15944
            //            AssertSql(
            //                @"SELECT [f].[Name], [t].[ThreatLevel] AS [Threat]
            //FROM [ConditionalFactions] AS [f]
            //LEFT JOIN (
            //    SELECT [f.Commander].*
            //    FROM [LocustLeaders] AS [f.Commander]
            //    WHERE [f.Commander].[Discriminator] = N'LocustCommander'
            //) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
            //WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
            //ORDER BY [f].[Name]");
        }

        public override void Navigation_access_on_derived_materialized_entity_using_cast()
        {
            base.Navigation_access_on_derived_materialized_entity_using_cast();

            // issue #15944
            //            AssertSql(
            //                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t].[ThreatLevel]
            //FROM [ConditionalFactions] AS [f]
            //LEFT JOIN (
            //    SELECT [f.Commander].*
            //    FROM [LocustLeaders] AS [f.Commander]
            //    WHERE [f.Commander].[Discriminator] = N'LocustCommander'
            //) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
            //WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
            //ORDER BY [f].[Name]");
        }

        public override void Navigation_access_via_EFProperty_on_derived_entity_using_cast()
        {
            base.Navigation_access_via_EFProperty_on_derived_entity_using_cast();

            AssertSql(
                @"SELECT [f].[Name], [t].[ThreatLevel] AS [Threat]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t] ON [f].[CommanderName] = [t].[Name]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Navigation_access_fk_on_derived_entity_using_cast()
        {
            base.Navigation_access_fk_on_derived_entity_using_cast();

            AssertSql(
                @"SELECT [f].[Name], [t].[Name] AS [CommanderName]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t] ON [f].[CommanderName] = [t].[Name]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Collection_navigation_access_on_derived_entity_using_cast()
        {
            base.Collection_navigation_access_on_derived_entity_using_cast();

            AssertSql(
                @"SELECT [f].[Name], (
    SELECT COUNT(*)
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander') AND (([f].[Id] = [l].[LocustHordeId]) AND [l].[LocustHordeId] IS NOT NULL)) AS [LeadersCount]
FROM [Factions] AS [f]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name]");
        }

        public override void Collection_navigation_access_on_derived_entity_using_cast_in_SelectMany()
        {
            base.Collection_navigation_access_on_derived_entity_using_cast_in_SelectMany();

            AssertSql(
                @"SELECT [f].[Name], [t].[Name] AS [LeaderName]
FROM [Factions] AS [f]
INNER JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')
) AS [t] ON [f].[Id] = [t].[LocustHordeId]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [t].[Name]");
        }

        public override void Include_on_derived_entity_using_OfType()
        {
            base.Include_on_derived_entity_using_OfType();

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId], [t0].[Name], [t0].[Discriminator], [t0].[LocustHordeId], [t0].[ThreatLevel], [t0].[DefeatedByNickname], [t0].[DefeatedBySquadId], [t0].[HighCommandId]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t] ON [f].[CommanderName] = [t].[Name]
LEFT JOIN (
    SELECT [l0].[Name], [l0].[Discriminator], [l0].[LocustHordeId], [l0].[ThreatLevel], [l0].[DefeatedByNickname], [l0].[DefeatedBySquadId], [l0].[HighCommandId]
    FROM [LocustLeaders] AS [l0]
    WHERE [l0].[Discriminator] IN (N'LocustLeader', N'LocustCommander')
) AS [t0] ON [f].[Id] = [t0].[LocustHordeId]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name], [f].[Id], [t0].[Name]");
        }

        public override void Include_on_derived_entity_using_subquery_with_cast()
        {
            base.Include_on_derived_entity_using_subquery_with_cast();

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId]
FROM [ConditionalFactions] AS [f]
LEFT JOIN (
    SELECT [f.Commander].*
    FROM [LocustLeaders] AS [f.Commander]
    WHERE [f.Commander].[Discriminator] = N'LocustCommander'
) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name], [f].[Id]",
                //
                @"SELECT [f.Leaders].[Name], [f.Leaders].[Discriminator], [f.Leaders].[LocustHordeId], [f.Leaders].[ThreatLevel], [f.Leaders].[DefeatedByNickname], [f.Leaders].[DefeatedBySquadId], [f.Leaders].[HighCommandId]
FROM [LocustLeaders] AS [f.Leaders]
INNER JOIN (
    SELECT DISTINCT [f0].[Id], [f0].[Name]
    FROM [ConditionalFactions] AS [f0]
    LEFT JOIN (
        SELECT [f.Commander0].*
        FROM [LocustLeaders] AS [f.Commander0]
        WHERE [f.Commander0].[Discriminator] = N'LocustCommander'
    ) AS [t0] ON ([f0].[Discriminator] = N'LocustHorde') AND ([f0].[CommanderName] = [t0].[Name])
    WHERE ([f0].[Discriminator] = N'LocustHorde') AND ([f0].[Discriminator] = N'LocustHorde')
) AS [t1] ON [f.Leaders].[LocustHordeId] = [t1].[Id]
WHERE [f.Leaders].[Discriminator] IN (N'LocustCommander', N'LocustLeader')
ORDER BY [t1].[Name], [t1].[Id]");
        }

        public override void Include_on_derived_entity_using_subquery_with_cast_AsNoTracking()
        {
            base.Include_on_derived_entity_using_subquery_with_cast_AsNoTracking();

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId]
FROM [ConditionalFactions] AS [f]
LEFT JOIN (
    SELECT [f.Commander].*
    FROM [LocustLeaders] AS [f.Commander]
    WHERE [f.Commander].[Discriminator] = N'LocustCommander'
) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Name], [f].[Id]",
                //
                @"SELECT [f.Leaders].[Name], [f.Leaders].[Discriminator], [f.Leaders].[LocustHordeId], [f.Leaders].[ThreatLevel], [f.Leaders].[DefeatedByNickname], [f.Leaders].[DefeatedBySquadId], [f.Leaders].[HighCommandId]
FROM [LocustLeaders] AS [f.Leaders]
INNER JOIN (
    SELECT DISTINCT [f0].[Id], [f0].[Name]
    FROM [ConditionalFactions] AS [f0]
    LEFT JOIN (
        SELECT [f.Commander0].*
        FROM [LocustLeaders] AS [f.Commander0]
        WHERE [f.Commander0].[Discriminator] = N'LocustCommander'
    ) AS [t0] ON ([f0].[Discriminator] = N'LocustHorde') AND ([f0].[CommanderName] = [t0].[Name])
    WHERE ([f0].[Discriminator] = N'LocustHorde') AND ([f0].[Discriminator] = N'LocustHorde')
) AS [t1] ON [f.Leaders].[LocustHordeId] = [t1].[Id]
WHERE [f.Leaders].[Discriminator] IN (N'LocustCommander', N'LocustLeader')
ORDER BY [t1].[Name], [t1].[Id]");
        }

        public override void Include_on_derived_entity_using_subquery_with_cast_cross_product_base_entity()
        {
            base.Include_on_derived_entity_using_subquery_with_cast_cross_product_base_entity();

            AssertSql(
                @"SELECT [f2].[Id], [f2].[CapitalName], [f2].[Discriminator], [f2].[Name], [f2].[CommanderName], [f2].[Eradicated], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId], [ff].[Id], [ff].[CapitalName], [ff].[Discriminator], [ff].[Name], [ff].[CommanderName], [ff].[Eradicated], [ff.Capital].[Name], [ff.Capital].[Location], [ff.Capital].[Nation]
FROM [ConditionalFactions] AS [f2]
LEFT JOIN (
    SELECT [f2.Commander].*
    FROM [LocustLeaders] AS [f2.Commander]
    WHERE [f2.Commander].[Discriminator] = N'LocustCommander'
) AS [t] ON ([f2].[Discriminator] = N'LocustHorde') AND ([f2].[CommanderName] = [t].[Name])
CROSS JOIN [ConditionalFactions] AS [ff]
LEFT JOIN [Cities] AS [ff.Capital] ON [ff].[CapitalName] = [ff.Capital].[Name]
WHERE ([f2].[Discriminator] = N'LocustHorde') AND ([f2].[Discriminator] = N'LocustHorde')
ORDER BY [f2].[Name], [ff].[Name], [f2].[Id]",
                //
                @"SELECT [f2.Leaders].[Name], [f2.Leaders].[Discriminator], [f2.Leaders].[LocustHordeId], [f2.Leaders].[ThreatLevel], [f2.Leaders].[DefeatedByNickname], [f2.Leaders].[DefeatedBySquadId], [f2.Leaders].[HighCommandId]
FROM [LocustLeaders] AS [f2.Leaders]
INNER JOIN (
    SELECT DISTINCT [f20].[Id], [f20].[Name], [ff0].[Name] AS [Name0]
    FROM [ConditionalFactions] AS [f20]
    LEFT JOIN (
        SELECT [f2.Commander0].*
        FROM [LocustLeaders] AS [f2.Commander0]
        WHERE [f2.Commander0].[Discriminator] = N'LocustCommander'
    ) AS [t0] ON ([f20].[Discriminator] = N'LocustHorde') AND ([f20].[CommanderName] = [t0].[Name])
    CROSS JOIN [ConditionalFactions] AS [ff0]
    LEFT JOIN [Cities] AS [ff.Capital0] ON [ff0].[CapitalName] = [ff.Capital0].[Name]
    WHERE ([f20].[Discriminator] = N'LocustHorde') AND ([f20].[Discriminator] = N'LocustHorde')
) AS [t1] ON [f2.Leaders].[LocustHordeId] = [t1].[Id]
WHERE [f2.Leaders].[Discriminator] IN (N'LocustCommander', N'LocustLeader')
ORDER BY [t1].[Name], [t1].[Name0], [t1].[Id]");
        }

        public override void Distinct_on_subquery_doesnt_get_lifted()
        {
            base.Distinct_on_subquery_doesnt_get_lifted();

            AssertSql(
                @"SELECT [t].[HasSoulPatch]
FROM (
    SELECT DISTINCT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t]");
        }

        public override void Cast_result_operator_on_subquery_is_properly_lifted_to_a_convert()
        {
            base.Cast_result_operator_on_subquery_is_properly_lifted_to_a_convert();

            AssertSql(
                @"SELECT [f].[Eradicated]
FROM [Factions] AS [f]
WHERE [f].[Discriminator] = N'LocustHorde'");
        }

        public override void Comparing_two_collection_navigations_composite_key()
        {
            base.Comparing_two_collection_navigations_composite_key();

            AssertSql(
                @"SELECT [g].[Nickname] AS [Nickname1], [t].[Nickname] AS [Nickname2]
FROM [Gears] AS [g]
CROSS JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[Nickname] = [t].[Nickname]) AND ([g].[SquadId] = [t].[SquadId]))
ORDER BY [g].[Nickname]");
        }

        public override void Comparing_two_collection_navigations_inheritance()
        {
            base.Comparing_two_collection_navigations_inheritance();

            AssertSql(
                @"SELECT [f].[Name], [t].[Nickname]
FROM [Factions] AS [f]
CROSS JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
) AS [t]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t0] ON [f].[CommanderName] = [t0].[Name]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t0].[DefeatedByNickname] = [t1].[Nickname]) AND [t0].[DefeatedByNickname] IS NOT NULL) AND (([t0].[DefeatedBySquadId] = [t1].[SquadId]) AND [t0].[DefeatedBySquadId] IS NOT NULL)
WHERE (([f].[Discriminator] = N'LocustHorde') AND (([f].[Discriminator] = N'LocustHorde') AND ([t].[HasSoulPatch] = CAST(1 AS bit)))) AND ((([t1].[Nickname] = [t].[Nickname]) AND [t1].[Nickname] IS NOT NULL) AND (([t1].[SquadId] = [t].[SquadId]) AND [t1].[SquadId] IS NOT NULL))");
        }

        public override void Comparing_entities_using_Equals_inheritance()
        {
            base.Comparing_entities_using_Equals_inheritance();

            AssertSql(
                "");
        }

        public override void Contains_on_nullable_array_produces_correct_sql()
        {
            base.Contains_on_nullable_array_produces_correct_sql();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
LEFT JOIN [Cities] AS [c] ON [g].[AssignedCityName] = [c].[Name]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[SquadId] < 2) AND ([c].[Name] IN (N'Ephyra') OR [c].[Name] IS NULL))");
        }

        public override void Optional_navigation_with_collection_composite_key()
        {
            base.Optional_navigation_with_collection_composite_key();

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE (([t0].[Discriminator] = N'Officer') AND [t0].[Discriminator] IS NOT NULL) AND ((
    SELECT COUNT(*)
    FROM [Gears] AS [g0]
    WHERE ([g0].[Discriminator] IN (N'Gear', N'Officer') AND (((([t0].[Nickname] = [g0].[LeaderNickname]) AND ([t0].[Nickname] IS NOT NULL AND [g0].[LeaderNickname] IS NOT NULL)) OR ([t0].[Nickname] IS NULL AND [g0].[LeaderNickname] IS NULL)) AND (([t0].[SquadId] = [g0].[LeaderSquadId]) AND [t0].[SquadId] IS NOT NULL))) AND ([g0].[Nickname] = N'Dom')) > 0)");
        }

        public override void Select_null_conditional_with_inheritance()
        {
            base.Select_null_conditional_with_inheritance();

            //            AssertSql(
            //                @"SELECT [f].[CommanderName]
            //FROM [ConditionalFactions] AS [f]
            //WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')");
        }

        public override void Select_null_conditional_with_inheritance_negative()
        {
            base.Select_null_conditional_with_inheritance_negative();

            AssertSql(
                @"SELECT CASE
    WHEN [f].[CommanderName] IS NOT NULL THEN [f].[Eradicated]
    ELSE NULL
END
FROM [Factions] AS [f]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')");
        }

        public override void Project_collection_navigation_with_inheritance1()
        {
            base.Project_collection_navigation_with_inheritance1();

            AssertSql(
                @"SELECT [f].[Id], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l0].[Name], [l0].[Discriminator], [l0].[LocustHordeId], [l0].[ThreatLevel], [l0].[DefeatedByNickname], [l0].[DefeatedBySquadId], [l0].[HighCommandId]
    FROM [LocustLeaders] AS [l0]
    WHERE [l0].[Discriminator] = N'LocustCommander'
) AS [t0] ON [f].[CommanderName] = [t0].[Name]
LEFT JOIN (
    SELECT [f0].[Id], [f0].[CapitalName], [f0].[Discriminator], [f0].[Name], [f0].[CommanderName], [f0].[Eradicated]
    FROM [Factions] AS [f0]
    WHERE [f0].[Discriminator] = N'LocustHorde'
) AS [t1] ON [t0].[Name] = [t1].[CommanderName]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')
) AS [t] ON [t1].[Id] = [t].[LocustHordeId]
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Id], [t].[Name]");
        }

        public override void Project_collection_navigation_with_inheritance2()
        {
            base.Project_collection_navigation_with_inheritance2();

            AssertSql(
                @"SELECT [f].[Id], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t0] ON [f].[CommanderName] = [t0].[Name]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t0].[DefeatedByNickname] = [t1].[Nickname]) AND [t0].[DefeatedByNickname] IS NOT NULL) AND (([t0].[DefeatedBySquadId] = [t1].[SquadId]) AND [t0].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON ((([t1].[Nickname] = [t].[LeaderNickname]) AND ([t1].[Nickname] IS NOT NULL AND [t].[LeaderNickname] IS NOT NULL)) OR ([t1].[Nickname] IS NULL AND [t].[LeaderNickname] IS NULL)) AND (([t1].[SquadId] = [t].[LeaderSquadId]) AND [t1].[SquadId] IS NOT NULL)
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Id], [t].[Nickname], [t].[SquadId]");
        }

        public override void Project_collection_navigation_with_inheritance3()
        {
            base.Project_collection_navigation_with_inheritance3();

            AssertSql(
                @"SELECT [f].[Id], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t0] ON [f].[CommanderName] = [t0].[Name]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON (([t0].[DefeatedByNickname] = [t1].[Nickname]) AND [t0].[DefeatedByNickname] IS NOT NULL) AND (([t0].[DefeatedBySquadId] = [t1].[SquadId]) AND [t0].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON ((([t1].[Nickname] = [t].[LeaderNickname]) AND ([t1].[Nickname] IS NOT NULL AND [t].[LeaderNickname] IS NOT NULL)) OR ([t1].[Nickname] IS NULL AND [t].[LeaderNickname] IS NULL)) AND (([t1].[SquadId] = [t].[LeaderSquadId]) AND [t1].[SquadId] IS NOT NULL)
WHERE ([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')
ORDER BY [f].[Id], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_reference_on_derived_type_using_string(bool isAsync)
        {
            await base.Include_reference_on_derived_type_using_string(isAsync);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')");
        }

        public override async Task Include_reference_on_derived_type_using_string_nested1(bool isAsync)
        {
            await base.Include_reference_on_derived_type_using_string_nested1(isAsync);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [s].[Id], [s].[InternalNumber], [s].[Name]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN [Squads] AS [s] ON [t].[SquadId] = [s].[Id]
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')");
        }

        public override async Task Include_reference_on_derived_type_using_string_nested2(bool isAsync)
        {
            await base.Include_reference_on_derived_type_using_string_nested2(isAsync);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [t0].[Name], [t0].[Location], [t0].[Nation]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [c].[Name], [c].[Location], [c].[Nation]
    FROM [Gears] AS [g0]
    INNER JOIN [Cities] AS [c] ON [g0].[CityOrBirthName] = [c].[Name]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON ((([t].[Nickname] = [t0].[LeaderNickname]) AND ([t].[Nickname] IS NOT NULL AND [t0].[LeaderNickname] IS NOT NULL)) OR ([t].[Nickname] IS NULL AND [t0].[LeaderNickname] IS NULL)) AND (([t].[SquadId] = [t0].[LeaderSquadId]) AND [t].[SquadId] IS NOT NULL)
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')
ORDER BY [l].[Name], [t0].[Nickname], [t0].[SquadId], [t0].[Name]");
        }

        public override async Task Include_reference_on_derived_type_using_lambda(bool isAsync)
        {
            await base.Include_reference_on_derived_type_using_lambda(isAsync);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')");
        }

        public override async Task Include_reference_on_derived_type_using_lambda_with_soft_cast(bool isAsync)
        {
            await base.Include_reference_on_derived_type_using_lambda_with_soft_cast(isAsync);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')");
        }

        public override async Task Include_reference_on_derived_type_using_lambda_with_tracking(bool isAsync)
        {
            await base.Include_reference_on_derived_type_using_lambda_with_tracking(isAsync);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')");
        }

        public override async Task Include_collection_on_derived_type_using_string(bool isAsync)
        {
            await base.Include_collection_on_derived_type_using_string(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_collection_on_derived_type_using_lambda(bool isAsync)
        {
            await base.Include_collection_on_derived_type_using_lambda(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_collection_on_derived_type_using_lambda_with_soft_cast(bool isAsync)
        {
            await base.Include_collection_on_derived_type_using_lambda_with_soft_cast(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_base_navigation_on_derived_entity(bool isAsync)
        {
            await base.Include_base_navigation_on_derived_entity(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [w].[Id]");
        }

        public override async Task ThenInclude_collection_on_derived_after_base_reference(bool isAsync)
        {
            await base.ThenInclude_collection_on_derived_after_base_reference(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [t0].[FullName] = [w].[OwnerFullName]
ORDER BY [t].[Id], [w].[Id]");
        }

        public override async Task ThenInclude_collection_on_derived_after_derived_reference(bool isAsync)
        {
            await base.ThenInclude_collection_on_derived_after_derived_reference(isAsync);

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [t1].[Nickname], [t1].[SquadId], [t1].[AssignedCityName], [t1].[CityOrBirthName], [t1].[Discriminator], [t1].[FullName], [t1].[HasSoulPatch], [t1].[LeaderNickname], [t1].[LeaderSquadId], [t1].[Rank]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t] ON [f].[CommanderName] = [t].[Name]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[DefeatedByNickname] = [t0].[Nickname]) AND [t].[DefeatedByNickname] IS NOT NULL) AND (([t].[DefeatedBySquadId] = [t0].[SquadId]) AND [t].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON ((([t0].[Nickname] = [t1].[LeaderNickname]) AND ([t0].[Nickname] IS NOT NULL AND [t1].[LeaderNickname] IS NOT NULL)) OR ([t0].[Nickname] IS NULL AND [t1].[LeaderNickname] IS NULL)) AND (([t0].[SquadId] = [t1].[LeaderSquadId]) AND [t0].[SquadId] IS NOT NULL)
WHERE [f].[Discriminator] = N'LocustHorde'
ORDER BY [f].[Id], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task ThenInclude_collection_on_derived_after_derived_collection(bool isAsync)
        {
            await base.ThenInclude_collection_on_derived_after_derived_collection(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [t0].[Nickname0], [t0].[SquadId0], [t0].[AssignedCityName0], [t0].[CityOrBirthName0], [t0].[Discriminator0], [t0].[FullName0], [t0].[HasSoulPatch0], [t0].[LeaderNickname0], [t0].[LeaderSquadId0], [t0].[Rank0]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [t].[Nickname] AS [Nickname0], [t].[SquadId] AS [SquadId0], [t].[AssignedCityName] AS [AssignedCityName0], [t].[CityOrBirthName] AS [CityOrBirthName0], [t].[Discriminator] AS [Discriminator0], [t].[FullName] AS [FullName0], [t].[HasSoulPatch] AS [HasSoulPatch0], [t].[LeaderNickname] AS [LeaderNickname0], [t].[LeaderSquadId] AS [LeaderSquadId0], [t].[Rank] AS [Rank0]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [g1].[Nickname], [g1].[SquadId], [g1].[AssignedCityName], [g1].[CityOrBirthName], [g1].[Discriminator], [g1].[FullName], [g1].[HasSoulPatch], [g1].[LeaderNickname], [g1].[LeaderSquadId], [g1].[Rank]
        FROM [Gears] AS [g1]
        WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON (([g0].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g0].[SquadId] = [t].[LeaderSquadId])
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([g].[Nickname] = [t0].[LeaderNickname]) AND [t0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t0].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t0].[Nickname], [t0].[SquadId], [t0].[Nickname0], [t0].[SquadId0]");
        }

        public override async Task ThenInclude_reference_on_derived_after_derived_collection(bool isAsync)
        {
            await base.ThenInclude_reference_on_derived_after_derived_collection(isAsync);

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t0].[Name], [t0].[Discriminator], [t0].[LocustHordeId], [t0].[ThreatLevel], [t0].[DefeatedByNickname], [t0].[DefeatedBySquadId], [t0].[HighCommandId], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator0], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator] AS [Discriminator0], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
    FROM [LocustLeaders] AS [l]
    LEFT JOIN (
        SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
        FROM [Gears] AS [g]
        WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON (([l].[DefeatedByNickname] = [t].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
    WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')
) AS [t0] ON [f].[Id] = [t0].[LocustHordeId]
WHERE [f].[Discriminator] = N'LocustHorde'
ORDER BY [f].[Id], [t0].[Name]");
        }

        public override async Task Multiple_derived_included_on_one_method(bool isAsync)
        {
            await base.Multiple_derived_included_on_one_method(isAsync);

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank], [t1].[Nickname], [t1].[SquadId], [t1].[AssignedCityName], [t1].[CityOrBirthName], [t1].[Discriminator], [t1].[FullName], [t1].[HasSoulPatch], [t1].[LeaderNickname], [t1].[LeaderSquadId], [t1].[Rank]
FROM [Factions] AS [f]
LEFT JOIN (
    SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
    FROM [LocustLeaders] AS [l]
    WHERE [l].[Discriminator] = N'LocustCommander'
) AS [t] ON [f].[CommanderName] = [t].[Name]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[DefeatedByNickname] = [t0].[Nickname]) AND [t].[DefeatedByNickname] IS NOT NULL) AND (([t].[DefeatedBySquadId] = [t0].[SquadId]) AND [t].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON ((([t0].[Nickname] = [t1].[LeaderNickname]) AND ([t0].[Nickname] IS NOT NULL AND [t1].[LeaderNickname] IS NOT NULL)) OR ([t0].[Nickname] IS NULL AND [t1].[LeaderNickname] IS NULL)) AND (([t0].[SquadId] = [t1].[LeaderSquadId]) AND [t0].[SquadId] IS NOT NULL)
WHERE [f].[Discriminator] = N'LocustHorde'
ORDER BY [f].[Id], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task Include_on_derived_multi_level(bool isAsync)
        {
            await base.Include_on_derived_multi_level(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [t].[Id], [t].[InternalNumber], [t].[Name], [t].[SquadId0], [t].[MissionId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank], [s].[Id], [s].[InternalNumber], [s].[Name], [s0].[SquadId] AS [SquadId0], [s0].[MissionId]
    FROM [Gears] AS [g0]
    INNER JOIN [Squads] AS [s] ON [g0].[SquadId] = [s].[Id]
    LEFT JOIN [SquadMissions] AS [s0] ON [s].[Id] = [s0].[SquadId]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId], [t].[Id], [t].[SquadId0], [t].[MissionId]");
        }

        public override async Task Projecting_nullable_bool_in_conditional_works(bool isAsync)
        {
            await base.Projecting_nullable_bool_in_conditional_works(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [t].[Nickname] IS NOT NULL THEN [t].[HasSoulPatch]
    ELSE CAST(0 AS bit)
END AS [Prop]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)");
        }

        public override async Task Enum_ToString_is_client_eval(bool isAsync)
        {
            await base.Enum_ToString_is_client_eval(isAsync);

            AssertSql(
                @"SELECT [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[SquadId], [g].[Nickname]");
        }

        public override async Task Correlated_collections_naked_navigation_with_ToList(bool isAsync)
        {
            await base.Correlated_collections_naked_navigation_with_ToList(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [w].[Id]");
        }

        public override async Task Correlated_collections_naked_navigation_with_ToList_followed_by_projecting_count(bool isAsync)
        {
            await base.Correlated_collections_naked_navigation_with_ToList_followed_by_projecting_count(isAsync);

            AssertSql(
                @"SELECT (
    SELECT COUNT(*)
    FROM [Weapons] AS [w]
    WHERE [g].[FullName] = [w].[OwnerFullName]
)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname]");
        }

        public override async Task Correlated_collections_naked_navigation_with_ToArray(bool isAsync)
        {
            await base.Correlated_collections_naked_navigation_with_ToArray(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [w].[Id]");
        }

        public override async Task Correlated_collections_basic_projection(bool isAsync)
        {
            await base.Correlated_collections_basic_projection(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id]");
        }

        public override async Task Correlated_collections_basic_projection_explicit_to_list(bool isAsync)
        {
            await base.Correlated_collections_basic_projection_explicit_to_list(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id]");
        }

        public override async Task Correlated_collections_basic_projection_explicit_to_array(bool isAsync)
        {
            await base.Correlated_collections_basic_projection_explicit_to_array(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id]");
        }

        public override async Task Correlated_collections_basic_projection_ordered(bool isAsync)
        {
            await base.Correlated_collections_basic_projection_ordered(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Name] DESC, [t].[Id]");
        }

        public override async Task Correlated_collections_basic_projection_composite_key(bool isAsync)
        {
            await base.Correlated_collections_basic_projection_composite_key(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[FullName], [t].[SquadId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[FullName], [g0].[SquadId], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')) AND ([g].[Nickname] <> N'Foo')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collections_basic_projecting_single_property(bool isAsync)
        {
            await base.Correlated_collections_basic_projecting_single_property(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Name], [t].[Id]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Name], [w].[Id], [w].[OwnerFullName]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id]");
        }

        public override async Task Correlated_collections_basic_projecting_constant(bool isAsync)
        {
            await base.Correlated_collections_basic_projecting_constant(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[c], [t].[Id]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT N'BFG' AS [c], [w].[Id], [w].[OwnerFullName]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id]");
        }

        public override async Task Correlated_collections_basic_projecting_constant_bool(bool isAsync)
        {
            await base.Correlated_collections_basic_projecting_constant_bool(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[c], [t].[Id]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT CAST(1 AS bit) AS [c], [w].[Id], [w].[OwnerFullName]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id]");
        }

        public override async Task Correlated_collections_projection_of_collection_thru_navigation(bool isAsync)
        {
            await base.Correlated_collections_projection_of_collection_thru_navigation(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [s].[Id], [t].[SquadId], [t].[MissionId]
FROM [Gears] AS [g]
INNER JOIN [Squads] AS [s] ON [g].[SquadId] = [s].[Id]
LEFT JOIN (
    SELECT [s0].[SquadId], [s0].[MissionId]
    FROM [SquadMissions] AS [s0]
    WHERE [s0].[MissionId] <> 17
) AS [t] ON [s].[Id] = [t].[SquadId]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Marcus')
ORDER BY [g].[FullName], [g].[Nickname], [g].[SquadId], [s].[Id], [t].[SquadId], [t].[MissionId]");
        }

        public override async Task Correlated_collections_project_anonymous_collection_result(bool isAsync)
        {
            await base.Correlated_collections_project_anonymous_collection_result(isAsync);

            AssertSql(
                @"SELECT [s].[Name], [s].[Id], [t].[FullName], [t].[Rank], [t].[Nickname], [t].[SquadId]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [g].[FullName], [g].[Rank], [g].[Nickname], [g].[SquadId]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [s].[Id] = [t].[SquadId]
WHERE [s].[Id] < 20
ORDER BY [s].[Id], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collections_nested(bool isAsync)
        {
            await base.Correlated_collections_nested(isAsync);

            AssertSql(
                @"SELECT [s].[Id], [t0].[SquadId], [t0].[MissionId], [t0].[Id], [t0].[SquadId0], [t0].[MissionId0]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [s0].[SquadId], [s0].[MissionId], [m].[Id], [t].[SquadId] AS [SquadId0], [t].[MissionId] AS [MissionId0]
    FROM [SquadMissions] AS [s0]
    INNER JOIN [Missions] AS [m] ON [s0].[MissionId] = [m].[Id]
    LEFT JOIN (
        SELECT [s1].[SquadId], [s1].[MissionId]
        FROM [SquadMissions] AS [s1]
        WHERE [s1].[SquadId] < 7
    ) AS [t] ON [m].[Id] = [t].[MissionId]
    WHERE [s0].[MissionId] < 42
) AS [t0] ON [s].[Id] = [t0].[SquadId]
ORDER BY [s].[Id], [t0].[SquadId], [t0].[MissionId], [t0].[Id], [t0].[SquadId0], [t0].[MissionId0]");
        }

        public override async Task Correlated_collections_nested_mixed_streaming_with_buffer1(bool isAsync)
        {
            await base.Correlated_collections_nested_mixed_streaming_with_buffer1(isAsync);

            AssertSql(
                @"SELECT [s].[Id], [t0].[SquadId], [t0].[MissionId], [t0].[Id], [t0].[SquadId0], [t0].[MissionId0]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [s0].[SquadId], [s0].[MissionId], [m].[Id], [t].[SquadId] AS [SquadId0], [t].[MissionId] AS [MissionId0]
    FROM [SquadMissions] AS [s0]
    INNER JOIN [Missions] AS [m] ON [s0].[MissionId] = [m].[Id]
    LEFT JOIN (
        SELECT [s1].[SquadId], [s1].[MissionId]
        FROM [SquadMissions] AS [s1]
        WHERE [s1].[SquadId] < 2
    ) AS [t] ON [m].[Id] = [t].[MissionId]
    WHERE [s0].[MissionId] < 3
) AS [t0] ON [s].[Id] = [t0].[SquadId]
ORDER BY [s].[Id], [t0].[SquadId], [t0].[MissionId], [t0].[Id], [t0].[SquadId0], [t0].[MissionId0]");
        }

        public override async Task Correlated_collections_nested_mixed_streaming_with_buffer2(bool isAsync)
        {
            await base.Correlated_collections_nested_mixed_streaming_with_buffer2(isAsync);

            AssertSql(
                @"SELECT [s].[Id], [t0].[SquadId], [t0].[MissionId], [t0].[Id], [t0].[SquadId0], [t0].[MissionId0]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [s0].[SquadId], [s0].[MissionId], [m].[Id], [t].[SquadId] AS [SquadId0], [t].[MissionId] AS [MissionId0]
    FROM [SquadMissions] AS [s0]
    INNER JOIN [Missions] AS [m] ON [s0].[MissionId] = [m].[Id]
    LEFT JOIN (
        SELECT [s1].[SquadId], [s1].[MissionId]
        FROM [SquadMissions] AS [s1]
        WHERE [s1].[SquadId] < 7
    ) AS [t] ON [m].[Id] = [t].[MissionId]
    WHERE [s0].[MissionId] < 42
) AS [t0] ON [s].[Id] = [t0].[SquadId]
ORDER BY [s].[Id], [t0].[SquadId], [t0].[MissionId], [t0].[Id], [t0].[SquadId0], [t0].[MissionId0]");
        }

        public override async Task Correlated_collections_nested_with_custom_ordering(bool isAsync)
        {
            await base.Correlated_collections_nested_with_custom_ordering(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t0].[FullName], [t0].[Nickname], [t0].[SquadId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId], [g0].[Rank], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE ([w].[Name] <> N'Bar') OR [w].[Name] IS NULL
    ) AS [t] ON [g0].[FullName] = [t].[OwnerFullName]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[FullName] <> N'Foo')
) AS [t0] ON (([g].[Nickname] = [t0].[LeaderNickname]) AND [t0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t0].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[HasSoulPatch] DESC, [g].[Nickname], [g].[SquadId], [t0].[Rank], [t0].[Nickname], [t0].[SquadId], [t0].[IsAutomatic], [t0].[Id]");
        }

        public override async Task Correlated_collections_same_collection_projected_multiple_times(bool isAsync)
        {
            await base.Correlated_collections_same_collection_projected_multiple_times(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE [w].[IsAutomatic] = CAST(1 AS bit)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
LEFT JOIN (
    SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
    FROM [Weapons] AS [w0]
    WHERE [w0].[IsAutomatic] = CAST(1 AS bit)
) AS [t0] ON [g].[FullName] = [t0].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id], [t0].[Id]");
        }

        public override async Task Correlated_collections_similar_collection_projected_multiple_times(bool isAsync)
        {
            await base.Correlated_collections_similar_collection_projected_multiple_times(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE [w].[IsAutomatic] = CAST(1 AS bit)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
LEFT JOIN (
    SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
    FROM [Weapons] AS [w0]
    WHERE [w0].[IsAutomatic] <> CAST(1 AS bit)
) AS [t0] ON [g].[FullName] = [t0].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Rank], [g].[Nickname], [g].[SquadId], [t].[OwnerFullName], [t].[Id], [t0].[IsAutomatic], [t0].[Id]");
        }

        public override async Task Correlated_collections_different_collections_projected(bool isAsync)
        {
            await base.Correlated_collections_different_collections_projected(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Name], [t].[IsAutomatic], [t].[Id], [t0].[Nickname], [t0].[Rank], [t0].[SquadId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Name], [w].[IsAutomatic], [w].[Id], [w].[OwnerFullName]
    FROM [Weapons] AS [w]
    WHERE [w].[IsAutomatic] = CAST(1 AS bit)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[Rank], [g0].[SquadId], [g0].[FullName], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([g].[Nickname] = [t0].[LeaderNickname]) AND [t0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t0].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[FullName], [g].[Nickname], [g].[SquadId], [t].[Id], [t0].[FullName], [t0].[Nickname], [t0].[SquadId]");
        }

        public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys(bool isAsync)
        {
            await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys(isAsync);

            AssertSql(
                @"SELECT [g].[FullName]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')) AND EXISTS (
    SELECT 1
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ((([g].[Nickname] = [g0].[LeaderNickname]) AND [g0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g0].[LeaderSquadId])))
ORDER BY [g].[HasSoulPatch] DESC, [t].[Note]");
        }

        public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery(bool isAsync)
        {
            await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t1] ON (([g].[Nickname] = [t1].[GearNickName]) AND [t1].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t1].[GearSquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g1].[Nickname], [g1].[SquadId], [g1].[AssignedCityName], [g1].[CityOrBirthName], [g1].[Discriminator], [g1].[FullName], [g1].[HasSoulPatch], [g1].[LeaderNickname], [g1].[LeaderSquadId], [g1].[Rank]
    FROM [Gears] AS [g1]
    WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t1].[GearNickName] = [t2].[Nickname]) AND [t1].[GearNickName] IS NOT NULL) AND (([t1].[GearSquadId] = [t2].[SquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [t].[Nickname]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
) AS [t0] ON [t2].[FullName] = [t0].[OwnerFullName]
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')) AND EXISTS (
    SELECT 1
    FROM [Gears] AS [g2]
    WHERE [g2].[Discriminator] IN (N'Gear', N'Officer') AND ((([g].[Nickname] = [g2].[LeaderNickname]) AND [g2].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g2].[LeaderSquadId])))
ORDER BY [g].[HasSoulPatch] DESC, [t1].[Note], [g].[Nickname], [g].[SquadId], [t0].[IsAutomatic], [t0].[Nickname] DESC, [t0].[Id]");
        }

        public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_duplicated_orderings(bool isAsync)
        {
            await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_duplicated_orderings(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t1] ON (([g].[Nickname] = [t1].[GearNickName]) AND [t1].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t1].[GearSquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g1].[Nickname], [g1].[SquadId], [g1].[AssignedCityName], [g1].[CityOrBirthName], [g1].[Discriminator], [g1].[FullName], [g1].[HasSoulPatch], [g1].[LeaderNickname], [g1].[LeaderSquadId], [g1].[Rank]
    FROM [Gears] AS [g1]
    WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t1].[GearNickName] = [t2].[Nickname]) AND [t1].[GearNickName] IS NOT NULL) AND (([t1].[GearSquadId] = [t2].[SquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [t].[Nickname]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
) AS [t0] ON [t2].[FullName] = [t0].[OwnerFullName]
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')) AND EXISTS (
    SELECT 1
    FROM [Gears] AS [g2]
    WHERE [g2].[Discriminator] IN (N'Gear', N'Officer') AND ((([g].[Nickname] = [g2].[LeaderNickname]) AND [g2].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g2].[LeaderSquadId])))
ORDER BY [g].[HasSoulPatch] DESC, [t1].[Note], [g].[Nickname], [g].[SquadId], [t0].[IsAutomatic], [t0].[Nickname] DESC, [t0].[Id]");
        }

        public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_complex_orderings(bool isAsync)
        {
            await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_complex_orderings(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t1] ON (([g].[Nickname] = [t1].[GearNickName]) AND [t1].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t1].[GearSquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g1].[Nickname], [g1].[SquadId], [g1].[AssignedCityName], [g1].[CityOrBirthName], [g1].[Discriminator], [g1].[FullName], [g1].[HasSoulPatch], [g1].[LeaderNickname], [g1].[LeaderSquadId], [g1].[Rank]
    FROM [Gears] AS [g1]
    WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t1].[GearNickName] = [t2].[Nickname]) AND [t1].[GearNickName] IS NOT NULL) AND (([t1].[GearSquadId] = [t2].[SquadId]) AND [t1].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], (
        SELECT COUNT(*)
        FROM [Weapons] AS [w0]
        WHERE (([t].[FullName] = [w0].[OwnerFullName]) AND ([t].[FullName] IS NOT NULL AND [w0].[OwnerFullName] IS NOT NULL)) OR ([t].[FullName] IS NULL AND [w0].[OwnerFullName] IS NULL)) AS [c]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
) AS [t0] ON [t2].[FullName] = [t0].[OwnerFullName]
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')) AND EXISTS (
    SELECT 1
    FROM [Gears] AS [g2]
    WHERE [g2].[Discriminator] IN (N'Gear', N'Officer') AND ((([g].[Nickname] = [g2].[LeaderNickname]) AND [g2].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g2].[LeaderSquadId])))
ORDER BY [g].[HasSoulPatch] DESC, [t1].[Note], [g].[Nickname], [g].[SquadId], [t0].[Id] DESC, [t0].[c]");
        }

        public override async Task Correlated_collections_multiple_nested_complex_collections(bool isAsync)
        {
            await base.Correlated_collections_multiple_nested_complex_collections(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t2].[FullName], [t2].[Nickname], [t2].[SquadId], [t2].[Id], [t2].[Name], [t2].[IsAutomatic], [t2].[Id0], [t2].[Nickname0], [t2].[HasSoulPatch], [t2].[SquadId0], [t4].[Id], [t4].[AmmunitionType], [t4].[IsAutomatic], [t4].[Name], [t4].[OwnerFullName], [t4].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t5] ON (([g].[Nickname] = [t5].[GearNickName]) AND [t5].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t5].[GearSquadId]) AND [t5].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g4].[Nickname], [g4].[SquadId], [g4].[AssignedCityName], [g4].[CityOrBirthName], [g4].[Discriminator], [g4].[FullName], [g4].[HasSoulPatch], [g4].[LeaderNickname], [g4].[LeaderSquadId], [g4].[Rank]
    FROM [Gears] AS [g4]
    WHERE [g4].[Discriminator] IN (N'Gear', N'Officer')
) AS [t6] ON (([t5].[GearNickName] = [t6].[Nickname]) AND [t5].[GearNickName] IS NOT NULL) AND (([t5].[GearSquadId] = [t6].[SquadId]) AND [t5].[GearSquadId] IS NOT NULL)
LEFT JOIN (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId], [t1].[Id], [t1].[Name], [t1].[IsAutomatic], [t1].[Id0], [t1].[Nickname] AS [Nickname0], [t1].[HasSoulPatch], [t1].[SquadId] AS [SquadId0], [g0].[Rank], [t1].[IsAutomatic0], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [w].[Id], [w0].[Name], [w0].[IsAutomatic], [w0].[Id] AS [Id0], [t].[Nickname], [t].[HasSoulPatch], [t].[SquadId], [w].[IsAutomatic] AS [IsAutomatic0], [w].[OwnerFullName]
        FROM [Weapons] AS [w]
        LEFT JOIN (
            SELECT [g2].[Nickname], [g2].[SquadId], [g2].[AssignedCityName], [g2].[CityOrBirthName], [g2].[Discriminator], [g2].[FullName], [g2].[HasSoulPatch], [g2].[LeaderNickname], [g2].[LeaderSquadId], [g2].[Rank]
            FROM [Gears] AS [g2]
            WHERE [g2].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t0] ON [w].[OwnerFullName] = [t0].[FullName]
        LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
        LEFT JOIN [Weapons] AS [w0] ON [t0].[FullName] = [w0].[OwnerFullName]
        LEFT JOIN (
            SELECT [g1].[Nickname], [g1].[HasSoulPatch], [g1].[SquadId]
            FROM [Gears] AS [g1]
            WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t] ON [s].[Id] = [t].[SquadId]
        WHERE ([w].[Name] <> N'Bar') OR [w].[Name] IS NULL
    ) AS [t1] ON [g0].[FullName] = [t1].[OwnerFullName]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[FullName] <> N'Foo')
) AS [t2] ON (([g].[Nickname] = [t2].[LeaderNickname]) AND [t2].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t2].[LeaderSquadId])
LEFT JOIN (
    SELECT [w1].[Id], [w1].[AmmunitionType], [w1].[IsAutomatic], [w1].[Name], [w1].[OwnerFullName], [w1].[SynergyWithId], [t3].[Nickname]
    FROM [Weapons] AS [w1]
    LEFT JOIN (
        SELECT [g3].[Nickname], [g3].[SquadId], [g3].[AssignedCityName], [g3].[CityOrBirthName], [g3].[Discriminator], [g3].[FullName], [g3].[HasSoulPatch], [g3].[LeaderNickname], [g3].[LeaderSquadId], [g3].[Rank]
        FROM [Gears] AS [g3]
        WHERE [g3].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t3] ON [w1].[OwnerFullName] = [t3].[FullName]
) AS [t4] ON [t6].[FullName] = [t4].[OwnerFullName]
WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')) AND EXISTS (
    SELECT 1
    FROM [Gears] AS [g5]
    WHERE [g5].[Discriminator] IN (N'Gear', N'Officer') AND ((([g].[Nickname] = [g5].[LeaderNickname]) AND [g5].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g5].[LeaderSquadId])))
ORDER BY [g].[HasSoulPatch] DESC, [t5].[Note], [g].[Nickname], [g].[SquadId], [t2].[Rank], [t2].[Nickname], [t2].[SquadId], [t2].[IsAutomatic0], [t2].[Id], [t2].[Id0], [t2].[Nickname0], [t2].[SquadId0], [t4].[IsAutomatic], [t4].[Nickname] DESC, [t4].[Id]");
        }

        public override async Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool isAsync)
        {
            await base.Correlated_collections_inner_subquery_selector_references_outer_qsre(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t].[FullName], [t].[FullName0], [t].[Nickname], [t].[SquadId]
FROM [Gears] AS [g]
OUTER APPLY (
    SELECT [g0].[FullName], [g].[FullName] AS [FullName0], [g0].[Nickname], [g0].[SquadId]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ((([g].[Nickname] = [g0].[LeaderNickname]) AND [g0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g0].[LeaderSquadId]))
) AS [t]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool isAsync)
        {
            await base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t].[FullName], [t].[Nickname], [t].[SquadId]
FROM [Gears] AS [g]
OUTER APPLY (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId]
    FROM [Gears] AS [g0]
    WHERE ([g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[FullName] <> N'Foo')) AND ((([g].[Nickname] = [g0].[LeaderNickname]) AND [g0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g0].[LeaderSquadId]))
) AS [t]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool isAsync)
        {
            await base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t0].[FullName], [t0].[Nickname], [t0].[SquadId], [t0].[Name], [t0].[Nickname0], [t0].[Id]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId], [t].[Name], [t].[Nickname] AS [Nickname0], [t].[Id], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    OUTER APPLY (
        SELECT [w].[Name], [g0].[Nickname], [w].[Id]
        FROM [Weapons] AS [w]
        WHERE (([w].[Name] <> N'Bar') OR [w].[Name] IS NULL) AND (([g0].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL)
    ) AS [t]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[FullName] <> N'Foo')
) AS [t0] ON (([g].[Nickname] = [t0].[LeaderNickname]) AND [t0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t0].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t0].[Nickname], [t0].[SquadId], [t0].[Id]");
        }

        public override async Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool isAsync)
        {
            await base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t0].[FullName], [t0].[Nickname], [t0].[SquadId], [t0].[Name], [t0].[Nickname0], [t0].[Id]
FROM [Gears] AS [g]
OUTER APPLY (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId], [t].[Name], [t].[Nickname] AS [Nickname0], [t].[Id]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [w].[Name], [g].[Nickname], [w].[Id], [w].[OwnerFullName]
        FROM [Weapons] AS [w]
        WHERE ([w].[Name] <> N'Bar') OR [w].[Name] IS NULL
    ) AS [t] ON [g0].[FullName] = [t].[OwnerFullName]
    WHERE ([g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[FullName] <> N'Foo')) AND ((([g].[Nickname] = [g0].[LeaderNickname]) AND [g0].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [g0].[LeaderSquadId]))
) AS [t0]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t0].[Nickname], [t0].[SquadId], [t0].[Id]");
        }

        public override async Task Correlated_collections_on_select_many(bool isAsync)
        {
            await base.Correlated_collections_on_select_many(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [s].[Name], [g].[SquadId], [s].[Id], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Gears] AS [g]
CROSS JOIN [Squads] AS [s]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Weapons] AS [w]
    WHERE ([w].[IsAutomatic] = CAST(1 AS bit)) OR (([w].[Name] <> N'foo') OR [w].[Name] IS NULL)
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t0] ON [s].[Id] = [t0].[SquadId]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))
ORDER BY [g].[Nickname], [s].[Id] DESC, [g].[SquadId], [t].[Id], [t0].[Nickname], [t0].[SquadId]");
        }

        public override async Task Correlated_collections_with_Skip(bool isAsync)
        {
            await base.Correlated_collections_with_Skip(isAsync);

            AssertSql(
                @"SELECT [s].[Id]
FROM [Squads] AS [s]
ORDER BY [s].[Name]",
                //
                @"@_outer_Id='1'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])
ORDER BY [g].[Nickname]
OFFSET 1 ROWS",
                //
                @"@_outer_Id='2'

SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])
ORDER BY [g].[Nickname]
OFFSET 1 ROWS");
        }

        public override async Task Correlated_collections_with_Take(bool isAsync)
        {
            await base.Correlated_collections_with_Take(isAsync);

            AssertSql(
                @"SELECT [s].[Id]
FROM [Squads] AS [s]
ORDER BY [s].[Name]",
                //
                @"@_outer_Id='1'

SELECT TOP(2) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])
ORDER BY [g].[Nickname]",
                //
                @"@_outer_Id='2'

SELECT TOP(2) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])
ORDER BY [g].[Nickname]");
        }

        public override async Task Correlated_collections_with_Distinct(bool isAsync)
        {
            await base.Correlated_collections_with_Distinct(isAsync);

            AssertSql(
                @"SELECT [s].[Id], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT DISTINCT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [s].[Id] = [t].[SquadId]
ORDER BY [s].[Name], [s].[Id], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collections_with_FirstOrDefault(bool isAsync)
        {
            await base.Correlated_collections_with_FirstOrDefault(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [g].[FullName]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([s].[Id] = [g].[SquadId])
    ORDER BY [g].[Nickname])
FROM [Squads] AS [s]
ORDER BY [s].[Name]");
        }

        public override async Task Correlated_collections_on_left_join_with_predicate(bool isAsync)
        {
            await base.Correlated_collections_on_left_join_with_predicate(isAsync);

            AssertSql(
                @"SELECT [t].[Nickname], [t0].[Id], [w].[Name], [w].[Id]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [t0].[GearNickName] = [t].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
WHERE [t].[HasSoulPatch] <> CAST(1 AS bit)
ORDER BY [t0].[Id], [w].[Id]");
        }

        public override async Task Correlated_collections_on_left_join_with_null_value(bool isAsync)
        {
            await base.Correlated_collections_on_left_join_with_null_value(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [w].[Name], [w].[Id]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON [t].[GearNickName] = [t0].[Nickname]
LEFT JOIN [Weapons] AS [w] ON [t0].[FullName] = [w].[OwnerFullName]
ORDER BY [t].[Note], [t].[Id], [w].[Id]");
        }

        public override async Task Correlated_collections_left_join_with_self_reference(bool isAsync)
        {
            await base.Correlated_collections_left_join_with_self_reference(isAsync);

            AssertSql(
                @"SELECT [t].[Note], [t].[Id], [t0].[FullName], [t0].[Nickname], [t0].[SquadId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[Discriminator] = N'Officer')
) AS [t1] ON [t].[GearNickName] = [t1].[Nickname]
LEFT JOIN (
    SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [g].[LeaderNickname], [g].[LeaderSquadId]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON ((([t1].[Nickname] = [t0].[LeaderNickname]) AND ([t1].[Nickname] IS NOT NULL AND [t0].[LeaderNickname] IS NOT NULL)) OR ([t1].[Nickname] IS NULL AND [t0].[LeaderNickname] IS NULL)) AND (([t1].[SquadId] = [t0].[LeaderSquadId]) AND [t1].[SquadId] IS NOT NULL)
ORDER BY [t].[Id], [t0].[Nickname], [t0].[SquadId]");
        }

        public override async Task Correlated_collections_deeply_nested_left_join(bool isAsync)
        {
            await base.Correlated_collections_deeply_nested_left_join(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t1].[Nickname], [t1].[SquadId], [t1].[Id], [t1].[AmmunitionType], [t1].[IsAutomatic], [t1].[Name], [t1].[OwnerFullName], [t1].[SynergyWithId]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON [t].[GearNickName] = [t2].[Nickname]
LEFT JOIN [Squads] AS [s] ON [t2].[SquadId] = [s].[Id]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
    FROM [Gears] AS [g]
    LEFT JOIN (
        SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE [w].[IsAutomatic] = CAST(1 AS bit)
    ) AS [t0] ON [g].[FullName] = [t0].[OwnerFullName]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))
) AS [t1] ON [s].[Id] = [t1].[SquadId]
ORDER BY [t].[Note], [t2].[Nickname] DESC, [t].[Id], [t1].[Nickname], [t1].[SquadId], [t1].[Id]");
        }

        public override async Task Correlated_collections_from_left_join_with_additional_elements_projected_of_that_join(bool isAsync)
        {
            await base.Correlated_collections_from_left_join_with_additional_elements_projected_of_that_join(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [t0].[Rank], [t0].[Nickname], [t0].[SquadId], [t0].[Id], [t0].[AmmunitionType], [t0].[IsAutomatic], [t0].[Name], [t0].[OwnerFullName], [t0].[SynergyWithId]
FROM [Weapons] AS [w]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t1] ON [w].[OwnerFullName] = [t1].[FullName]
LEFT JOIN [Squads] AS [s] ON [t1].[SquadId] = [s].[Id]
LEFT JOIN (
    SELECT [g].[Rank], [g].[Nickname], [g].[SquadId], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId], [g].[FullName]
    FROM [Gears] AS [g]
    LEFT JOIN (
        SELECT [w0].[Id], [w0].[AmmunitionType], [w0].[IsAutomatic], [w0].[Name], [w0].[OwnerFullName], [w0].[SynergyWithId]
        FROM [Weapons] AS [w0]
        WHERE [w0].[IsAutomatic] <> CAST(1 AS bit)
    ) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON [s].[Id] = [t0].[SquadId]
ORDER BY [w].[Name], [w].[Id], [t0].[FullName] DESC, [t0].[Nickname], [t0].[SquadId], [t0].[Id]");
        }

        public override async Task Correlated_collections_complex_scenario1(bool isAsync)
        {
            await base.Correlated_collections_complex_scenario1(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t1].[Id], [t1].[Nickname], [t1].[HasSoulPatch], [t1].[SquadId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [t].[Nickname], [t].[HasSoulPatch], [t].[SquadId], [w].[OwnerFullName]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g1].[Nickname], [g1].[SquadId], [g1].[AssignedCityName], [g1].[CityOrBirthName], [g1].[Discriminator], [g1].[FullName], [g1].[HasSoulPatch], [g1].[LeaderNickname], [g1].[LeaderSquadId], [g1].[Rank]
        FROM [Gears] AS [g1]
        WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON [w].[OwnerFullName] = [t0].[FullName]
    LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[HasSoulPatch], [g0].[SquadId]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [s].[Id] = [t].[SquadId]
) AS [t1] ON [g].[FullName] = [t1].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t1].[Id], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task Correlated_collections_complex_scenario2(bool isAsync)
        {
            await base.Correlated_collections_complex_scenario2(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t2].[FullName], [t2].[Nickname], [t2].[SquadId], [t2].[Id], [t2].[Nickname0], [t2].[HasSoulPatch], [t2].[SquadId0]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId], [t1].[Id], [t1].[Nickname] AS [Nickname0], [t1].[HasSoulPatch], [t1].[SquadId] AS [SquadId0], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [w].[Id], [t].[Nickname], [t].[HasSoulPatch], [t].[SquadId], [w].[OwnerFullName]
        FROM [Weapons] AS [w]
        LEFT JOIN (
            SELECT [g2].[Nickname], [g2].[SquadId], [g2].[AssignedCityName], [g2].[CityOrBirthName], [g2].[Discriminator], [g2].[FullName], [g2].[HasSoulPatch], [g2].[LeaderNickname], [g2].[LeaderSquadId], [g2].[Rank]
            FROM [Gears] AS [g2]
            WHERE [g2].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t0] ON [w].[OwnerFullName] = [t0].[FullName]
        LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
        LEFT JOIN (
            SELECT [g1].[Nickname], [g1].[HasSoulPatch], [g1].[SquadId]
            FROM [Gears] AS [g1]
            WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t] ON [s].[Id] = [t].[SquadId]
    ) AS [t1] ON [g0].[FullName] = [t1].[OwnerFullName]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([g].[Nickname] = [t2].[LeaderNickname]) AND [t2].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t2].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t2].[Nickname], [t2].[SquadId], [t2].[Id], [t2].[Nickname0], [t2].[SquadId0]");
        }

        public override async Task Correlated_collections_with_funky_orderby_complex_scenario1(bool isAsync)
        {
            await base.Correlated_collections_with_funky_orderby_complex_scenario1(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t1].[Id], [t1].[Nickname], [t1].[HasSoulPatch], [t1].[SquadId]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [w].[Id], [t].[Nickname], [t].[HasSoulPatch], [t].[SquadId], [w].[OwnerFullName]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g1].[Nickname], [g1].[SquadId], [g1].[AssignedCityName], [g1].[CityOrBirthName], [g1].[Discriminator], [g1].[FullName], [g1].[HasSoulPatch], [g1].[LeaderNickname], [g1].[LeaderSquadId], [g1].[Rank]
        FROM [Gears] AS [g1]
        WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON [w].[OwnerFullName] = [t0].[FullName]
    LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[HasSoulPatch], [g0].[SquadId]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [s].[Id] = [t].[SquadId]
) AS [t1] ON [g].[FullName] = [t1].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[FullName], [g].[Nickname] DESC, [g].[SquadId], [t1].[Id], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task Correlated_collections_with_funky_orderby_complex_scenario2(bool isAsync)
        {
            await base.Correlated_collections_with_funky_orderby_complex_scenario2(isAsync);

            AssertSql(
                @"SELECT [g].[FullName], [g].[Nickname], [g].[SquadId], [t2].[FullName], [t2].[Nickname], [t2].[SquadId], [t2].[Id], [t2].[Nickname0], [t2].[HasSoulPatch], [t2].[SquadId0]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId], [t1].[Id], [t1].[Nickname] AS [Nickname0], [t1].[HasSoulPatch], [t1].[SquadId] AS [SquadId0], [g0].[HasSoulPatch] AS [HasSoulPatch0], [t1].[IsAutomatic], [t1].[Name], [g0].[LeaderNickname], [g0].[LeaderSquadId]
    FROM [Gears] AS [g0]
    LEFT JOIN (
        SELECT [w].[Id], [t].[Nickname], [t].[HasSoulPatch], [t].[SquadId], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName]
        FROM [Weapons] AS [w]
        LEFT JOIN (
            SELECT [g2].[Nickname], [g2].[SquadId], [g2].[AssignedCityName], [g2].[CityOrBirthName], [g2].[Discriminator], [g2].[FullName], [g2].[HasSoulPatch], [g2].[LeaderNickname], [g2].[LeaderSquadId], [g2].[Rank]
            FROM [Gears] AS [g2]
            WHERE [g2].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t0] ON [w].[OwnerFullName] = [t0].[FullName]
        LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
        LEFT JOIN (
            SELECT [g1].[Nickname], [g1].[HasSoulPatch], [g1].[SquadId]
            FROM [Gears] AS [g1]
            WHERE [g1].[Discriminator] IN (N'Gear', N'Officer')
        ) AS [t] ON [s].[Id] = [t].[SquadId]
    ) AS [t1] ON [g0].[FullName] = [t1].[OwnerFullName]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([g].[Nickname] = [t2].[LeaderNickname]) AND [t2].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t2].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[HasSoulPatch], [g].[LeaderNickname], [g].[FullName], [g].[Nickname], [g].[SquadId], [t2].[FullName], [t2].[HasSoulPatch0] DESC, [t2].[Nickname], [t2].[SquadId], [t2].[IsAutomatic], [t2].[Name] DESC, [t2].[Id], [t2].[Nickname0], [t2].[SquadId0]");
        }

        public override void Correlated_collection_with_top_level_FirstOrDefault()
        {
            base.Correlated_collection_with_top_level_FirstOrDefault();

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM (
    SELECT TOP(1) [g].[Nickname], [g].[SquadId], [g].[FullName]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g].[Nickname]
) AS [t]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
ORDER BY [t].[Nickname], [t].[SquadId], [w].[Id]");
        }

        public override async Task Correlated_collection_with_top_level_Count(bool isAsync)
        {
            await base.Correlated_collection_with_top_level_Count(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override void Correlated_collection_with_top_level_Last_with_orderby_on_outer()
        {
            base.Correlated_collection_with_top_level_Last_with_orderby_on_outer();

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM (
    SELECT TOP(1) [g].[Nickname], [g].[SquadId], [g].[FullName]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g].[FullName]
) AS [t]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
ORDER BY [t].[FullName], [t].[Nickname], [t].[SquadId], [w].[Id]");
        }

        public override void Correlated_collection_with_top_level_Last_with_order_by_on_inner()
        {
            base.Correlated_collection_with_top_level_Last_with_order_by_on_inner();

            AssertSql(
                @"SELECT [t].[Nickname], [t].[SquadId], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM (
    SELECT TOP(1) [g].[Nickname], [g].[SquadId], [g].[FullName]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g].[FullName] DESC
) AS [t]
LEFT JOIN [Weapons] AS [w] ON [t].[FullName] = [w].[OwnerFullName]
ORDER BY [t].[FullName] DESC, [t].[Nickname], [t].[SquadId], [w].[Name], [w].[Id]");
        }

        public override void Include_with_group_by_and_last()
        {
            base.Include_with_group_by_and_last();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Rank], [g].[HasSoulPatch] DESC, [g].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT [g0].[FullName], [g0].[Rank], [g0].[HasSoulPatch]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Rank], [t].[HasSoulPatch] DESC, [t].[FullName]");
        }

        public override void Include_with_group_by_with_composite_group_key()
        {
            base.Include_with_group_by_with_composite_group_key();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Rank], [g].[HasSoulPatch], [g].[Nickname], [g].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT [g0].[FullName], [g0].[Rank], [g0].[HasSoulPatch], [g0].[Nickname]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Rank], [t].[HasSoulPatch], [t].[Nickname], [t].[FullName]");
        }

        public override void Include_with_group_by_order_by_take()
        {
            base.Include_with_group_by_order_by_take();

            AssertSql(
                @"@__p_0='3'

SELECT TOP(@__p_0) [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname], [g].[FullName]",
                //
                @"@__p_0='3'

SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT TOP(@__p_0) [g0].[FullName], [g0].[Nickname]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
    ORDER BY [g0].[Nickname], [g0].[FullName]
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Nickname], [t].[FullName]");
        }

        public override void Include_with_group_by_distinct()
        {
            base.Include_with_group_by_distinct();

            AssertSql(
                @"SELECT DISTINCT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname], [g].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT DISTINCT [g0].[FullName], [g0].[Nickname]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Nickname], [t].[FullName]");
        }

        public override async Task Null_semantics_on_nullable_bool_from_inner_join_subquery_is_fully_applied(bool isAsync)
        {
            await base.Null_semantics_on_nullable_bool_from_inner_join_subquery_is_fully_applied(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[CapitalName], [t].[Discriminator], [t].[Name], [t].[CommanderName], [t].[Eradicated]
FROM [LocustLeaders] AS [l]
INNER JOIN (
    SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated]
    FROM [Factions] AS [f]
    WHERE (([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')) AND (([f].[Name] = N'Swarm') AND [f].[Name] IS NOT NULL)
) AS [t] ON [l].[Name] = [t].[CommanderName]
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander') AND (([t].[Eradicated] <> CAST(1 AS bit)) OR [t].[Eradicated] IS NULL)");
        }

        public override async Task Null_semantics_on_nullable_bool_from_left_join_subquery_is_fully_applied(bool isAsync)
        {
            await base.Null_semantics_on_nullable_bool_from_left_join_subquery_is_fully_applied(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[CapitalName], [t].[Discriminator], [t].[Name], [t].[CommanderName], [t].[Eradicated]
FROM [LocustLeaders] AS [l]
LEFT JOIN (
    SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated]
    FROM [Factions] AS [f]
    WHERE (([f].[Discriminator] = N'LocustHorde') AND ([f].[Discriminator] = N'LocustHorde')) AND (([f].[Name] = N'Swarm') AND [f].[Name] IS NOT NULL)
) AS [t] ON [l].[Name] = [t].[CommanderName]
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander') AND (([t].[Eradicated] <> CAST(1 AS bit)) OR [t].[Eradicated] IS NULL)");
        }

        public override void Include_collection_group_by_reference()
        {
            base.Include_collection_group_by_reference();

            AssertSql(" ");
        }

        public override async Task Include_on_derived_type_with_order_by_and_paging(bool isAsync)
        {
            await base.Include_on_derived_type_with_order_by_and_paging(isAsync);

            AssertSql(
                @"@__p_0='10'

SELECT [t1].[Name], [t1].[Discriminator], [t1].[LocustHordeId], [t1].[ThreatLevel], [t1].[DefeatedByNickname], [t1].[DefeatedBySquadId], [t1].[HighCommandId], [t2].[Nickname], [t2].[SquadId], [t2].[AssignedCityName], [t2].[CityOrBirthName], [t2].[Discriminator], [t2].[FullName], [t2].[HasSoulPatch], [t2].[LeaderNickname], [t2].[LeaderSquadId], [t2].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM (
    SELECT TOP(@__p_0) [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[ThreatLevel], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId], [t].[Note]
    FROM [LocustLeaders] AS [l]
    LEFT JOIN (
        SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
        FROM [Gears] AS [g]
        WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON (([l].[DefeatedByNickname] = [t0].[Nickname]) AND [l].[DefeatedByNickname] IS NOT NULL) AND (([l].[DefeatedBySquadId] = [t0].[SquadId]) AND [l].[DefeatedBySquadId] IS NOT NULL)
    LEFT JOIN [Tags] AS [t] ON ((([t0].[Nickname] = [t].[GearNickName]) AND ([t0].[Nickname] IS NOT NULL AND [t].[GearNickName] IS NOT NULL)) OR ([t0].[Nickname] IS NULL AND [t].[GearNickName] IS NULL)) AND ((([t0].[SquadId] = [t].[GearSquadId]) AND ([t0].[SquadId] IS NOT NULL AND [t].[GearSquadId] IS NOT NULL)) OR ([t0].[SquadId] IS NULL AND [t].[GearSquadId] IS NULL))
    WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander')
    ORDER BY [t].[Note]
) AS [t1]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t2] ON (([t1].[DefeatedByNickname] = [t2].[Nickname]) AND [t1].[DefeatedByNickname] IS NOT NULL) AND (([t1].[DefeatedBySquadId] = [t2].[SquadId]) AND [t1].[DefeatedBySquadId] IS NOT NULL)
LEFT JOIN [Weapons] AS [w] ON [t2].[FullName] = [w].[OwnerFullName]
ORDER BY [t1].[Note], [t1].[Name], [w].[Id]");
        }

        public override async Task Select_required_navigation_on_derived_type(bool isAsync)
        {
            await base.Select_required_navigation_on_derived_type(isAsync);

            //            AssertSql(
            //                @"SELECT [ll.HighCommand].[Name]
            //FROM [LocustLeaders] AS [ll]
            //LEFT JOIN [LocustHighCommands] AS [ll.HighCommand] ON ([ll].[Discriminator] = N'LocustCommander') AND ([ll].[HighCommandId] = [ll.HighCommand].[Id])
            //WHERE [ll].[Discriminator] IN (N'LocustCommander', N'LocustLeader')");
        }

        public override async Task Select_required_navigation_on_the_same_type_with_cast(bool isAsync)
        {
            await base.Select_required_navigation_on_the_same_type_with_cast(isAsync);

            AssertSql(
                @"SELECT [c].[Name]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [c] ON [g].[CityOrBirthName] = [c].[Name]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Where_required_navigation_on_derived_type(bool isAsync)
        {
            await base.Where_required_navigation_on_derived_type(isAsync);

            //            AssertSql(
            //                @"SELECT [ll].[Name], [ll].[Discriminator], [ll].[LocustHordeId], [ll].[ThreatLevel], [ll].[DefeatedByNickname], [ll].[DefeatedBySquadId], [ll].[HighCommandId]
            //FROM [LocustLeaders] AS [ll]
            //LEFT JOIN [LocustHighCommands] AS [ll.HighCommand] ON ([ll].[Discriminator] = N'LocustCommander') AND ([ll].[HighCommandId] = [ll.HighCommand].[Id])
            //WHERE [ll].[Discriminator] IN (N'LocustCommander', N'LocustLeader') AND ([ll.HighCommand].[IsOperational] = CAST(1 AS bit))");
        }

        public override async Task Outer_parameter_in_join_key(bool isAsync)
        {
            await base.Outer_parameter_in_join_key(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t1].[Note], [t1].[Id], [t1].[Nickname], [t1].[SquadId]
FROM [Gears] AS [g]
OUTER APPLY (
    SELECT [t].[Note], [t].[Id], [t0].[Nickname], [t0].[SquadId]
    FROM [Tags] AS [t]
    INNER JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON [g].[FullName] = [t0].[FullName]
) AS [t1]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t1].[Id], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task Outer_parameter_in_join_key_inner_and_outer(bool isAsync)
        {
            await base.Outer_parameter_in_join_key_inner_and_outer(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t1].[Note], [t1].[Id], [t1].[Nickname], [t1].[SquadId]
FROM [Gears] AS [g]
OUTER APPLY (
    SELECT [t].[Note], [t].[Id], [t0].[Nickname], [t0].[SquadId]
    FROM [Tags] AS [t]
    INNER JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON [g].[FullName] = [g].[Nickname]
) AS [t1]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t1].[Id], [t1].[Nickname], [t1].[SquadId]");
        }

        public override async Task Outer_parameter_in_group_join_key(bool isAsync)
        {
            await base.Outer_parameter_in_group_join_key(isAsync);

            AssertSql(
                @"SELECT [o].[FullName]
FROM [Gears] AS [o]
WHERE [o].[Discriminator] = N'Officer'
ORDER BY [o].[Nickname]",
                //
                @"@_outer_FullName1='Damon Baird' (Size = 450)

SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].*
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON @_outer_FullName1 = [t0].[FullName]",
                //
                @"@_outer_FullName1='Marcus Fenix' (Size = 450)

SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].*
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON @_outer_FullName1 = [t0].[FullName]");
        }

        public override async Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool isAsync)
        {
            await base.Outer_parameter_in_group_join_with_DefaultIfEmpty(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t1].[Note], [t1].[Id]
FROM [Gears] AS [g]
OUTER APPLY (
    SELECT [t].[Note], [t].[Id]
    FROM [Tags] AS [t]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON [g].[FullName] = [t0].[FullName]
) AS [t1]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [t1].[Id]");
        }

        public override async Task Include_collection_with_concat(bool isAsync)
        {
            await base.Include_collection_with_concat(isAsync);

            AssertSql(
                "");
        }

        public override async Task Negated_bool_ternary_inside_anonymous_type_in_projection(bool isAsync)
        {
            await base.Negated_bool_ternary_inside_anonymous_type_in_projection(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN CASE
        WHEN [t].[HasSoulPatch] = CAST(1 AS bit) THEN CAST(1 AS bit)
        ELSE COALESCE([t].[HasSoulPatch], CAST(1 AS bit))
    END <> CAST(1 AS bit) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END AS [c]
FROM [Tags] AS [t0]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([t0].[GearNickName] = [t].[Nickname]) AND [t0].[GearNickName] IS NOT NULL) AND (([t0].[GearSquadId] = [t].[SquadId]) AND [t0].[GearSquadId] IS NOT NULL)");
        }

        public override async Task Order_by_entity_qsre(bool isAsync)
        {
            await base.Order_by_entity_qsre(isAsync);

            AssertSql(
                @"SELECT [g].[FullName]
FROM [Gears] AS [g]
LEFT JOIN [Cities] AS [c] ON [g].[AssignedCityName] = [c].[Name]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [c].[Name], [g].[Nickname] DESC");
        }

        public override async Task Order_by_entity_qsre_with_inheritance(bool isAsync)
        {
            await base.Order_by_entity_qsre_with_inheritance(isAsync);

            AssertSql(
                @"SELECT [l].[Name]
FROM [LocustLeaders] AS [l]
INNER JOIN [LocustHighCommands] AS [l0] ON [l].[HighCommandId] = [l0].[Id]
WHERE [l].[Discriminator] IN (N'LocustLeader', N'LocustCommander') AND ([l].[Discriminator] = N'LocustCommander')
ORDER BY [l0].[Id], [l].[Name]");
        }

        public override async Task Order_by_entity_qsre_composite_key(bool isAsync)
        {
            await base.Order_by_entity_qsre_composite_key(isAsync);

            AssertSql(
                @"SELECT [w].[Name]
FROM [Weapons] AS [w]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Nickname], [t].[SquadId], [w].[Id]");
        }

        public override async Task Order_by_entity_qsre_with_other_orderbys(bool isAsync)
        {
            await base.Order_by_entity_qsre_with_other_orderbys(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
LEFT JOIN [Weapons] AS [w0] ON [w].[SynergyWithId] = [w0].[Id]
ORDER BY [w].[IsAutomatic], [t].[Nickname] DESC, [t].[SquadId] DESC, [w0].[Id], [w].[Name]");
        }

        public override async Task Join_on_entity_qsre_keys(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys(isAsync);

            AssertSql(
                @"SELECT [w].[Name] AS [Name1], [w0].[Name] AS [Name2]
FROM [Weapons] AS [w]
INNER JOIN [Weapons] AS [w0] ON [w].[Id] = [w0].[Id]");
        }

        public override async Task Join_on_entity_qsre_keys_composite_key(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys_composite_key(isAsync);

            // issue #16081
            //            AssertSql(
            //                @"SELECT [g1].[FullName] AS [GearName1], [g2].[FullName] AS [GearName2]
            //FROM [Gears] AS [g1]
            //INNER JOIN [Gears] AS [g2] ON ([g1].[Nickname] = [g2].[Nickname]) AND ([g1].[SquadId] = [g2].[SquadId])
            //WHERE [g1].[Discriminator] IN (N'Officer', N'Gear') AND [g2].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Join_on_entity_qsre_keys_inheritance(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys_inheritance(isAsync);

            // issue #16081
            //            AssertSql(
            //                @"SELECT [g].[FullName] AS [GearName], [o].[FullName] AS [OfficerName]
            //FROM [Gears] AS [g]
            //INNER JOIN [Gears] AS [o] ON ([g].[Nickname] = [o].[Nickname]) AND ([g].[SquadId] = [o].[SquadId])
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([o].[Discriminator] = N'Officer')");
        }

        public override async Task Join_on_entity_qsre_keys_outer_key_is_navigation(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys_outer_key_is_navigation(isAsync);

            AssertSql(
                @"SELECT [w].[Name] AS [Name1], [w0].[Name] AS [Name2]
FROM [Weapons] AS [w]
LEFT JOIN [Weapons] AS [w1] ON [w].[SynergyWithId] = [w1].[Id]
INNER JOIN [Weapons] AS [w0] ON [w1].[Id] = [w0].[Id]");
        }

        public override async Task Join_on_entity_qsre_keys_inner_key_is_navigation(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys_inner_key_is_navigation(isAsync);

            AssertSql(
                @"SELECT [c].[Name] AS [CityName], [t].[Nickname] AS [GearNickname]
FROM [Cities] AS [c]
INNER JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [c0].[Name], [c0].[Location], [c0].[Nation]
    FROM [Gears] AS [g]
    LEFT JOIN [Cities] AS [c0] ON [g].[AssignedCityName] = [c0].[Name]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [c].[Name] = [t].[Name]");
        }

        public override async Task Join_on_entity_qsre_keys_inner_key_is_navigation_composite_key(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys_inner_key_is_navigation_composite_key(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [t1].[Note]
FROM [Gears] AS [g]
INNER JOIN (
    SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
    FROM [Tags] AS [t]
    LEFT JOIN (
        SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
        FROM [Gears] AS [g0]
        WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
    WHERE (([t].[Note] = N'Cole''s Tag') AND [t].[Note] IS NOT NULL) OR (([t].[Note] = N'Dom''s Tag') AND [t].[Note] IS NOT NULL)
) AS [t1] ON (([g].[Nickname] = [t1].[Nickname]) AND [t1].[Nickname] IS NOT NULL) AND (([g].[SquadId] = [t1].[SquadId]) AND [t1].[SquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Join_on_entity_qsre_keys_inner_key_is_nested_navigation(bool isAsync)
        {
            await base.Join_on_entity_qsre_keys_inner_key_is_nested_navigation(isAsync);

            AssertSql(
                @"SELECT [s].[Name] AS [SquadName], [t0].[Name] AS [WeaponName]
FROM [Squads] AS [s]
INNER JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [s0].[Id] AS [Id0], [s0].[InternalNumber], [s0].[Name] AS [Name0]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
        FROM [Gears] AS [g]
        WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
    LEFT JOIN [Squads] AS [s0] ON [t].[SquadId] = [s0].[Id]
    WHERE [w].[IsAutomatic] = CAST(1 AS bit)
) AS [t0] ON [s].[Id] = [t0].[Id0]");
        }

        public override async Task GroupJoin_on_entity_qsre_keys_inner_key_is_nested_navigation(bool isAsync)
        {
            await base.GroupJoin_on_entity_qsre_keys_inner_key_is_nested_navigation(isAsync);

            AssertSql(
                @"SELECT [s].[Name] AS [SquadName], [t0].[Name] AS [WeaponName]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [s0].[Id] AS [Id0], [s0].[InternalNumber], [s0].[Name] AS [Name0]
    FROM [Weapons] AS [w]
    LEFT JOIN (
        SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
        FROM [Gears] AS [g]
        WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ) AS [t] ON [w].[OwnerFullName] = [t].[FullName]
    LEFT JOIN [Squads] AS [s0] ON [t].[SquadId] = [s0].[Id]
) AS [t0] ON [s].[Id] = [t0].[Id0]");
        }

        public override void Include_with_group_by_on_entity_qsre()
        {
            base.Include_with_group_by_on_entity_qsre();

            AssertSql(
                @"SELECT [s].[Id], [s].[InternalNumber], [s].[Name]
FROM [Squads] AS [s]
ORDER BY [s].[Id]",
                //
                @"SELECT [s.Members].[Nickname], [s.Members].[SquadId], [s.Members].[AssignedCityName], [s.Members].[CityOrBirthName], [s.Members].[Discriminator], [s.Members].[FullName], [s.Members].[HasSoulPatch], [s.Members].[LeaderNickname], [s.Members].[LeaderSquadId], [s.Members].[Rank]
FROM [Gears] AS [s.Members]
INNER JOIN (
    SELECT [s0].[Id]
    FROM [Squads] AS [s0]
) AS [t] ON [s.Members].[SquadId] = [t].[Id]
WHERE [s.Members].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [t].[Id]");
        }

        public override void Include_with_group_by_on_entity_qsre_with_composite_key()
        {
            base.Include_with_group_by_on_entity_qsre_with_composite_key();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname], [g].[SquadId], [g].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT [g0].[FullName], [g0].[Nickname], [g0].[SquadId]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Nickname], [t].[SquadId], [t].[FullName]");
        }

        public override void Include_with_group_by_on_entity_navigation()
        {
            base.Include_with_group_by_on_entity_navigation();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [g.Squad].[Id], [g.Squad].[InternalNumber], [g.Squad].[Name]
FROM [Gears] AS [g]
INNER JOIN [Squads] AS [g.Squad] ON [g].[SquadId] = [g.Squad].[Id]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear') AND ([g].[HasSoulPatch] = 0)
ORDER BY [g.Squad].[Id], [g].[FullName]",
                //
                @"SELECT [g.Weapons].[Id], [g.Weapons].[AmmunitionType], [g.Weapons].[IsAutomatic], [g.Weapons].[Name], [g.Weapons].[OwnerFullName], [g.Weapons].[SynergyWithId]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT DISTINCT [g0].[FullName], [g.Squad0].[Id]
    FROM [Gears] AS [g0]
    INNER JOIN [Squads] AS [g.Squad0] ON [g0].[SquadId] = [g.Squad0].[Id]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear') AND ([g0].[HasSoulPatch] = 0)
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Id], [t].[FullName]");
        }

        public override void Include_with_group_by_on_entity_navigation_with_inheritance()
        {
            base.Include_with_group_by_on_entity_navigation_with_inheritance();

            AssertSql(
                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated], [t0].[Nickname], [t0].[SquadId], [t0].[AssignedCityName], [t0].[CityOrBirthName], [t0].[Discriminator], [t0].[FullName], [t0].[HasSoulPatch], [t0].[LeaderNickname], [t0].[LeaderSquadId], [t0].[Rank]
FROM [ConditionalFactions] AS [f]
LEFT JOIN (
    SELECT [l.Commander].*
    FROM [LocustLeaders] AS [l.Commander]
    WHERE [l.Commander].[Discriminator] = N'LocustCommander'
) AS [t] ON [f].[CommanderName] = [t].[Name]
LEFT JOIN (
    SELECT [l.Commander.DefeatedBy].*
    FROM [Gears] AS [l.Commander.DefeatedBy]
    WHERE [l.Commander.DefeatedBy].[Discriminator] IN (N'Officer', N'Gear')
) AS [t0] ON ([t].[DefeatedByNickname] = [t0].[Nickname]) AND ([t].[DefeatedBySquadId] = [t0].[SquadId])
WHERE [f].[Discriminator] = N'LocustHorde'
ORDER BY [t0].[Nickname], [t0].[SquadId], [f].[Id]",
                //
                @"SELECT [l.Leaders].[Name], [l.Leaders].[Discriminator], [l.Leaders].[LocustHordeId], [l.Leaders].[ThreatLevel], [l.Leaders].[DefeatedByNickname], [l.Leaders].[DefeatedBySquadId], [l.Leaders].[HighCommandId]
FROM [LocustLeaders] AS [l.Leaders]
INNER JOIN (
    SELECT DISTINCT [f0].[Id], [t2].[Nickname], [t2].[SquadId]
    FROM [ConditionalFactions] AS [f0]
    LEFT JOIN (
        SELECT [l.Commander0].*
        FROM [LocustLeaders] AS [l.Commander0]
        WHERE [l.Commander0].[Discriminator] = N'LocustCommander'
    ) AS [t1] ON [f0].[CommanderName] = [t1].[Name]
    LEFT JOIN (
        SELECT [l.Commander.DefeatedBy0].*
        FROM [Gears] AS [l.Commander.DefeatedBy0]
        WHERE [l.Commander.DefeatedBy0].[Discriminator] IN (N'Officer', N'Gear')
    ) AS [t2] ON ([t1].[DefeatedByNickname] = [t2].[Nickname]) AND ([t1].[DefeatedBySquadId] = [t2].[SquadId])
    WHERE [f0].[Discriminator] = N'LocustHorde'
) AS [t3] ON [l.Leaders].[LocustHordeId] = [t3].[Id]
WHERE [l.Leaders].[Discriminator] IN (N'LocustCommander', N'LocustLeader')
ORDER BY [t3].[Nickname], [t3].[SquadId], [t3].[Id]");
        }

        public override void Streaming_correlated_collection_issue_11403()
        {
            base.Streaming_correlated_collection_issue_11403();

            AssertSql(
                @"SELECT TOP(1) [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname]",
                //
                @"@_outer_FullName='Damon Baird' (Size = 450)

SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE (@_outer_FullName = [w].[OwnerFullName]) AND ([w].[IsAutomatic] = CAST(0 AS bit))
ORDER BY [w].[Id]");
        }

        public override async Task Project_one_value_type_from_empty_collection(bool isAsync)
        {
            await base.Project_one_value_type_from_empty_collection(isAsync);

            // issue #15864
            //            AssertSql(
            //                @"SELECT [s].[Name], COALESCE((
            //    SELECT TOP(1) [g].[SquadId]
            //    FROM [Gears] AS [g]
            //    WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND ([s].[Id] = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))
            //), 0) AS [SquadId]
            //FROM [Squads] AS [s]
            //WHERE [s].[Name] = N'Kilo'");
        }

        public override async Task Filter_on_subquery_projecting_one_value_type_from_empty_collection(bool isAsync)
        {
            await base.Filter_on_subquery_projecting_one_value_type_from_empty_collection(isAsync);

            AssertSql(
                @"SELECT [s].[Name]
FROM [Squads] AS [s]
WHERE ([s].[Name] = N'Kilo') AND (COALESCE((
    SELECT TOP(1) [g].[SquadId]
    FROM [Gears] AS [g]
    WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND ([s].[Id] = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))
), 0) <> 0)");
        }

        public override async Task Select_subquery_projecting_single_constant_int(bool isAsync)
        {
            await base.Select_subquery_projecting_single_constant_int(isAsync);

            // issue #15864
            //            AssertSql(
            //                @"SELECT [s].[Name], COALESCE((
            //    SELECT TOP(1) 42
            //    FROM [Gears] AS [g]
            //    WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND ([s].[Id] = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))
            //), 0) AS [Gear]
            //FROM [Squads] AS [s]");
        }

        public override async Task Select_subquery_projecting_single_constant_string(bool isAsync)
        {
            await base.Select_subquery_projecting_single_constant_string(isAsync);

            AssertSql(
                @"SELECT [s].[Name], (
    SELECT TOP(1) N'Foo'
    FROM [Gears] AS [g]
    WHERE ([g].[Discriminator] IN (N'Gear', N'Officer') AND ([s].[Id] = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))) AS [Gear]
FROM [Squads] AS [s]");
        }

        public override async Task Select_subquery_projecting_single_constant_bool(bool isAsync)
        {
            await base.Select_subquery_projecting_single_constant_bool(isAsync);

            // issue #15864
            //            AssertSql(
            //                @"SELECT [s].[Name], COALESCE((
            //    SELECT TOP(1) CAST(1 AS bit)
            //    FROM [Gears] AS [g]
            //    WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND ([s].[Id] = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))
            //), CAST(0 AS bit)) AS [Gear]
            //FROM [Squads] AS [s]");
        }

        public override async Task Select_subquery_projecting_single_constant_inside_anonymous(bool isAsync)
        {
            await base.Select_subquery_projecting_single_constant_inside_anonymous(isAsync);

            AssertSql(
                @"SELECT [s].[Name], [s].[Id]
FROM [Squads] AS [s]",
                //
                @"@_outer_Id='1'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))",
                //
                @"@_outer_Id='2'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_projecting_multiple_constants_inside_anonymous(bool isAsync)
        {
            await base.Select_subquery_projecting_multiple_constants_inside_anonymous(isAsync);

            AssertSql(
                @"SELECT [s].[Name], [s].[Id]
FROM [Squads] AS [s]",
                //
                @"@_outer_Id='1'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))",
                //
                @"@_outer_Id='2'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Include_with_order_by_constant(bool isAsync)
        {
            await base.Include_with_order_by_constant(isAsync);

            AssertSql(
                @"SELECT [s].[Id], [s].[InternalNumber], [s].[Name], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [s].[Id] = [t].[SquadId]
ORDER BY [s].[Id], [t].[Nickname], [t].[SquadId]");
        }

        public override void Include_groupby_constant()
        {
            base.Include_groupby_constant();

            AssertSql(
                @"SELECT [s].[Id], [s].[InternalNumber], [s].[Name]
FROM [Squads] AS [s]
ORDER BY [s].[Id]",
                //
                @"SELECT [s.Members].[Nickname], [s.Members].[SquadId], [s.Members].[AssignedCityName], [s.Members].[CityOrBirthName], [s.Members].[Discriminator], [s.Members].[FullName], [s.Members].[HasSoulPatch], [s.Members].[LeaderNickname], [s.Members].[LeaderSquadId], [s.Members].[Rank]
FROM [Gears] AS [s.Members]
INNER JOIN (
    SELECT [s0].[Id], 1 AS [c]
    FROM [Squads] AS [s0]
) AS [t] ON [s.Members].[SquadId] = [t].[Id]
WHERE [s.Members].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [t].[c], [t].[Id]");
        }

        public override async Task Correlated_collection_order_by_constant(bool isAsync)
        {
            await base.Correlated_collection_order_by_constant(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [w].[Name], [w].[Id]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [w].[Id]");
        }

        public override async Task Select_subquery_projecting_single_constant_null_of_non_mapped_type(bool isAsync)
        {
            await base.Select_subquery_projecting_single_constant_null_of_non_mapped_type(isAsync);

            AssertSql(
                @"SELECT [s].[Name], [s].[Id]
FROM [Squads] AS [s]",
                //
                @"@_outer_Id='1'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))",
                //
                @"@_outer_Id='2'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_projecting_single_constant_of_non_mapped_type(bool isAsync)
        {
            await base.Select_subquery_projecting_single_constant_of_non_mapped_type(isAsync);

            AssertSql(
                @"SELECT [s].[Name], [s].[Id]
FROM [Squads] AS [s]",
                //
                @"@_outer_Id='1'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))",
                //
                @"@_outer_Id='2'

SELECT TOP(1) 1
FROM [Gears] AS [g]
WHERE ([g].[Discriminator] IN (N'Officer', N'Gear') AND (@_outer_Id = [g].[SquadId])) AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Include_with_order_by_constant_null_of_non_mapped_type(bool isAsync)
        {
            await base.Include_with_order_by_constant_null_of_non_mapped_type(isAsync);

            AssertSql(
                "");
        }

        public override void Include_groupby_constant_null_of_non_mapped_type()
        {
            base.Include_groupby_constant_null_of_non_mapped_type();

            AssertSql(
                "");
        }

        public override void GroupBy_composite_key_with_Include()
        {
            base.GroupBy_composite_key_with_Include();

            AssertSql(
                @"SELECT [o].[Nickname], [o].[SquadId], [o].[AssignedCityName], [o].[CityOrBirthName], [o].[Discriminator], [o].[FullName], [o].[HasSoulPatch], [o].[LeaderNickname], [o].[LeaderSquadId], [o].[Rank]
FROM [Gears] AS [o]
WHERE [o].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [o].[Rank], [o].[Nickname], [o].[FullName]",
                //
                @"SELECT [o.Weapons].[Id], [o.Weapons].[AmmunitionType], [o.Weapons].[IsAutomatic], [o.Weapons].[Name], [o.Weapons].[OwnerFullName], [o.Weapons].[SynergyWithId]
FROM [Weapons] AS [o.Weapons]
INNER JOIN (
    SELECT [o0].[FullName], [o0].[Rank], 1 AS [c], [o0].[Nickname]
    FROM [Gears] AS [o0]
    WHERE [o0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [o.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[Rank], [t].[c], [t].[Nickname], [t].[FullName]");
        }

        public override async Task Include_collection_OrderBy_aggregate(bool isAsync)
        {
            await base.Include_collection_OrderBy_aggregate(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY (
    SELECT COUNT(*)
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL), [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_collection_with_complex_OrderBy2(bool isAsync)
        {
            await base.Include_collection_with_complex_OrderBy2(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY (
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id]), [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Include_collection_with_complex_OrderBy3(bool isAsync)
        {
            await base.Include_collection_with_complex_OrderBy3(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY (
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id]), [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collection_with_complex_OrderBy(bool isAsync)
        {
            await base.Correlated_collection_with_complex_OrderBy(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY (
    SELECT COUNT(*)
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL), [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Correlated_collection_with_very_complex_order_by(bool isAsync)
        {
            await base.Correlated_collection_with_very_complex_order_by(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank]
FROM [Gears] AS [g]
LEFT JOIN (
    SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOrBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[Rank]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Gear', N'Officer') AND ([g0].[HasSoulPatch] <> CAST(1 AS bit))
) AS [t] ON (([g].[Nickname] = [t].[LeaderNickname]) AND [t].[LeaderNickname] IS NOT NULL) AND ([g].[SquadId] = [t].[LeaderSquadId])
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY (
    SELECT COUNT(*)
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[IsAutomatic] = (
        SELECT TOP(1) [g1].[HasSoulPatch]
        FROM [Gears] AS [g1]
        WHERE [g1].[Discriminator] IN (N'Gear', N'Officer') AND ([g1].[Nickname] = N'Marcus'))) AND (
        SELECT TOP(1) [g1].[HasSoulPatch]
        FROM [Gears] AS [g1]
        WHERE [g1].[Discriminator] IN (N'Gear', N'Officer') AND ([g1].[Nickname] = N'Marcus')) IS NOT NULL)), [g].[Nickname], [g].[SquadId], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Cast_to_derived_type_after_OfType_works(bool isAsync)
        {
            await base.Cast_to_derived_type_after_OfType_works(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')");
        }

        public override async Task Select_subquery_boolean(bool isAsync)
        {
            await base.Select_subquery_boolean(isAsync);

            // issue #15864
            //            AssertSql(
            //                @"SELECT COALESCE((
            //    SELECT TOP(1) [w].[IsAutomatic]
            //    FROM [Weapons] AS [w]
            //    WHERE [g].[FullName] = [w].[OwnerFullName]
            //    ORDER BY [w].[Id]
            //), CAST(0 AS bit))
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Select_subquery_boolean_with_pushdown(bool isAsync)
        {
            await base.Select_subquery_boolean_with_pushdown(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_int_with_inside_cast_and_coalesce(bool isAsync)
        {
            await base.Select_subquery_int_with_inside_cast_and_coalesce(isAsync);

            AssertSql(
                @"SELECT COALESCE((
    SELECT TOP(1) [w].[Id]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id]), 42)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_int_with_outside_cast_and_coalesce(bool isAsync)
        {
            await base.Select_subquery_int_with_outside_cast_and_coalesce(isAsync);

            AssertSql(
                @"SELECT COALESCE((
    SELECT TOP(1) [w].[Id]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id]), 42)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_int_with_pushdown_and_coalesce(bool isAsync)
        {
            await base.Select_subquery_int_with_pushdown_and_coalesce(isAsync);

            AssertSql(
                @"SELECT COALESCE((
    SELECT TOP(1) [w].[Id]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id]), 42)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_int_with_pushdown_and_coalesce2(bool isAsync)
        {
            await base.Select_subquery_int_with_pushdown_and_coalesce2(isAsync);

            AssertSql(
                @"SELECT COALESCE((
    SELECT TOP(1) [w].[Id]
    FROM [Weapons] AS [w]
    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL
    ORDER BY [w].[Id]), (
    SELECT TOP(1) [w0].[Id]
    FROM [Weapons] AS [w0]
    WHERE ([g].[FullName] = [w0].[OwnerFullName]) AND [w0].[OwnerFullName] IS NOT NULL
    ORDER BY [w0].[Id]))
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_boolean_empty(bool isAsync)
        {
            await base.Select_subquery_boolean_empty(isAsync);

            // issue #15864
            //            AssertSql(
            //                @"SELECT COALESCE((
            //    SELECT TOP(1) [w].[IsAutomatic]
            //    FROM [Weapons] AS [w]
            //    WHERE ([g].[FullName] = [w].[OwnerFullName]) AND ([w].[Name] = N'BFG')
            //    ORDER BY [w].[Id]
            //), CAST(0 AS bit))
            //FROM [Gears] AS [g]
            //WHERE [g].[Discriminator] IN (N'Officer', N'Gear')");
        }

        public override async Task Select_subquery_boolean_empty_with_pushdown(bool isAsync)
        {
            await base.Select_subquery_boolean_empty_with_pushdown(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[Name] = N'BFG') AND [w].[Name] IS NOT NULL)
    ORDER BY [w].[Id])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_boolean_empty_with_pushdown_without_convert_to_nullable1(bool isAsync)
        {
            await base.Select_subquery_boolean_empty_with_pushdown_without_convert_to_nullable1(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[Name] = N'BFG') AND [w].[Name] IS NOT NULL)
    ORDER BY [w].[Id])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_boolean_empty_with_pushdown_without_convert_to_nullable2(bool isAsync)
        {
            await base.Select_subquery_boolean_empty_with_pushdown_without_convert_to_nullable2(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [w].[Id]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[Name] = N'BFG') AND [w].[Name] IS NOT NULL)
    ORDER BY [w].[Id])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Select_subquery_distinct_singleordefault_boolean1(bool isAsync)
        {
            await base.Select_subquery_distinct_singleordefault_boolean1(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Lancer', [w].[Name]) > 0)
    ) AS [t])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_distinct_singleordefault_boolean2(bool isAsync)
        {
            await base.Select_subquery_distinct_singleordefault_boolean2(isAsync);

            AssertSql(
                @"SELECT (
    SELECT DISTINCT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Lancer', [w].[Name]) > 0))
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool isAsync)
        {
            await base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Lancer', [w].[Name]) > 0)
    ) AS [t])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_distinct_singleordefault_boolean_empty1(bool isAsync)
        {
            await base.Select_subquery_distinct_singleordefault_boolean_empty1(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[Name] = N'BFG') AND [w].[Name] IS NOT NULL)
    ) AS [t])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_distinct_singleordefault_boolean_empty2(bool isAsync)
        {
            await base.Select_subquery_distinct_singleordefault_boolean_empty2(isAsync);

            AssertSql(
                @"SELECT (
    SELECT DISTINCT TOP(1) [w].[IsAutomatic]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[Name] = N'BFG') AND [w].[Name] IS NOT NULL))
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool isAsync)
        {
            await base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(isAsync);

            AssertSql(
                @"SELECT (
    SELECT TOP(1) [t].[IsAutomatic]
    FROM (
        SELECT DISTINCT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
        FROM [Weapons] AS [w]
        WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND (([w].[Name] = N'BFG') AND [w].[Name] IS NOT NULL)
    ) AS [t])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[HasSoulPatch] = CAST(1 AS bit))");
        }

        public override async Task Cast_subquery_to_base_type_using_typed_ToList(bool isAsync)
        {
            await base.Cast_subquery_to_base_type_using_typed_ToList(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [t].[CityOrBirthName], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Nickname], [t].[Rank], [t].[SquadId]
FROM [Cities] AS [c]
LEFT JOIN (
    SELECT [g].[CityOrBirthName], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Nickname], [g].[Rank], [g].[SquadId], [g].[AssignedCityName]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [c].[Name] = [t].[AssignedCityName]
WHERE [c].[Name] = N'Ephyra'
ORDER BY [c].[Name], [t].[Nickname], [t].[SquadId]");
        }

        public override async Task Cast_ordered_subquery_to_base_type_using_typed_ToArray(bool isAsync)
        {
            await base.Cast_ordered_subquery_to_base_type_using_typed_ToArray(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [t].[CityOrBirthName], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Nickname], [t].[Rank], [t].[SquadId]
FROM [Cities] AS [c]
LEFT JOIN (
    SELECT [g].[CityOrBirthName], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Nickname], [g].[Rank], [g].[SquadId], [g].[AssignedCityName]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [c].[Name] = [t].[AssignedCityName]
WHERE [c].[Name] = N'Ephyra'
ORDER BY [c].[Name], [t].[Nickname] DESC, [t].[SquadId]");
        }

        public override async Task Correlated_collection_with_complex_order_by_funcletized_to_constant_bool(bool isAsync)
        {
            await base.Correlated_collection_with_complex_order_by_funcletized_to_constant_bool(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[FullName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Nickname], [g].[SquadId], [g].[FullName]",
                //
                @"SELECT [t].[c], [t].[Nickname], [t].[SquadId], [t].[FullName], [g.Weapons].[Name], [g.Weapons].[OwnerFullName]
FROM [Weapons] AS [g.Weapons]
INNER JOIN (
    SELECT CAST(0 AS bit) AS [c], [g0].[Nickname], [g0].[SquadId], [g0].[FullName]
    FROM [Gears] AS [g0]
    WHERE [g0].[Discriminator] IN (N'Officer', N'Gear')
) AS [t] ON [g.Weapons].[OwnerFullName] = [t].[FullName]
ORDER BY [t].[c] DESC, [t].[Nickname], [t].[SquadId], [t].[FullName]");
        }

        public override async Task Double_order_by_on_nullable_bool_coming_from_optional_navigation(bool isAsync)
        {
            await base.Double_order_by_on_nullable_bool_coming_from_optional_navigation(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w0]
LEFT JOIN [Weapons] AS [w] ON [w0].[SynergyWithId] = [w].[Id]
ORDER BY [w].[IsAutomatic], [w].[Id]");
        }

        public override async Task Double_order_by_on_Like(bool isAsync)
        {
            await base.Double_order_by_on_Like(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w0]
LEFT JOIN [Weapons] AS [w] ON [w0].[SynergyWithId] = [w].[Id]
ORDER BY CASE
    WHEN [w].[Name] LIKE N'%Lancer' THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END");
        }

        public override async Task Double_order_by_on_is_null(bool isAsync)
        {
            await base.Double_order_by_on_is_null(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w0]
LEFT JOIN [Weapons] AS [w] ON [w0].[SynergyWithId] = [w].[Id]
ORDER BY CASE
    WHEN [w].[Name] IS NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END");
        }

        public override async Task Double_order_by_on_string_compare(bool isAsync)
        {
            await base.Double_order_by_on_string_compare(isAsync);

            // issue #16092
            //            AssertSql(
            //                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
            //FROM [Weapons] AS [w]
            //ORDER BY CASE
            //    WHEN [w].[Name] = N'Marcus'' Lancer'
            //    THEN CAST(1 AS bit) ELSE CAST(0 AS bit)
            //END, [w].[Id]");
        }

        public override async Task Double_order_by_binary_expression(bool isAsync)
        {
            await base.Double_order_by_binary_expression(isAsync);

            AssertSql(
                @"SELECT [w].[Id] + 2 AS [Binary]
FROM [Weapons] AS [w]
ORDER BY [w].[Id] + 2");
        }

        public override async Task String_compare_with_null_conditional_argument(bool isAsync)
        {
            await base.String_compare_with_null_conditional_argument(isAsync);

            // issue #16092
            //            AssertSql(
            //                @"SELECT [w.SynergyWith].[Id], [w.SynergyWith].[AmmunitionType], [w.SynergyWith].[IsAutomatic], [w.SynergyWith].[Name], [w.SynergyWith].[OwnerFullName], [w.SynergyWith].[SynergyWithId]
            //FROM [Weapons] AS [w]
            //LEFT JOIN [Weapons] AS [w.SynergyWith] ON [w].[SynergyWithId] = [w.SynergyWith].[Id]
            //ORDER BY CASE
            //    WHEN [w.SynergyWith].[Name] = N'Marcus'' Lancer'
            //    THEN CAST(1 AS bit) ELSE CAST(0 AS bit)
            //END");
        }

        public override async Task String_compare_with_null_conditional_argument2(bool isAsync)
        {
            await base.String_compare_with_null_conditional_argument2(isAsync);

            // issue #16092
            //            AssertSql(
            //                @"SELECT [w.SynergyWith].[Id], [w.SynergyWith].[AmmunitionType], [w.SynergyWith].[IsAutomatic], [w.SynergyWith].[Name], [w.SynergyWith].[OwnerFullName], [w.SynergyWith].[SynergyWithId]
            //FROM [Weapons] AS [w]
            //LEFT JOIN [Weapons] AS [w.SynergyWith] ON [w].[SynergyWithId] = [w.SynergyWith].[Id]
            //ORDER BY CASE
            //    WHEN N'Marcus'' Lancer' = [w.SynergyWith].[Name]
            //    THEN CAST(1 AS bit) ELSE CAST(0 AS bit)
            //END");
        }

        public override async Task String_concat_with_null_conditional_argument(bool isAsync)
        {
            await base.String_concat_with_null_conditional_argument(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w0]
LEFT JOIN [Weapons] AS [w] ON [w0].[SynergyWithId] = [w].[Id]
ORDER BY [w].[Name] + CAST(5 AS nvarchar(max))");
        }

        public override async Task String_concat_with_null_conditional_argument2(bool isAsync)
        {
            await base.String_concat_with_null_conditional_argument2(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w0]
LEFT JOIN [Weapons] AS [w] ON [w0].[SynergyWithId] = [w].[Id]
ORDER BY [w].[Name] + N'Marcus'' Lancer'");
        }

        public override async Task String_concat_on_various_types(bool isAsync)
        {
            await base.String_concat_on_various_types(isAsync);

            AssertSql(
                "");
        }

        public override async Task Time_of_day_datetimeoffset(bool isAsync)
        {
            await base.Time_of_day_datetimeoffset(isAsync);

            AssertSql(
                @"SELECT CAST([m].[Timeline] AS time)
FROM [Missions] AS [m]");
        }

        public override async Task GroupBy_Property_Include_Select_Average(bool isAsync)
        {
            await base.GroupBy_Property_Include_Select_Average(isAsync);

            AssertSql(
                @"SELECT AVG(CAST([g].[SquadId] AS float))
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
GROUP BY [g].[Rank]");
        }

        public override async Task GroupBy_Property_Include_Select_Sum(bool isAsync)
        {
            await base.GroupBy_Property_Include_Select_Sum(isAsync);

            AssertSql(
                @"SELECT SUM([g].[SquadId])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
GROUP BY [g].[Rank]");
        }

        public override async Task GroupBy_Property_Include_Select_Count(bool isAsync)
        {
            await base.GroupBy_Property_Include_Select_Count(isAsync);

            AssertSql(
                @"SELECT COUNT(*)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
GROUP BY [g].[Rank]");
        }

        public override async Task GroupBy_Property_Include_Select_LongCount(bool isAsync)
        {
            await base.GroupBy_Property_Include_Select_LongCount(isAsync);

            AssertSql(
                @"SELECT COUNT_BIG(*)
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
GROUP BY [g].[Rank]");
        }

        public override async Task GroupBy_Property_Include_Select_Min(bool isAsync)
        {
            await base.GroupBy_Property_Include_Select_Min(isAsync);

            AssertSql(
                @"SELECT MIN([g].[SquadId])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
GROUP BY [g].[Rank]");
        }

        public override async Task GroupBy_Property_Include_Aggregate_with_anonymous_selector(bool isAsync)
        {
            await base.GroupBy_Property_Include_Aggregate_with_anonymous_selector(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname] AS [Key], COUNT(*) AS [c]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
GROUP BY [g].[Nickname]
ORDER BY [g].[Nickname]");
        }

        public override async Task Group_by_entity_key_with_include_on_that_entity_with_key_in_result_selector(bool isAsync)
        {
            await base.Group_by_entity_key_with_include_on_that_entity_with_key_in_result_selector(isAsync);

            AssertSql(
                "");
        }

        public override async Task Group_by_entity_key_with_include_on_that_entity_with_key_in_result_selector_using_EF_Property(
            bool isAsync)
        {
            await base.Group_by_entity_key_with_include_on_that_entity_with_key_in_result_selector_using_EF_Property(isAsync);

            AssertSql(
                "");
        }

        public override async Task Group_by_with_include_with_entity_in_result_selector(bool isAsync)
        {
            await base.Group_by_with_include_with_entity_in_result_selector(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [g.CityOfBirth].[Name], [g.CityOfBirth].[Location], [g.CityOfBirth].[Nation]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [g.CityOfBirth] ON [g].[CityOrBirthName] = [g.CityOfBirth].[Name]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Rank]");
        }

        public override async Task GroupBy_Property_Include_Select_Max(bool isAsync)
        {
            await base.GroupBy_Property_Include_Select_Max(isAsync);

            AssertSql(
                @"SELECT MAX([g].[SquadId])
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
GROUP BY [g].[Rank]");
        }

        public override async Task Include_with_group_by_and_FirstOrDefault_gets_properly_applied(bool isAsync)
        {
            await base.Include_with_group_by_and_FirstOrDefault_gets_properly_applied(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [g.CityOfBirth].[Name], [g.CityOfBirth].[Location], [g.CityOfBirth].[Nation]
FROM [Gears] AS [g]
INNER JOIN [Cities] AS [g.CityOfBirth] ON [g].[CityOrBirthName] = [g.CityOfBirth].[Name]
WHERE [g].[Discriminator] IN (N'Officer', N'Gear')
ORDER BY [g].[Rank]");
        }

        public override async Task Include_collection_with_Cast_to_base(bool isAsync)
        {
            await base.Include_collection_with_Cast_to_base(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Discriminator] = N'Officer')
ORDER BY [g].[Nickname], [g].[SquadId], [w].[Id]");
        }

        public override void Include_with_client_method_and_member_access_still_applies_includes()
        {
            base.Include_with_client_method_and_member_access_still_applies_includes();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Gears] AS [g]
LEFT JOIN [Tags] AS [t] ON (([g].[Nickname] = [t].[GearNickName]) AND [t].[GearNickName] IS NOT NULL) AND (([g].[SquadId] = [t].[GearSquadId]) AND [t].[GearSquadId] IS NOT NULL)
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override void Include_with_projection_of_unmapped_property_still_gets_applied()
        {
            base.Include_with_projection_of_unmapped_property_still_gets_applied();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')");
        }

        public override async Task Multiple_includes_with_client_method_around_entity_and_also_projecting_included_collection()
        {
            await base.Multiple_includes_with_client_method_around_entity_and_also_projecting_included_collection();

            AssertSql(
                @"SELECT [s].[Name], [s].[Id], [s].[InternalNumber], [t].[Nickname], [t].[SquadId], [t].[AssignedCityName], [t].[CityOrBirthName], [t].[Discriminator], [t].[FullName], [t].[HasSoulPatch], [t].[LeaderNickname], [t].[LeaderSquadId], [t].[Rank], [t].[Id], [t].[AmmunitionType], [t].[IsAutomatic], [t].[Name], [t].[OwnerFullName], [t].[SynergyWithId]
FROM [Squads] AS [s]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
    FROM [Gears] AS [g]
    LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t] ON [s].[Id] = [t].[SquadId]
WHERE ([s].[Name] = N'Delta') AND [s].[Name] IS NOT NULL
ORDER BY [s].[Id], [t].[Nickname], [t].[SquadId], [t].[Id]");
        }

        public override async Task OrderBy_same_expression_containing_IsNull_correctly_deduplicates_the_ordering(bool isAsync)
        {
            await base.OrderBy_same_expression_containing_IsNull_correctly_deduplicates_the_ordering(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[LeaderNickname] IS NOT NULL THEN CASE
        WHEN CAST(LEN([g].[Nickname]) AS int) = 5 THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END
    ELSE NULL
END
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY CASE
    WHEN CASE
        WHEN [g].[LeaderNickname] IS NOT NULL THEN CASE
            WHEN CAST(LEN([g].[Nickname]) AS int) = 5 THEN CAST(1 AS bit)
            ELSE CAST(0 AS bit)
        END
        ELSE NULL
    END IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END");
        }

        public override async Task GetValueOrDefault_in_projection(bool isAsync)
        {
            await base.GetValueOrDefault_in_projection(isAsync);

            AssertSql(
                @"SELECT COALESCE([w].[SynergyWithId], 0)
FROM [Weapons] AS [w]");
        }

        public override async Task GetValueOrDefault_in_filter(bool isAsync)
        {
            await base.GetValueOrDefault_in_filter(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE COALESCE([w].[SynergyWithId], 0) = 0");
        }

        public override async Task GetValueOrDefault_in_filter_non_nullable_column(bool isAsync)
        {
            await base.GetValueOrDefault_in_filter_non_nullable_column(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE COALESCE([w].[Id], 0) = 0");
        }

        public override async Task GetValueOrDefault_in_order_by(bool isAsync)
        {
            await base.GetValueOrDefault_in_order_by(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
ORDER BY COALESCE([w].[SynergyWithId], 0), [w].[Id]");
        }

        public override async Task GetValueOrDefault_with_argument(bool isAsync)
        {
            await base.GetValueOrDefault_with_argument(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE COALESCE([w].[SynergyWithId], [w].[Id]) = 1");
        }

        public override async Task GetValueOrDefault_with_argument_complex(bool isAsync)
        {
            await base.GetValueOrDefault_with_argument_complex(isAsync);

            AssertSql(
                @"SELECT [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Weapons] AS [w]
WHERE COALESCE([w].[SynergyWithId], CAST(LEN([w].[Name]) AS int) + 42) > 10");
        }

        public override async Task Filter_with_complex_predicate_containing_subquery(bool isAsync)
        {
            await base.Filter_with_complex_predicate_containing_subquery(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND (([g].[FullName] <> N'Dom') AND (
    SELECT TOP(1) [w].[Id]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND ([w].[IsAutomatic] = CAST(1 AS bit))
    ORDER BY [w].[Id]) IS NOT NULL)");
        }

        public override async Task Query_with_complex_let_containing_ordering_and_filter_projecting_firstOrDefault_element_of_let(bool isAsync)
        {
            await base.Query_with_complex_let_containing_ordering_and_filter_projecting_firstOrDefault_element_of_let(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], (
    SELECT TOP(1) [w].[Name]
    FROM [Weapons] AS [w]
    WHERE (([g].[FullName] = [w].[OwnerFullName]) AND [w].[OwnerFullName] IS NOT NULL) AND ([w].[IsAutomatic] = CAST(1 AS bit))
    ORDER BY [w].[AmmunitionType] DESC) AS [WeaponName]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND ([g].[Nickname] <> N'Dom')");
        }

        public override async Task
            Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation(bool isAsync)
        {
            await base.Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation(isAsync);

            AssertSql(
                @"");
        }

        public override async Task
            Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation_complex(bool isAsync)
        {
            await base.Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation_complex(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
LEFT JOIN (
    SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
) AS [t0] ON (([t].[GearNickName] = [t0].[Nickname]) AND [t].[GearNickName] IS NOT NULL) AND (([t].[GearSquadId] = [t0].[SquadId]) AND [t].[GearSquadId] IS NOT NULL)
LEFT JOIN [Squads] AS [s] ON [t0].[SquadId] = [s].[Id]
WHERE ((SUBSTRING([t].[Note], 0 + 1, CAST(LEN([s].[Name]) AS int)) = [t].[GearNickName]) AND (SUBSTRING([t].[Note], 0 + 1, CAST(LEN([s].[Name]) AS int)) IS NOT NULL AND [t].[GearNickName] IS NOT NULL)) OR (SUBSTRING([t].[Note], 0 + 1, CAST(LEN([s].[Name]) AS int)) IS NULL AND [t].[GearNickName] IS NULL)");
        }

        public override async Task Filter_with_new_Guid(bool isAsync)
        {
            await base.Filter_with_new_Guid(isAsync);

            AssertSql(
                @"SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
WHERE [t].[Id] = 'df36f493-463f-4123-83f9-6b135deeb7ba'");
        }

        public override async Task Filter_with_new_Guid_closure(bool isAsync)
        {
            await base.Filter_with_new_Guid_closure(isAsync);

            AssertSql(
                @"@__p_0='df36f493-463f-4123-83f9-6b135deeb7bd'

SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
WHERE [t].[Id] = @__p_0",
                //
                @"@__p_0='b39a6fba-9026-4d69-828e-fd7068673e57'

SELECT [t].[Id], [t].[GearNickName], [t].[GearSquadId], [t].[Note]
FROM [Tags] AS [t]
WHERE [t].[Id] = @__p_0");
        }

        public override void OfTypeNav1()
        {
            base.OfTypeNav1();

            // issue #16094
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //LEFT JOIN [Tags] AS [g.Tag] ON ([g].[Nickname] = [g.Tag].[GearNickName]) AND ([g].[SquadId] = [g.Tag].[GearSquadId])
            //LEFT JOIN [Tags] AS [o.Tag] ON ([g].[Nickname] = [o.Tag].[GearNickName]) AND ([g].[SquadId] = [o.Tag].[GearSquadId])
            //WHERE (([g].[Discriminator] = N'Officer') AND (([g.Tag].[Note] <> N'Foo') OR [g.Tag].[Note] IS NULL)) AND (([o.Tag].[Note] <> N'Bar') OR [o.Tag].[Note] IS NULL)");
        }

        public override void OfTypeNav2()
        {
            base.OfTypeNav2();

            // issue #16094
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //LEFT JOIN [Tags] AS [g.Tag] ON ([g].[Nickname] = [g.Tag].[GearNickName]) AND ([g].[SquadId] = [g.Tag].[GearSquadId])
            //LEFT JOIN [Cities] AS [o.AssignedCity] ON [g].[AssignedCityName] = [o.AssignedCity].[Name]
            //WHERE (([g].[Discriminator] = N'Officer') AND (([g.Tag].[Note] <> N'Foo') OR [g.Tag].[Note] IS NULL)) AND (([o.AssignedCity].[Location] <> 'Bar') OR [o.AssignedCity].[Location] IS NULL)");
        }

        public override void OfTypeNav3()
        {
            base.OfTypeNav3();

            // issue #16094
            //            AssertSql(
            //                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
            //FROM [Gears] AS [g]
            //LEFT JOIN [Tags] AS [g.Tag] ON ([g].[Nickname] = [g.Tag].[GearNickName]) AND ([g].[SquadId] = [g.Tag].[GearSquadId])
            //INNER JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
            //LEFT JOIN [Tags] AS [o.Tag] ON ([g].[Nickname] = [o.Tag].[GearNickName]) AND ([g].[SquadId] = [o.Tag].[GearSquadId])
            //WHERE (([g].[Discriminator] = N'Officer') AND (([g.Tag].[Note] <> N'Foo') OR [g.Tag].[Note] IS NULL)) AND (([o.Tag].[Note] <> N'Bar') OR [o.Tag].[Note] IS NULL)");
        }

        public override void Nav_rewrite_Distinct_with_convert()
        {
            base.Nav_rewrite_Distinct_with_convert();

            AssertSql(
                @"");
        }

        public override void Nav_rewrite_Distinct_with_convert_anonymous()
        {
            base.Nav_rewrite_Distinct_with_convert_anonymous();

            AssertSql(
                @"");
        }

        public override void Nav_rewrite_with_convert1()
        {
            base.Nav_rewrite_with_convert1();

            // issue #15994
            //            AssertSql(
            //                @"SELECT [t].[Name], [t].[Discriminator], [t].[LocustHordeId], [t].[ThreatLevel], [t].[DefeatedByNickname], [t].[DefeatedBySquadId], [t].[HighCommandId]
            //FROM [ConditionalFactions] AS [f]
            //LEFT JOIN [Cities] AS [f.Capital] ON [f].[CapitalName] = [f.Capital].[Name]
            //LEFT JOIN (
            //    SELECT [f.Capital.Commander].*
            //    FROM [LocustLeaders] AS [f.Capital.Commander]
            //    WHERE [f.Capital.Commander].[Discriminator] = N'LocustCommander'
            //) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
            //WHERE ([f].[Discriminator] = N'LocustHorde') AND (([f.Capital].[Name] <> N'Foo') OR [f.Capital].[Name] IS NULL)");
        }

        public override void Nav_rewrite_with_convert2()
        {
            base.Nav_rewrite_with_convert2();

            // issue #15994
            //            AssertSql(
            //                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated]
            //FROM [ConditionalFactions] AS [f]
            //LEFT JOIN [Cities] AS [f.Capital] ON [f].[CapitalName] = [f.Capital].[Name]
            //LEFT JOIN (
            //    SELECT [f.Capital.Commander].*
            //    FROM [LocustLeaders] AS [f.Capital.Commander]
            //    WHERE [f.Capital.Commander].[Discriminator] = N'LocustCommander'
            //) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
            //WHERE (([f].[Discriminator] = N'LocustHorde') AND (([f.Capital].[Name] <> N'Foo') OR [f.Capital].[Name] IS NULL)) AND (([t].[Name] <> N'Bar') OR [t].[Name] IS NULL)");
        }

        public override void Nav_rewrite_with_convert3()
        {
            base.Nav_rewrite_with_convert3();

            // issue #15994
            //            AssertSql(
            //                @"SELECT [f].[Id], [f].[CapitalName], [f].[Discriminator], [f].[Name], [f].[CommanderName], [f].[Eradicated]
            //FROM [ConditionalFactions] AS [f]
            //LEFT JOIN [Cities] AS [f.Capital] ON [f].[CapitalName] = [f.Capital].[Name]
            //LEFT JOIN (
            //    SELECT [f.Capital.Commander].*
            //    FROM [LocustLeaders] AS [f.Capital.Commander]
            //    WHERE [f.Capital.Commander].[Discriminator] = N'LocustCommander'
            //) AS [t] ON ([f].[Discriminator] = N'LocustHorde') AND ([f].[CommanderName] = [t].[Name])
            //WHERE (([f].[Discriminator] = N'LocustHorde') AND (([f.Capital].[Name] <> N'Foo') OR [f.Capital].[Name] IS NULL)) AND (([t].[Name] <> N'Bar') OR [t].[Name] IS NULL)");
        }

        public override async Task Where_contains_on_navigation_with_composite_keys(bool isAsync)
        {
            await base.Where_contains_on_navigation_with_composite_keys(isAsync);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank]
FROM [Gears] AS [g]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer') AND EXISTS (
    SELECT 1
    FROM [Cities] AS [c]
    WHERE EXISTS (
        SELECT 1
        FROM [Gears] AS [g0]
        WHERE ([g0].[Discriminator] IN (N'Gear', N'Officer') AND ([c].[Name] = [g0].[CityOrBirthName])) AND (([g0].[Nickname] = [g].[Nickname]) AND ([g0].[SquadId] = [g].[SquadId]))))");
        }

        public override void Include_with_complex_order_by()
        {
            base.Include_with_complex_order_by();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOrBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[Rank], [w].[Id], [w].[AmmunitionType], [w].[IsAutomatic], [w].[Name], [w].[OwnerFullName], [w].[SynergyWithId]
FROM [Gears] AS [g]
LEFT JOIN [Weapons] AS [w] ON [g].[FullName] = [w].[OwnerFullName]
WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
ORDER BY (
    SELECT TOP(1) [w0].[Name]
    FROM [Weapons] AS [w0]
    WHERE (([g].[FullName] = [w0].[OwnerFullName]) AND [w0].[OwnerFullName] IS NOT NULL) AND (CHARINDEX(N'Gnasher', [w0].[Name]) > 0)), [g].[Nickname], [g].[SquadId], [w].[Id]");
        }

        public override async Task Anonymous_projection_take_followed_by_projecting_single_element_from_collection_navigation(bool isAsync)
        {
            await base.Anonymous_projection_take_followed_by_projecting_single_element_from_collection_navigation(isAsync);

            AssertSql(
                @"");
        }

        public override async Task Bool_projection_from_subquery_treated_appropriately_in_where(bool isAsync)
        {
            await base.Bool_projection_from_subquery_treated_appropriately_in_where(isAsync);

            AssertSql(
                @"SELECT [c].[Name], [c].[Location], [c].[Nation]
FROM [Cities] AS [c]
WHERE (
    SELECT TOP(1) [g].[HasSoulPatch]
    FROM [Gears] AS [g]
    WHERE [g].[Discriminator] IN (N'Gear', N'Officer')
    ORDER BY [g].[Nickname], [g].[SquadId]) = CAST(1 AS bit)");
        }

        public override async Task DateTimeOffset_Contains_Less_than_Greater_than(bool isAsync)
        {
            await base.DateTimeOffset_Contains_Less_than_Greater_than(isAsync);

            AssertSql(
                @"@__start_0='1902-01-01T10:00:00.1234567+01:30'
@__end_1='1902-01-03T10:00:00.1234567+01:30'

SELECT [m].[Id], [m].[CodeName], [m].[Rating], [m].[Timeline]
FROM [Missions] AS [m]
WHERE ((@__start_0 <= CAST(CONVERT(date, [m].[Timeline]) AS datetimeoffset)) AND ([m].[Timeline] < @__end_1)) AND [m].[Timeline] IN ('1902-01-02T10:00:00.1234567+01:30')");
        }

        public override async Task Navigation_inside_interpolated_string_expanded(bool isAsync)
        {
            await base.Navigation_inside_interpolated_string_expanded(isAsync);

            AssertSql(
                @"SELECT CASE
    WHEN [w].[SynergyWithId] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [w0].[OwnerFullName]
FROM [Weapons] AS [w]
LEFT JOIN [Weapons] AS [w0] ON [w].[SynergyWithId] = [w0].[Id]");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
