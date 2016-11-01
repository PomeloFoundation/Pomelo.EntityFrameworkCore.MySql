using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Internal
{
	public class MySqlQueryContextFactory : RelationalQueryContextFactory
	{
		private readonly IRelationalConnection _connection;

		public MySqlQueryContextFactory(
			[NotNull] ICurrentDbContext currentContext,
			[NotNull] IConcurrencyDetector concurrencyDetector,
			[NotNull] IRelationalConnection connection,
			[NotNull] IExecutionStrategyFactory executionStrategyFactory)
			: base(currentContext, concurrencyDetector, connection, executionStrategyFactory)
		{
			_connection = connection;
		}

		public override QueryContext Create()
			=> new MySqlQueryContext(CreateQueryBuffer, _connection, StateManager, ConcurrencyDetector, ExecutionStrategyFactory);

	}
}
