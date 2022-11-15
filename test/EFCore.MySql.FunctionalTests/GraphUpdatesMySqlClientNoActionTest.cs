using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class GraphUpdatesMySqlClientNoActionTest : GraphUpdatesMySqlTestBase<GraphUpdatesMySqlClientNoActionTest.MySqlFixture>
    {
        public GraphUpdatesMySqlClientNoActionTest(MySqlFixture fixture)
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
}
