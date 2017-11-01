using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace EFCore.MySql.Internal
{
    public class MySqlRetryNoDependendiciesExecutionStrategy
    {
        private MySqlRetryingExecutionStrategy _mySqlRetryingExecutionStrategy { get; }

        private ICollection<int> _additionalErrorNumbers { get; set; }

        private int _maxRetryCount { get; set; }

        private TimeSpan _maxRetryDelay { get; set; }

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

            var tryCount = _maxRetryCount;

            while (true)
            {
                try
                {
                    action();
                    break; // success!
                }
                catch (Exception exception)
                {
                    if (--tryCount == 0)
                        throw;

                    if (!_mySqlRetryingExecutionStrategy.ShouldRetryOnPublic(exception))
                    {
                        throw;
                    }

                    Thread.Sleep(_maxRetryDelay);
                }
            }
        }
    }
}