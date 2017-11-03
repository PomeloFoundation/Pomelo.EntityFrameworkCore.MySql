using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.MySql.Internal
{
    public class MySqlRetryNoDependendiciesExecutionStrategy
    {
        private MySqlRetryingExecutionStrategy _mySqlRetryingExecutionStrategy { get; }

        private ICollection<int> _additionalErrorNumbers { get; set; }

        private int _maxRetryCount { get; set; }

        private TimeSpan _maxRetryDelay { get; set; }

        /// <summary>
        ///     The default maximum random factor, must not be lesser than 1.
        /// </summary>
        private const double DefaultRandomFactor = 1.1;

        /// <summary>
        ///     The default base for the exponential function used to compute the delay between retries, must be positive.
        /// </summary>
        private const double DefaultExponentialBase = 2;

        /// <summary>
        ///     The default coefficient for the exponential function used to compute the delay between retries, must be nonnegative.
        /// </summary>
        private static readonly TimeSpan _defaultCoefficient = TimeSpan.FromSeconds(1);

        /// <summary>
        ///     A pseudo-random number generator that can be used to vary the delay between retries.
        /// </summary>
        private static Random Random { get; } = new Random();


        /// <summary>
        /// Configure for only 1 attempt.
        /// </summary>        
        public MySqlRetryNoDependendiciesExecutionStrategy()
        {
            _maxRetryCount = 1;
            _maxRetryDelay = TimeSpan.FromMilliseconds(0);
        }

        public MySqlRetryNoDependendiciesExecutionStrategy(MySqlRetryingExecutionStrategy mySqlRetryingExecutionStrategy)
        {
            _mySqlRetryingExecutionStrategy = mySqlRetryingExecutionStrategy;
            _additionalErrorNumbers = mySqlRetryingExecutionStrategy.AdditionalErrorNumbers;
            _maxRetryCount = Convert.ToInt32(GetProtectedProperty(mySqlRetryingExecutionStrategy, "MaxRetryCount"));
            _maxRetryDelay = (TimeSpan)GetProtectedProperty(mySqlRetryingExecutionStrategy, "MaxRetryDelay");
        }

        /// <summary>
        /// Get property value from a protected class field
        /// </summary>
        /// <param name="fromObject"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private object GetProtectedProperty(Object fromObject, string property)
        {

            Type type = fromObject.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(property,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.GetProperty);

            return propertyInfo.GetValue(fromObject);
        }

        /// <summary>
        /// Execute the passed action and follow the retry logic defined in the MySqlRetryingExecutionStrategy class.
        /// </summary>
        /// <param name="action">code to execute</param>
        /// <param name="mySqlRetryingExecutionStrategy">execution strategy missing the ExecutionStrategyDependancies</param>
        public void Execute(Action action)
        {
            if (_maxRetryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_maxRetryCount));
            }

            var tryCount = 0;

            while (true)
            {
                try
                {
                    action();
                    break; // success!
                }
                catch (Exception exception)
                {
                    if (!_mySqlRetryingExecutionStrategy.ShouldRetryOnPublic(exception))
                    {
                        throw;
                    }

                    var delay = GetNextDelay(tryCount);

                    if (delay == null)
                    {
                        throw new RetryLimitExceededException(CoreStrings.RetryLimitExceeded(_maxRetryCount, GetType().Name), exception);
                    }
                    else
                    {
                        using (var waitEvent = new ManualResetEventSlim(false))
                        {
                            waitEvent.WaitHandle.WaitOne(delay.Value);
                        }     
                    }
                    
                    tryCount++;
                }
            }
        }

        /// <summary>
        ///     Determines whether the operation should be retried and the delay before the next attempt.
        /// </summary>
        /// <param name="currentRetryCount">Current retry count</param>
        /// <returns>
        ///     Returns the delay indicating how long to wait for before the next execution attempt if the operation should be retried;
        ///     <c>null</c> otherwise
        /// </returns>

        protected virtual TimeSpan? GetNextDelay(int currentRetryCount)
        {
            if (currentRetryCount < _maxRetryCount)
            {
                var delta = (Math.Pow(DefaultExponentialBase, currentRetryCount) - 1.0)
                            * (1.0 + Random.NextDouble() * (DefaultRandomFactor - 1.0));

                var delay = Math.Min(
                    _defaultCoefficient.TotalMilliseconds * delta,
                    _maxRetryDelay.TotalMilliseconds);

                return TimeSpan.FromMilliseconds(delay);
            }

            return null;
        }        
    }
}
