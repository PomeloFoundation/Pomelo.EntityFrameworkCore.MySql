using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindWhereQueryMySqlTest : NorthwindWhereQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindWhereQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "issue #573")]
        public override Task Where_as_queryable_expression(bool async)
        {
            return base.Where_as_queryable_expression(async);
        }

        [ConditionalTheory(Skip = "issue #552")]
        public override Task Where_multiple_contains_in_subquery_with_and(bool async)
        {
            return base.Where_multiple_contains_in_subquery_with_and(async);
        }

        [ConditionalTheory(Skip = "issue #552")]
        public override Task Where_multiple_contains_in_subquery_with_or(bool async)
        {
            return base.Where_multiple_contains_in_subquery_with_or(async);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_remove(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.City.Remove(3) == "Sea"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE SUBSTRING(`c`.`City`, 1, 3) = 'Sea'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_remove_count(bool async)
        {
            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => c.City.Remove(3, 1) == "Seatle"));

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CONCAT(SUBSTRING(`c`.`City`, 1, 3), SUBSTRING(`c`.`City`, (3 + 1) + 1, CHAR_LENGTH(`c`.`City`) - (3 + 1))) = 'Seatle'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_guid(bool async)
        {
            var guidParameter = new Guid("4D68FE70-DDB0-47D7-B6DB-437684FA3E1F");

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => guidParameter == Guid.NewGuid()),
                assertEmpty: true);

            AssertSql(
"""
@guidParameter='4d68fe70-ddb0-47d7-b6db-437684fa3e1f'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE @guidParameter = UUID()
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_single_object(bool async)
        {
            object i = 1;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object(bool async)
        {
            object i = 1;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i, c.CustomerID) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@i='1' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object_2(bool async)
        {
            object i = 1;
            object j = 2;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(i, j, c.CustomerID) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@i='1' (Size = 4000)
@j='2' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_object_3(bool async)
        {
            object i = 1;
            object j = 2;
            object k = 3;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] { i, j, k, c.CustomerID }) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

        AssertSql(
"""
@i='1' (Size = 4000)
@j='2' (Size = 4000)
@k='3' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, @k, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_params_string_array(bool async)
        {
            var i = "A";
            var j = "B";
            var k = "C";
            var m = "D";

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] { i, j, k, m, c.CustomerID }) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

        AssertSql(
"""
@i='A' (Size = 4000)
@j='B' (Size = 4000)
@k='C' (Size = 4000)
@m='D' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, @k, @m, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_string_array(bool async)
        {
            var array = new[] {"A", "B", "C", "D"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='ABCD' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_string_array_single_element(bool async)
        {
            var array = new[] {"A"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='A' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_params_object_array(bool async)
        {
            object i = 1;
            object j = 2;
            object k = 3;
            object m = 4;

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(new[] { i, j, k, m, c.CustomerID }) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

        AssertSql(
"""
@i='1' (Size = 4000)
@j='2' (Size = 4000)
@k='3' (Size = 4000)
@m='4' (Size = 4000)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE CONCAT(@i, @j, @k, @m, `c`.`CustomerID`) = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_object_array(bool async)
        {
            var array = new object[] {1, 2, 3, 4};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1234' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_explicit_object_array_single_element(bool async)
        {
            var array = new object[] {1};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(array) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_string_enumerable(bool async)
        {
            IEnumerable<string> enumerable = new[] {"A", "B", "C", "D"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='ABCD' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_string_enumerable_single_element(bool async)
        {
            IEnumerable<string> enumerable = new[] {"A"};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='A' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_generic_enumerable(bool async)
        {
            IEnumerable<int> enumerable = new[] {1, 2, 3, 4};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1234' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_string_concat_method_comparison_generic_enumerable_single_element(bool async)
        {
            IEnumerable<int> enumerable = new[] {1};

            await AssertQuery(
                async,
                ss => ss.Set<Customer>().Where(c => string.Concat(enumerable) == c.CompanyName).Select(c => c.CustomerID),
                assertEmpty: true);

            AssertSql(
"""
@Concat='1' (Size = 40)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE @Concat = `c`.`CompanyName`
""");
        }

        // TODO: 9.0
        [SupportedServerVersionBetweenCondition("11.4.2-mariadb", "11.5.0-mariadb", Invert = true, Skip =
"""
There is some strange collation behavior with MariaDB 11.4.x and this test (seems fixed in 11.5).
The default utf8mb4 collation was changed in 11.4.2 from utf8mb4_general_ci to utf8mb4_uca1400_ai_ci.
We changed MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation accordingly.
If we run the this test against a Ubuntu hosted MariaDB, the test always works.
If we run the this test against a Windows hosted MariaDB, the test fails only on the first execution (when the database does not preexist). But it works on all consecutive runs.
If we change MariaDbServerVersion.DefaultUtf8CiCollation and MariaDbServerVersion.DefaultUtf8CsCollation to use the new default collations only from 11.5.0, the Ubuntu/Windows behavior flips to Windows always working and Ubuntu not successfully executing the test on the first run.
The error is:
    MySqlConnector.MySqlException : Illegal mix of collations (utf8mb4_bin,NONE) and (utf8mb4_uca1400_ai_ci,COERCIBLE) for operation '='
""")]
        public override Task Using_same_parameter_twice_in_query_generates_one_sql_parameter(bool async)
        {
            return base.Using_same_parameter_twice_in_query_generates_one_sql_parameter(async);
        }

        public override async Task Where_compare_constructed_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_constructed_multi_value_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_constructed_multi_value_not_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_multi_value_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_create_constructed_multi_value_not_equal(bool async)
        {
            //  Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_create_constructed_multi_value_not_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_multi_value_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_multi_value_equal(async));

            AssertSql();
        }

        public override async Task Where_compare_tuple_constructed_multi_value_not_equal(bool async)
        {
            // Anonymous type to constant comparison. Issue #14672.
            await AssertTranslationFailed(() => base.Where_compare_tuple_constructed_multi_value_not_equal(async));

            AssertSql();
        }


    public override async Task Where_bool_client_side_negated(bool async)
    {
        await base.Where_bool_client_side_negated(async);
        AssertSql();
    }

    public override async Task EF_MultipleParameters_with_non_evaluatable_argument_throws(bool async)
    {
        await base.EF_MultipleParameters_with_non_evaluatable_argument_throws(async);
        AssertSql();
    }

    public override async Task Where_simple(bool async)
    {
        await base.Where_simple(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
""");
    }

    public override async Task Where_indexer_closure(bool async)
    {
        await base.Where_indexer_closure(async);
        AssertSql(
            """
@p='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @p
""");
    }

    public override async Task Where_dictionary_key_access_closure(bool async)
    {
        await base.Where_dictionary_key_access_closure(async);
        AssertSql(
            """
@get_Item='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @get_Item
""");
    }

    public override async Task Where_tuple_item_closure(bool async)
    {
        await base.Where_tuple_item_closure(async);
        AssertSql(
            """
@predicateTuple_Item2='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @predicateTuple_Item2
""");
    }

    public override async Task Where_named_tuple_item_closure(bool async)
    {
        await base.Where_named_tuple_item_closure(async);
        AssertSql(
            """
@predicateTuple_Item2='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @predicateTuple_Item2
""");
    }

    public override async Task Where_simple_closure_constant(bool async)
    {
        await base.Where_simple_closure_constant(async);
        AssertSql(
            """
@predicate='True'

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE @predicate
""");
    }

    public override async Task Where_simple_closure_via_query_cache(bool async)
    {
        await base.Where_simple_closure_via_query_cache(async);
        AssertSql(
            """
@city='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city
""",
            //
            """
@city='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city
""");
    }

    public override async Task Where_method_call_nullable_type_closure_via_query_cache(bool async)
    {
        await base.Where_method_call_nullable_type_closure_via_query_cache(async);
        AssertSql(
            """
@p='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
            //
            """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""");
    }

    public override async Task Where_method_call_nullable_type_reverse_closure_via_query_cache(bool async)
    {
        await base.Where_method_call_nullable_type_reverse_closure_via_query_cache(async);
        AssertSql(
            """
@p='1' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`EmployeeID` AS signed) > @p
""",
            //
            """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`EmployeeID` AS signed) > @p
""");
    }

    public override async Task Where_method_call_closure_via_query_cache(bool async)
    {
        await base.Where_method_call_closure_via_query_cache(async);
        AssertSql(
            """
@GetCity='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @GetCity
""",
            //
            """
@GetCity='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @GetCity
""");
    }

    public override async Task Where_field_access_closure_via_query_cache(bool async)
    {
        await base.Where_field_access_closure_via_query_cache(async);
        AssertSql(
            """
@city_InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstanceFieldValue
""",
            //
            """
@city_InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstanceFieldValue
""");
    }

    public override async Task Where_property_access_closure_via_query_cache(bool async)
    {
        await base.Where_property_access_closure_via_query_cache(async);
        AssertSql(
            """
@city_InstancePropertyValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstancePropertyValue
""",
            //
            """
@city_InstancePropertyValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_InstancePropertyValue
""");
    }

    public override async Task Where_static_field_access_closure_via_query_cache(bool async)
    {
        await base.Where_static_field_access_closure_via_query_cache(async);
        AssertSql(
            """
@StaticFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticFieldValue
""",
            //
            """
@StaticFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticFieldValue
""");
    }

    public override async Task Where_static_property_access_closure_via_query_cache(bool async)
    {
        await base.Where_static_property_access_closure_via_query_cache(async);
        AssertSql(
            """
@StaticPropertyValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticPropertyValue
""",
            //
            """
@StaticPropertyValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @StaticPropertyValue
""");
    }

    public override async Task Where_nested_field_access_closure_via_query_cache(bool async)
    {
        await base.Where_nested_field_access_closure_via_query_cache(async);
        AssertSql(
            """
@city_Nested_InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstanceFieldValue
""",
            //
            """
@city_Nested_InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstanceFieldValue
""");
    }

    public override async Task Where_nested_property_access_closure_via_query_cache(bool async)
    {
        await base.Where_nested_property_access_closure_via_query_cache(async);
        AssertSql(
            """
@city_Nested_InstancePropertyValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstancePropertyValue
""",
            //
            """
@city_Nested_InstancePropertyValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @city_Nested_InstancePropertyValue
""");
    }

    public override async Task Where_nested_field_access_closure_via_query_cache_error_null(bool async)
    {
        await base.Where_nested_field_access_closure_via_query_cache_error_null(async);
        AssertSql();
    }

    public override async Task Where_nested_field_access_closure_via_query_cache_error_method_null(bool async)
    {
        await base.Where_nested_field_access_closure_via_query_cache_error_method_null(async);
        AssertSql();
    }

    public override async Task Where_new_instance_field_access_query_cache(bool async)
    {
        await base.Where_new_instance_field_access_query_cache(async);
        AssertSql(
            """
@InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""",
            //
            """
@InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""");
    }

    public override async Task Where_new_instance_field_access_closure_via_query_cache(bool async)
    {
        await base.Where_new_instance_field_access_closure_via_query_cache(async);
        AssertSql(
            """
@InstanceFieldValue='London' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""",
            //
            """
@InstanceFieldValue='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @InstanceFieldValue
""");
    }

    public override async Task Where_simple_closure_via_query_cache_nullable_type(bool async)
    {
        await base.Where_simple_closure_via_query_cache_nullable_type(async);
        AssertSql(
            """
@p='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
            //
            """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
            //
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""");
    }

    public override async Task Where_simple_closure_via_query_cache_nullable_type_reverse(bool async)
    {
        await base.Where_simple_closure_via_query_cache_nullable_type_reverse(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""",
            //
            """
@p='5' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""",
            //
            """
@p='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE CAST(`e`.`ReportsTo` AS signed) = @p
""");
    }

    public override async Task Where_subquery_closure_via_query_cache(bool async)
    {
        await base.Where_subquery_closure_via_query_cache(async);
        AssertSql(
            """
@customerID='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`CustomerID` = @customerID) AND (`o`.`CustomerID` = `c`.`CustomerID`))
""",
            //
            """
@customerID='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`o`.`CustomerID` = @customerID) AND (`o`.`CustomerID` = `c`.`CustomerID`))
""");
    }

    public override async Task Where_simple_shadow(bool async)
    {
        await base.Where_simple_shadow(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`Title` = 'Sales Representative'
""");
    }

    public override async Task Where_simple_shadow_projection(bool async)
    {
        await base.Where_simple_shadow_projection(async);
        AssertSql(
            """
SELECT `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`Title` = 'Sales Representative'
""");
    }

    public override async Task Where_simple_shadow_projection_mixed(bool async)
    {
        await base.Where_simple_shadow_projection_mixed(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`Title` = 'Sales Representative'
""");
    }

    public override async Task Where_simple_shadow_subquery(bool async)
    {
        await base.Where_simple_shadow_subquery(async);
        AssertSql(
            """
@p='5'

SELECT `e0`.`EmployeeID`, `e0`.`City`, `e0`.`Country`, `e0`.`FirstName`, `e0`.`ReportsTo`, `e0`.`Title`
FROM (
    SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
    FROM `Employees` AS `e`
    ORDER BY `e`.`EmployeeID`
    LIMIT @p
) AS `e0`
WHERE `e0`.`Title` = 'Sales Representative'
ORDER BY `e0`.`EmployeeID`
""");
    }

    public override async Task Where_shadow_subquery_FirstOrDefault(bool async)
    {
        await base.Where_shadow_subquery_FirstOrDefault(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE (`e`.`Title` = (
    SELECT `e0`.`Title`
    FROM `Employees` AS `e0`
    ORDER BY `e0`.`Title`
    LIMIT 1)) OR (`e`.`Title` IS NULL AND ((
    SELECT `e0`.`Title`
    FROM `Employees` AS `e0`
    ORDER BY `e0`.`Title`
    LIMIT 1) IS NULL))
""");
    }

    public override async Task Where_client(bool async)
    {
        await base.Where_client(async);
        AssertSql();
    }

    public override async Task Where_subquery_correlated(bool async)
    {
        await base.Where_subquery_correlated(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Customers` AS `c0`
    WHERE `c`.`CustomerID` = `c0`.`CustomerID`)
""");
    }

    public override async Task Where_subquery_correlated_client_eval(bool async)
    {
        await base.Where_subquery_correlated_client_eval(async);
        AssertSql();
    }

    public override async Task Where_client_and_server_top_level(bool async)
    {
        await base.Where_client_and_server_top_level(async);
        AssertSql();
    }

    public override async Task Where_client_or_server_top_level(bool async)
    {
        await base.Where_client_or_server_top_level(async);
        AssertSql();
    }

    public override async Task Where_client_and_server_non_top_level(bool async)
    {
        await base.Where_client_and_server_non_top_level(async);
        AssertSql();
    }

    public override async Task Where_client_deep_inside_predicate_and_server_top_level(bool async)
    {
        await base.Where_client_deep_inside_predicate_and_server_top_level(async);
        AssertSql();
    }

    public override async Task Where_equals_method_int(bool async)
    {
        await base.Where_equals_method_int(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`EmployeeID` = 1
""");
    }

    public override async Task Where_equals_using_object_overload_on_mismatched_types(bool async)
    {
        await base.Where_equals_using_object_overload_on_mismatched_types(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""");
    }

    public override async Task Where_equals_using_int_overload_on_mismatched_types(bool async)
    {
        await base.Where_equals_using_int_overload_on_mismatched_types(async);
        AssertSql(
            """
@p='1'

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`EmployeeID` = @p
""");
    }

    public override async Task Where_equals_on_mismatched_types_nullable_int_long(bool async)
    {
        await base.Where_equals_on_mismatched_types_nullable_int_long(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""",
            //
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""");
    }

    public override async Task Where_equals_on_mismatched_types_int_nullable_int(bool async)
    {
        await base.Where_equals_on_mismatched_types_int_nullable_int(async);
        AssertSql(
            """
@intPrm='2'

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` = @intPrm
""",
            //
            """
@intPrm='2'

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE @intPrm = `e`.`ReportsTo`
""");
    }

    public override async Task Where_equals_on_mismatched_types_nullable_long_nullable_int(bool async)
    {
        await base.Where_equals_on_mismatched_types_nullable_long_nullable_int(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""",
            //
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE FALSE
""");
    }

    public override async Task Where_equals_on_matched_nullable_int_types(bool async)
    {
        await base.Where_equals_on_matched_nullable_int_types(async);
        AssertSql(
            """
@nullableIntPrm='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE @nullableIntPrm = `e`.`ReportsTo`
""",
            //
            """
@nullableIntPrm='2' (Nullable = true)

SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` = @nullableIntPrm
""");
    }

    public override async Task Where_equals_on_null_nullable_int_types(bool async)
    {
        await base.Where_equals_on_null_nullable_int_types(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""",
            //
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""");
    }

    public override async Task Where_comparison_nullable_type_not_null(bool async)
    {
        await base.Where_comparison_nullable_type_not_null(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` = 2
""");
    }

    public override async Task Where_comparison_nullable_type_null(bool async)
    {
        await base.Where_comparison_nullable_type_null(async);
        AssertSql(
            """
SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Employees` AS `e`
WHERE `e`.`ReportsTo` IS NULL
""");
    }

    public override async Task Where_simple_reversed(bool async)
    {
        await base.Where_simple_reversed(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE 'London' = `c`.`City`
""");
    }

    public override async Task Where_is_null(bool async)
    {
        await base.Where_is_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL
""");
    }

    public override async Task Where_null_is_null(bool async)
    {
        await base.Where_null_is_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task Where_constant_is_null(bool async)
    {
        await base.Where_constant_is_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""");
    }

    public override async Task Where_is_not_null(bool async)
    {
        await base.Where_is_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` IS NOT NULL
""");
    }

    public override async Task Where_null_is_not_null(bool async)
    {
        await base.Where_null_is_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""");
    }

    public override async Task Where_constant_is_not_null(bool async)
    {
        await base.Where_constant_is_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task Where_identity_comparison(bool async)
    {
        await base.Where_identity_comparison(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`City` = `c`.`City`) OR `c`.`City` IS NULL
""");
    }

    public override async Task Where_in_optimization_multiple(bool async)
    {
        await base.Where_in_optimization_multiple(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Customers` AS `c`
CROSS JOIN `Employees` AS `e`
WHERE (`c`.`City` IN ('London', 'Berlin') OR (`c`.`CustomerID` = 'ALFKI')) OR (`c`.`CustomerID` = 'ABCDE')
""");
    }

    public override async Task Where_not_in_optimization1(bool async)
    {
        await base.Where_not_in_optimization1(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Customers` AS `c`
CROSS JOIN `Employees` AS `e`
WHERE ((`c`.`City` <> 'London') OR `c`.`City` IS NULL) AND ((`e`.`City` <> 'London') OR `e`.`City` IS NULL)
""");
    }

    public override async Task Where_not_in_optimization2(bool async)
    {
        await base.Where_not_in_optimization2(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Customers` AS `c`
CROSS JOIN `Employees` AS `e`
WHERE `c`.`City` NOT IN ('London', 'Berlin') OR (`c`.`City` IS NULL)
""");
    }

    public override async Task Where_not_in_optimization3(bool async)
    {
        await base.Where_not_in_optimization3(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Customers` AS `c`
CROSS JOIN `Employees` AS `e`
WHERE `c`.`City` NOT IN ('London', 'Berlin', 'Seattle') OR (`c`.`City` IS NULL)
""");
    }

    public override async Task Where_not_in_optimization4(bool async)
    {
        await base.Where_not_in_optimization4(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Customers` AS `c`
CROSS JOIN `Employees` AS `e`
WHERE `c`.`City` NOT IN ('London', 'Berlin', 'Seattle', 'Lisboa') OR (`c`.`City` IS NULL)
""");
    }

    public override async Task Where_select_many_and(bool async)
    {
        await base.Where_select_many_and(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`, `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
FROM `Customers` AS `c`
CROSS JOIN `Employees` AS `e`
WHERE ((`c`.`City` = 'London') AND (`c`.`Country` = 'UK')) AND ((`e`.`City` = 'London') AND (`e`.`Country` = 'UK'))
""");
    }

    public override async Task Where_primitive(bool async)
    {
        await base.Where_primitive(async);
        AssertSql(
            """
@p='9'

SELECT `e0`.`EmployeeID`
FROM (
    SELECT `e`.`EmployeeID`
    FROM `Employees` AS `e`
    LIMIT @p
) AS `e0`
WHERE `e0`.`EmployeeID` = 5
""");
    }

    public override async Task Where_primitive_tracked(bool async)
    {
        await base.Where_primitive_tracked(async);
        AssertSql(
            """
@p='9'

SELECT `e0`.`EmployeeID`, `e0`.`City`, `e0`.`Country`, `e0`.`FirstName`, `e0`.`ReportsTo`, `e0`.`Title`
FROM (
    SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
    FROM `Employees` AS `e`
    LIMIT @p
) AS `e0`
WHERE `e0`.`EmployeeID` = 5
""");
    }

    public override async Task Where_primitive_tracked2(bool async)
    {
        await base.Where_primitive_tracked2(async);
        AssertSql(
            """
@p='9'

SELECT `e0`.`EmployeeID`, `e0`.`City`, `e0`.`Country`, `e0`.`FirstName`, `e0`.`ReportsTo`, `e0`.`Title`
FROM (
    SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
    FROM `Employees` AS `e`
    LIMIT @p
) AS `e0`
WHERE `e0`.`EmployeeID` = 5
""");
    }

    public override async Task Where_bool_member(bool async)
    {
        await base.Where_bool_member(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued`
""");
    }

    public override async Task Where_bool_member_false(bool async)
    {
        await base.Where_bool_member_false(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE NOT (`p`.`Discontinued`)
""");
    }

    public override async Task Where_bool_member_negated_twice(bool async)
    {
        await base.Where_bool_member_negated_twice(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued`
""");
    }

    public override async Task Where_bool_member_shadow(bool async)
    {
        await base.Where_bool_member_shadow(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued`
""");
    }

    public override async Task Where_bool_member_false_shadow(bool async)
    {
        await base.Where_bool_member_false_shadow(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE NOT (`p`.`Discontinued`)
""");
    }

    public override async Task Where_bool_member_equals_constant(bool async)
    {
        await base.Where_bool_member_equals_constant(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued`
""");
    }

    public override async Task Where_bool_member_in_complex_predicate(bool async)
    {
        await base.Where_bool_member_in_complex_predicate(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE ((`p`.`ProductID` > 100) AND `p`.`Discontinued`) OR `p`.`Discontinued`
""");
    }

    public override async Task Where_bool_member_compared_to_binary_expression(bool async)
    {
        await base.Where_bool_member_compared_to_binary_expression(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued` = (`p`.`ProductID` > 50)
""");
    }

    public override async Task Where_not_bool_member_compared_to_not_bool_member(bool async)
    {
        await base.Where_not_bool_member_compared_to_not_bool_member(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
""");
    }

    public override async Task Where_negated_boolean_expression_compared_to_another_negated_boolean_expression(bool async)
    {
        await base.Where_negated_boolean_expression_compared_to_another_negated_boolean_expression(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE (`p`.`ProductID` <= 50) = (`p`.`ProductID` <= 20)
""");
    }

    public override async Task Where_not_bool_member_compared_to_binary_expression(bool async)
    {
        await base.Where_not_bool_member_compared_to_binary_expression(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued` <> (`p`.`ProductID` > 50)
""");
    }

    public override async Task Where_bool_parameter(bool async)
    {
        await base.Where_bool_parameter(async);
        AssertSql(
            """
@prm='True'

SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE @prm
""");
    }

    public override async Task Where_bool_parameter_compared_to_binary_expression(bool async)
    {
        await base.Where_bool_parameter_compared_to_binary_expression(async);
        AssertSql(
            """
@prm='True'

SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE (`p`.`ProductID` > 50) <> @prm
""");
    }

    public override async Task Where_bool_member_and_parameter_compared_to_binary_expression_nested(bool async)
    {
        await base.Where_bool_member_and_parameter_compared_to_binary_expression_nested(async);
        AssertSql(
            """
@prm='True'

SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`Discontinued` = ((`p`.`ProductID` > 50) <> @prm)
""");
    }

    public override async Task Where_de_morgan_or_optimized(bool async)
    {
        await base.Where_de_morgan_or_optimized(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE NOT (`p`.`Discontinued`) AND (`p`.`ProductID` >= 20)
""");
    }

    public override async Task Where_de_morgan_and_optimized(bool async)
    {
        await base.Where_de_morgan_and_optimized(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE NOT (`p`.`Discontinued`) OR (`p`.`ProductID` >= 20)
""");
    }

    public override async Task Where_complex_negated_expression_optimized(bool async)
    {
        await base.Where_complex_negated_expression_optimized(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE (NOT (`p`.`Discontinued`) AND (`p`.`ProductID` < 60)) AND (`p`.`ProductID` > 30)
""");
    }

    public override async Task Where_short_member_comparison(bool async)
    {
        await base.Where_short_member_comparison(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`UnitsInStock` > 10
""");
    }

    public override async Task Where_comparison_to_nullable_bool(bool async)
    {
        await base.Where_comparison_to_nullable_bool(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` LIKE '%KI'
""");
    }

    public override async Task Where_true(bool async)
    {
        await base.Where_true(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task Where_false(bool async)
    {
        await base.Where_false(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""");
    }

    public override async Task Where_bool_closure(bool async)
    {
        await base.Where_bool_closure(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""",
            //
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""",
            //
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""",
            //
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task Where_poco_closure(bool async)
    {
        await base.Where_poco_closure(async);
        AssertSql(
            """
@entity_equality_customer_CustomerID='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @entity_equality_customer_CustomerID
""",
            //
            """
@entity_equality_customer_CustomerID='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @entity_equality_customer_CustomerID
""");
    }

    public override async Task Where_default(bool async)
    {
        await base.Where_default(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Fax` IS NULL
""");
    }

    public override async Task Where_expression_invoke_1(bool async)
    {
        await base.Where_expression_invoke_1(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task Where_expression_invoke_2(bool async)
    {
        await base.Where_expression_invoke_2(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
LEFT JOIN `Customers` AS `c` ON `o`.`CustomerID` = `c`.`CustomerID`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task Where_expression_invoke_3(bool async)
    {
        await base.Where_expression_invoke_3(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task Where_ternary_boolean_condition_true(bool async)
    {
        await base.Where_ternary_boolean_condition_true(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`UnitsInStock` >= 20
""");
    }

    public override async Task Where_ternary_boolean_condition_false(bool async)
    {
        await base.Where_ternary_boolean_condition_false(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`UnitsInStock` < 20
""");
    }

    public override async Task Where_ternary_boolean_condition_with_another_condition(bool async)
    {
        await base.Where_ternary_boolean_condition_with_another_condition(async);
        AssertSql(
            """
@productId='15'

SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE (`p`.`ProductID` < @productId) AND (`p`.`UnitsInStock` >= 20)
""");
    }

    public override async Task Where_ternary_boolean_condition_with_false_as_result_true(bool async)
    {
        await base.Where_ternary_boolean_condition_with_false_as_result_true(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE `p`.`UnitsInStock` >= 20
""");
    }

    public override async Task Where_ternary_boolean_condition_with_false_as_result_false(bool async)
    {
        await base.Where_ternary_boolean_condition_with_false_as_result_false(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE FALSE
""");
    }

    public override async Task Where_ternary_boolean_condition_negated(bool async)
    {
        await base.Where_ternary_boolean_condition_negated(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE CASE
    WHEN `p`.`UnitsInStock` >= 20 THEN TRUE
    ELSE FALSE
END
""");
    }

    public override async Task Where_compare_null(bool async)
    {
        await base.Where_compare_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL AND (`c`.`Country` = 'UK')
""");
    }

    public override async Task Where_compare_null_with_cast_to_object(bool async)
    {
        await base.Where_compare_null_with_cast_to_object(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL
""");
    }

    public override async Task Where_compare_with_both_cast_to_object(bool async)
    {
        await base.Where_compare_with_both_cast_to_object(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
""");
    }

    public override async Task Where_projection(bool async)
    {
        await base.Where_projection(async);
        AssertSql(
            """
SELECT `c`.`CompanyName`
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
""");
    }

    public override async Task Where_Is_on_same_type(bool async)
    {
        await base.Where_Is_on_same_type(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task Where_chain(bool async)
    {
        await base.Where_chain(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE (`o`.`CustomerID` = 'QUICK') AND (`o`.`OrderDate` > TIMESTAMP '1998-01-01 00:00:00')
""");
    }

    public override async Task Where_navigation_contains(bool async)
    {
        await base.Where_navigation_contains(async);
        AssertSql(
            """
SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`, `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    WHERE `c`.`CustomerID` = 'ALFKI'
    LIMIT 2
) AS `c0`
LEFT JOIN `Orders` AS `o` ON `c0`.`CustomerID` = `o`.`CustomerID`
ORDER BY `c0`.`CustomerID`
""",
            //
            """
@entity_equality_customer_Orders_OrderID1='10643'
@entity_equality_customer_Orders_OrderID2='10692'
@entity_equality_customer_Orders_OrderID3='10702'
@entity_equality_customer_Orders_OrderID4='10835'
@entity_equality_customer_Orders_OrderID5='10952'
@entity_equality_customer_Orders_OrderID6='11011'
@entity_equality_customer_Orders_OrderID7='11011'
@entity_equality_customer_Orders_OrderID8='11011'
@entity_equality_customer_Orders_OrderID9='11011'
@entity_equality_customer_Orders_OrderID10='11011'

SELECT `o`.`OrderID`, `o`.`ProductID`, `o`.`Discount`, `o`.`Quantity`, `o`.`UnitPrice`
FROM `Order Details` AS `o`
INNER JOIN `Orders` AS `o0` ON `o`.`OrderID` = `o0`.`OrderID`
WHERE `o0`.`OrderID` IN (@entity_equality_customer_Orders_OrderID1, @entity_equality_customer_Orders_OrderID2, @entity_equality_customer_Orders_OrderID3, @entity_equality_customer_Orders_OrderID4, @entity_equality_customer_Orders_OrderID5, @entity_equality_customer_Orders_OrderID6, @entity_equality_customer_Orders_OrderID7, @entity_equality_customer_Orders_OrderID8, @entity_equality_customer_Orders_OrderID9, @entity_equality_customer_Orders_OrderID10)
""");
    }

    public override async Task Where_array_index(bool async)
    {
        await base.Where_array_index(async);
        AssertSql(
            """
@p='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @p
""");
    }

    public override async Task Where_contains_on_navigation(bool async)
    {
        await base.Where_contains_on_navigation(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE EXISTS (
    SELECT 1
    FROM `Customers` AS `c`
    WHERE `o`.`OrderID` IN (
        SELECT `o0`.`OrderID`
        FROM `Orders` AS `o0`
        WHERE `c`.`CustomerID` = `o0`.`CustomerID`
    ))
""");
    }

    public override async Task Where_subquery_FirstOrDefault_is_null(bool async)
    {
        await base.Where_subquery_FirstOrDefault_is_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task Where_subquery_FirstOrDefault_compared_to_entity(bool async)
    {
        await base.Where_subquery_FirstOrDefault_compared_to_entity(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    ORDER BY `o`.`OrderID`
    LIMIT 1) = 10276
""");
    }

    public override async Task TypeBinary_short_circuit(bool async)
    {
        await base.TypeBinary_short_circuit(async);
        AssertSql(
            """
@p='False'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE @p
""");
    }

    public override async Task Decimal_cast_to_double_works(bool async)
    {
        await base.Decimal_cast_to_double_works(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE CAST(`p`.`UnitPrice` AS double) > 100.0
""");
    }

    public override async Task Where_is_conditional(bool async)
    {
        await base.Where_is_conditional(async);
        AssertSql(
            """
SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`
FROM `Products` AS `p`
WHERE FALSE
""");
    }

    public override async Task Enclosing_class_settable_member_generates_parameter(bool async)
    {
        await base.Enclosing_class_settable_member_generates_parameter(async);
        AssertSql(
            """
@SettableProperty='10274'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = @SettableProperty
""",
            //
            """
@SettableProperty='10275'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = @SettableProperty
""");
    }

    public override async Task Enclosing_class_readonly_member_generates_parameter(bool async)
    {
        await base.Enclosing_class_readonly_member_generates_parameter(async);
        AssertSql(
            """
@ReadOnlyProperty='10275'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = @ReadOnlyProperty
""");
    }

    public override async Task Enclosing_class_const_member_does_not_generate_parameter(bool async)
    {
        await base.Enclosing_class_const_member_does_not_generate_parameter(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = 10274
""");
    }

    public override async Task Generic_Ilist_contains_translates_to_server(bool async)
    {
        await base.Generic_Ilist_contains_translates_to_server(async);
        AssertSql(
            """
@cities1='Seattle' (Size = 15)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`City` = @cities1
""");
    }

    public override async Task Filter_non_nullable_value_after_FirstOrDefault_on_empty_collection(bool async)
    {
        await base.Filter_non_nullable_value_after_FirstOrDefault_on_empty_collection(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (
    SELECT CHAR_LENGTH(`o`.`CustomerID`)
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = 'John Doe'
    LIMIT 1) = 0
""");
    }

    public override async Task Two_parameters_with_same_name_get_uniquified(bool async)
    {
        await base.Two_parameters_with_same_name_get_uniquified(async);
        AssertSql(
            """
@customerId='ANATR' (Size = 5) (DbType = StringFixedLength)
@customerId0='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` = @customerId) OR (`c`.`CustomerID` = @customerId0)
""");
    }

    public override async Task Two_parameters_with_same_case_insensitive_name_get_uniquified(bool async)
    {
        await base.Two_parameters_with_same_case_insensitive_name_get_uniquified(async);
        AssertSql(
            """
@customerID='ANATR' (Size = 5) (DbType = StringFixedLength)
@customerId0='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`CustomerID` = @customerID) OR (`c`.`CustomerID` = @customerId0)
""");
    }

    public override async Task Where_Queryable_ToList_Count(bool async)
    {
        await base.Where_Queryable_ToList_Count(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE (
    SELECT COUNT(*)
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`) = 0
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_ToList_Contains(bool async)
    {
        await base.Where_Queryable_ToList_Contains(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`CustomerID`, `o0`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE 'ALFKI' IN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`
)
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_ToArray_Count(bool async)
    {
        await base.Where_Queryable_ToArray_Count(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE (
    SELECT COUNT(*)
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`) = 0
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_ToArray_Contains(bool async)
    {
        await base.Where_Queryable_ToArray_Contains(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`CustomerID`, `o0`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE 'ALFKI' IN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`
)
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_AsEnumerable_Count(bool async)
    {
        await base.Where_Queryable_AsEnumerable_Count(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE (
    SELECT COUNT(*)
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`) = 0
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_AsEnumerable_Contains(bool async)
    {
        await base.Where_Queryable_AsEnumerable_Contains(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`CustomerID`, `o0`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE 'ALFKI' IN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`
)
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_AsEnumerable_Contains_negated(bool async)
    {
        await base.Where_Queryable_AsEnumerable_Contains_negated(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`CustomerID`, `o0`.`OrderID`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE NOT (COALESCE('ALFKI' IN (
    SELECT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`
), FALSE))
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_ToList_Count_member(bool async)
    {
        await base.Where_Queryable_ToList_Count_member(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE (
    SELECT COUNT(*)
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`) = 0
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_Queryable_ToArray_Length_member(bool async)
    {
        await base.Where_Queryable_ToArray_Length_member(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE (
    SELECT COUNT(*)
    FROM `Orders` AS `o`
    WHERE `o`.`CustomerID` = `c`.`CustomerID`) = 0
ORDER BY `c`.`CustomerID`
""");
    }

    public override async Task Where_collection_navigation_ToList_Count(bool async)
    {
        await base.Where_collection_navigation_ToList_Count(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
FROM `Orders` AS `o`
LEFT JOIN `Order Details` AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
WHERE (`o`.`OrderID` < 10300) AND ((
    SELECT COUNT(*)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) = 4)
ORDER BY `o`.`OrderID`, `o1`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_ToList_Contains(bool async)
    {
        await base.Where_collection_navigation_ToList_Contains(async);
        AssertSql(
            """
@entity_equality_order_OrderID='10248' (Nullable = true)

SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`c`.`CustomerID` = `o`.`CustomerID`) AND (`o`.`OrderID` = @entity_equality_order_OrderID))
ORDER BY `c`.`CustomerID`, `o0`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_ToArray_Count(bool async)
    {
        await base.Where_collection_navigation_ToArray_Count(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
FROM `Orders` AS `o`
LEFT JOIN `Order Details` AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
WHERE (`o`.`OrderID` < 10300) AND ((
    SELECT COUNT(*)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) = 4)
ORDER BY `o`.`OrderID`, `o1`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_ToArray_Contains(bool async)
    {
        await base.Where_collection_navigation_ToArray_Contains(async);
        AssertSql(
            """
@entity_equality_order_OrderID='10248' (Nullable = true)

SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`c`.`CustomerID` = `o`.`CustomerID`) AND (`o`.`OrderID` = @entity_equality_order_OrderID))
ORDER BY `c`.`CustomerID`, `o0`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_AsEnumerable_Count(bool async)
    {
        await base.Where_collection_navigation_AsEnumerable_Count(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
FROM `Orders` AS `o`
LEFT JOIN `Order Details` AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
WHERE (`o`.`OrderID` < 10300) AND ((
    SELECT COUNT(*)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) = 5)
ORDER BY `o`.`OrderID`, `o1`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_AsEnumerable_Contains(bool async)
    {
        await base.Where_collection_navigation_AsEnumerable_Contains(async);
        AssertSql(
            """
@entity_equality_order_OrderID='10248' (Nullable = true)

SELECT `c`.`CustomerID`, `o0`.`OrderID`, `o0`.`CustomerID`, `o0`.`EmployeeID`, `o0`.`OrderDate`
FROM `Customers` AS `c`
LEFT JOIN `Orders` AS `o0` ON `c`.`CustomerID` = `o0`.`CustomerID`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE (`c`.`CustomerID` = `o`.`CustomerID`) AND (`o`.`OrderID` = @entity_equality_order_OrderID))
ORDER BY `c`.`CustomerID`, `o0`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_ToList_Count_member(bool async)
    {
        await base.Where_collection_navigation_ToList_Count_member(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
FROM `Orders` AS `o`
LEFT JOIN `Order Details` AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
WHERE (`o`.`OrderID` < 10300) AND ((
    SELECT COUNT(*)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) = 3)
ORDER BY `o`.`OrderID`, `o1`.`OrderID`
""");
    }

    public override async Task Where_collection_navigation_ToArray_Length_member(bool async)
    {
        await base.Where_collection_navigation_ToArray_Length_member(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o1`.`OrderID`, `o1`.`ProductID`, `o1`.`Discount`, `o1`.`Quantity`, `o1`.`UnitPrice`
FROM `Orders` AS `o`
LEFT JOIN `Order Details` AS `o1` ON `o`.`OrderID` = `o1`.`OrderID`
WHERE (`o`.`OrderID` < 10300) AND ((
    SELECT COUNT(*)
    FROM `Order Details` AS `o0`
    WHERE `o`.`OrderID` = `o0`.`OrderID`) = 3)
ORDER BY `o`.`OrderID`, `o1`.`OrderID`
""");
    }

    public override async Task Where_list_object_contains_over_value_type(bool async)
    {
        await base.Where_list_object_contains_over_value_type(async);
        AssertSql(
            """
@orderIds1='10248'
@orderIds2='10249'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` IN (@orderIds1, @orderIds2)
""");
    }

    public override async Task Where_array_of_object_contains_over_value_type(bool async)
    {
        await base.Where_array_of_object_contains_over_value_type(async);
        AssertSql(
            """
@orderIds1='10248'
@orderIds2='10249'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` IN (@orderIds1, @orderIds2)
""");
    }

    public override async Task Multiple_OrElse_on_same_column_converted_to_in_with_overlap(bool async)
    {
        await base.Multiple_OrElse_on_same_column_converted_to_in_with_overlap(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN ('ALFKI', 'ANATR', 'ANTON')
""");
    }

    public override async Task Multiple_OrElse_on_same_column_with_null_constant_comparison_converted_to_in(bool async)
    {
        await base.Multiple_OrElse_on_same_column_with_null_constant_comparison_converted_to_in(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IN ('WA', 'OR', 'BC') OR (`c`.`Region` IS NULL)
""");
    }

    public override async Task Constant_array_Contains_OrElse_comparison_with_constant_gets_combined_to_one_in(bool async)
    {
        await base.Constant_array_Contains_OrElse_comparison_with_constant_gets_combined_to_one_in(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN ('ALFKI', 'ANATR', 'ANTON')
""");
    }

    public override async Task Constant_array_Contains_OrElse_comparison_with_constant_gets_combined_to_one_in_with_overlap(bool async)
    {
        await base.Constant_array_Contains_OrElse_comparison_with_constant_gets_combined_to_one_in_with_overlap(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN ('ANTON', 'ALFKI', 'ANATR')
""");
    }

    public override async Task Constant_array_Contains_OrElse_another_Contains_gets_combined_to_one_in_with_overlap(bool async)
    {
        await base.Constant_array_Contains_OrElse_another_Contains_gets_combined_to_one_in_with_overlap(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN ('ALFKI', 'ANATR', 'ANTON')
""");
    }

    public override async Task Constant_array_Contains_AndAlso_another_Contains_gets_combined_to_one_in_with_overlap(bool async)
    {
        await base.Constant_array_Contains_AndAlso_another_Contains_gets_combined_to_one_in_with_overlap(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` NOT IN ('ALFKI', 'ANATR', 'ANTON')
""");
    }

    public override async Task Multiple_AndAlso_on_same_column_converted_to_in_using_parameters(bool async)
    {
        await base.Multiple_AndAlso_on_same_column_converted_to_in_using_parameters(async);
        AssertSql(
            """
@prm1='ALFKI' (Size = 5) (DbType = StringFixedLength)
@prm2='ANATR' (Size = 5) (DbType = StringFixedLength)
@prm3='ANTON' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` <> @prm1) AND (`c`.`CustomerID` <> @prm2)) AND (`c`.`CustomerID` <> @prm3)
""");
    }

    public override async Task Array_of_parameters_Contains_OrElse_comparison_with_constant_gets_combined_to_one_in(bool async)
    {
        await base.Array_of_parameters_Contains_OrElse_comparison_with_constant_gets_combined_to_one_in(async);
        AssertSql(
            """
@prm1='ALFKI' (Size = 5) (DbType = StringFixedLength)
@prm2='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN (@prm1, @prm2, 'ANTON')
""");
    }

    public override async Task Multiple_OrElse_on_same_column_with_null_parameter_comparison_converted_to_in(bool async)
    {
        await base.Multiple_OrElse_on_same_column_with_null_parameter_comparison_converted_to_in(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (`c`.`Region` IN ('WA', 'OR') OR (`c`.`Region` IS NULL)) OR (`c`.`Region` = 'BC')
""");
    }

    public override async Task Parameter_array_Contains_OrElse_comparison_with_constant(bool async)
    {
        await base.Parameter_array_Contains_OrElse_comparison_with_constant(async);
        AssertSql(
            """
@array1='ALFKI' (Size = 5) (DbType = StringFixedLength)
@array2='ANATR' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN (@array1, @array2) OR (`c`.`CustomerID` = 'ANTON')
""");
    }

    public override async Task Parameter_array_Contains_OrElse_comparison_with_parameter_with_overlap(bool async)
    {
        await base.Parameter_array_Contains_OrElse_comparison_with_parameter_with_overlap(async);
        AssertSql(
            """
@prm1='ANTON' (Size = 5) (DbType = StringFixedLength)
@array1='ALFKI' (Size = 5) (DbType = StringFixedLength)
@array2='ANATR' (Size = 5) (DbType = StringFixedLength)
@prm2='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE ((`c`.`CustomerID` = @prm1) OR `c`.`CustomerID` IN (@array1, @array2)) OR (`c`.`CustomerID` = @prm2)
""");
    }

    public override async Task Two_sets_of_comparison_combine_correctly(bool async)
    {
        await base.Two_sets_of_comparison_combine_correctly(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ANATR'
""");
    }

    public override async Task Two_sets_of_comparison_combine_correctly2(bool async)
    {
        await base.Two_sets_of_comparison_combine_correctly2(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NOT NULL AND (`c`.`Region` <> 'WA')
""");
    }

    public override async Task Filter_with_property_compared_to_null_wrapped_in_explicit_convert_to_object(bool async)
    {
        await base.Filter_with_property_compared_to_null_wrapped_in_explicit_convert_to_object(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`Region` IS NULL
""");
    }

    public override async Task Filter_with_EF_Property_using_closure_for_property_name(bool async)
    {
        await base.Filter_with_EF_Property_using_closure_for_property_name(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task Filter_with_EF_Property_using_function_for_property_name(bool async)
    {
        await base.Filter_with_EF_Property_using_function_for_property_name(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task FirstOrDefault_over_scalar_projection_compared_to_null(bool async)
    {
        await base.FirstOrDefault_over_scalar_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    LIMIT 1) IS NULL
""");
    }

    public override async Task FirstOrDefault_over_scalar_projection_compared_to_not_null(bool async)
    {
        await base.FirstOrDefault_over_scalar_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    LIMIT 1) IS NOT NULL
""");
    }

    public override async Task FirstOrDefault_over_custom_projection_compared_to_null(bool async)
    {
        await base.FirstOrDefault_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task FirstOrDefault_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.FirstOrDefault_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task SingleOrDefault_over_custom_projection_compared_to_null(bool async)
    {
        await base.SingleOrDefault_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task SingleOrDefault_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.SingleOrDefault_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task LastOrDefault_over_custom_projection_compared_to_null(bool async)
    {
        await base.LastOrDefault_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task LastOrDefault_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.LastOrDefault_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task First_over_custom_projection_compared_to_null(bool async)
    {
        await base.First_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task First_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.First_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task ElementAt_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.ElementAt_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    LIMIT 18446744073709551610 OFFSET 3)
""");
    }

    public override async Task ElementAtOrDefault_over_custom_projection_compared_to_null(bool async)
    {
        await base.ElementAtOrDefault_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`
    LIMIT 18446744073709551610 OFFSET 7)
""");
    }

    public override async Task Single_over_custom_projection_compared_to_null(bool async)
    {
        await base.Single_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task Single_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.Single_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task Last_over_custom_projection_compared_to_null(bool async)
    {
        await base.Last_over_custom_projection_compared_to_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task Last_over_custom_projection_compared_to_not_null(bool async)
    {
        await base.Last_over_custom_projection_compared_to_not_null(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`)
""");
    }

    public override async Task Where_Contains_and_comparison(bool async)
    {
        await base.Where_Contains_and_comparison(async);
        AssertSql(
            """
@customerIds1='ALFKI' (Size = 5) (DbType = StringFixedLength)
@customerIds2='FISSA' (Size = 5) (DbType = StringFixedLength)
@customerIds3='WHITC' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN (@customerIds1, @customerIds2, @customerIds3) AND (`c`.`City` = 'Seattle')
""");
    }

    public override async Task Where_Contains_or_comparison(bool async)
    {
        await base.Where_Contains_or_comparison(async);
        AssertSql(
            """
@customerIds1='ALFKI' (Size = 5) (DbType = StringFixedLength)
@customerIds2='FISSA' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` IN (@customerIds1, @customerIds2) OR (`c`.`City` = 'Seattle')
""");
    }

    public override async Task GetType_on_non_hierarchy1(bool async)
    {
        await base.GetType_on_non_hierarchy1(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task GetType_on_non_hierarchy2(bool async)
    {
        await base.GetType_on_non_hierarchy2(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""");
    }

    public override async Task GetType_on_non_hierarchy3(bool async)
    {
        await base.GetType_on_non_hierarchy3(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE FALSE
""");
    }

    public override async Task GetType_on_non_hierarchy4(bool async)
    {
        await base.GetType_on_non_hierarchy4(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
""");
    }

    public override async Task Case_block_simplification_works_correctly(bool async)
    {
        await base.Case_block_simplification_works_correctly(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE CASE
    WHEN `c`.`Region` IS NULL THEN 'OR'
    ELSE `c`.`Region`
END = 'OR'
""");
    }

    public override async Task EF_Constant(bool async)
    {
        await base.EF_Constant(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task EF_Constant_with_subtree(bool async)
    {
        await base.EF_Constant_with_subtree(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = 'ALFKI'
""");
    }

    public override async Task EF_Constant_does_not_parameterized_as_part_of_bigger_subtree(bool async)
    {
        await base.EF_Constant_does_not_parameterized_as_part_of_bigger_subtree(async);
        AssertSql(
            """
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = (CONCAT('ALF', 'KI'))
""");
    }

    public override async Task EF_Constant_with_non_evaluatable_argument_throws(bool async)
    {
        await base.EF_Constant_with_non_evaluatable_argument_throws(async);
        AssertSql();
    }

    public override async Task EF_Parameter(bool async)
    {
        await base.EF_Parameter(async);
        AssertSql(
            """
@p='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @p
""");
    }

    public override async Task EF_Parameter_with_subtree(bool async)
    {
        await base.EF_Parameter_with_subtree(async);
        AssertSql(
            """
@p='ALFKI' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = @p
""");
    }

    public override async Task EF_Parameter_does_not_parameterized_as_part_of_bigger_subtree(bool async)
    {
        await base.EF_Parameter_does_not_parameterized_as_part_of_bigger_subtree(async);
        AssertSql(
            """
@id='ALF' (Size = 5) (DbType = StringFixedLength)

SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
FROM `Customers` AS `c`
WHERE `c`.`CustomerID` = (CONCAT(@id, 'KI'))
""");
    }

    public override async Task EF_Parameter_with_non_evaluatable_argument_throws(bool async)
    {
        await base.EF_Parameter_with_non_evaluatable_argument_throws(async);
        AssertSql();
    }

    public override async Task Implicit_cast_in_predicate(bool async)
    {
        await base.Implicit_cast_in_predicate(async);
        AssertSql(
            """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` = '1337'
""",
            //
            """
@prm_Value='1337' (Size = 5) (DbType = StringFixedLength)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` = @prm_Value
""",
            //
            """
@ToString='1337' (Size = 5) (DbType = StringFixedLength)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` = @ToString
""",
            //
            """
@p='1337' (Size = 5) (DbType = StringFixedLength)

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` = @p
""",
            //
            """
SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`CustomerID` = '1337'
""");
    }

    public override async Task Interface_casting_though_generic_method(bool async)
    {
        await base.Interface_casting_though_generic_method(async);
        AssertSql(
            """
@id='10252'

SELECT `o`.`OrderID` AS `Id`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = @id
""",
            //
            """
SELECT `o`.`OrderID` AS `Id`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = 10252
""",
            //
            """
SELECT `o`.`OrderID` AS `Id`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = 10252
""",
            //
            """
SELECT `o`.`OrderID` AS `Id`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = 10252
""");
    }

    public override async Task Simplifiable_coalesce_over_nullable(bool async)
    {
        await base.Simplifiable_coalesce_over_nullable(async);
        AssertSql(
            """
@p='10248'

SELECT `o`.`OrderID`, `o`.`CustomerID`, `o`.`EmployeeID`, `o`.`OrderDate`
FROM `Orders` AS `o`
WHERE `o`.`OrderID` = @p
""");
    }

    public override async Task Take_and_Where_evaluation_order(bool async)
    {
        await base.Take_and_Where_evaluation_order(async);
        AssertSql(
            """
@p='3'

SELECT `e0`.`EmployeeID`, `e0`.`City`, `e0`.`Country`, `e0`.`FirstName`, `e0`.`ReportsTo`, `e0`.`Title`
FROM (
    SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
    FROM `Employees` AS `e`
    ORDER BY `e`.`EmployeeID`
    LIMIT @p
) AS `e0`
WHERE (`e0`.`EmployeeID` % 2) = 0
ORDER BY `e0`.`EmployeeID`
""");
    }

    public override async Task Skip_and_Where_evaluation_order(bool async)
    {
        await base.Skip_and_Where_evaluation_order(async);
        AssertSql(
            """
@p='3'

SELECT `e0`.`EmployeeID`, `e0`.`City`, `e0`.`Country`, `e0`.`FirstName`, `e0`.`ReportsTo`, `e0`.`Title`
FROM (
    SELECT `e`.`EmployeeID`, `e`.`City`, `e`.`Country`, `e`.`FirstName`, `e`.`ReportsTo`, `e`.`Title`
    FROM `Employees` AS `e`
    ORDER BY `e`.`EmployeeID`
    LIMIT 18446744073709551610 OFFSET @p
) AS `e0`
WHERE (`e0`.`EmployeeID` % 2) = 0
ORDER BY `e0`.`EmployeeID`
""");
    }

    public override async Task Take_and_Distinct_evaluation_order(bool async)
    {
        await base.Take_and_Distinct_evaluation_order(async);
        AssertSql(
            """
@p='3'

SELECT DISTINCT `c0`.`ContactTitle`
FROM (
    SELECT `c`.`ContactTitle`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactTitle`
    LIMIT @p
) AS `c0`
""");
    }

    public override async Task<string> Where_simple_closure(bool async)
    {
        return await base.Where_simple_closure(async);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
            => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
