using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public abstract class MySqlTestFixtureBase : IDisposable
    {
        public abstract void SetupDatabase();
        public abstract DbContext CreateDefaultDbContext();

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class MySqlTestFixtureBase<TContext>
        : MySqlTestFixtureBase
    where TContext : ContextBase, new()
    {
        private const string FixtureSuffix = "Fixture";

        public MySqlTestFixtureBase(bool initializeEmpty = false)
        {
            // We branch here, because CreateDefaultDbContext depends on TestStore.Name by default, which would not be available yet in
            // the MySqlTestStore.RecreateInitialized(StoreName) call.
            if (initializeEmpty)
            {
                TestStore = MySqlTestStore.RecreateInitialized(StoreName);
            }
            else
            {
                TestStore = MySqlTestStore.Create(StoreName);

                TestStore.InitializeMySql(null, CreateDefaultDbContext, null, c =>
                {
                    c.Database.EnsureDeleted();
                    c.Database.EnsureCreated();
                });
            }

            SetupDatabase();
        }

        protected override void Dispose(bool disposing)
        {
            TestStore.Dispose();
            base.Dispose(disposing);
        }

        protected virtual string StoreName
        {
            get
            {
                var typeName = GetType().Name;
                return typeName.EndsWith(FixtureSuffix)
                    ? typeName.Substring(0, typeName.Length - FixtureSuffix.Length)
                    : typeName;
            }
        }

        protected virtual MySqlTestStore TestStore { get; }
        protected virtual string SetupDatabaseScript { get; }
        protected virtual List<string> SqlCommands { get; } = new List<string>();
        protected virtual string Sql => string.Join("\n\n", SqlCommands);

        public virtual TContext CreateContext(
            Action<MySqlDbContextOptionsBuilder> mySqlOptions = null,
            Action<IServiceProvider, DbContextOptionsBuilder> options = null,
            Action<ModelBuilder> model = null,
            Action<IServiceCollection> serviceCollection = null,
            string databaseName = null)
        {
            var context = new TContext();

            var collection = new ServiceCollection()
                .AddEntityFrameworkMySql();

            serviceCollection?.Invoke(collection);

            context.Initialize(
                databaseName ?? TestStore.Name,
                command => SqlCommands.Add(command.CommandText),
                model,
                options,
                collection,
                mySqlOptions);

            return context;
        }

        public override DbContext CreateDefaultDbContext()
            => CreateContext();

        public override void SetupDatabase()
        {
            if (!string.IsNullOrEmpty(SetupDatabaseScript))
            {
                TestStore.ExecuteScript(SetupDatabaseScript);
            }
        }
    }
}
