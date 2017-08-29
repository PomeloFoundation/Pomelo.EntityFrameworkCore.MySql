using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Tests.Models
{

    public class DataTypesTest
    {

        [Fact]
        public async Task TestDataTypesSimple()
        {
            void TestEmpty(DataTypesSimple emptyDb)
            {
                // bool
                Assert.Equal(default(bool), emptyDb.TypeBool);
                // nullable bool
                Assert.Equal(null, emptyDb.TypeBoolN);

                // integers
                Assert.Equal(default(short), emptyDb.TypeShort);
                Assert.Equal(default(ushort), emptyDb.TypeUshort);
                Assert.Equal(default(int), emptyDb.TypeInt);
                Assert.Equal(default(uint), emptyDb.TypeUint);
                Assert.Equal(default(long), emptyDb.TypeLong);
                Assert.Equal(default(ulong), emptyDb.TypeUlong);
                // nullable integers
                Assert.Equal(null, emptyDb.TypeShortN);
                Assert.Equal(null, emptyDb.TypeUshortN);
                Assert.Equal(null, emptyDb.TypeIntN);
                Assert.Equal(null, emptyDb.TypeUintN);
                Assert.Equal(null, emptyDb.TypeLongN);
                Assert.Equal(null, emptyDb.TypeUlongN);

                // decimals
                Assert.Equal(default(decimal), emptyDb.TypeDecimal);
                Assert.Equal(default(double), emptyDb.TypeDouble);
                Assert.Equal(default(float), emptyDb.TypeFloat);
                // nullable decimals
                Assert.Equal(null, emptyDb.TypeDecimalN);
                Assert.Equal(null, emptyDb.TypeDoubleN);
                Assert.Equal(null, emptyDb.TypeFloatN);

                // byte
                Assert.Equal(default(sbyte), emptyDb.TypeSbyte);
                Assert.Equal(default(byte), emptyDb.TypeByte);
                Assert.Equal(default(char), emptyDb.TypeChar);
                // nullable byte
                Assert.Equal(null, emptyDb.TypeSbyteN);
                Assert.Equal(null, emptyDb.TypeByteN);
                Assert.Equal(null, emptyDb.TypeCharN);

                // DateTime
                Assert.Equal(default(DateTime), emptyDb.TypeDateTime);
                Assert.Equal(default(DateTimeOffset), emptyDb.TypeDateTimeOffset);
                Assert.Equal(default(TimeSpan), emptyDb.TypeTimeSpan);
                // nullable DateTime
                Assert.Equal(null, emptyDb.TypeDateTimeN);
                Assert.Equal(null, emptyDb.TypeDateTimeOffsetN);
                Assert.Equal(null, emptyDb.TypeTimeSpanN);

                // Enum
                Assert.Equal(default(TestEnum), emptyDb.TypeEnum);
                Assert.Equal(default(TestEnumByte), emptyDb.TypeEnumByte);
                // nullableEnum
                Assert.Equal(null, emptyDb.TypeEnumN);
                Assert.Equal(null, emptyDb.TypeEnumByteN);

                // guid
                Assert.Equal(default(Guid), emptyDb.TypeGuid);
                // nullable guid
                Assert.Equal(null, emptyDb.TypeGuidN);
            }

            const sbyte testSbyte = (sbyte) -128;
	        const byte testByte = (byte) 255;
	        const char testChar = 'a';
	        const float testFloat = (float) 1.23456789e38;

	        var dateTime = new DateTime(2016, 10, 11, 1, 2, 3, 456);
	        var dateTimeOffset = dateTime + TimeSpan.FromMilliseconds(123.456);
	        var timeSpan = new TimeSpan(1, 2, 3, 4, 5);
			const TestEnum testEnum = TestEnum.TestOne;
			const TestEnumByte testEnumByte = TestEnumByte.TestOne;
	        var guid = Guid.NewGuid();

	        // test each data type with a valid value
	        // ReSharper disable once ObjectCreationAsStatement
            DataTypesSimple NewValueMem() => new DataTypesSimple
            {
                // bool
                TypeBool = true,
                // nullable bool
                TypeBoolN = true,

                // integers
                TypeShort = short.MinValue,
                TypeUshort = ushort.MaxValue,
                TypeInt = int.MinValue,
                TypeUint = uint.MaxValue,
                TypeLong = long.MinValue,
                TypeUlong = ulong.MaxValue,
                // nullable integers
                TypeShortN = short.MinValue,
                TypeUshortN = ushort.MaxValue,
                TypeIntN = int.MinValue,
                TypeUintN = uint.MaxValue,
                TypeLongN = long.MinValue,
                TypeUlongN = ulong.MaxValue,

                // decimals
                TypeDecimal = decimal.MaxValue,
                TypeDouble = double.MaxValue,
                TypeFloat = testFloat,
                // nullable decimals
                TypeDecimalN = decimal.MaxValue,
                TypeDoubleN = double.MaxValue,
                TypeFloatN = testFloat,

                // byte
                TypeSbyte = testSbyte,
                TypeByte = testByte,
                TypeChar = testChar,
                // nullable byte
                TypeSbyteN = testSbyte,
                TypeByteN = testByte,
                TypeCharN = testChar,

                // DateTime
                TypeDateTime = dateTime,
                TypeDateTimeOffset = dateTimeOffset,
                TypeTimeSpan = timeSpan,
                // nullable DateTime
                TypeDateTimeN = dateTime,
                TypeDateTimeOffsetN = dateTimeOffset,
                TypeTimeSpanN = timeSpan,

                // Enum
                TypeEnum = testEnum,
                TypeEnumByte = testEnumByte,
                // nullable Enum
                TypeEnumN = testEnum,
                TypeEnumByteN = testEnumByte,

                // guid
                TypeGuid = guid,
                // nullable guid
                TypeGuidN = guid,
            };

            void TestValue(DataTypesSimple valueDb)
            {
                // bool
                Assert.Equal(true, valueDb.TypeBool);
                // nullable bool
                Assert.Equal(true, valueDb.TypeBoolN);

                // integers
                Assert.Equal(short.MinValue, valueDb.TypeShort);
                Assert.Equal(ushort.MaxValue, valueDb.TypeUshort);
                Assert.Equal(int.MinValue, valueDb.TypeInt);
                Assert.Equal(uint.MaxValue, valueDb.TypeUint);
                Assert.Equal(long.MinValue, valueDb.TypeLong);
                Assert.Equal(ulong.MaxValue, valueDb.TypeUlong);
                // nullable integers
                Assert.Equal(short.MinValue, valueDb.TypeShortN);
                Assert.Equal(ushort.MaxValue, valueDb.TypeUshortN);
                Assert.Equal(int.MinValue, valueDb.TypeIntN);
                Assert.Equal(uint.MaxValue, valueDb.TypeUintN);
                Assert.Equal(long.MinValue, valueDb.TypeLongN);
                Assert.Equal(ulong.MaxValue, valueDb.TypeUlongN);

                // decimals
                Assert.Equal(decimal.MaxValue, valueDb.TypeDecimal);
                Assert.Equal(double.MaxValue, valueDb.TypeDouble);
                Assert.InRange(valueDb.TypeFloat, testFloat * (1 - 7e-1), testFloat * (1 + 7e-1)); // floats have 7 digits of precision
                // nullable decimals
                Assert.Equal(decimal.MaxValue, valueDb.TypeDecimalN);
                Assert.Equal(double.MaxValue, valueDb.TypeDoubleN);
                Assert.InRange(valueDb.TypeFloatN.GetValueOrDefault(), testFloat * (1 - 7e-1), testFloat * (1 + 7e-1)); // floats have 7 digits of precision

                // byte
                Assert.Equal(testSbyte, valueDb.TypeSbyte);
                Assert.Equal(testByte, valueDb.TypeByte);
                Assert.Equal(testChar, valueDb.TypeChar);
                // nullable byte
                Assert.Equal(testSbyte, valueDb.TypeSbyte);
                Assert.Equal(testByte, valueDb.TypeByteN);
                Assert.Equal(testChar, valueDb.TypeCharN);

                // DateTime
                Assert.Equal(dateTime, valueDb.TypeDateTime);
                Assert.Equal(dateTimeOffset, valueDb.TypeDateTimeOffset);
                Assert.Equal(timeSpan, valueDb.TypeTimeSpan);
                // nullable DateTime
                Assert.Equal(dateTime, valueDb.TypeDateTimeN);
                Assert.Equal(dateTimeOffset, valueDb.TypeDateTimeOffsetN);
                Assert.Equal(timeSpan, valueDb.TypeTimeSpanN);

                // Enum
                Assert.Equal(testEnum, valueDb.TypeEnum);
                Assert.Equal(testEnumByte, valueDb.TypeEnumByte);
                // nullable Enum
                Assert.Equal(testEnum, valueDb.TypeEnumN);
                Assert.Equal(testEnumByte, valueDb.TypeEnumByteN);

                // guid
                Assert.Equal(guid, valueDb.TypeGuid);
                // nullable guid
                Assert.Equal(guid, valueDb.TypeGuidN);
            }

            // create test data objects
	        var emptyMemAsync = new DataTypesSimple();
	        var emptyMemSync = new DataTypesSimple();
	        var valueMemAsync = NewValueMem();
	        var valueMemSync = NewValueMem();

	        // save them to the database
	        using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
		        db.DataTypesSimple.Add(emptyMemAsync);
		        db.DataTypesSimple.Add(valueMemAsync);
		        await db.SaveChangesAsync();

		        db.DataTypesSimple.Add(emptyMemSync);
		        db.DataTypesSimple.Add(valueMemSync);
		        db.SaveChanges();
	        }

	        // load them from the database and run tests
	        using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
		        // ReSharper disable once AccessToDisposedClosure
	            async Task<DataTypesSimple> FromDbAsync(DataTypesSimple dt) => await db.DataTypesSimple.FirstOrDefaultAsync(m => m.Id == dt.Id);

	            // ReSharper disable once AccessToDisposedClosure
	            DataTypesSimple FromDbSync(DataTypesSimple dt) => db.DataTypesSimple.FirstOrDefault(m => m.Id == dt.Id);

	            TestEmpty(await FromDbAsync(emptyMemAsync));
		        TestEmpty(FromDbSync(emptyMemSync));
		        TestValue(await FromDbAsync(valueMemAsync));
		        TestValue(FromDbSync(valueMemSync));
	        }
        }

	    [Fact]
	    public async Task TestDataTypesVariable()
	    {
	        void TestEmpty(DataTypesVariable valueDb)
	        {
	            // string not null
	            Assert.Equal("", valueDb.TypeString);
	            Assert.Equal("", valueDb.TypeString255);
	            // string null
	            Assert.Equal(null, valueDb.TypeStringN);
	            Assert.Equal(null, valueDb.TypeString255N);

	            // binary not null
	            Assert.Equal(DataTypesVariable.EmptyByteArray, valueDb.TypeByteArray);
	            Assert.Equal(DataTypesVariable.EmptyByteArray, valueDb.TypeByteArray255);
	            // binary null
	            Assert.Equal(null, valueDb.TypeByteArrayN);
	            Assert.Equal(null, valueDb.TypeByteArray255N);

	            // json not null
	            Assert.Equal(DataTypesVariable.EmptyJsonArray.Json, valueDb.TypeJsonArray.Json);
	            Assert.Equal(DataTypesVariable.EmptyJsonObject.Json, valueDb.TypeJsonObject.Json);
	            // json null
	            Assert.Equal(null, valueDb.TypeJsonArrayN);
	            Assert.Equal(null, valueDb.TypeJsonObjectN);
	        }

	        var string255 = new string('a', 255);
            var string10K = new string('a', 10000);

            var byte255 = new byte[255];
            var byte10K = new byte[10000];
            for (var i = 0; i < byte10K.Length; i++)
            {
                if (i < 255)
                    byte255[i] = (byte) 'a';
                byte10K[i] = (byte) 'a';
            }

            var jsonArray = new JsonObject<List<string>>(new List<string> {"test"});
            var jsonObject = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string> {{"test", "test"}});

            // test each data type with a valid value
	        DataTypesVariable NewValueMem() => new DataTypesVariable
	        {
	            // string not null
	            TypeString = string10K,
	            TypeString255 = string255, // should be truncated by DBMS
	            // string null
	            TypeStringN = string10K,
	            TypeString255N = string255, // should be truncated by DBMS

	            // binary not null
	            TypeByteArray = byte10K,
	            TypeByteArray255 = byte255, // should be truncated by DBMS
	            // binary null
	            TypeByteArrayN = byte10K,
	            TypeByteArray255N = byte255, // should be truncated by DBMS

	            // json not null
	            TypeJsonArray = jsonArray,
	            TypeJsonObject = jsonObject,
	            // json null
	            TypeJsonArrayN = jsonArray,
	            TypeJsonObjectN = jsonObject,
	        };

	        void TestValue(DataTypesVariable valueDb)
	        {
	            // string not null
	            Assert.Equal(string10K, valueDb.TypeString);
	            Assert.Equal(string255, valueDb.TypeString255);
	            // string null
	            Assert.Equal(string10K, valueDb.TypeStringN);
	            Assert.Equal(string255, valueDb.TypeString255N);

	            // binary not null
	            Assert.Equal(byte10K, valueDb.TypeByteArray);
	            Assert.Equal(byte255, valueDb.TypeByteArray255);
	            // binary null
	            Assert.Equal(byte10K, valueDb.TypeByteArrayN);
	            Assert.Equal(byte255, valueDb.TypeByteArray255N);

	            // json not null
	            Assert.Equal(jsonArray.Json, valueDb.TypeJsonArray.Json);
	            Assert.Equal(jsonObject.Json, valueDb.TypeJsonObject.Json);
	            // json null
	            Assert.Equal(jsonArray.Json, valueDb.TypeJsonArrayN.Json);
	            Assert.Equal(jsonObject.Json, valueDb.TypeJsonObjectN.Json);
	        }

	        // create test data objects
		    var emptyMemAsync = DataTypesVariable.CreateEmpty();
		    var emptyMemSync = DataTypesVariable.CreateEmpty();
		    var valueMemAsync = NewValueMem();
		    var valueMemSync = NewValueMem();

		    // save them to the database
		    using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
			    db.DataTypesVariable.Add(emptyMemAsync);
			    db.DataTypesVariable.Add(valueMemAsync);
			    await db.SaveChangesAsync();

			    db.DataTypesVariable.Add(emptyMemSync);
			    db.DataTypesVariable.Add(valueMemSync);
			    db.SaveChanges();
		    }

		    // load them from the database and run tests
		    using (var scope = new AppDbScope())
			{
				var db = scope.AppDb;
			    // ReSharper disable once AccessToDisposedClosure
		        async Task<DataTypesVariable> FromDbAsync(DataTypesVariable dt) => await db.DataTypesVariable.FirstOrDefaultAsync(m => m.Id == dt.Id);

		        // ReSharper disable once AccessToDisposedClosure
		        DataTypesVariable FromDbSync(DataTypesVariable dt) => db.DataTypesVariable.FirstOrDefault(m => m.Id == dt.Id);

		        TestEmpty(await FromDbAsync(emptyMemAsync));
			    TestEmpty(FromDbSync(emptyMemSync));
			    TestValue(await FromDbAsync(valueMemAsync));
			    TestValue(FromDbSync(valueMemSync));
		    }

	    }

    }

}
