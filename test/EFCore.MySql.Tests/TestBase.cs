using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public class TestBase<TContext> : IDisposable, IAsyncLifetime
        where TContext : ContextBase, new()
    {
        public async Task InitializeAsync()
        {
            TestStore = await MySqlTestStore.CreateInitializedAsync(StoreName);
        }

        public Task DisposeAsync()
            => Task.CompletedTask;

        public virtual void Dispose() => TestStore.Dispose();

        public virtual string StoreName => GetType().Name;
        public virtual MySqlTestStore TestStore { get; private set; }
        public virtual List<string> SqlCommands { get; } = new List<string>();
        public virtual string Sql => string.Join("\n\n", SqlCommands);

        public virtual async Task<TContext> CreateContext(Action<MySqlDbContextOptionsBuilder> jetOptions = null,
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

            await TestStore.CleanAsync(context);

            return context;
        }
    }
}
