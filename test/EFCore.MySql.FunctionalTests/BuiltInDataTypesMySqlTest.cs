using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class BuiltInDataTypesMySqlTest :
        BuiltInDataTypesTestBase<BuiltInDataTypesMySqlTest.BuiltInDataTypesMySqlFixture>
    {
        public BuiltInDataTypesMySqlTest(BuiltInDataTypesMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // Blocked by EF #11929
        public override void Can_query_using_any_data_type()
        {
        }

        public override void Can_query_using_any_nullable_data_type()
        {
        }

        public override void Can_query_using_any_nullable_data_type_as_literal()
        {
        }

        public override void Can_query_using_any_data_type_shadow()
        {
        }

        public override void Can_query_using_any_data_type_nullable_shadow()
        {
        }

        public override void Can_perform_query_with_ansi_strings_test()
        {
        }

        [Fact]
        public void Sql_translation_uses_type_mapper_when_constant()
        {
            using (var context = CreateContext())
            {
                var results
                    = context.Set<MappedNullableDataTypes>()
                        .Where(e => e.TimeSpanAsTime == new TimeSpan(0, 1, 2))
                        .Select(e => e.Int)
                        .ToList();

                Assert.Empty(results);
                Assert.Equal(
                    @"SELECT `e`.`Int`
FROM `MappedNullableDataTypes` AS `e`
WHERE `e`.`TimeSpanAsTime` = '00:01:02'",
                    Sql,
                    ignoreLineEndingDifferences: true);
            }
        }

        [Fact]
        public void Sql_translation_uses_type_mapper_when_parameter()
        {
            using (var context = CreateContext())
            {
                var timeSpan = new TimeSpan(2, 1, 0);

                var results
                    = context.Set<MappedNullableDataTypes>()
                        .Where(e => e.TimeSpanAsTime == timeSpan)
                        .Select(e => e.Int)
                        .ToList();

                Assert.Empty(results);
                Assert.Equal(
                    @"@__timeSpan_0='02:01:00' (Size = 3)

SELECT `e`.`Int`
FROM `MappedNullableDataTypes` AS `e`
WHERE `e`.`TimeSpanAsTime` = @__timeSpan_0",
                    Sql,
                    ignoreLineEndingDifferences: true);
            }
        }

        [Fact]
        public virtual void Can_query_using_any_mapped_data_type()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(
                    new MappedNullableDataTypes
                    {
                        Int = 999,
                        LongAsBigint = 78L,
                        ShortAsSmallint = 79,
                        ByteAsTinyint = 80,
                        UintAsInt = uint.MaxValue,
                        UlongAsBigint = ulong.MaxValue,
                        UShortAsSmallint = ushort.MaxValue,
                        SbyteAsTinyint = sbyte.MinValue,
                        BoolAsBit = true,
                        DecimalAsDecimal = 81.1m,
                        DecimalAsFixed = 82.2m,
                        DoubleAsReal = 83.3,
                        FloatAsFloat = 84.4f,
                        DoubleAsDoublePrecision = 85.5,
                        DateTimeAsDate = new DateTime(1605, 1, 2, 10, 11, 12),
                        DateTimeOffsetAsDatetime = new DateTimeOffset(new DateTime(), TimeSpan.Zero),
                        DateTimeOffsetAsTimestamp = new DateTimeOffset(new DateTime(2018, 1, 2, 14, 11, 12), TimeSpan.Zero),
                        DateTimeAsDatetime = new DateTime(2019, 1, 2, 14, 11, 12),
                        TimeSpanAsTime = new TimeSpan(0, 11, 15, 12, 2),
                        StringAsChar = "C",
                        StringAsNChar = "Your",
                        StringAsVarchar = "strong",
                        StringAsNvarchar = "don't",
                        StringAsTinytext = "help",
                        StringAsMediumtext = "anyone!",
                        StringAsText = "Gumball Rules!",
                        StringAsNtext = "Gumball Rules OK!",
                        BytesAsVarbinary = new byte[] { 89, 90, 91, 92 },
                        BytesAsBinary = new byte[] { 93, 94, 95, 96 },
                        BytesAsBlob = new byte[] { 97, 98, 99, 100 },
                        GuidAsUniqueidentifier = new Guid("A8F9F951-145F-4545-AC60-B92FF57ADA47"),
                        UintAsBigint = uint.MaxValue,
                        UlongAsDecimal200 = ulong.MaxValue,
                        UShortAsInt = ushort.MaxValue,
                        SByteAsSmallint = sbyte.MinValue,
                        CharAsVarchar = 'A',
                        CharAsNvarchar = 'D',
                        CharAsText = 'G',
                        CharAsInt = 'I',
                        EnumAsNvarchar20 = StringEnumU16.Value4,
                        EnumAsVarchar20 = StringEnum16.Value2
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999);

                long? param1 = 78L;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.LongAsBigint == param1));

                short? param2 = 79;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.ShortAsSmallint == param2));

                byte? param3 = 80;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.ByteAsTinyint == param3));

                bool? param4 = true;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.BoolAsBit == param4));

                decimal? param5 = 81.1m;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.DecimalAsDecimal == param5));

                decimal? param6 = 82.2m;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.DecimalAsFixed == param6));

                double? param7a = 83.29;
                double? param7aa = 83.31;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(
                        e => e.Int == 999 && e.DoubleAsReal >= param7a && e.DoubleAsReal <= param7aa));

                float? param7b = 84.39f;
                float? param7bb = 84.41f;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(
                        e => e.Int == 999 && e.FloatAsFloat >= param7b && e.FloatAsFloat <= param7bb));

                double? param7c = 85.49;
                double? param7cc = 85.51;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(
                    e => e.Int == 999 && e.DoubleAsDoublePrecision >= param7c && e.DoubleAsDoublePrecision <= param7cc));

                DateTime? param8 = new DateTime(1605, 1, 2);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.DateTimeAsDate == param8));

                DateTimeOffset? param9 = new DateTimeOffset(new DateTime(), TimeSpan.Zero);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.DateTimeOffsetAsDatetime == param9));

                DateTimeOffset? param10 = new DateTimeOffset(new DateTime(2018, 1, 2, 14, 11, 12), TimeSpan.Zero);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.DateTimeOffsetAsTimestamp == param10));

                DateTime? param11 = new DateTime(2019, 1, 2, 14, 11, 12);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.DateTimeAsDatetime == param11));

                TimeSpan? param13 = new TimeSpan(0, 11, 15, 12, 2);
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.TimeSpanAsTime == param13));

                var param19 = "C";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.StringAsChar == param19));

                var param20 = "Your";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.StringAsNChar == param20));

                var param21 = "strong";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.StringAsVarchar == param21));

                var param27 = "don't";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.StringAsNvarchar == param27));

                var param28 = "help";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.StringAsTinytext == param28));

                var param29 = "anyone!";
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.StringAsMediumtext == param29));

                var param35 = new byte[] { 89, 90, 91, 92 };
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.BytesAsVarbinary == param35));

                var param36 = new byte[] { 93, 94, 95, 96, 0, 0 };
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.BytesAsBinary == param36));

                uint? param41 = uint.MaxValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.UintAsInt == param41));

                ulong? param42 = ulong.MaxValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.UlongAsBigint == param42));

                ushort? param43 = ushort.MaxValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.UShortAsSmallint == param43));

                sbyte? param44 = sbyte.MinValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.SbyteAsTinyint == param44));

                uint? param45 = uint.MaxValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.UintAsBigint == param45));

                ulong? param46 = ulong.MaxValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.UlongAsDecimal200 == param46));

                ushort? param47 = ushort.MaxValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.UShortAsInt == param47));

                sbyte? param48 = sbyte.MinValue;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.SByteAsSmallint == param48));

                Guid? param49 = new Guid("A8F9F951-145F-4545-AC60-B92FF57ADA47");
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.GuidAsUniqueidentifier == param49));

                char? param50 = 'A';
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.CharAsVarchar == param50));

                char? param53 = 'D';
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.CharAsNvarchar == param53));

                char? param58 = 'I';
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.CharAsInt == param58));

                StringEnumU16? param59 = StringEnumU16.Value4;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.EnumAsNvarchar20 == param59));

                StringEnum16? param60 = StringEnum16.Value2;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 999 && e.EnumAsVarchar20 == param60));
            }
        }

        [Fact]
        public virtual void Can_query_using_any_mapped_data_types_with_nulls()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(
                    new MappedNullableDataTypes
                    {
                        Int = 911
                    });

                Assert.Equal(1, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                var entity = context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911);

                long? param1 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.LongAsBigint == param1));

                short? param2 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.ShortAsSmallint == param2));
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && (long?)(int?)e.ShortAsSmallint == param2));

                byte? param3 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.ByteAsTinyint == param3));

                bool? param4 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.BoolAsBit == param4));

                decimal? param5 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DecimalAsDecimal == param5));

                decimal? param6 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DecimalAsFixed == param6));

                double? param7a = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DoubleAsReal == param7a));

                float? param7b = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.FloatAsFloat == param7b));

                double? param7c = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DoubleAsDoublePrecision == param7c));

                DateTime? param8 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DateTimeAsDate == param8));

                DateTimeOffset? param9 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DateTimeOffsetAsDatetime == param9));

                DateTime? param10 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DateTimeOffsetAsTimestamp == param10));

                DateTime? param11 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.DateTimeAsDatetime == param11));

                TimeSpan? param13 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.TimeSpanAsTime == param13));

                string param19 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsChar == param19));

                string param20 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsNChar == param20));

                string param21 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsVarchar == param21));

                string param27 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsNvarchar == param27));

                string param28 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsTinytext == param28));

                string param29 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsMediumtext == param29));

                string param30 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsText == param30));

                string param31 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.StringAsNtext == param31));

                byte[] param35 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.BytesAsVarbinary == param35));

                byte[] param36 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.BytesAsBinary == param36));

                byte[] param37 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.BytesAsBlob == param37));

                uint? param41 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.UintAsInt == param41));

                ulong? param42 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.UlongAsBigint == param42));

                ushort? param43 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.UShortAsSmallint == param43));

                sbyte? param44 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.SbyteAsTinyint == param44));

                uint? param45 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.UintAsBigint == param45));

                ulong? param46 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.UlongAsDecimal200 == param46));

                ushort? param47 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.UShortAsInt == param47));

                sbyte? param48 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.SByteAsSmallint == param48));

                Guid? param49 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.GuidAsUniqueidentifier == param49));

                char? param50 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.CharAsVarchar == param50));

                char? param53 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.CharAsNvarchar == param53));

                char? param56 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.CharAsText == param56));

                char? param58 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.CharAsInt == param58));

                StringEnumU16? param59 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.EnumAsNvarchar20 == param59));

                StringEnum16? param60 = null;
                Assert.Same(entity, context.Set<MappedNullableDataTypes>().Single(e => e.Int == 911 && e.EnumAsVarchar20 == param60));
            }
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_data_types()
        {
            var entity = CreateMappedDataTypes(77);
            using (var context = CreateContext())
            {
                context.Set<MappedDataTypes>().Add(entity);

                Assert.Equal(1, context.SaveChanges());
            }

            var parameters = DumpParameters();
            Assert.Equal(
                @"@p0='77'
@p1='True'
@p2='80'
@p3='0x5D5E5F60' (Nullable = false) (Size = 5)
@p4='0x61626365' (Nullable = false) (Size = 8000)
@p5='0x61626367' (Nullable = false) (Size = 8000)
@p6='0x61626366' (Nullable = false) (Size = 8000)
@p7='0x61626364' (Nullable = false) (Size = 8000)
@p8='0x595A5B5C' (Nullable = false) (Size = 255)
@p9='73'
@p10='D' (Nullable = false) (Size = 20)
@p11='A' (Nullable = false) (Size = 1) (DbType = AnsiString)
@p12='2015-01-02T10:11:12' (DbType = Date)
@p13='2019-01-02T14:11:12' (DbType = DateTime)
@p14='2016-01-02T11:11:12.0000000+00:00'
@p15='2017-01-02T12:11:12.0000000+02:00'
@p16='81.1'
@p17='85.5'
@p18='83.3'
@p19='Value4' (Nullable = false) (Size = 20)
@p20='Value2' (Nullable = false) (Size = 20) (DbType = AnsiString)
@p21='a8f9f951-145f-4545-ac60-b92ff57ada47' (Size = 36)
@p22='78'
@p23='-128'
@p24='128'
@p25='79'
@p26='Your' (Nullable = false) (Size = 10) (DbType = AnsiStringFixedLength)
@p27='arm' (Nullable = false) (Size = 8000) (DbType = AnsiString)
@p28='anyone!' (Nullable = false) (Size = 8000) (DbType = AnsiString)
@p29='strong' (Nullable = false) (Size = 10) (DbType = StringFixedLength)
@p30='Gumball Rules OK!' (Nullable = false) (Size = 4000)
@p31='" + entity.StringAsNvarchar + @"' (Nullable = false) (Size = -1)
@p32='Gumball Rules!' (Nullable = false) (Size = 8000) (DbType = AnsiString)
@p33='help' (Nullable = false) (Size = 8000) (DbType = AnsiString)
@p34='" + entity.StringAsVarchar + @"' (Nullable = false) (Size = -1) (DbType = AnsiString)
@p35='11:15:12'
@p36='65535'
@p37='65535'
@p38='4294967295'
@p39='4294967295'
@p40='18446744073709551615'
@p41='18446744073709551615'",
                parameters,
                ignoreLineEndingDifferences: true);

            using (var context = CreateContext())
            {
                AssertMappedDataTypes(context.Set<MappedDataTypes>().Single(e => e.Int == 77), 77);
            }
        }

        private string DumpParameters()
            => Fixture.TestSqlLoggerFactory.Parameters.Single().Replace(", ", _eol);

        private static void AssertMappedDataTypes(MappedDataTypes entity, int id)
        {
            var expected = CreateMappedDataTypes(id);
            Assert.Equal(id, entity.Int);
            Assert.Equal(78, entity.LongAsBigInt);
            Assert.Equal(79, entity.ShortAsSmallint);
            Assert.Equal(80, entity.ByteAsTinyint);
            Assert.Equal(uint.MaxValue, entity.UintAsInt);
            Assert.Equal(ulong.MaxValue, entity.UlongAsBigint);
            Assert.Equal(ushort.MaxValue, entity.UShortAsSmallint);
            Assert.Equal(sbyte.MinValue, entity.SByteAsTinyint);
            Assert.True(entity.BoolAsBit);
            Assert.Equal(81.1m, entity.DecimalAsDecimal);
            Assert.Equal(83.3, entity.DoubleAsFloat, 1);
            Assert.Equal(85.5, entity.DoubleAsDouble, 1);
            Assert.Equal(new DateTime(2015, 1, 2), entity.DateTimeAsDate);
            Assert.Equal(new DateTimeOffset(new DateTime(2016, 1, 2, 11, 11, 12), TimeSpan.Zero), entity.DateTimeOffsetAsDatetime);
            Assert.Equal(new DateTimeOffset(new DateTime(2017, 1, 2, 12, 11, 12), TimeSpan.FromHours(2)), entity.DateTimeOffsetAsTimestamp);
            Assert.Equal(new DateTime(2019, 1, 2, 14, 11, 12), entity.DateTimeAsDatetime);
            Assert.Equal(new TimeSpan(11, 15, 12), entity.TimeSpanAsTime);
            Assert.Equal(expected.StringAsVarchar, entity.StringAsVarchar);
            Assert.Equal("Your", entity.StringAsChar);
            Assert.Equal("strong", entity.StringAsNChar);
            Assert.Equal(expected.StringAsNvarchar, entity.StringAsNvarchar);
            Assert.Equal("help", entity.StringAsTinytext);
            Assert.Equal("anyone!", entity.StringAsMediumtext);
            Assert.Equal("arm", entity.StringAsLongtext);
            Assert.Equal("Gumball Rules!", entity.StringAsText);
            Assert.Equal("Gumball Rules OK!", entity.StringAsNtext);
            Assert.Equal(new byte[] { 89, 90, 91, 92 }, entity.BytesAsVarbinary);
            Assert.Equal(new byte[] { 93, 94, 95, 96, 0 }, entity.BytesAsBinary);
            Assert.Equal(new byte[] { 97, 98, 99, 100 }, entity.BytesAsTinyblob);
            Assert.Equal(new byte[] { 97, 98, 99, 101 }, entity.BytesAsBlob);
            Assert.Equal(new byte[] { 97, 98, 99, 102 }, entity.BytesAsMediumblob);
            Assert.Equal(new byte[] { 97, 98, 99, 103 }, entity.BytesAsLongblob);
            Assert.Equal(new Guid("A8F9F951-145F-4545-AC60-B92FF57ADA47"), entity.GuidAsUniqueidentifier);
            Assert.Equal(uint.MaxValue, entity.UintAsBigint);
            Assert.Equal(ulong.MaxValue, entity.UlongAsDecimal200);
            Assert.Equal(ushort.MaxValue, entity.UShortAsInt);
            Assert.Equal(sbyte.MinValue, entity.SByteAsSmallint);
            Assert.Equal('A', entity.CharAsVarchar);
            Assert.Equal('D', entity.CharAsNvarchar);
            Assert.Equal('I', entity.CharAsInt);
            Assert.Equal(StringEnum16.Value2, entity.EnumAsVarchar20);
            Assert.Equal(StringEnumU16.Value4, entity.EnumAsNvarchar20);
        }

        private static MappedDataTypes CreateMappedDataTypes(int id)
            => new MappedDataTypes
            {
                Int = id,
                LongAsBigInt = 78L,
                ShortAsSmallint = 79,
                ByteAsTinyint = 80,
                UintAsInt = uint.MaxValue,
                UlongAsBigint = ulong.MaxValue,
                UShortAsSmallint = ushort.MaxValue,
                SByteAsTinyint = sbyte.MinValue,
                BoolAsBit = true,
                DecimalAsDecimal = 81.1m,
                DoubleAsFloat = 83.3,
                DoubleAsDouble = 85.5,
                DateTimeAsDate = new DateTime(2015, 1, 2, 10, 11, 12),
                DateTimeOffsetAsDatetime = new DateTimeOffset(new DateTime(2016, 1, 2, 11, 11, 12), TimeSpan.Zero),
                DateTimeOffsetAsTimestamp = new DateTimeOffset(new DateTime(2017, 1, 2, 12, 11, 12), TimeSpan.FromHours(2)),
                DateTimeAsDatetime = new DateTime(2019, 1, 2, 14, 11, 12),
                TimeSpanAsTime = new TimeSpan(11, 15, 12),
                StringAsVarchar = string.Concat(Enumerable.Repeat("C", 8001)),
                StringAsChar = "Your",
                StringAsNChar = "strong",
                StringAsNvarchar = string.Concat(Enumerable.Repeat("D", 4001)),
                StringAsTinytext = "help",
                StringAsMediumtext = "anyone!",
                StringAsLongtext = "arm",
                StringAsText = "Gumball Rules!",
                StringAsNtext = "Gumball Rules OK!",
                BytesAsVarbinary = new byte[] {89, 90, 91, 92},
                BytesAsBinary = new byte[] {93, 94, 95, 96},
                BytesAsTinyblob = new byte[] {97, 98, 99, 100},
                BytesAsBlob = new byte[] {97, 98, 99, 101},
                BytesAsMediumblob = new byte[] {97, 98, 99, 102},
                BytesAsLongblob = new byte[] {97, 98, 99, 103},
                GuidAsUniqueidentifier = new Guid("A8F9F951-145F-4545-AC60-B92FF57ADA47"),
                UintAsBigint = uint.MaxValue,
                UlongAsDecimal200 = ulong.MaxValue,
                UShortAsInt = ushort.MaxValue,
                SByteAsSmallint = sbyte.MinValue,
                CharAsVarchar = 'A',
                CharAsNvarchar = 'D',
                CharAsInt = 'I',
                EnumAsNvarchar20 = StringEnumU16.Value4,
                EnumAsVarchar20 = StringEnum16.Value2
            };

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_nullable_data_types()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(CreateMappedNullableDataTypes(77));

                Assert.Equal(1, context.SaveChanges());
            }

            var parameters = DumpParameters();
            Assert.Equal(
                @"@p0='77'
@p1='True' (Nullable = true)
@p2='80' (Nullable = true)
@p3='0x5D5E5F60' (Size = 6)
@p4='0x61626364' (Size = 8000)
@p5='0x595A5B5C' (Size = 255)
@p6='73' (Nullable = true)
@p7='D' (Size = 1)
@p8='G' (Size = 1) (DbType = AnsiString)
@p9='A' (Size = 1) (DbType = AnsiString)
@p10='2015-01-02T10:11:12' (Nullable = true) (DbType = Date)
@p11='2019-01-02T14:11:12' (Nullable = true) (DbType = DateTime)
@p12='2016-01-02T11:11:12.0000000+00:00' (Nullable = true) (Size = 6)
@p13='2017-01-02T12:11:12.0000000+06:00' (Nullable = true) (Size = 6)
@p14='81.1' (Nullable = true)
@p15='82.2' (Nullable = true)
@p16='85.5' (Nullable = true)
@p17='83.3' (Nullable = true)
@p18='Value4' (Size = 20)
@p19='Value2' (Size = 20) (DbType = AnsiString)
@p20='84.4' (Nullable = true)
@p21='a8f9f951-145f-4545-ac60-b92ff57ada47' (Nullable = true) (Size = 36)
@p22='78' (Nullable = true)
@p23='-128' (Nullable = true)
@p24='-128' (Nullable = true)
@p25='79' (Nullable = true)
@p26='C' (Size = 20) (DbType = AnsiStringFixedLength)
@p27='anyone!' (Size = 4000)
@p28='Your' (Size = 20) (DbType = StringFixedLength)
@p29='Gumball Rules OK!' (Size = 4000)
@p30='don't' (Size = 55)
@p31='Gumball Rules!' (Size = 55) (DbType = AnsiString)
@p32='help' (Size = 4000)
@p33='strong' (Size = 55) (DbType = AnsiString)
@p34='11:15:12' (Nullable = true) (Size = 3)
@p35='65535' (Nullable = true)
@p36='-1' (Nullable = true)
@p37='4294967295' (Nullable = true)
@p38='-1' (Nullable = true)
@p39='-1' (Nullable = true)
@p40='18446744073709551615' (Nullable = true)",
                parameters,
                ignoreLineEndingDifferences: true);

            using (var context = CreateContext())
            {
                AssertMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 77), 77);
            }
        }

        private static void AssertMappedNullableDataTypes(MappedNullableDataTypes entity, int id)
        {
            Assert.Equal(id, entity.Int);
            Assert.Equal(78, entity.LongAsBigint);
            Assert.Equal(79, entity.ShortAsSmallint.Value);
            Assert.Equal(80, entity.ByteAsTinyint.Value);
            Assert.Equal(uint.MaxValue, entity.UintAsInt);
            Assert.Equal(ulong.MaxValue, entity.UlongAsBigint);
            Assert.Equal(ushort.MaxValue, entity.UShortAsSmallint);
            Assert.Equal(sbyte.MinValue, entity.SbyteAsTinyint);
            Assert.True(entity.BoolAsBit);
            Assert.Equal(81.1m, entity.DecimalAsDecimal);
            Assert.Equal(82.2m, entity.DecimalAsFixed);
            Assert.Equal(83.3, entity.DoubleAsReal.Value, 1);
            Assert.Equal(84.4f, entity.FloatAsFloat.Value, 1);
            Assert.Equal(85.5, entity.DoubleAsDoublePrecision.Value, 1);
            Assert.Equal(new DateTime(2015, 1, 2), entity.DateTimeAsDate);
            Assert.Equal(new DateTimeOffset(new DateTime(2016, 1, 2, 11, 11, 12), TimeSpan.Zero), entity.DateTimeOffsetAsDatetime);
            Assert.Equal(new DateTimeOffset(new DateTime(2017, 1, 2, 12, 11, 12), TimeSpan.FromHours(6)),
                entity.DateTimeOffsetAsTimestamp);
            Assert.Equal(new DateTime(2019, 1, 2, 14, 11, 12), entity.DateTimeAsDatetime);
            Assert.Equal(new TimeSpan(11, 15, 12), entity.TimeSpanAsTime);
            Assert.Equal("C", entity.StringAsChar);
            Assert.Equal("Your", entity.StringAsNChar);
            Assert.Equal("strong", entity.StringAsVarchar);
            Assert.Equal("don't", entity.StringAsNvarchar);
            Assert.Equal("help", entity.StringAsTinytext);
            Assert.Equal("anyone!", entity.StringAsMediumtext);
            Assert.Equal("Gumball Rules!", entity.StringAsText);
            Assert.Equal("Gumball Rules OK!", entity.StringAsNtext);
            Assert.Equal(new byte[] { 89, 90, 91, 92 }, entity.BytesAsVarbinary);
            Assert.Equal(new byte[] { 93, 94, 95, 96, 0, 0 }, entity.BytesAsBinary);
            Assert.Equal(new byte[] { 97, 98, 99, 100 }, entity.BytesAsBlob);
            Assert.Equal(new Guid("A8F9F951-145F-4545-AC60-B92FF57ADA47"), entity.GuidAsUniqueidentifier);
            Assert.Equal(uint.MaxValue, entity.UintAsBigint);
            Assert.Equal(ulong.MaxValue, entity.UlongAsDecimal200);
            Assert.Equal(ushort.MaxValue, entity.UShortAsInt);
            Assert.Equal(sbyte.MinValue, entity.SByteAsSmallint);
            Assert.Equal('A', entity.CharAsVarchar);
            Assert.Equal('D', entity.CharAsNvarchar);
            Assert.Equal('G', entity.CharAsText);
            Assert.Equal('I', entity.CharAsInt);
            Assert.Equal(StringEnum16.Value2, entity.EnumAsVarchar20);
            Assert.Equal(StringEnumU16.Value4, entity.EnumAsNvarchar20);
        }

        private static MappedNullableDataTypes CreateMappedNullableDataTypes(int id)
            => new MappedNullableDataTypes
            {
                Int = id,
                LongAsBigint = 78L,
                ShortAsSmallint = 79,
                ByteAsTinyint = 80,
                UintAsInt = uint.MaxValue,
                UlongAsBigint = ulong.MaxValue,
                UShortAsSmallint = ushort.MaxValue,
                SbyteAsTinyint = sbyte.MinValue,
                BoolAsBit = true,
                DecimalAsDecimal = 81.1m,
                DecimalAsFixed = 82.2m,
                DoubleAsReal = 83.3,
                FloatAsFloat = 84.4f,
                DoubleAsDoublePrecision = 85.5,
                DateTimeAsDate = new DateTime(2015, 1, 2, 10, 11, 12),
                DateTimeOffsetAsDatetime = new DateTimeOffset(new DateTime(2016, 1, 2, 11, 11, 12), TimeSpan.Zero),
                DateTimeOffsetAsTimestamp = new DateTimeOffset(new DateTime(2017, 1, 2, 12, 11, 12), TimeSpan.FromHours(6)),
                DateTimeAsDatetime = new DateTime(2019, 1, 2, 14, 11, 12),
                TimeSpanAsTime = new TimeSpan(11, 15, 12),
                StringAsChar = "C",
                StringAsNChar = "Your",
                StringAsVarchar = "strong",
                StringAsNvarchar = "don't",
                StringAsTinytext = "help",
                StringAsMediumtext = "anyone!",
                StringAsText = "Gumball Rules!",
                StringAsNtext = "Gumball Rules OK!",
                BytesAsVarbinary = new byte[] { 89, 90, 91, 92 },
                BytesAsBinary = new byte[] { 93, 94, 95, 96 },
                BytesAsBlob = new byte[] { 97, 98, 99, 100 },
                GuidAsUniqueidentifier = new Guid("A8F9F951-145F-4545-AC60-B92FF57ADA47"),
                UintAsBigint = uint.MaxValue,
                UlongAsDecimal200 = ulong.MaxValue,
                UShortAsInt = ushort.MaxValue,
                SByteAsSmallint = sbyte.MinValue,
                CharAsVarchar = 'A',
                CharAsNvarchar = 'D',
                CharAsText = 'G',
                CharAsInt = 'I',
                EnumAsNvarchar20 = StringEnumU16.Value4,
                EnumAsVarchar20 = StringEnum16.Value2
            };

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_data_types_set_to_null()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(new MappedNullableDataTypes { Int = 78 });

                Assert.Equal(1, context.SaveChanges());
            }


            var parameters = DumpParameters();
            Assert.Equal(
                @"@p0='78'
@p1='' (DbType = Boolean)
@p2='' (DbType = SByte)
@p3='' (Size = 6) (DbType = Binary)
@p4='' (Size = 8000) (DbType = Binary)
@p5='' (Size = 255) (DbType = Binary)
@p6='' (DbType = Int32)
@p7='' (Size = 1)
@p8='' (Size = 1) (DbType = AnsiString)
@p9='' (Size = 1) (DbType = AnsiString)
@p10='' (DbType = Date)
@p11='' (DbType = DateTime)
@p12='' (Size = 6) (DbType = DateTimeOffset)
@p13='' (Size = 6) (DbType = DateTimeOffset)
@p14='' (DbType = Decimal)
@p15='' (DbType = Decimal)
@p16='' (DbType = Double)
@p17='' (DbType = Double)
@p18='' (Size = 20)
@p19='' (Size = 20) (DbType = AnsiString)
@p20='' (DbType = Single)
@p21='' (Size = 36) (DbType = Guid)
@p22='' (DbType = Int64)
@p23='' (DbType = Int16)
@p24='' (DbType = SByte)
@p25='' (DbType = Int16)
@p26='' (Size = 20) (DbType = AnsiStringFixedLength)
@p27='' (Size = 4000)
@p28='' (Size = 20) (DbType = StringFixedLength)
@p29='' (Size = 4000)
@p30='' (Size = 55)
@p31='' (Size = 55) (DbType = AnsiString)
@p32='' (Size = 4000)
@p33='' (Size = 55) (DbType = AnsiString)
@p34='' (Size = 3) (DbType = Time)
@p35='' (DbType = Int32)
@p36='' (DbType = Int16)
@p37='' (DbType = Int64)
@p38='' (DbType = Int32)
@p39='' (DbType = Int64)
@p40='' (DbType = Decimal)",
                parameters,
                ignoreLineEndingDifferences: true);

            using (var context = CreateContext())
            {
                AssertNullMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 78), 78);
            }
        }

        private static void AssertNullMappedNullableDataTypes(MappedNullableDataTypes entity, int id)
        {
            Assert.Equal(id, entity.Int);
            Assert.Null(entity.LongAsBigint);
            Assert.Null(entity.ShortAsSmallint);
            Assert.Null(entity.ByteAsTinyint);
            Assert.Null(entity.UintAsInt);
            Assert.Null(entity.UlongAsBigint);
            Assert.Null(entity.UShortAsSmallint);
            Assert.Null(entity.SbyteAsTinyint);
            Assert.Null(entity.BoolAsBit);
            Assert.Null(entity.DecimalAsDecimal);
            Assert.Null(entity.DecimalAsFixed);
            Assert.Null(entity.DoubleAsReal);
            Assert.Null(entity.FloatAsFloat);
            Assert.Null(entity.DoubleAsDoublePrecision);
            Assert.Null(entity.DateTimeAsDate);
            Assert.Null(entity.DateTimeOffsetAsDatetime);
            Assert.Null(entity.DateTimeOffsetAsTimestamp);
            Assert.Null(entity.DateTimeAsDatetime);
            Assert.Null(entity.TimeSpanAsTime);
            Assert.Null(entity.StringAsChar);
            Assert.Null(entity.StringAsNChar);
            Assert.Null(entity.StringAsVarchar);
            Assert.Null(entity.StringAsNvarchar);
            Assert.Null(entity.StringAsTinytext);
            Assert.Null(entity.StringAsMediumtext);
            Assert.Null(entity.StringAsText);
            Assert.Null(entity.StringAsNtext);
            Assert.Null(entity.BytesAsVarbinary);
            Assert.Null(entity.BytesAsBinary);
            Assert.Null(entity.BytesAsBlob);
            Assert.Null(entity.GuidAsUniqueidentifier);
            Assert.Null(entity.UintAsBigint);
            Assert.Null(entity.UlongAsDecimal200);
            Assert.Null(entity.UShortAsInt);
            Assert.Null(entity.SByteAsSmallint);
            Assert.Null(entity.CharAsVarchar);
            Assert.Null(entity.CharAsNvarchar);
            Assert.Null(entity.CharAsText);
            Assert.Null(entity.CharAsInt);
            Assert.Null(entity.EnumAsNvarchar20);
            Assert.Null(entity.EnumAsVarchar20);
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_data_types_in_batch()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedDataTypes>().Add(CreateMappedDataTypes(177));
                context.Set<MappedDataTypes>().Add(CreateMappedDataTypes(178));
                context.Set<MappedDataTypes>().Add(CreateMappedDataTypes(179));

                Assert.Equal(3, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                AssertMappedDataTypes(context.Set<MappedDataTypes>().Single(e => e.Int == 177), 177);
                AssertMappedDataTypes(context.Set<MappedDataTypes>().Single(e => e.Int == 178), 178);
                AssertMappedDataTypes(context.Set<MappedDataTypes>().Single(e => e.Int == 179), 179);
            }
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_nullable_data_types_in_batch()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(CreateMappedNullableDataTypes(177));
                context.Set<MappedNullableDataTypes>().Add(CreateMappedNullableDataTypes(178));
                context.Set<MappedNullableDataTypes>().Add(CreateMappedNullableDataTypes(179));

                Assert.Equal(3, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                AssertMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 177), 177);
                AssertMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 178), 178);
                AssertMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 179), 179);
            }
        }

        [Fact]
        public virtual void Can_insert_and_read_back_all_mapped_data_types_set_to_null_in_batch()
        {
            using (var context = CreateContext())
            {
                context.Set<MappedNullableDataTypes>().Add(new MappedNullableDataTypes { Int = 278 });
                context.Set<MappedNullableDataTypes>().Add(new MappedNullableDataTypes { Int = 279 });
                context.Set<MappedNullableDataTypes>().Add(new MappedNullableDataTypes { Int = 280 });

                Assert.Equal(3, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                AssertNullMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 278), 278);
                AssertNullMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 279), 279);
                AssertNullMappedNullableDataTypes(context.Set<MappedNullableDataTypes>().Single(e => e.Int == 280), 280);
            }
        }

        [ConditionalFact]
        public virtual void Columns_have_expected_data_types()
        {
            var actual = QueryForColumnTypes(CreateContext());

            const string expected = @"binaryforeignkeydatatype.BinaryKeyDataTypeId ---> [nullable varbinary] [MaxLength = 3072]
binaryforeignkeydatatype.Id ---> [int] [Precision = 10 Scale = 0]
binarykeydatatype.Id ---> [varbinary] [MaxLength = 3072]
builtindatatypes.Enum16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypes.Enum32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypes.Enum64 ---> [bigint] [Precision = 19 Scale = 0]
builtindatatypes.Enum8 ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypes.EnumS8 ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypes.EnumU16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypes.EnumU32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypes.EnumU64 ---> [bigint] [Precision = 20 Scale = 0]
builtindatatypes.Id ---> [int] [Precision = 10 Scale = 0]
builtindatatypes.PartitionId ---> [int] [Precision = 10 Scale = 0]
builtindatatypes.TestBoolean ---> [bit] [Precision = 1]
builtindatatypes.TestByte ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypes.TestCharacter ---> [varchar] [MaxLength = 1]
builtindatatypes.TestDateTime ---> [datetime] [Precision = 6]
builtindatatypes.TestDateTimeOffset ---> [datetime] [Precision = 6]
builtindatatypes.TestDecimal ---> [decimal] [Precision = 65 Scale = 30]
builtindatatypes.TestDouble ---> [double] [Precision = 22]
builtindatatypes.TestInt16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypes.TestInt32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypes.TestInt64 ---> [bigint] [Precision = 19 Scale = 0]
builtindatatypes.TestSignedByte ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypes.TestSingle ---> [float] [Precision = 12]
builtindatatypes.TestTimeSpan ---> [time] [Precision = 6]
builtindatatypes.TestUnsignedInt16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypes.TestUnsignedInt32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypes.TestUnsignedInt64 ---> [bigint] [Precision = 20 Scale = 0]
builtindatatypesshadow.Enum16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypesshadow.Enum32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypesshadow.Enum64 ---> [bigint] [Precision = 19 Scale = 0]
builtindatatypesshadow.Enum8 ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypesshadow.EnumS8 ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypesshadow.EnumU16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypesshadow.EnumU32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypesshadow.EnumU64 ---> [bigint] [Precision = 20 Scale = 0]
builtindatatypesshadow.Id ---> [int] [Precision = 10 Scale = 0]
builtindatatypesshadow.PartitionId ---> [int] [Precision = 10 Scale = 0]
builtindatatypesshadow.TestBoolean ---> [bit] [Precision = 1]
builtindatatypesshadow.TestByte ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypesshadow.TestCharacter ---> [varchar] [MaxLength = 1]
builtindatatypesshadow.TestDateTime ---> [datetime] [Precision = 6]
builtindatatypesshadow.TestDateTimeOffset ---> [datetime] [Precision = 6]
builtindatatypesshadow.TestDecimal ---> [decimal] [Precision = 65 Scale = 30]
builtindatatypesshadow.TestDouble ---> [double] [Precision = 22]
builtindatatypesshadow.TestInt16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypesshadow.TestInt32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypesshadow.TestInt64 ---> [bigint] [Precision = 19 Scale = 0]
builtindatatypesshadow.TestSignedByte ---> [tinyint] [Precision = 3 Scale = 0]
builtindatatypesshadow.TestSingle ---> [float] [Precision = 12]
builtindatatypesshadow.TestTimeSpan ---> [time] [Precision = 6]
builtindatatypesshadow.TestUnsignedInt16 ---> [smallint] [Precision = 5 Scale = 0]
builtindatatypesshadow.TestUnsignedInt32 ---> [int] [Precision = 10 Scale = 0]
builtindatatypesshadow.TestUnsignedInt64 ---> [bigint] [Precision = 20 Scale = 0]
builtinnullabledatatypes.Enum16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypes.Enum32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypes.Enum64 ---> [nullable bigint] [Precision = 19 Scale = 0]
builtinnullabledatatypes.Enum8 ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypes.EnumS8 ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypes.EnumU16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypes.EnumU32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypes.EnumU64 ---> [nullable bigint] [Precision = 20 Scale = 0]
builtinnullabledatatypes.Id ---> [int] [Precision = 10 Scale = 0]
builtinnullabledatatypes.PartitionId ---> [int] [Precision = 10 Scale = 0]
builtinnullabledatatypes.TestByteArray ---> [nullable longblob] [MaxLength = -1]
builtinnullabledatatypes.TestNullableBoolean ---> [nullable bit] [Precision = 1]
builtinnullabledatatypes.TestNullableByte ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypes.TestNullableCharacter ---> [nullable varchar] [MaxLength = 1]
builtinnullabledatatypes.TestNullableDateTime ---> [nullable datetime] [Precision = 6]
builtinnullabledatatypes.TestNullableDateTimeOffset ---> [nullable datetime] [Precision = 6]
builtinnullabledatatypes.TestNullableDecimal ---> [nullable decimal] [Precision = 65 Scale = 30]
builtinnullabledatatypes.TestNullableDouble ---> [nullable double] [Precision = 22]
builtinnullabledatatypes.TestNullableInt16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypes.TestNullableInt32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypes.TestNullableInt64 ---> [nullable bigint] [Precision = 19 Scale = 0]
builtinnullabledatatypes.TestNullableSignedByte ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypes.TestNullableSingle ---> [nullable float] [Precision = 12]
builtinnullabledatatypes.TestNullableTimeSpan ---> [nullable time] [Precision = 6]
builtinnullabledatatypes.TestNullableUnsignedInt16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypes.TestNullableUnsignedInt32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypes.TestNullableUnsignedInt64 ---> [nullable bigint] [Precision = 20 Scale = 0]
builtinnullabledatatypes.TestString ---> [nullable longtext] [MaxLength = -1]
builtinnullabledatatypesshadow.Enum16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypesshadow.Enum32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypesshadow.Enum64 ---> [nullable bigint] [Precision = 19 Scale = 0]
builtinnullabledatatypesshadow.Enum8 ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypesshadow.EnumS8 ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypesshadow.EnumU16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypesshadow.EnumU32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypesshadow.EnumU64 ---> [nullable bigint] [Precision = 20 Scale = 0]
builtinnullabledatatypesshadow.Id ---> [int] [Precision = 10 Scale = 0]
builtinnullabledatatypesshadow.PartitionId ---> [int] [Precision = 10 Scale = 0]
builtinnullabledatatypesshadow.TestByteArray ---> [nullable longblob] [MaxLength = -1]
builtinnullabledatatypesshadow.TestNullableBoolean ---> [nullable bit] [Precision = 1]
builtinnullabledatatypesshadow.TestNullableByte ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypesshadow.TestNullableCharacter ---> [nullable varchar] [MaxLength = 1]
builtinnullabledatatypesshadow.TestNullableDateTime ---> [nullable datetime] [Precision = 6]
builtinnullabledatatypesshadow.TestNullableDateTimeOffset ---> [nullable datetime] [Precision = 6]
builtinnullabledatatypesshadow.TestNullableDecimal ---> [nullable decimal] [Precision = 65 Scale = 30]
builtinnullabledatatypesshadow.TestNullableDouble ---> [nullable double] [Precision = 22]
builtinnullabledatatypesshadow.TestNullableInt16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypesshadow.TestNullableInt32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypesshadow.TestNullableInt64 ---> [nullable bigint] [Precision = 19 Scale = 0]
builtinnullabledatatypesshadow.TestNullableSignedByte ---> [nullable tinyint] [Precision = 3 Scale = 0]
builtinnullabledatatypesshadow.TestNullableSingle ---> [nullable float] [Precision = 12]
builtinnullabledatatypesshadow.TestNullableTimeSpan ---> [nullable time] [Precision = 6]
builtinnullabledatatypesshadow.TestNullableUnsignedInt16 ---> [nullable smallint] [Precision = 5 Scale = 0]
builtinnullabledatatypesshadow.TestNullableUnsignedInt32 ---> [nullable int] [Precision = 10 Scale = 0]
builtinnullabledatatypesshadow.TestNullableUnsignedInt64 ---> [nullable bigint] [Precision = 20 Scale = 0]
builtinnullabledatatypesshadow.TestString ---> [nullable longtext] [MaxLength = -1]
mappeddatatypes.BoolAsBit ---> [bit] [Precision = 1]
mappeddatatypes.ByteAsTinyint ---> [tinyint] [Precision = 3 Scale = 0]
mappeddatatypes.BytesAsBinary ---> [binary] [MaxLength = 5]
mappeddatatypes.BytesAsBlob ---> [blob] [MaxLength = 65535]
mappeddatatypes.BytesAsLongblob ---> [longblob] [MaxLength = -1]
mappeddatatypes.BytesAsMediumblob ---> [mediumblob] [MaxLength = 16777215]
mappeddatatypes.BytesAsTinyblob ---> [tinyblob] [MaxLength = 255]
mappeddatatypes.BytesAsVarbinary ---> [varbinary] [MaxLength = 255]
mappeddatatypes.CharAsInt ---> [int] [Precision = 10 Scale = 0]
mappeddatatypes.CharAsNvarchar ---> [varchar] [MaxLength = 20]
mappeddatatypes.CharAsVarchar ---> [varchar] [MaxLength = 1]
mappeddatatypes.DateTimeAsDate ---> [date]
mappeddatatypes.DateTimeAsDatetime ---> [datetime] [Precision = 0]
mappeddatatypes.DateTimeOffsetAsDatetime ---> [datetime] [Precision = 0]
mappeddatatypes.DateTimeOffsetAsTimestamp ---> [timestamp] [Precision = 0]
mappeddatatypes.DecimalAsDecimal ---> [decimal] [Precision = 8 Scale = 2]
mappeddatatypes.DoubleAsDouble ---> [double] [Precision = 22]
mappeddatatypes.DoubleAsFloat ---> [float] [Precision = 12]
mappeddatatypes.EnumAsNvarchar20 ---> [varchar] [MaxLength = 20]
mappeddatatypes.EnumAsVarchar20 ---> [varchar] [MaxLength = 20]
mappeddatatypes.GuidAsUniqueidentifier ---> [char] [MaxLength = 36]
mappeddatatypes.Int ---> [int] [Precision = 10 Scale = 0]
mappeddatatypes.LongAsBigInt ---> [bigint] [Precision = 19 Scale = 0]
mappeddatatypes.SByteAsSmallint ---> [smallint] [Precision = 5 Scale = 0]
mappeddatatypes.SByteAsTinyint ---> [tinyint] [Precision = 3 Scale = 0]
mappeddatatypes.ShortAsSmallint ---> [smallint] [Precision = 5 Scale = 0]
mappeddatatypes.StringAsChar ---> [char] [MaxLength = 10]
mappeddatatypes.StringAsLongtext ---> [longtext] [MaxLength = -1]
mappeddatatypes.StringAsMediumtext ---> [mediumtext] [MaxLength = 16777215]
mappeddatatypes.StringAsNChar ---> [char] [MaxLength = 10]
mappeddatatypes.StringAsNtext ---> [text] [MaxLength = 32767]
mappeddatatypes.StringAsNvarchar ---> [varchar] [MaxLength = 4001]
mappeddatatypes.StringAsText ---> [text] [MaxLength = 65535]
mappeddatatypes.StringAsTinytext ---> [tinytext] [MaxLength = 255]
mappeddatatypes.StringAsVarchar ---> [varchar] [MaxLength = 8001]
mappeddatatypes.TimeSpanAsTime ---> [time] [Precision = 0]
mappeddatatypes.UintAsBigint ---> [bigint] [Precision = 19 Scale = 0]
mappeddatatypes.UintAsInt ---> [int] [Precision = 10 Scale = 0]
mappeddatatypes.UlongAsBigint ---> [bigint] [Precision = 20 Scale = 0]
mappeddatatypes.UlongAsDecimal200 ---> [decimal] [Precision = 20 Scale = 0]
mappeddatatypes.UShortAsInt ---> [int] [Precision = 10 Scale = 0]
mappeddatatypes.UShortAsSmallint ---> [smallint] [Precision = 5 Scale = 0]
mappednullabledatatypes.BoolAsBit ---> [nullable bit] [Precision = 1]
mappednullabledatatypes.ByteAsTinyint ---> [nullable tinyint] [Precision = 3 Scale = 0]
mappednullabledatatypes.BytesAsBinary ---> [nullable binary] [MaxLength = 6]
mappednullabledatatypes.BytesAsBlob ---> [nullable blob] [MaxLength = 65535]
mappednullabledatatypes.BytesAsVarbinary ---> [nullable varbinary] [MaxLength = 255]
mappednullabledatatypes.CharAsInt ---> [nullable int] [Precision = 10 Scale = 0]
mappednullabledatatypes.CharAsNvarchar ---> [nullable varchar] [MaxLength = 1]
mappednullabledatatypes.CharAsText ---> [nullable text] [MaxLength = 65535]
mappednullabledatatypes.CharAsVarchar ---> [nullable varchar] [MaxLength = 1]
mappednullabledatatypes.DateTimeAsDate ---> [nullable date]
mappednullabledatatypes.DateTimeAsDatetime ---> [nullable datetime] [Precision = 0]
mappednullabledatatypes.DateTimeOffsetAsDatetime ---> [nullable datetime] [Precision = 6]
mappednullabledatatypes.DateTimeOffsetAsTimestamp ---> [nullable timestamp] [Precision = 6]
mappednullabledatatypes.DecimalAsDecimal ---> [nullable decimal] [Precision = 8 Scale = 2]
mappednullabledatatypes.DecimalAsFixed ---> [nullable decimal] [Precision = 8 Scale = 2]
mappednullabledatatypes.DoubleAsDoublePrecision ---> [nullable double] [Precision = 22]
mappednullabledatatypes.DoubleAsReal ---> [nullable double] [Precision = 32 Scale = 30]
mappednullabledatatypes.EnumAsNvarchar20 ---> [nullable varchar] [MaxLength = 20]
mappednullabledatatypes.EnumAsVarchar20 ---> [nullable varchar] [MaxLength = 20]
mappednullabledatatypes.FloatAsFloat ---> [nullable float] [Precision = 20 Scale = 4]
mappednullabledatatypes.GuidAsUniqueidentifier ---> [nullable char] [MaxLength = 36]
mappednullabledatatypes.Int ---> [int] [Precision = 10 Scale = 0]
mappednullabledatatypes.LongAsBigint ---> [nullable bigint] [Precision = 19 Scale = 0]
mappednullabledatatypes.SByteAsSmallint ---> [nullable smallint] [Precision = 5 Scale = 0]
mappednullabledatatypes.SbyteAsTinyint ---> [nullable tinyint] [Precision = 3 Scale = 0]
mappednullabledatatypes.ShortAsSmallint ---> [nullable smallint] [Precision = 5 Scale = 0]
mappednullabledatatypes.StringAsChar ---> [nullable char] [MaxLength = 20]
mappednullabledatatypes.StringAsMediumtext ---> [nullable mediumtext] [MaxLength = 8388607]
mappednullabledatatypes.StringAsNChar ---> [nullable char] [MaxLength = 20]
mappednullabledatatypes.StringAsNtext ---> [nullable text] [MaxLength = 65535]
mappednullabledatatypes.StringAsNvarchar ---> [nullable varchar] [MaxLength = 55]
mappednullabledatatypes.StringAsText ---> [nullable tinytext] [MaxLength = 255]
mappednullabledatatypes.StringAsTinytext ---> [nullable tinytext] [MaxLength = 127]
mappednullabledatatypes.StringAsVarchar ---> [nullable varchar] [MaxLength = 55]
mappednullabledatatypes.TimeSpanAsTime ---> [nullable time] [Precision = 3]
mappednullabledatatypes.UintAsBigint ---> [nullable bigint] [Precision = 19 Scale = 0]
mappednullabledatatypes.UintAsInt ---> [nullable int] [Precision = 10 Scale = 0]
mappednullabledatatypes.UlongAsBigint ---> [nullable bigint] [Precision = 19 Scale = 0]
mappednullabledatatypes.UlongAsDecimal200 ---> [nullable decimal] [Precision = 20 Scale = 0]
mappednullabledatatypes.UShortAsInt ---> [nullable int] [Precision = 10 Scale = 0]
mappednullabledatatypes.UShortAsSmallint ---> [nullable smallint] [Precision = 5 Scale = 0]
maxlengthdatatypes.ByteArray5 ---> [nullable varbinary] [MaxLength = 5]
maxlengthdatatypes.ByteArray9000 ---> [nullable longblob] [MaxLength = -1]
maxlengthdatatypes.Id ---> [int] [Precision = 10 Scale = 0]
maxlengthdatatypes.String3 ---> [nullable varchar] [MaxLength = 3]
maxlengthdatatypes.String9000 ---> [nullable longtext] [MaxLength = -1]
stringforeignkeydatatype.Id ---> [int] [Precision = 10 Scale = 0]
stringforeignkeydatatype.StringKeyDataTypeId ---> [nullable varchar] [MaxLength = 255]
stringkeydatatype.Id ---> [varchar] [MaxLength = 255]
unicodedatatypes.Id ---> [int] [Precision = 10 Scale = 0]
unicodedatatypes.StringAnsi ---> [nullable longtext] [MaxLength = -1]
unicodedatatypes.StringAnsi3 ---> [nullable varchar] [MaxLength = 3]
unicodedatatypes.StringAnsi9000 ---> [nullable longtext] [MaxLength = -1]
unicodedatatypes.StringDefault ---> [nullable longtext] [MaxLength = -1]
unicodedatatypes.StringUnicode ---> [nullable longtext] [MaxLength = -1]
";

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }

        public static string QueryForColumnTypes(DbContext context)
        {
            const string query
                = @"SELECT
                        TABLE_NAME,
                        COLUMN_NAME,
                        DATA_TYPE,
                        IS_NULLABLE,
                        CHARACTER_MAXIMUM_LENGTH,
                        NUMERIC_PRECISION,
                        NUMERIC_SCALE,
                        DATETIME_PRECISION
                    FROM INFORMATION_SCHEMA.COLUMNS
 WHERE `TABLE_SCHEMA` = 'builtindatatypes'";

            var columns = new List<ColumnInfo>();

            using (context)
            {
                var connection = context.Database.GetDbConnection();

                var command = connection.CreateCommand();
                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var columnInfo = new ColumnInfo
                        {
                            TableName = reader.GetString(0),
                            ColumnName = reader.GetString(1),
                            DataType = reader.GetString(2),
                            IsNullable = reader.IsDBNull(3) ? null : (bool?)(reader.GetString(3) == "YES"),
                            MaxLength = reader.IsDBNull(4) ? null : (int?)reader.GetInt64(4),
                            NumericPrecision = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5),
                            NumericScale = reader.IsDBNull(6) ? null : (int?)reader.GetInt32(6),
                            DateTimePrecision = reader.IsDBNull(7) ? null : (int?)reader.GetInt16(7)
                        };

                        columns.Add(columnInfo);
                    }
                }
            }

            var builder = new StringBuilder();

            foreach (var column in columns.OrderBy(e => e.TableName).ThenBy(e => e.ColumnName))
            {
                builder.Append(column.TableName);
                builder.Append(".");
                builder.Append(column.ColumnName);
                builder.Append(" ---> [");

                if (column.IsNullable == true)
                {
                    builder.Append("nullable ");
                }

                builder.Append(column.DataType);
                builder.Append("]");

                if (column.MaxLength.HasValue)
                {
                    builder.Append(" [MaxLength = ");
                    builder.Append(column.MaxLength);
                    builder.Append("]");
                }

                if (column.NumericPrecision.HasValue)
                {
                    builder.Append(" [Precision = ");
                    builder.Append(column.NumericPrecision);
                }

                if (column.DateTimePrecision.HasValue)
                {
                    builder.Append(" [Precision = ");
                    builder.Append(column.DateTimePrecision);
                }

                if (column.NumericScale.HasValue)
                {
                    builder.Append(" Scale = ");
                    builder.Append(column.NumericScale);
                }

                if (column.NumericPrecision.HasValue
                    || column.DateTimePrecision.HasValue
                    || column.NumericScale.HasValue)
                {
                    builder.Append("]");
                }

                builder.AppendLine();
            }

            var actual = builder.ToString();
            return actual;
        }

        [Fact]
        public void Can_get_column_types_from_built_model()
        {
            using (var context = CreateContext())
            {
                var mappingSource = context.GetService<IRelationalTypeMappingSource>();

                foreach (var property in context.Model.GetEntityTypes().SelectMany(e => e.GetDeclaredProperties()))
                {
                    var columnType = property.Relational().ColumnType;
                    Assert.NotNull(columnType);

                    if (property[RelationalAnnotationNames.ColumnType] == null)
                    {
                        Assert.Equal(
                            columnType.ToLowerInvariant(),
                            mappingSource.FindMapping(property).StoreType.ToLowerInvariant());
                    }
                }
            }
        }

        private static readonly string _eol = Environment.NewLine;
        private string Sql => Fixture.TestSqlLoggerFactory.Sql;

        public class BuiltInDataTypesMySqlFixture : BuiltInDataTypesFixtureBase
        {
            public override bool StrictEquality => false;

            public override bool SupportsAnsi => true;

            public override bool SupportsUnicodeToAnsiConversion => false;

            public override bool SupportsLargeStringComparisons => true;

            public override bool SupportsBinaryKeys => true;

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base.AddOptions(builder).ConfigureWarnings(
                    c => c.Log(RelationalEventId.QueryClientEvaluationWarning));

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                modelBuilder.Entity<MappedDataTypes>(
                    b =>
                        {
                            b.HasKey(e => e.Int);
                            b.Property(e => e.Int).ValueGeneratedNever();
                        });

                modelBuilder.Entity<MappedNullableDataTypes>(
                    b =>
                        {
                            b.HasKey(e => e.Int);
                            b.Property(e => e.Int).ValueGeneratedNever();
                        });

                MakeRequired<MappedDataTypes>(modelBuilder);
            }

            public override DateTime DefaultDateTime => new DateTime();
        }

        [Flags]
        protected enum StringEnum16 : short
        {
            Value1 = 1,
            Value2 = 2,
            Value4 = 4
        }

        [Flags]
        protected enum StringEnumU16 : ushort
        {
            Value1 = 1,
            Value2 = 2,
            Value4 = 4
        }

        protected class MappedDataTypes
        {
            [Column(TypeName = "int")]
            public int Int { get; set; }

            [Column(TypeName = "bigint")]
            public long LongAsBigInt { get; set; }

            [Column(TypeName = "smallint")]
            public short ShortAsSmallint { get; set; }

            [Column(TypeName = "tinyint")]
            public byte ByteAsTinyint { get; set; }

            [Column(TypeName = "int unsigned")]
            public uint UintAsInt { get; set; }

            [Column(TypeName = "bigint unsigned")]
            public ulong UlongAsBigint { get; set; }

            [Column(TypeName = "smallint unsigned")]
            public ushort UShortAsSmallint { get; set; }

            [Column(TypeName = "tinyint unsigned")]
            public sbyte SByteAsTinyint { get; set; }

            [Column(TypeName = "bit")]
            public bool BoolAsBit { get; set; }

            [Column(TypeName = "decimal(8,2)")]
            public decimal DecimalAsDecimal { get; set; }

            [Column(TypeName = "float")]
            public double DoubleAsFloat { get; set; }

            [Column(TypeName = "double")]
            public double DoubleAsDouble { get; set; }

            [Column(TypeName = "date")]
            public DateTime DateTimeAsDate { get; set; }

            [Column(TypeName = "datetime")]
            public DateTimeOffset DateTimeOffsetAsDatetime { get; set; }

            [Column(TypeName = "timestamp")]
            public DateTimeOffset DateTimeOffsetAsTimestamp { get; set; }

            [Column(TypeName = "datetime")]
            public DateTime DateTimeAsDatetime { get; set; }

            [Column(TypeName = "time")]
            public TimeSpan TimeSpanAsTime { get; set; }

            [Column(TypeName = "char(10)")]
            public string StringAsChar { get; set; }

            [Column(TypeName = "nchar(10)")]
            public string StringAsNChar { get; set; }

            [Column(TypeName = "varchar(8001)")]
            public string StringAsVarchar { get; set; }

            [Column(TypeName = "nvarchar(4001)")]
            public string StringAsNvarchar { get; set; }

            [Column(TypeName = "text")]
            public string StringAsText { get; set; }

            [Column(TypeName = "text CHARACTER SET ucs2")]
            public string StringAsNtext { get; set; }

            [Column(TypeName = "tinytext")]
            public string StringAsTinytext { get; set; }

            [Column(TypeName = "mediumtext")]
            public string StringAsMediumtext { get; set; }

            [Column(TypeName = "longtext")]
            public string StringAsLongtext { get; set; }

            [Column(TypeName = "varbinary(255)")]
            public byte[] BytesAsVarbinary { get; set; }

            [Column(TypeName = "binary(5)")]
            public byte[] BytesAsBinary { get; set; }

            [Column(TypeName = "tinyblob")]
            public byte[] BytesAsTinyblob { get; set; }

            [Column(TypeName = "blob")]
            public byte[] BytesAsBlob { get; set; }

            [Column(TypeName = "mediumblob")]
            public byte[] BytesAsMediumblob { get; set; }

            [Column(TypeName = "longblob")]
            public byte[] BytesAsLongblob { get; set; }

            [Column(TypeName = "char(36)")]
            public Guid GuidAsUniqueidentifier { get; set; }

            [Column(TypeName = "bigint")]
            public uint UintAsBigint { get; set; }

            [Column(TypeName = "decimal(20,0)")]
            public ulong UlongAsDecimal200 { get; set; }

            [Column(TypeName = "int")]
            public ushort UShortAsInt { get; set; }

            [Column(TypeName = "smallint")]
            public sbyte SByteAsSmallint { get; set; }

            [Column(TypeName = "varchar(1)")]
            public char CharAsVarchar { get; set; }

            [Column(TypeName = "nvarchar(20)")]
            public char CharAsNvarchar { get; set; }

            [Column(TypeName = "int")]
            public char CharAsInt { get; set; }

            [Column(TypeName = "varchar(20)")]
            public StringEnum16 EnumAsVarchar20 { get; set; }

            [Column(TypeName = "nvarchar(20)")]
            public StringEnumU16 EnumAsNvarchar20 { get; set; }
        }

        protected class MappedNullableDataTypes
        {
            [Column(TypeName = "int")]
            public int? Int { get; set; }

            [Column(TypeName = "bigint")]
            public long? LongAsBigint { get; set; }

            [Column(TypeName = "smallint")]
            public short? ShortAsSmallint { get; set; }

            [Column(TypeName = "tinyint")]
            public byte? ByteAsTinyint { get; set; }

            [Column(TypeName = "int")]
            public uint? UintAsInt { get; set; }

            [Column(TypeName = "bigint")]
            public ulong? UlongAsBigint { get; set; }

            [Column(TypeName = "smallint")]
            public ushort? UShortAsSmallint { get; set; }

            [Column(TypeName = "tinyint")]
            public sbyte? SbyteAsTinyint { get; set; }

            [Column(TypeName = "bit")]
            public bool? BoolAsBit { get; set; }

            [Column(TypeName = "decimal(8,2)")]
            public decimal? DecimalAsDecimal { get; set; }

            [Column(TypeName = "fixed(8,2)")]
            public decimal? DecimalAsFixed { get; set; }

            [Column(TypeName = "float(20,4)")]
            public float? FloatAsFloat { get; set; }

            [Column(TypeName = "real(32,30)")]
            public double? DoubleAsReal { get; set; }

            [Column(TypeName = "double precision")]
            public double? DoubleAsDoublePrecision { get; set; }

            [Column(TypeName = "date")]
            public DateTime? DateTimeAsDate { get; set; }

            [Column(TypeName = "datetime(6)")]
            public DateTimeOffset? DateTimeOffsetAsDatetime { get; set; }

            [Column(TypeName = "timestamp(6)")]
            public DateTimeOffset? DateTimeOffsetAsTimestamp { get; set; }

            [Column(TypeName = "datetime")]
            public DateTime? DateTimeAsDatetime { get; set; }

            [Column(TypeName = "time(3)")]
            public TimeSpan? TimeSpanAsTime { get; set; }

            [Column(TypeName = "char(20)")]
            public string StringAsChar { get; set; }

            [Column(TypeName = "nchar(20)")]
            public string StringAsNChar { get; set; }

            [Column(TypeName = "varchar(55)")]
            public string StringAsVarchar { get; set; }

            [Column(TypeName = "nvarchar(55)")]
            public string StringAsNvarchar { get; set; }

            [Column(TypeName = "text(55) CHARACTER SET latin1")]
            public string StringAsText { get; set; }

            [Column(TypeName = "text CHARACTER SET utf8mb3")]
            public string StringAsNtext { get; set; }

            [Column(TypeName = "tinytext CHARACTER SET ucs2")]
            public string StringAsTinytext { get; set; }

            [Column(TypeName = "mediumtext CHARACTER SET ucs2")]
            public string StringAsMediumtext { get; set; }

            [Column(TypeName = "varbinary(255)")]
            public byte[] BytesAsVarbinary { get; set; }

            [Column(TypeName = "binary(6)")]
            public byte[] BytesAsBinary { get; set; }

            [Column(TypeName = "blob")]
            public byte[] BytesAsBlob { get; set; }

            [Column(TypeName = "char(36)")]
            public Guid? GuidAsUniqueidentifier { get; set; }

            [Column(TypeName = "bigint")]
            public uint? UintAsBigint { get; set; }

            [Column(TypeName = "decimal(20,0)")]
            public ulong? UlongAsDecimal200 { get; set; }

            [Column(TypeName = "int")]
            public ushort? UShortAsInt { get; set; }

            [Column(TypeName = "smallint")]
            public sbyte? SByteAsSmallint { get; set; }

            [Column(TypeName = "varchar(1)")]
            public char? CharAsVarchar { get; set; }

            [Column(TypeName = "nvarchar(1)")]
            public char? CharAsNvarchar { get; set; }

            [Column(TypeName = "text")]
            public char? CharAsText { get; set; }

            [Column(TypeName = "int")]
            public char? CharAsInt { get; set; }

            [Column(TypeName = "varchar(20)")]
            public StringEnum16? EnumAsVarchar20 { get; set; }

            [Column(TypeName = "nvarchar(20)")]
            public StringEnumU16? EnumAsNvarchar20 { get; set; }
        }

        public class ColumnInfo
        {
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public string DataType { get; set; }
            public bool? IsNullable { get; set; }
            public int? MaxLength { get; set; }
            public int? NumericPrecision { get; set; }
            public int? NumericScale { get; set; }
            public int? DateTimePrecision { get; set; }
        }
    }
}
