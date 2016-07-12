// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using Pomelo.Data.MySql;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalConnection : RelationalConnection
    {
        public MySqlRelationalConnection(
            [NotNull] IDbContextOptions options,
            // ReSharper disable once SuggestBaseTypeForParameter
            [NotNull] ILogger<MySqlConnection> logger)
            : base(options, logger)
        {
        }

        private MySqlRelationalConnection(
            [NotNull] IDbContextOptions options, [NotNull] ILogger logger)
            : base(options, logger)
        {
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new MySqlConnection(ConnectionString);

        public MySqlRelationalConnection CreateMasterConnection()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString) {
                Database = "mysql",
                Pooling = false
            };
            
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql(csb.GetConnectionString(true));
            return new MySqlRelationalConnection(optionsBuilder.Options, Logger);
        }
    }
}
