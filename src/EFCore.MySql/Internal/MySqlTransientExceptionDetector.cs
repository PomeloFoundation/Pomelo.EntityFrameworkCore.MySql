using MySql.Data.MySqlClient;
using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     Detects the exceptions caused by PostgreSQL or network transient failures.
    /// </summary>
    public class MySqlTransientExceptionDetector
    {
        public static bool ShouldRetryOn(Exception ex)
            => ((ex as MySqlException)?.Number >= 2000 && (ex as MySqlException)?.Number <= 2027)
                || ((ex as MySqlException)?.Number >= 2047 && (ex as MySqlException)?.Number <= 2050)
                || (ex as MySqlException)?.Number == 0
                || ex is TimeoutException;
    }
}
