using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class GraphUpdatesMySqlClientCascadeTest : GraphUpdatesMySqlTestBase<GraphUpdatesMySqlClientCascadeTest.MySqlFixture>
{
    public GraphUpdatesMySqlClientCascadeTest(MySqlFixture fixture)
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
