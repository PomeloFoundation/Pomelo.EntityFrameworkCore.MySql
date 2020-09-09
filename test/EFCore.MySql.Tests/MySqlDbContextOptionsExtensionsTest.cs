using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public class MySqlDbContextOptionsBuilderExtensionsTest
    {
        [Fact]
        public void Multiple_UseMySql_calls_each_get_fully_applied()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "Server=first;",
                options =>
                    options.DefaultDataTypeMappings(
                        mappings =>
                            mappings.WithClrBoolean(MySqlBooleanType.Bit1)));

            builder.UseMySql(
                "Server=second;",
                options =>
                    options.DefaultDataTypeMappings(
                        mappings =>
                            mappings.WithClrBoolean(MySqlBooleanType.TinyInt1)));

            var mySqlOptionsExtension = builder.Options.GetExtension<MySqlOptionsExtension>();
            Assert.StartsWith("Server=second;", mySqlOptionsExtension.ConnectionString, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(MySqlBooleanType.TinyInt1, mySqlOptionsExtension.DefaultDataTypeMappings.ClrBoolean);
        }
    }
}
