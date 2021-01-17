using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public class RawSqlTestWithFixture<TFixture> : TestWithFixture<TFixture>
        where TFixture : MySqlTestFixtureBase
    {
        private readonly DbContext _context;
        protected MySqlConnection Connection { get; }

        protected RawSqlTestWithFixture(TFixture fixture)
            : base(fixture)
        {
            _context = Fixture.CreateDefaultDbContext();
            _context.Database.OpenConnection();

            Connection = (MySqlConnection)_context.Database.GetDbConnection();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
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
            Fixture.SetupDatabase();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Fixture.Dispose();
            }
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
