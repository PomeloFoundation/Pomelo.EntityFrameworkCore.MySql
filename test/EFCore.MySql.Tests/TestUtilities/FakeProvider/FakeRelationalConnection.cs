// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider
{
    public class FakeRelationalConnection : RelationalConnection
    {
        private DbConnection _connection;

        private readonly List<FakeDbConnection> _dbConnections = new List<FakeDbConnection>();

        public FakeRelationalConnection(IDbContextOptions options)
            : base(
                new RelationalConnectionDependencies(
                    options,
                    new FakeDiagnosticsLogger<DbLoggerCategory.Database.Transaction>(),
                    new FakeDiagnosticsLogger<DbLoggerCategory.Database.Connection>(),
                    new NamedConnectionStringResolver(options),
                    new RelationalTransactionFactory(new RelationalTransactionFactoryDependencies()),
                    null))
        {
        }

        public void UseConnection(DbConnection connection) => _connection = connection;

        public override DbConnection DbConnection => _connection ?? base.DbConnection;

        public IReadOnlyList<FakeDbConnection> DbConnections => _dbConnections;

        protected override DbConnection CreateDbConnection()
        {
            var connection = new FakeDbConnection(ConnectionString);

            _dbConnections.Add(connection);

            return connection;
        }
    }
}
