using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class GraphUpdatesMySqlTest
    {
        public class ClientCascade : GraphUpdatesMySqlTestBase<ClientCascade.MySqlFixture>
        {
            public ClientCascade(MySqlFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public class MySqlFixture : GraphUpdatesMySqlFixtureBase
            {
                public override bool NoStoreCascades
                    => true;

                protected override string StoreName { get; } = "GraphClientCascadeUpdatesTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    base.OnModelCreating(modelBuilder, context);

                    foreach (var foreignKey in modelBuilder.Model
                        .GetEntityTypes()
                        .SelectMany(e => e.GetDeclaredForeignKeys())
                        .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade))
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
                    }
                }
            }
        }

        public class ClientNoAction : GraphUpdatesMySqlTestBase<ClientNoAction.MySqlFixture>
        {
            public ClientNoAction(MySqlFixture fixture)
                : base(fixture)
            {
            }

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public class MySqlFixture : GraphUpdatesMySqlFixtureBase
            {
                public override bool ForceClientNoAction
                    => true;

                protected override string StoreName { get; } = "GraphClientNoActionUpdatesTest";

                protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
                {
                    base.OnModelCreating(modelBuilder, context);

                    foreach (var foreignKey in modelBuilder.Model
                        .GetEntityTypes()
                        .SelectMany(e => e.GetDeclaredForeignKeys()))
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.ClientNoAction;
                    }
                }
            }
        }

        // TODO: UseIdentityColumns()
        // public class Identity : GraphUpdatesMySqlTestBase<Identity.MySqlFixture>
        // {
        //     public Identity(MySqlFixture fixture)
        //         : base(fixture)
        //     {
        //     }
        //
        //     protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        //         => facade.UseTransaction(transaction.GetDbTransaction());
        //
        //     public class MySqlFixture : GraphUpdatesMySqlFixtureBase
        //     {
        //         protected override string StoreName { get; } = "GraphIdentityUpdatesTest";
        //
        //         protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        //         {
        //             modelBuilder.UseIdentityColumns();
        //
        //             base.OnModelCreating(modelBuilder, context);
        //         }
        //     }
        // }

        public abstract class GraphUpdatesMySqlTestBase<TFixture> : GraphUpdatesTestBase<TFixture>
            where TFixture : GraphUpdatesMySqlTestBase<TFixture>.GraphUpdatesMySqlFixtureBase, new()
        {
            protected GraphUpdatesMySqlTestBase(TFixture fixture)
                : base(fixture)
            {
            }

            protected override IQueryable<Root> ModifyQueryRoot(IQueryable<Root> query)
                => query.AsSplitQuery();

            protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
                => facade.UseTransaction(transaction.GetDbTransaction());

            public abstract class GraphUpdatesMySqlFixtureBase : GraphUpdatesFixtureBase
            {
                public TestSqlLoggerFactory TestSqlLoggerFactory
                    => (TestSqlLoggerFactory)ListLoggerFactory;

                protected override ITestStoreFactory TestStoreFactory
                    => MySqlTestStoreFactory.Instance;
            }
        }
    }
}
