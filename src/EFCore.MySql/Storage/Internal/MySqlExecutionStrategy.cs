// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlExecutionStrategy : IExecutionStrategy
    {
        private ExecutionStrategyDependencies Dependencies { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies)
        {
            Dependencies = dependencies;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool RetriesOnFailure => false;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual TResult Execute<TState, TResult>(
            TState state,
            Func<DbContext, TState, TResult> operation,
            Func<DbContext, TState, ExecutionResult<TResult>> verifySucceeded)
        {
            try
            {
                return operation(Dependencies.CurrentDbContext.Context, state);
            }
            catch (Exception ex) when (ExecutionStrategy.CallOnWrappedException(ex, MySqlTransientExceptionDetector.ShouldRetryOn))
            {
                throw new InvalidOperationException("Transient exception detected", ex);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual async Task<TResult> ExecuteAsync<TState, TResult>(
            TState state,
            Func<DbContext, TState, CancellationToken, Task<TResult>> operation,
            Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
            CancellationToken cancellationToken)
        {
            try
            {
                return await operation(Dependencies.CurrentDbContext.Context, state, cancellationToken);
            }
            catch (Exception ex) when (ExecutionStrategy.CallOnWrappedException(ex, MySqlTransientExceptionDetector.ShouldRetryOn))
            {
                throw new InvalidOperationException("Transient exception detected", ex);
            }
        }
    }
}
