using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public class RawSqlTestWithFixture<TFixture> : TestWithFixture<TFixture>
        where TFixture : MySqlTestFixtureBase
    {
        protected DbContext Context { get; }
        protected MySqlConnection Connection { get; }

        protected RawSqlTestWithFixture(TFixture fixture)
            : base(fixture)
        {
            Context = Fixture.CreateDefaultDbContext();
            Context.Database.OpenConnection();

            Connection = (MySqlConnection)Context.Database.GetDbConnection();
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }
    }

    public class TestWithFixture<TFixture>
        : IClassFixture<TFixture>, IDisposable
        where TFixture : MySqlTestFixtureBase
    {
        protected TestWithFixture(TFixture fixture)
        {
            Fixture = fixture;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TestWithFixture()
        {
            Dispose(false);
        }

        protected TFixture Fixture { get; }
    }
}
