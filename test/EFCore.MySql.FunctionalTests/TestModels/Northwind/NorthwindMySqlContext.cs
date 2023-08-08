using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestModels.Northwind;

public class NorthwindSqliteContext : NorthwindRelationalContext
{
    public NorthwindSqliteContext(DbContextOptions options)
        : base(options)
    {
    }
}
