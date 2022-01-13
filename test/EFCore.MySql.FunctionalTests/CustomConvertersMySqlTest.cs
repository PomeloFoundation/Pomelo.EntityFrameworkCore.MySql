using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class CustomConvertersMySqlTest : CustomConvertersTestBase<CustomConvertersMySqlTest.CustomConvertersMySqlFixture>
    {
        public CustomConvertersMySqlTest(CustomConvertersMySqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Value_conversion_on_enum_collection_contains()
        {
            Assert.Contains(
                CoreStrings.TranslationFailed("").Substring(47),
                Assert.Throws<InvalidOperationException>(() => base.Value_conversion_on_enum_collection_contains()).Message);
        }

        // TODO: Needed to customize:
        //          `private bool Equal(DateTimeOffset left, DateTimeOffset right)` Recheck/remove once fixed upstream.
        //       Recheck/remove after `https://github.com/dotnet/efcore/issues/26068` has been fixed upstream.
        #region https://github.com/dotnet/efcore/issues/26068

        [ConditionalFact]
        public override void Can_insert_and_read_back_all_non_nullable_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<BuiltInDataTypes>().Add(
                    new BuiltInDataTypes
                    {
                        Id = 1,
                        PartitionId = 1,
                        TestInt16 = -1234,
                        TestInt32 = -123456789,
                        TestInt64 = -1234567890123456789L,
                        TestDouble = -1.23456789,
                        TestDecimal = -1234567890.01M,
                        TestDateTime = DateTime.Parse("01/01/2000 12:34:56", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
                        TestDateTimeOffset = new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                        TestTimeSpan = new TimeSpan(0, 10, 9, 8, 7),
                        TestSingle = -1.234F,
                        TestBoolean = true,
                        TestByte = 255,
                        TestUnsignedInt16 = 1234,
                        TestUnsignedInt32 = 1234565789U,
                        TestUnsignedInt64 = 1234567890123456789UL,
                        TestCharacter = 'a',
                        TestSignedByte = -128,
                        Enum64 = Enum64.SomeValue,
                        Enum32 = Enum32.SomeValue,
                        Enum16 = Enum16.SomeValue,
                        Enum8 = Enum8.SomeValue,
                        EnumU64 = EnumU64.SomeValue,
                        EnumU32 = EnumU32.SomeValue,
                        EnumU16 = EnumU16.SomeValue,
                        EnumS8 = EnumS8.SomeValue
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var dt = context.Set<BuiltInDataTypes>().Where(e => e.Id == 1).ToList().Single();

                var entityType = context.Model.FindEntityType(typeof(BuiltInDataTypes));
                AssertEqualIfMapped(entityType, (short)-1234, () => dt.TestInt16);
                AssertEqualIfMapped(entityType, -123456789, () => dt.TestInt32);
                AssertEqualIfMapped(entityType, -1234567890123456789L, () => dt.TestInt64);
                AssertEqualIfMapped(entityType, -1.23456789, () => dt.TestDouble);
                AssertEqualIfMapped(entityType, -1234567890.01M, () => dt.TestDecimal);
                AssertEqualIfMapped(entityType, DateTime.Parse("01/01/2000 12:34:56", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal),
                    () => dt.TestDateTime);
                AssertEqualIfMapped(
                    entityType, new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                    () => dt.TestDateTimeOffset);
                AssertEqualIfMapped(entityType, new TimeSpan(0, 10, 9, 8, 7), () => dt.TestTimeSpan);
                AssertEqualIfMapped(entityType, -1.234F, () => dt.TestSingle);
                AssertEqualIfMapped(entityType, true, () => dt.TestBoolean);
                AssertEqualIfMapped(entityType, (byte)255, () => dt.TestByte);
                AssertEqualIfMapped(entityType, Enum64.SomeValue, () => dt.Enum64);
                AssertEqualIfMapped(entityType, Enum32.SomeValue, () => dt.Enum32);
                AssertEqualIfMapped(entityType, Enum16.SomeValue, () => dt.Enum16);
                AssertEqualIfMapped(entityType, Enum8.SomeValue, () => dt.Enum8);
                AssertEqualIfMapped(entityType, (ushort)1234, () => dt.TestUnsignedInt16);
                AssertEqualIfMapped(entityType, 1234565789U, () => dt.TestUnsignedInt32);
                AssertEqualIfMapped(entityType, 1234567890123456789UL, () => dt.TestUnsignedInt64);
                AssertEqualIfMapped(entityType, 'a', () => dt.TestCharacter);
                AssertEqualIfMapped(entityType, (sbyte)-128, () => dt.TestSignedByte);
                AssertEqualIfMapped(entityType, EnumU64.SomeValue, () => dt.EnumU64);
                AssertEqualIfMapped(entityType, EnumU32.SomeValue, () => dt.EnumU32);
                AssertEqualIfMapped(entityType, EnumU16.SomeValue, () => dt.EnumU16);
                AssertEqualIfMapped(entityType, EnumS8.SomeValue, () => dt.EnumS8);
            }
        }

        [ConditionalFact]
        public override void Can_insert_and_read_back_non_nullable_backed_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<NonNullableBackedDataTypes>().Add(
                    new NonNullableBackedDataTypes
                    {
                        Id = 101,
                        PartitionId = 101,
                        Int16 = -1234,
                        Int32 = -123456789,
                        Int64 = -1234567890123456789L,
                        Double = -1.23456789,
                        Decimal = -1234567890.01M,
                        DateTime = DateTime.Parse("01/01/2000 12:34:56"),
                        DateTimeOffset = new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                        TimeSpan = new TimeSpan(0, 10, 9, 8, 7),
                        Single = -1.234F,
                        Boolean = true,
                        Byte = 255,
                        UnsignedInt16 = 1234,
                        UnsignedInt32 = 1234565789U,
                        UnsignedInt64 = 1234567890123456789UL,
                        Character = 'a',
                        SignedByte = -128,
                        Enum64 = Enum64.SomeValue,
                        Enum32 = Enum32.SomeValue,
                        Enum16 = Enum16.SomeValue,
                        Enum8 = Enum8.SomeValue,
                        EnumU64 = EnumU64.SomeValue,
                        EnumU32 = EnumU32.SomeValue,
                        EnumU16 = EnumU16.SomeValue,
                        EnumS8 = EnumS8.SomeValue
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var dt = context.Set<NonNullableBackedDataTypes>().Where(ndt => ndt.Id == 101).ToList().Single();

                var entityType = context.Model.FindEntityType(typeof(NonNullableBackedDataTypes));
                AssertEqualIfMapped(entityType, (short)-1234, () => dt.Int16);
                AssertEqualIfMapped(entityType, -123456789, () => dt.Int32);
                AssertEqualIfMapped(entityType, -1234567890123456789L, () => dt.Int64);
                AssertEqualIfMapped(entityType, -1234567890123456789L, () => dt.Int64);
                AssertEqualIfMapped(entityType, -1.23456789, () => dt.Double);
                AssertEqualIfMapped(entityType, -1234567890.01M, () => dt.Decimal);
                AssertEqualIfMapped(entityType, DateTime.Parse("01/01/2000 12:34:56"), () => dt.DateTime);
                AssertEqualIfMapped(
                    entityType, new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                    () => dt.DateTimeOffset);
                AssertEqualIfMapped(entityType, new TimeSpan(0, 10, 9, 8, 7), () => dt.TimeSpan);
                AssertEqualIfMapped(entityType, -1.234F, () => dt.Single);
                AssertEqualIfMapped(entityType, true, () => dt.Boolean);
                AssertEqualIfMapped(entityType, (byte)255, () => dt.Byte);
                AssertEqualIfMapped(entityType, Enum64.SomeValue, () => dt.Enum64);
                AssertEqualIfMapped(entityType, Enum32.SomeValue, () => dt.Enum32);
                AssertEqualIfMapped(entityType, Enum16.SomeValue, () => dt.Enum16);
                AssertEqualIfMapped(entityType, Enum8.SomeValue, () => dt.Enum8);
                AssertEqualIfMapped(entityType, (ushort)1234, () => dt.UnsignedInt16);
                AssertEqualIfMapped(entityType, 1234565789U, () => dt.UnsignedInt32);
                AssertEqualIfMapped(entityType, 1234567890123456789UL, () => dt.UnsignedInt64);
                AssertEqualIfMapped(entityType, 'a', () => dt.Character);
                AssertEqualIfMapped(entityType, (sbyte)-128, () => dt.SignedByte);
                AssertEqualIfMapped(entityType, EnumU64.SomeValue, () => dt.EnumU64);
                AssertEqualIfMapped(entityType, EnumU32.SomeValue, () => dt.EnumU32);
                AssertEqualIfMapped(entityType, EnumU16.SomeValue, () => dt.EnumU16);
                AssertEqualIfMapped(entityType, EnumS8.SomeValue, () => dt.EnumS8);
            }
        }

        [ConditionalFact]
        public override void Can_insert_and_read_back_nullable_backed_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<NullableBackedDataTypes>().Add(
                    new NullableBackedDataTypes
                    {
                        Id = 101,
                        PartitionId = 101,
                        Int16 = -1234,
                        Int32 = -123456789,
                        Int64 = -1234567890123456789L,
                        Double = -1.23456789,
                        Decimal = -1234567890.01M,
                        DateTime = DateTime.Parse("01/01/2000 12:34:56"),
                        DateTimeOffset = new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                        TimeSpan = new TimeSpan(0, 10, 9, 8, 7),
                        Single = -1.234F,
                        Boolean = false,
                        Byte = 255,
                        UnsignedInt16 = 1234,
                        UnsignedInt32 = 1234565789U,
                        UnsignedInt64 = 1234567890123456789UL,
                        Character = 'a',
                        SignedByte = -128,
                        Enum64 = Enum64.SomeValue,
                        Enum32 = Enum32.SomeValue,
                        Enum16 = Enum16.SomeValue,
                        Enum8 = Enum8.SomeValue,
                        EnumU64 = EnumU64.SomeValue,
                        EnumU32 = EnumU32.SomeValue,
                        EnumU16 = EnumU16.SomeValue,
                        EnumS8 = EnumS8.SomeValue
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var dt = context.Set<NullableBackedDataTypes>().Where(ndt => ndt.Id == 101).ToList().Single();

                var entityType = context.Model.FindEntityType(typeof(NullableBackedDataTypes));
                AssertEqualIfMapped(entityType, (short)-1234, () => dt.Int16);
                AssertEqualIfMapped(entityType, -123456789, () => dt.Int32);
                AssertEqualIfMapped(entityType, -1234567890123456789L, () => dt.Int64);
                AssertEqualIfMapped(entityType, -1.23456789, () => dt.Double);
                AssertEqualIfMapped(entityType, -1234567890.01M, () => dt.Decimal);
                AssertEqualIfMapped(entityType, DateTime.Parse("01/01/2000 12:34:56"), () => dt.DateTime);
                AssertEqualIfMapped(
                    entityType, new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                    () => dt.DateTimeOffset);
                AssertEqualIfMapped(entityType, new TimeSpan(0, 10, 9, 8, 7), () => dt.TimeSpan);
                AssertEqualIfMapped(entityType, -1.234F, () => dt.Single);
                AssertEqualIfMapped(entityType, false, () => dt.Boolean);
                AssertEqualIfMapped(entityType, (byte)255, () => dt.Byte);
                AssertEqualIfMapped(entityType, Enum64.SomeValue, () => dt.Enum64);
                AssertEqualIfMapped(entityType, Enum32.SomeValue, () => dt.Enum32);
                AssertEqualIfMapped(entityType, Enum16.SomeValue, () => dt.Enum16);
                AssertEqualIfMapped(entityType, Enum8.SomeValue, () => dt.Enum8);
                AssertEqualIfMapped(entityType, (ushort)1234, () => dt.UnsignedInt16);
                AssertEqualIfMapped(entityType, 1234565789U, () => dt.UnsignedInt32);
                AssertEqualIfMapped(entityType, 1234567890123456789UL, () => dt.UnsignedInt64);
                AssertEqualIfMapped(entityType, 'a', () => dt.Character);
                AssertEqualIfMapped(entityType, (sbyte)-128, () => dt.SignedByte);
                AssertEqualIfMapped(entityType, EnumU64.SomeValue, () => dt.EnumU64);
                AssertEqualIfMapped(entityType, EnumU32.SomeValue, () => dt.EnumU32);
                AssertEqualIfMapped(entityType, EnumU16.SomeValue, () => dt.EnumU16);
                AssertEqualIfMapped(entityType, EnumS8.SomeValue, () => dt.EnumS8);
            }
        }

        [ConditionalFact]
        public override void Can_insert_and_read_back_object_backed_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<ObjectBackedDataTypes>().Add(
                    new ObjectBackedDataTypes
                    {
                        Id = 101,
                        PartitionId = 101,
                        String = "TestString",
                        Bytes = new byte[] { 10, 9, 8, 7, 6 },
                        Int16 = -1234,
                        Int32 = -123456789,
                        Int64 = -1234567890123456789L,
                        Double = -1.23456789,
                        Decimal = -1234567890.01M,
                        DateTime = DateTime.Parse("01/01/2000 12:34:56"),
                        DateTimeOffset = new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                        TimeSpan = new TimeSpan(0, 10, 9, 8, 7),
                        Single = -1.234F,
                        Boolean = false,
                        Byte = 255,
                        UnsignedInt16 = 1234,
                        UnsignedInt32 = 1234565789U,
                        UnsignedInt64 = 1234567890123456789UL,
                        Character = 'a',
                        SignedByte = -128,
                        Enum64 = Enum64.SomeValue,
                        Enum32 = Enum32.SomeValue,
                        Enum16 = Enum16.SomeValue,
                        Enum8 = Enum8.SomeValue,
                        EnumU64 = EnumU64.SomeValue,
                        EnumU32 = EnumU32.SomeValue,
                        EnumU16 = EnumU16.SomeValue,
                        EnumS8 = EnumS8.SomeValue
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var dt = context.Set<ObjectBackedDataTypes>().Where(ndt => ndt.Id == 101).ToList().Single();

                var entityType = context.Model.FindEntityType(typeof(ObjectBackedDataTypes));
                AssertEqualIfMapped(entityType, "TestString", () => dt.String);
                AssertEqualIfMapped(entityType, new byte[] { 10, 9, 8, 7, 6 }, () => dt.Bytes);
                AssertEqualIfMapped(entityType, (short)-1234, () => dt.Int16);
                AssertEqualIfMapped(entityType, -123456789, () => dt.Int32);
                AssertEqualIfMapped(entityType, -1234567890123456789L, () => dt.Int64);
                AssertEqualIfMapped(entityType, -1.23456789, () => dt.Double);
                AssertEqualIfMapped(entityType, -1234567890.01M, () => dt.Decimal);
                AssertEqualIfMapped(entityType, DateTime.Parse("01/01/2000 12:34:56"), () => dt.DateTime);
                AssertEqualIfMapped(
                    entityType, new DateTimeOffset(DateTime.Parse("01/01/2000 12:34:56"), TimeSpan.FromHours(-8.0)),
                    () => dt.DateTimeOffset);
                AssertEqualIfMapped(entityType, new TimeSpan(0, 10, 9, 8, 7), () => dt.TimeSpan);
                AssertEqualIfMapped(entityType, -1.234F, () => dt.Single);
                AssertEqualIfMapped(entityType, false, () => dt.Boolean);
                AssertEqualIfMapped(entityType, (byte)255, () => dt.Byte);
                AssertEqualIfMapped(entityType, Enum64.SomeValue, () => dt.Enum64);
                AssertEqualIfMapped(entityType, Enum32.SomeValue, () => dt.Enum32);
                AssertEqualIfMapped(entityType, Enum16.SomeValue, () => dt.Enum16);
                AssertEqualIfMapped(entityType, Enum8.SomeValue, () => dt.Enum8);
                AssertEqualIfMapped(entityType, (ushort)1234, () => dt.UnsignedInt16);
                AssertEqualIfMapped(entityType, 1234565789U, () => dt.UnsignedInt32);
                AssertEqualIfMapped(entityType, 1234567890123456789UL, () => dt.UnsignedInt64);
                AssertEqualIfMapped(entityType, 'a', () => dt.Character);
                AssertEqualIfMapped(entityType, (sbyte)-128, () => dt.SignedByte);
                AssertEqualIfMapped(entityType, EnumU64.SomeValue, () => dt.EnumU64);
                AssertEqualIfMapped(entityType, EnumU32.SomeValue, () => dt.EnumU32);
                AssertEqualIfMapped(entityType, EnumU16.SomeValue, () => dt.EnumU16);
                AssertEqualIfMapped(entityType, EnumS8.SomeValue, () => dt.EnumS8);
            }
        }

        private void AssertEqualIfMapped<T>(IEntityType entityType, T expected, Expression<Func<T>> actualExpression)
        {
            if (entityType.FindProperty(((MemberExpression)actualExpression.Body).Member.Name) != null)
            {
                var actual = actualExpression.Compile()();
                var type = UnwrapNullableEnumType(typeof(T));
                if (IsSignedInteger(type))
                {
                    Assert.True(Equal(Convert.ToInt64(expected), Convert.ToInt64(actual)), $"Expected:\t{expected}\r\nActual:\t{actual}");
                }
                else if (IsUnsignedInteger(type))
                {
                    Assert.True(Equal(Convert.ToUInt64(expected), Convert.ToUInt64(actual)), $"Expected:\t{expected}\r\nActual:\t{actual}");
                }
                else if(type == typeof(DateTime))
                {
                    Assert.True(Equal((DateTime)(object)expected, (DateTime)(object)actual), $"Expected:\t{expected:O}\r\nActual:\t{actual:O}");
                }
                else if (type == typeof(DateTimeOffset))
                {
                    Assert.True(Equal((DateTimeOffset)(object)expected, (DateTimeOffset)(object)actual), $"Expected:\t{expected:O}\r\nActual:\t{actual:O}");
                }
                else
                {
                    Assert.Equal(expected, actual);
                }
            }
        }

        private static bool IsSignedInteger(Type type)
            => type == typeof(int)
               || type == typeof(long)
               || type == typeof(short)
               || type == typeof(sbyte);

        private static bool IsUnsignedInteger(Type type)
            => type == typeof(byte)
               || type == typeof(uint)
               || type == typeof(ulong)
               || type == typeof(ushort)
               || type == typeof(char);

        private bool Equal(long left, long right)
        {
            if (left >= 0
                && right >= 0)
            {
                return Equal((ulong)left, (ulong)right);
            }

            if (left < 0
                && right < 0)
            {
                return Equal((ulong)-left, (ulong)-right);
            }

            return false;
        }

        private bool Equal(ulong left, ulong right)
        {
            if (Fixture.IntegerPrecision < 64)
            {
                var largestPrecise = 1ul << Fixture.IntegerPrecision;
                while (left > largestPrecise)
                {
                    left >>= 1;
                    right >>= 1;
                }
            }

            return left == right;
        }

        private bool Equal(DateTime left, DateTime right)
            => left.Equals(right) && (!Fixture.PreservesDateTimeKind || left.Kind == right.Kind);

        private bool Equal(DateTimeOffset left, DateTimeOffset right)
            => left.Equals(right); // => left.EqualsExact(right);

        #endregion

        public class CustomConvertersMySqlFixture : CustomConvertersFixtureBase
        {
            public override bool StrictEquality => false;

            public override bool SupportsAnsi => true;

            public override bool SupportsUnicodeToAnsiConversion => false;

            public override bool SupportsLargeStringComparisons => true;

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override bool SupportsBinaryKeys => true;

            public override DateTime DefaultDateTime => new DateTime();

            public override bool PreservesDateTimeKind => false;

            public override bool SupportsDecimalComparisons => false;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                var ciCollation = ((MySqlTestStore)TestStore).GetCaseInsensitiveUtf8Mb4Collation();

                // Needed to make Can_insert_and_read_back_with_case_insensitive_string_key() work.
                modelBuilder.Entity<StringForeignKeyDataType>()
                    .Property(e => e.StringKeyDataTypeId)
                    .UseCollation(ciCollation);
                modelBuilder.Entity<StringKeyDataType>()
                    .Property(e => e.Id)
                    .UseCollation(ciCollation);
            }
        }
    }
}
