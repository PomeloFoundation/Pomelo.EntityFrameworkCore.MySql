using System;
using System.Collections.Generic;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class QueryFilterFuncletizationMySqlTest
        : QueryFilterFuncletizationTestBase<QueryFilterFuncletizationMySqlTest.QueryFilterFuncletizationMySqlFixture>
    {
        public QueryFilterFuncletizationMySqlTest(
            QueryFilterFuncletizationMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override void DbContext_list_is_parameterized()
        {
            base.DbContext_list_is_parameterized();

            if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
            {
                AssertSql(
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE(NULL, '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""",
                //
                """
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""",
                //
                """
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[1]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""",
                //
                """
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (
    SELECT `e`.`value`
    FROM JSON_TABLE('[2,3]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `e`
)
""");
            }
            else
            {
                AssertSql(
"""
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE FALSE
""",
                //
                """
SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE FALSE
""",
                //
                """
@ef_filter__TenantIds1='1'

SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` = @ef_filter__TenantIds1
""",
                //
                """
@ef_filter__TenantIds1='2'
@ef_filter__TenantIds2='3'

SELECT `l`.`Id`, `l`.`Tenant`
FROM `ListFilter` AS `l`
WHERE `l`.`Tenant` IN (@ef_filter__TenantIds1, @ef_filter__TenantIds2)
""");
            }
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class QueryFilterFuncletizationMySqlFixture : QueryFilterFuncletizationRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}




