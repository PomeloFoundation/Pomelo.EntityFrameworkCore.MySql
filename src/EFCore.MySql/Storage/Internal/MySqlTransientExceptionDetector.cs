// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using MySql.Data.MySqlClient;
using JetBrains.Annotations;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     Detects the exceptions caused by SQL Server transient failures.
    /// </summary>
    public class MySqlTransientExceptionDetector
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static bool ShouldRetryOn([NotNull] Exception ex)
        {
            var mySqlException = ex as MySqlException;
            if (mySqlException != null)
            {
                switch (mySqlException.Number)
                {
                    // Too many connections
                    case 1040:
                    // Lock wait timeout exceeded; try restarting transaction
                    case 1205:
                    // Deadlock found when trying to get lock; try restarting transaction
                    case 1213:
                    // Transaction branch was rolled back: deadlock was detected
                    case 1614:
                    // Retry in all cases above
                        return true;
                }

                // Otherwise don't retry
                return false;
            }

            if (ex is TimeoutException)
            {
                return true;
            }

            return false;
        }
    }
}
