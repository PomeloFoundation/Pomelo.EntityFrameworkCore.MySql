using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class LoggingMySqlTest : LoggingRelationalTestBase<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>
    {
        protected override DbContextOptionsBuilder CreateOptionsBuilder(
            Action<RelationalDbContextOptionsBuilder<MySqlDbContextOptionsBuilder, MySqlOptionsExtension>> relationalAction)
            => new DbContextOptionsBuilder().UseMySql("Database=DummyDatabase", relationalAction);

        protected override string ProviderName => "Pomelo.EntityFrameworkCore.MySql";
    }
}
