// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
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
            if (ex is MySqlException mySqlException)
            {
                switch ((MySqlErrorCode)mySqlException.Number)
                {
                    // Thrown if timer queue couldn't be cleared while reading sockets
                    case MySqlErrorCode.CommandTimeoutExpired:
                    // Unable to open connection
                    case MySqlErrorCode.UnableToConnectToHost:
                    // Too many connections
                    case MySqlErrorCode.ConnectionCountError:
                    // Lock wait timeout exceeded; try restarting transaction
                    case MySqlErrorCode.LockWaitTimeout:
                    // Deadlock found when trying to get lock; try restarting transaction
                    case MySqlErrorCode.LockDeadlock:
                    // Transaction branch was rolled back: deadlock was detected
                    case MySqlErrorCode.XARBDeadlock:
                        // Retry in all cases above
                        return true;
                }

                // Otherwise don't retry
                return false;
            }

            return ex is TimeoutException;
        }
    }
}
