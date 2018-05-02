using System;
using EFCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class LoggingMySqlTest : LoggingRelationalTestBase<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>
    {
        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            Action<RelationalDbContextOptionsBuilder<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder().UseMySql("Database=DummyDatabase", relationalAction);

        protected override string ProviderName => "Pomelo.EntityFrameworkCore.MySql";
    }
}
