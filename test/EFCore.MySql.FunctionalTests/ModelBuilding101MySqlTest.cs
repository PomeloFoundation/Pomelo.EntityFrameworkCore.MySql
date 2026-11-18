using Microsoft.EntityFrameworkCore;
using Microting.EntityFrameworkCore.MySql.Tests;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests;

public class ModelBuilding101MySqlTest : ModelBuilding101RelationalTestBase
{
    protected override DbContextOptionsBuilder ConfigureContext(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(AppConfig.ServerVersion);
}
