using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

#nullable enable

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ValueConvertersEndToEndMySqlTest
        : ValueConvertersEndToEndTestBase<ValueConvertersEndToEndMySqlTest.ValueConvertersEndToEndMySqlFixture>
    {
        public ValueConvertersEndToEndMySqlTest(ValueConvertersEndToEndMySqlFixture fixture)
            : base(fixture)
        {
        }

        // CHECK: Do the UNSIGNED types make sense?
        [ConditionalTheory]
        [InlineData(nameof(ConvertingEntity.BoolAsChar), "varchar(1)", false)]
        [InlineData(nameof(ConvertingEntity.BoolAsNullableChar), "varchar(1)", false)]
        [InlineData(nameof(ConvertingEntity.BoolAsString), "varchar(3)", false)]
        [InlineData(nameof(ConvertingEntity.BoolAsInt), "int", false)]
        [InlineData(nameof(ConvertingEntity.BoolAsNullableString), "varchar(3)", false)]
        [InlineData(nameof(ConvertingEntity.BoolAsNullableInt), "int", false)]
        [InlineData(nameof(ConvertingEntity.IntAsLong), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.IntAsNullableLong), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.BytesAsString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.BytesAsNullableString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.CharAsString), "varchar(1)", false)]
        [InlineData(nameof(ConvertingEntity.CharAsNullableString), "varchar(1)", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeOffsetToBinary), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeOffsetToNullableBinary), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeOffsetToString), "varchar(48)", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeOffsetToNullableString), "varchar(48)", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeToBinary), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeToNullableBinary), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeToString), "varchar(48)", false)]
        [InlineData(nameof(ConvertingEntity.DateTimeToNullableString), "varchar(48)", false)]
        [InlineData(nameof(ConvertingEntity.EnumToString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.EnumToNullableString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.EnumToNumber), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.EnumToNullableNumber), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.GuidToString), "varchar(36)", false)]
        [InlineData(nameof(ConvertingEntity.GuidToNullableString), "varchar(36)", false)]
        [InlineData(nameof(ConvertingEntity.GuidToBytes), "varbinary(16)", false)]
        [InlineData(nameof(ConvertingEntity.GuidToNullableBytes), "varbinary(16)", false)]
        [InlineData(nameof(ConvertingEntity.IPAddressToString), "varchar(45)", false)]
        [InlineData(nameof(ConvertingEntity.IPAddressToNullableString), "varchar(45)", false)]
        [InlineData(nameof(ConvertingEntity.IPAddressToBytes), "varbinary(16)", false)]
        [InlineData(nameof(ConvertingEntity.IPAddressToNullableBytes), "varbinary(16)", false)]
        [InlineData(nameof(ConvertingEntity.PhysicalAddressToString), "varchar(20)", false)]
        [InlineData(nameof(ConvertingEntity.PhysicalAddressToNullableString), "varchar(20)", false)]
        [InlineData(nameof(ConvertingEntity.PhysicalAddressToBytes), "varbinary(8)", false)]
        [InlineData(nameof(ConvertingEntity.PhysicalAddressToNullableBytes), "varbinary(8)", false)]
        [InlineData(nameof(ConvertingEntity.NumberToString), "varchar(64)", false)]
        [InlineData(nameof(ConvertingEntity.NumberToNullableString), "varchar(64)", false)]
        [InlineData(nameof(ConvertingEntity.NumberToBytes), "varbinary(1)", false)]
        [InlineData(nameof(ConvertingEntity.NumberToNullableBytes), "varbinary(1)", false)]
        [InlineData(nameof(ConvertingEntity.StringToBool), "tinyint(1)", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableBool), "tinyint(1)", false)]
        [InlineData(nameof(ConvertingEntity.StringToBytes), "longblob", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableBytes), "longblob", false)]
        [InlineData(nameof(ConvertingEntity.StringToChar), "varchar(1)", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableChar), "varchar(1)", false)]
        [InlineData(nameof(ConvertingEntity.StringToDateTime), "datetime(6)", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableDateTime), "datetime(6)", false)]
        // [InlineData(nameof(ConvertingEntity.StringToDateTimeOffset), "datetime(6)", false)]
        // [InlineData(nameof(ConvertingEntity.StringToNullableDateTimeOffset), "datetime(6)", false)]
        [InlineData(nameof(ConvertingEntity.StringToEnum), "smallint unsigned", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableEnum), "smallint unsigned", false)]
        [InlineData(nameof(ConvertingEntity.StringToGuid), "char(36)", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableGuid), "char(36)", false)]
        [InlineData(nameof(ConvertingEntity.StringToNumber), "tinyint unsigned", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableNumber), "tinyint unsigned", false)]
        [InlineData(nameof(ConvertingEntity.StringToTimeSpan), "time(6)", false)]
        [InlineData(nameof(ConvertingEntity.StringToNullableTimeSpan), "time(6)", false)]
        [InlineData(nameof(ConvertingEntity.TimeSpanToTicks), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.TimeSpanToNullableTicks), "bigint", false)]
        [InlineData(nameof(ConvertingEntity.TimeSpanToString), "varchar(48)", false)]
        [InlineData(nameof(ConvertingEntity.TimeSpanToNullableString), "varchar(48)", false)]
        [InlineData(nameof(ConvertingEntity.UriToString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.UriToNullableString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.NullableCharAsString), "varchar(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableCharAsNullableString), "varchar(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableBoolAsChar), "varchar(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableBoolAsNullableChar), "varchar(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableBoolAsString), "varchar(3)", true)]
        [InlineData(nameof(ConvertingEntity.NullableBoolAsNullableString), "varchar(3)", true)]
        [InlineData(nameof(ConvertingEntity.NullableBoolAsInt), "int", true)]
        [InlineData(nameof(ConvertingEntity.NullableBoolAsNullableInt), "int", true)]
        [InlineData(nameof(ConvertingEntity.NullableIntAsLong), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableIntAsNullableLong), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableBytesAsString), "longtext", true)]
        [InlineData(nameof(ConvertingEntity.NullableBytesAsNullableString), "longtext", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeOffsetToBinary), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeOffsetToNullableBinary), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeOffsetToString), "varchar(48)", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeOffsetToNullableString), "varchar(48)", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeToBinary), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeToNullableBinary), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeToString), "varchar(48)", true)]
        [InlineData(nameof(ConvertingEntity.NullableDateTimeToNullableString), "varchar(48)", true)]
        [InlineData(nameof(ConvertingEntity.NullableEnumToString), "longtext", true)]
        [InlineData(nameof(ConvertingEntity.NullableEnumToNullableString), "longtext", true)]
        [InlineData(nameof(ConvertingEntity.NullableEnumToNumber), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableEnumToNullableNumber), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableGuidToString), "varchar(36)", true)]
        [InlineData(nameof(ConvertingEntity.NullableGuidToNullableString), "varchar(36)", true)]
        [InlineData(nameof(ConvertingEntity.NullableGuidToBytes), "varbinary(16)", true)]
        [InlineData(nameof(ConvertingEntity.NullableGuidToNullableBytes), "varbinary(16)", true)]
        [InlineData(nameof(ConvertingEntity.NullableIPAddressToString), "varchar(45)", true)]
        [InlineData(nameof(ConvertingEntity.NullableIPAddressToNullableString), "varchar(45)", true)]
        [InlineData(nameof(ConvertingEntity.NullableIPAddressToBytes), "varbinary(16)", true)]
        [InlineData(nameof(ConvertingEntity.NullableIPAddressToNullableBytes), "varbinary(16)", true)]
        [InlineData(nameof(ConvertingEntity.NullablePhysicalAddressToString), "varchar(20)", true)]
        [InlineData(nameof(ConvertingEntity.NullablePhysicalAddressToNullableString), "varchar(20)", true)]
        [InlineData(nameof(ConvertingEntity.NullablePhysicalAddressToBytes), "varbinary(8)", true)]
        [InlineData(nameof(ConvertingEntity.NullablePhysicalAddressToNullableBytes), "varbinary(8)", true)]
        [InlineData(nameof(ConvertingEntity.NullableNumberToString), "varchar(64)", true)]
        [InlineData(nameof(ConvertingEntity.NullableNumberToNullableString), "varchar(64)", true)]
        [InlineData(nameof(ConvertingEntity.NullableNumberToBytes), "varbinary(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableNumberToNullableBytes), "varbinary(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToBool), "tinyint(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableBool), "tinyint(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToBytes), "longblob", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableBytes), "longblob", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToChar), "varchar(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableChar), "varchar(1)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToDateTime), "datetime(6)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableDateTime), "datetime(6)", true)]
        // [InlineData(nameof(ConvertingEntity.NullableStringToDateTimeOffset), "datetime(6)", true)]
        // [InlineData(nameof(ConvertingEntity.NullableStringToNullableDateTimeOffset), "datetime(6)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToEnum), "smallint unsigned", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableEnum), "smallint unsigned", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToGuid), "char(36)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableGuid), "char(36)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNumber), "tinyint unsigned", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableNumber), "tinyint unsigned", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToTimeSpan), "time(6)", true)]
        [InlineData(nameof(ConvertingEntity.NullableStringToNullableTimeSpan), "time(6)", true)]
        [InlineData(nameof(ConvertingEntity.NullableTimeSpanToTicks), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableTimeSpanToNullableTicks), "bigint", true)]
        [InlineData(nameof(ConvertingEntity.NullableTimeSpanToString), "varchar(48)", true)]
        [InlineData(nameof(ConvertingEntity.NullableTimeSpanToNullableString), "varchar(48)", true)]
        [InlineData(nameof(ConvertingEntity.NullableUriToString), "longtext", true)]
        [InlineData(nameof(ConvertingEntity.NullableUriToNullableString), "longtext", true)]
        [InlineData(nameof(ConvertingEntity.NullStringToNonNullString), "longtext", false)]
        [InlineData(nameof(ConvertingEntity.NonNullStringToNullString), "longtext", true)]
        public virtual void Properties_with_conversions_map_to_appropriately_null_columns(
            string propertyName,
            string databaseType,
            bool isNullable)
        {
            using var context = CreateContext();

            var property = context.Model.FindEntityType(typeof(ConvertingEntity))!.FindProperty(propertyName);

            Assert.Equal(databaseType, property!.GetColumnType());
            Assert.Equal(isNullable, property!.IsNullable);
        }

        public class ValueConvertersEndToEndMySqlFixture : ValueConvertersEndToEndFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<ConvertingEntity>(
                    b =>
                    {
                        // We map DateTimeOffset to MySQL's 'datetime(6)', which doesn't store the time zone.
                        // Therefore, lossless round-tripping is impossible.
                        b.Ignore(e => e.StringToDateTimeOffset);
                        b.Ignore(e => e.StringToNullableDateTimeOffset);
                        b.Ignore(e => e.NullableStringToDateTimeOffset);
                        b.Ignore(e => e.NullableStringToNullableDateTimeOffset);
                    });
            }
        }
    }
}

#nullable restore
