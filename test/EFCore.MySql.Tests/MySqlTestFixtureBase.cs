using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        public MySqlTestFixtureBase()
        {
            TestStore = MySqlTestStore.RecreateInitialized(StoreName);
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
            Action<MySqlDbContextOptionsBuilder> jetOptions = null,
            Action<IServiceProvider, DbContextOptionsBuilder> options = null,
            Action<ModelBuilder> model = null)
        {
            var context = new TContext();

            context.Initialize(
                TestStore.Name,
                command => SqlCommands.Add(command.CommandText),
                model: model,
                options: options,
                mySqlOptions: jetOptions);

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
