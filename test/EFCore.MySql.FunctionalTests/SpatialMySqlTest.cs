using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class SpatialMySqlTest : SpatialTestBase<SpatialMySqlFixture>
    {
        public SpatialMySqlTest(SpatialMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        [ConditionalFact(Skip = "Point.Empty is currently not supported by MySQL and MariaDB.")]
        public override void Translators_handle_static_members()
            => base.Translators_handle_static_members();
    }
}
