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
			[NotNull] QueryContextDependencies dependencies,
			[NotNull] IRelationalConnection connection,
			[NotNull] IExecutionStrategyFactory executionStrategyFactory)
			: base(dependencies, connection, executionStrategyFactory)
		{
			_connection = connection;
		}

		public override QueryContext Create()
			=> new MySqlQueryContext(Dependencies, CreateQueryBuffer, _connection, ExecutionStrategyFactory);

	}
}
