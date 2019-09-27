using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlRelationalTransactionFactory : RelationalTransactionFactory
    {
        public MySqlRelationalTransactionFactory([NotNull] RelationalTransactionFactoryDependencies dependencies)
            : base(dependencies)
        {
        }

        public override RelationalTransaction Create(
            IRelationalConnection connection,
            DbTransaction transaction,
            Guid transactionId,
            IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger,
            bool transactionOwned)
            => new MySqlRelationalTransaction(connection, transaction, transactionId, logger, transactionOwned);
    }
}
