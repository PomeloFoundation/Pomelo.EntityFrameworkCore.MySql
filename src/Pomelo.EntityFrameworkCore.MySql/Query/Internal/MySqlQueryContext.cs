// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
			[NotNull] Func<IQueryBuffer> queryBufferFactory,
			[NotNull] IRelationalConnection connection,
			[NotNull] IStateManager stateManager,
			[NotNull] IConcurrencyDetector concurrencyDetector)
			: base(queryBufferFactory, connection, stateManager, concurrencyDetector)
		{
		}
	}
}
