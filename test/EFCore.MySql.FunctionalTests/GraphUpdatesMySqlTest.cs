using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public abstract class GraphUpdatesMySqlTest
    {
        public abstract class GraphUpdatesMySqlTestBase<TFixture> : GraphUpdatesTestBase<TFixture>
            where TFixture : GraphUpdatesMySqlTestBase<TFixture>.GraphUpdatesMySqlFixtureBase, new()
        {
            protected GraphUpdatesMySqlTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class GraphUpdatesMySqlFixtureBase : GraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
                protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
                protected virtual bool AutoDetectChanges => false;

                public override PoolableDbContext CreateContext()
                {
                    var context = base.CreateContext();
                    context.ChangeTracker.AutoDetectChangesEnabled = AutoDetectChanges;

                    return context;
                }
            }
        }

        public class SnapshotNotifications
            : GraphUpdatesMySqlTestBase<SnapshotNotifications.SnapshotNotificationsFixture>
        {
            public SnapshotNotifications(SnapshotNotificationsFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture)
            {
                //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
            }

            public class SnapshotNotificationsFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphUpdatesSnapshotTest";
                protected override bool AutoDetectChanges => true;

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot);

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class ChangedNotifications
            : GraphUpdatesMySqlTestBase<ChangedNotifications.ChangedNotificationsFixture>
        {
            public ChangedNotifications(ChangedNotificationsFixture fixture)
                : base(fixture)
            {
            }

            public class ChangedNotificationsFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphUpdatesChangedTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class ChangedChangingNotifications
            : GraphUpdatesMySqlTestBase<ChangedChangingNotifications.ChangedChangingNotificationsFixture>
        {
            public ChangedChangingNotifications(ChangedChangingNotificationsFixture fixture)
                : base(fixture)
            {
            }

            public class ChangedChangingNotificationsFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphUpdatesFullTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }

        public class FullWithOriginalsNotifications
            : GraphUpdatesMySqlTestBase<FullWithOriginalsNotifications.FullWithOriginalsNotificationsFixture>
        {
            public FullWithOriginalsNotifications(FullWithOriginalsNotificationsFixture fixture)
                : base(fixture)
            {
            }

            public class FullWithOriginalsNotificationsFixture : GraphUpdatesMySqlFixtureBase
            {
                protected override string StoreName { get; } = "GraphUpdatesOriginalsTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);

                    base.OnModelCreating(modelBuilder, context);
                }
            }
        }
    }
}
