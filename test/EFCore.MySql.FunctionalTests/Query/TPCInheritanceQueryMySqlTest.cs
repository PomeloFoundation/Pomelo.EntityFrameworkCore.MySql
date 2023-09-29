using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPCInheritanceQueryMySqlTest : TPCInheritanceQueryTestBase<TPCInheritanceQueryMySqlFixture>
{
    public TPCInheritanceQueryMySqlTest(
        TPCInheritanceQueryMySqlFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());
}
