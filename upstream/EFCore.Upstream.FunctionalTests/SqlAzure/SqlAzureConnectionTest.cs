// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlAzure.Model;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.SqlAzure
{
    [MySqlCondition(MySqlCondition.IsSqlAzure)]
    public class SqlAzureConnectionTest : IClassFixture<SqlAzureFixture>
    {
        public SqlAzureConnectionTest(SqlAzureFixture fixture)
        {
        }

        [ConditionalTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Connect_with_encryption(bool encryptionEnabled)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(MySqlTestStore.CreateConnectionString("adventureworks"))
            {
                Encrypt = encryptionEnabled
            };
            var options = new DbContextOptionsBuilder();
            options.UseMySql(connectionStringBuilder.ConnectionString, b => b.ApplyConfiguration());

            using (var context = new AdventureWorksContext(options.Options))
            {
                context.Database.OpenConnection();
                Assert.Equal(ConnectionState.Open, context.Database.GetDbConnection().State);
            }
        }
    }
}
