using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Xunit;

// ReSharper disable RedundantBoolCompare
// ReSharper disable NegativeEqualityExpression

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class GearsOfWarQueryMySqlTest
    {
        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> FALSE");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where !w.IsAutomatic
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE NOT (`w`.`IsAutomatic`)");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_equals_true(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic == true
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = TRUE");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_equals_false(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic == false
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = FALSE");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_equals_true(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic != true
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> TRUE");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_equals_false(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic != false
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> FALSE");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_parenthesis_equals_true(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where !(w.IsAutomatic == true)
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> TRUE");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_parenthesis_equals_false(bool isAsync)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                isAsync,
                ss => from w in ss.Set<Weapon>()
                    where !(w.IsAutomatic == false)
                    select w.Name);

            AssertSql(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> FALSE");
        }
    }
}
