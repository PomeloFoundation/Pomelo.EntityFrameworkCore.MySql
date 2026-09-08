using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    /// <summary>
    /// The row-level locking and UPSERT extensions emit MySQL 8.0+ only syntax:
    /// `FOR SHARE` (MySQL 8.0+) and `INSERT ... AS new ON DUPLICATE KEY UPDATE`
    /// (row alias, MySQL 8.0.19+). MariaDB supports neither, so the whole class
    /// is skipped on MariaDB and on MySQL versions below 8.0.19.
    /// </summary>
    [SupportedServerVersionCondition("8.0.19")]
    public class MySqlLockingAndUpsertExtensionsTest : TestBase<MySqlLockingAndUpsertExtensionsTest.MySqlLockingContext>
    {
        [ConditionalFact]
        public async Task ForUpdate_generates_for_update_clause()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<TestEntity>().ToTable("locking_test"));

            var _ = context.Set<TestEntity>()
                .Where(e => e.Id == 1)
                .ForUpdate()
                .ToList();

            Assert.Contains(
                SqlCommands,
                s => s.EndsWith("FOR UPDATE", StringComparison.OrdinalIgnoreCase));
        }

        [ConditionalFact]
        public async Task ForShare_generates_for_share_clause()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<TestEntity>().ToTable("locking_test"));

            var _ = context.Set<TestEntity>()
                .Where(e => e.Id == 1)
                .ForShare()
                .ToList();

            Assert.Contains(
                SqlCommands,
                s => s.EndsWith("FOR SHARE", StringComparison.OrdinalIgnoreCase));
        }

        [ConditionalFact]
        public async Task ForUpdateNoWait_generates_for_update_nowait_clause()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<TestEntity>().ToTable("locking_test"));

            var _ = context.Set<TestEntity>()
                .Where(e => e.Id == 1)
                .ForUpdateNoWait()
                .ToList();

            Assert.Contains(
                SqlCommands,
                s => s.EndsWith("FOR UPDATE NOWAIT", StringComparison.OrdinalIgnoreCase));
        }

        [ConditionalFact]
        public async Task ForUpdateSkipLocked_generates_for_update_skip_locked_clause()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<TestEntity>().ToTable("locking_test"));

            var _ = context.Set<TestEntity>()
                .Where(e => e.Id == 1)
                .ForUpdateSkipLocked()
                .ToList();

            Assert.Contains(
                SqlCommands,
                s => s.EndsWith("FOR UPDATE SKIP LOCKED", StringComparison.OrdinalIgnoreCase));
        }

        [ConditionalFact]
        public async Task UpsertAsync_generates_on_duplicate_key_update_sql()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<TestEntity>().ToTable("upsert_test"));

            await context.UpsertAsync(
                new TestEntity { Id = 1, Name = "test" },
                new[] { nameof(TestEntity.Id) });

            Assert.Contains(
                SqlCommands,
                s => s.Contains("INSERT INTO", StringComparison.OrdinalIgnoreCase)
                     && s.Contains("ON DUPLICATE KEY UPDATE", StringComparison.OrdinalIgnoreCase));
        }

        [ConditionalFact]
        public async Task UpsertRangeAsync_generates_multi_row_values_sql()
        {
            await using var context = await CreateContext(
                model: mb => mb.Entity<TestEntity>().ToTable("upsert_test"));

            await context.UpsertRangeAsync(
                new[]
                {
                    new TestEntity { Id = 1, Name = "a" },
                    new TestEntity { Id = 2, Name = "b" },
                },
                new[] { nameof(TestEntity.Id) });

            Assert.Contains(
                SqlCommands,
                s => s.Contains("INSERT INTO", StringComparison.OrdinalIgnoreCase)
                     && s.Contains("ON DUPLICATE KEY UPDATE", StringComparison.OrdinalIgnoreCase));
        }

        public class MySqlLockingContext : ContextBase
        {
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
