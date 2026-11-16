using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class GraphUpdatesMySqlClientNoActionTest : GraphUpdatesMySqlTestBase<GraphUpdatesMySqlClientNoActionTest.MySqlFixture>
    {
        public GraphUpdatesMySqlClientNoActionTest(MySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        // This test requires a specific delete behavior that is overridden to ClientNoAction in this fixture.
        public override Task ClientSetDefault_with_sentinel_value_sets_FK_to_sentinel_on_delete(bool async)
            => Task.CompletedTask;

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
                             .SelectMany(e => e.GetDeclaredForeignKeys())
                             .Where(e => !e.IsOwnership))
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.ClientNoAction;
                }
            }
        }
    }
}
