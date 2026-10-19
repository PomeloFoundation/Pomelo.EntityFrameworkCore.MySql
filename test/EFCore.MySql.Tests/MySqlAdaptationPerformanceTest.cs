using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql
{
    /// <summary>
    ///     Performance coverage for the code paths that were actually changed by the EF Core 10
    ///     adaptation. Native EF Core paths are untouched by this work, so this test targets only:
    ///     the migrated optimization pipeline (A5) and the three provider fixes.
    ///     Numbers are printed so they can be compared against the EF Core 9 baseline.
    /// </summary>
    public class MySqlAdaptationPerformanceTest : TestBase<MySqlAdaptationPerformanceTest.AdaptationPerfContext>
    {
        private const int Iterations = 200;

        private readonly ITestOutputHelper _output;

        public MySqlAdaptationPerformanceTest(ITestOutputHelper output)
            => _output = output;

        private void Report(string scenario, long elapsedMs)
        {
            _output.WriteLine($"[adaptation-perf] {scenario}: {elapsedMs} ms / {Iterations} iterations");
            Console.WriteLine($"[PERF2][{scenario}] {elapsedMs}");
        }

        /// <summary>
        ///     A5: parameterized predicate goes through the migrated Process pipeline and
        ///     MySqlParameterInliningExpressionVisitor.
        /// </summary>
        [ConditionalFact]
        public async Task Parameterized_predicate_pipeline()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PerfEntity>().ToTable("adapt_perf"));

            var id = 1;

            context.Set<PerfEntity>().Where(e => e.Id == id).ToList(); // warm-up

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                context.Set<PerfEntity>().Where(e => e.Id == id).ToList();
            }
            sw.Stop();

            Report("parameterized-predicate", sw.ElapsedMilliseconds);
        }

        /// <summary>
        ///     A5: boolean column query goes through MySqlBoolOptimizingExpressionVisitor.
        /// </summary>
        [ConditionalFact]
        public async Task Boolean_column_optimization()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PerfEntity>().ToTable("adapt_perf"));

            context.Set<PerfEntity>().Where(e => e.Flag).ToList(); // warm-up

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                context.Set<PerfEntity>().Where(e => e.Flag).ToList();
            }
            sw.Stop();

            Report("bool-optimization", sw.ElapsedMilliseconds);
        }

        /// <summary>
        ///     Fix ②: inline collection translation (Contains) renders MySQL compatible values via
        ///     the overridden GenerateValues path.
        /// </summary>
        [ConditionalFact]
        public async Task Inline_collection_contains()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PerfEntity>().ToTable("adapt_perf"));

            var ids = new List<int> { 1, 2, 3 };

            context.Set<PerfEntity>().Where(e => ids.Contains(e.Id)).ToList(); // warm-up

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                context.Set<PerfEntity>().Where(e => ids.Contains(e.Id)).ToList();
            }
            sw.Stop();

            Report("inline-collection-contains", sw.ElapsedMilliseconds);
        }

        /// <summary>
        ///     Fix ①: GREATEST/LEAST translation goes through the type mapping fallback that was added.
        /// </summary>
        [ConditionalFact]
        public async Task Greatest_least_type_mapping()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PerfEntity>().ToTable("adapt_perf"));

            context.Set<PerfEntity>().Select(e => Math.Max(e.Id, e.Value)).ToList(); // warm-up

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                context.Set<PerfEntity>().Select(e => Math.Max(e.Id, e.Value)).ToList();
            }
            sw.Stop();

            Report("greatest-least", sw.ElapsedMilliseconds);
        }

        /// <summary>
        ///     Fix ③: indexed access into a parameterized collection is translated by the added
        ///     branch of TranslateElementAtOrDefault.
        /// </summary>
        [ConditionalFact]
        public async Task Parameterized_collection_element_at()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<PerfEntity>().ToTable("adapt_perf"));

            var values = new List<int> { 10, 20, 30 };
            var index = 1;

            context.Set<PerfEntity>()
                .Where(e => e.Value == values[index])
                .ToList(); // warm-up

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                context.Set<PerfEntity>()
                    .Where(e => e.Value == values[index])
                    .ToList();
            }
            sw.Stop();

            Report("collection-element-at", sw.ElapsedMilliseconds);
        }

        public class AdaptationPerfContext : ContextBase
        {
        }

        public class PerfEntity
        {
            public int Id { get; set; }
            public int Value { get; set; }
            public bool Flag { get; set; }
            public string Name { get; set; }
        }
    }
}
