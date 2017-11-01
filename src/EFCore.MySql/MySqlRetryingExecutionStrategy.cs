using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MySql.Data.MySqlClient;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlRetryingExecutionStrategy : ExecutionStrategy
    {
        public readonly ICollection<int> AdditionalErrorNumbers;

        /// <summary>
        ///     Creates a new instance of <see cref="MySqlRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="context"> The context on which the operations will be invoked. </param>
        /// <remarks>
        ///     The default retry limit is 6, which means that the total amount of time spent before failing is about a minute.
        /// </remarks>
        public MySqlRetryingExecutionStrategy(
            DbContext context)
            : this(context, DefaultMaxRetryCount)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MySqlRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing service dependencies. </param>
        public MySqlRetryingExecutionStrategy(
            ExecutionStrategyDependencies dependencies)
            : this(dependencies, DefaultMaxRetryCount)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MySqlRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="context"> The context on which the operations will be invoked. </param>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        public MySqlRetryingExecutionStrategy(
            DbContext context,
            int maxRetryCount)
            : this(context, maxRetryCount, DefaultMaxDelay, errorNumbersToAdd: null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MySqlRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing service dependencies. </param>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        public MySqlRetryingExecutionStrategy(
            ExecutionStrategyDependencies dependencies,
            int maxRetryCount)
            : this(dependencies, maxRetryCount, DefaultMaxDelay, errorNumbersToAdd: null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MySqlRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="context"> The context on which the operations will be invoked. </param>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        /// <param name="maxRetryDelay"> The maximum delay between retries. </param>
        /// <param name="errorNumbersToAdd"> Additional SQL error numbers that should be considered transient. </param>
        public MySqlRetryingExecutionStrategy(
            DbContext context,
            int maxRetryCount,
            TimeSpan maxRetryDelay,
             ICollection<int> errorNumbersToAdd)
            : base(
                context,
                maxRetryCount,
                maxRetryDelay)
        {
            AdditionalErrorNumbers = errorNumbersToAdd;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MySqlRetryingExecutionStrategy" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing service dependencies. </param>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        /// <param name="maxRetryDelay"> The maximum delay between retries. </param>
        /// <param name="errorNumbersToAdd"> Additional SQL error numbers that should be considered transient. </param>
        public MySqlRetryingExecutionStrategy(
            ExecutionStrategyDependencies dependencies,
            int maxRetryCount,
            TimeSpan maxRetryDelay,
             ICollection<int> errorNumbersToAdd)
            : base(dependencies, maxRetryCount, maxRetryDelay)
        {
            AdditionalErrorNumbers = errorNumbersToAdd;
        }

        /// <summary>
        /// Public version of ShouldRetryOn since we cannot change the existing one as it's an override.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool ShouldRetryOnPublic(Exception exception)
        {
            return ShouldRetryOn(exception);
        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            if (AdditionalErrorNumbers != null)
            {
                if (exception is MySqlException sqlException)
                {
                    if(AdditionalErrorNumbers.Contains(((MySqlException)exception).Number)) {
                        return true;
                    }
                }
            }

            return MySqlTransientExceptionDetector.ShouldRetryOn(exception);
        }

        protected override TimeSpan? GetNextDelay(Exception lastException)
        {
            var baseDelay = base.GetNextDelay(lastException);
            if (baseDelay == null)
            {
                return null;
            }

            return baseDelay;
        }
    }
}
