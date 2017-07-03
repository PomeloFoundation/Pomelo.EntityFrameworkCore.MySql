/// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query
{
	public class MySqlQueryContext : RelationalQueryContext
	{
		public bool HasInclude = false;

		public MySqlQueryContext(
			[NotNull] QueryContextDependencies dependencies,
			[NotNull] Func<IQueryBuffer> queryBufferFactory,
			[NotNull] IRelationalConnection connection,
			[NotNull] IExecutionStrategyFactory executionStrategyFactory)
			: base(dependencies, queryBufferFactory, connection, executionStrategyFactory)
		{
		}
	}
}
