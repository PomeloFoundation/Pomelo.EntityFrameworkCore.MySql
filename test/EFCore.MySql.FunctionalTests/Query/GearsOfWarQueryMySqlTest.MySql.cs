using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Xunit;

// ReSharper disable RedundantBoolCompare
// ReSharper disable NegativeEqualityExpression

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class GearsOfWarQueryMySqlTest
    {
        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        private void AssertKeyUsage(string sql, params string[] keys)
        {
            if (keys.Length <= 0)
            {
                return;
            }

            var keysUsed = new HashSet<string>();

            using var context = CreateContext();
            var connection = context.Database.GetDbConnection();

            using var command = connection.CreateCommand();
            command.CommandText = "EXPLAIN " + Regex.Replace(
                sql,
                @"\r?\nFROM (?:`.*?`\.)?`.*?`(?: AS `.*?`)?(?=$|\r?\n)",
                $@"$0{Environment.NewLine}FORCE INDEX ({string.Join(", ", keys.Select(s => $"`{s}`"))})",
                RegexOptions.IgnoreCase);

            using var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var key = dataReader.GetValueOrDefault<string>("key");

                if (!string.IsNullOrEmpty(key))
                {
                    keysUsed.Add(key);
                }
            }

            Assert.Empty(keys.Except(keysUsed, StringComparer.OrdinalIgnoreCase));
        }

        private void AssertSingleStatementWithKeyUsage(string expected, params string[] keys)
        {
            AssertSql(expected);
            AssertKeyUsage(expected, keys);
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic
                    select w.Name);

            AssertSingleStatementWithKeyUsage(@"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = TRUE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where !w.IsAutomatic
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = FALSE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_equals_true(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic == true
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = TRUE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_equals_false(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic == false
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = FALSE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_equals_true(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic != true
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> TRUE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_equals_false(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where w.IsAutomatic != false
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> FALSE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_parenthesis_equals_true(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where !(w.IsAutomatic == true)
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> TRUE",
                "IX_Weapons_IsAutomatic");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Where_bool_optimization_not_parenthesis_equals_false(bool async)
        {
            // Relates to MySqlBoolOptimizingExpressionVisitor.
            await AssertQuery(
                async,
                ss => from w in ss.Set<Weapon>()
                    where !(w.IsAutomatic == false)
                    select w.Name);

            AssertSingleStatementWithKeyUsage(
                @"SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` <> FALSE",
                "IX_Weapons_IsAutomatic");
        }
    }
}
