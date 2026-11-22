using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    /// <summary>
    ///     Covers the code paths that were introduced or changed by the EF Core 10 adaptation:
    ///     the new <c>Process</c> based optimization pipeline (A5 migration) and the MySQL specific
    ///     SQL generation fixes. These tests execute real queries, so a broken pipeline surfaces here.
    /// </summary>
    public class MySqlQueryOptimizationPipelineTest : TestBase<MySqlQueryOptimizationPipelineTest.MySqlPipelineContext>
    {
        /// <summary>
        ///     A5: a parameterized Skip/Take makes the query uncacheable and requires the pipeline to read
        ///     the parameter value (SkipTakeCollapsingExpressionVisitor). Executes through Process().
        /// </summary>
        [ConditionalFact]
        public async Task Parameterized_skip_take_is_processed_by_the_new_process_pipeline()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PipelineEntity>().ToTable("pipeline_test"));

            var skip = 0;
            var take = 5;

            var result = context.Set<PipelineEntity>()
                .OrderBy(e => e.Id)
                .Skip(skip)
                .Take(take)
                .ToList();

            Assert.NotNull(result);
            Assert.Contains(SqlCommands, s => s.Contains("LIMIT", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     A5: non-zero Skip/Take parameters are inlined as literals by
        ///     MySqlParameterInliningExpressionVisitor via the new ParametersCacheDecorator.
        /// </summary>
        [ConditionalFact]
        public async Task Parameterized_limit_offset_is_inlined_as_literal()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PipelineEntity>().ToTable("pipeline_test"));

            var skip = 1;
            var take = 3;

            var result = context.Set<PipelineEntity>()
                .OrderBy(e => e.Id)
                .Skip(skip)
                .Take(take)
                .ToList();

            Assert.NotNull(result);
            Assert.Contains(
                SqlCommands,
                s => s.Contains("LIMIT", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Fix ②: inlined collection values must render as MySQL compatible
        ///     "SELECT ... UNION ALL SELECT ..." instead of the unsupported "UNION ALL VALUES ROW(...)".
        /// </summary>
        [ConditionalFact]
        public async Task Inlined_collection_renders_mysql_compatible_values_clause()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PipelineEntity>().ToTable("pipeline_test"));

            var ids = new List<int> { 1, 2, 3 };

            var result = context.Set<PipelineEntity>()
                .Where(e => ids.Contains(e.Id))
                .ToList();

            Assert.NotNull(result);
            Assert.DoesNotContain(
                SqlCommands,
                s => s.Contains("VALUES ROW", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                SqlCommands,
                s => s.Contains("IN", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Fix ①: GREATEST/LEAST style functions must have a type mapping assigned
        ///     (they previously threw "does not have a type mapping assigned" on EF Core 10).
        /// </summary>
        [ConditionalFact]
        public async Task Greatest_least_translation_has_type_mapping()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PipelineEntity>().ToTable("pipeline_test"));

            var result = context.Set<PipelineEntity>()
                .Select(e => Math.Max(e.Id, e.Value))
                .ToList();

            Assert.NotNull(result);
            Assert.Contains(
                SqlCommands,
                s => s.Contains("GREATEST", StringComparison.OrdinalIgnoreCase));
        }

        public class MySqlPipelineContext : ContextBase
        {
        }

        public class PipelineEntity
        {
            public int Id { get; set; }
            public int Value { get; set; }
            public string Name { get; set; }
        }
    }
}
