// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Data.SqlClient;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDbContextOptionsExtensionsTest
    {
        [ConditionalFact]
        public void Can_add_extension_with_max_batch_size()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=Crunchie", b => b.MaxBatchSize(123));

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.Equal(123, extension.MaxBatchSize);
        }

        [ConditionalFact]
        public void Can_add_extension_with_command_timeout()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=Crunchie", b => b.CommandTimeout(30));

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.Equal(30, extension.CommandTimeout);
        }

        [ConditionalFact]
        public void Can_add_extension_with_connection_string()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql("Database=Crunchie");

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.Equal("Database=Crunchie", extension.ConnectionString);
            Assert.Null(extension.Connection);
        }

        [ConditionalFact]
        public void Can_add_extension_with_connection_string_using_generic_options()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
            optionsBuilder.UseMySql("Database=Whisper");

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.Equal("Database=Whisper", extension.ConnectionString);
            Assert.Null(extension.Connection);
        }

        [ConditionalFact]
        public void Can_add_extension_with_connection()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            var connection = new SqlConnection();

            optionsBuilder.UseMySql(connection);

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.Same(connection, extension.Connection);
            Assert.Null(extension.ConnectionString);
        }

        [ConditionalFact]
        public void Can_add_extension_with_connection_using_generic_options()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
            var connection = new SqlConnection();

            optionsBuilder.UseMySql(connection);

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.Same(connection, extension.Connection);
            Assert.Null(extension.ConnectionString);
        }

        [ConditionalFact]
        public void Can_add_extension_with_legacy_paging()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

            optionsBuilder.UseMySql("Database=Kilimanjaro", b => b.UseRowNumberForPaging());

            var extension = optionsBuilder.Options.Extensions.OfType<MySqlOptionsExtension>().Single();

            Assert.True(extension.RowNumberPaging.HasValue);
            Assert.True(extension.RowNumberPaging.Value);
        }
    }
}
