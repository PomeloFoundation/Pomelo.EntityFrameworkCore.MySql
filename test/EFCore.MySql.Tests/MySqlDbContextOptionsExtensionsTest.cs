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

        [Fact]
        public void TreatTinyAsBoolean_true()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql("TreatTinyAsBoolean=True");

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.TinyInt1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_false()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql("TreatTinyAsBoolean=False");

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void TreatTinyAsBoolean_unspecified()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql("Server=foo");

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Default, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void Explicit_DefaultDataTypeMappings_take_precedence_over_TreatTinyAsBoolean_true()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "TreatTinyAsBoolean=True",
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.Bit1)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.Bit1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }

        [Fact]
        public void Explicit_DefaultDataTypeMappings_take_precedence_over_TreatTinyAsBoolean_false()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseMySql(
                "TreatTinyAsBoolean=False",
                b => b.DefaultDataTypeMappings(m => m.WithClrBoolean(MySqlBooleanType.TinyInt1)));

            var mySqlOptions = new MySqlOptions();
            mySqlOptions.Initialize(builder.Options);

            Assert.Equal(MySqlBooleanType.TinyInt1, mySqlOptions.DefaultDataTypeMappings.ClrBoolean);
        }
    }
}
