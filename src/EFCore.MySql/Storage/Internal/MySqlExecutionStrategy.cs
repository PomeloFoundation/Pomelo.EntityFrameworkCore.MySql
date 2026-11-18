// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microting.EntityFrameworkCore.MySql.Internal;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlExecutionStrategy : ExecutionStrategy
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies)
            : base(dependencies, maxRetryCount: 0, maxRetryDelay: TimeSpan.Zero)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <remarks>
        ///     This is the default non-retrying execution strategy that provides proper transaction management
        ///     without retry logic. For retry functionality with transient error handling, use MySqlRetryingExecutionStrategy.
        /// </remarks>
        protected override bool ShouldRetryOn(Exception exception)
            => false;
    }
}
