using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Tests.Models
{

    public class DataTypesTest
    {

        [Fact]
        public async Task TestDataTypesSimple()
        {
	        Action<DataTypesSimple> testEmpty = emptyDb =>
	        {
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
		        // nullable DateTime
		        Assert.Equal(null, emptyDb.TypeDateTimeN);
		        Assert.Equal(null, emptyDb.TypeDateTimeOffsetN);

		        // guid
		        Assert.Equal(default(Guid), emptyDb.TypeGuid);
		        // nullable guid
		        Assert.Equal(null, emptyDb.TypeGuidN);
	        };

	        const sbyte testSbyte = (sbyte) -128;
	        const byte testByte = (byte) 255;
	        const char testChar = 'a';
	        // by default, we use second granularity on dateTime
	        var dateTime = new DateTime(2016, 10, 6, 13, 3, 7);
	        // we only support up to millisecond granularity on dateTimeOffset
	        var dateTimeOffset = new DateTime(2016, 10, 6) + TimeSpan.FromMilliseconds(123456789);
	        var guid = Guid.NewGuid();

	        // test each data type with a valid value
	        // ReSharper disable once ObjectCreationAsStatement
	        Func<DataTypesSimple> newValueMem = () => new DataTypesSimple{
		        // integers
		        TypeShort      = short.MinValue,
		        TypeUshort     = ushort.MaxValue,
		        TypeInt        = int.MinValue,
		        TypeUint       = uint.MaxValue,
		        TypeLong       = long.MinValue,
		        TypeUlong      = ulong.MaxValue,
		        // nullable integers
		        TypeShortN     = short.MinValue,
		        TypeUshortN    = ushort.MaxValue,
		        TypeIntN       = int.MinValue,
		        TypeUintN      = uint.MaxValue,
		        TypeLongN      = long.MinValue,
		        TypeUlongN     = ulong.MaxValue,

		        // decimals
		        TypeDecimal    = decimal.MaxValue,
		        TypeDouble     = double.MaxValue,
		        TypeFloat      = float.MaxValue,
		        // nullable decimals
		        TypeDecimalN   = decimal.MaxValue,
		        TypeDoubleN    = double.MaxValue,
		        TypeFloatN     = float.MaxValue,

		        // byte
		        TypeSbyte      = testSbyte,
		        TypeByte       = testByte,
		        TypeChar       = testChar,
		        // nullable byte
		        TypeSbyteN     = testSbyte,
		        TypeByteN      = testByte,
		        TypeCharN      = testChar,

		        // DateTime
		        TypeDateTime        = dateTime,
		        TypeDateTimeOffset  = dateTimeOffset,
		        // nullable DateTime
		        TypeDateTimeN       = dateTime,
		        TypeDateTimeOffsetN = dateTimeOffset,

		        // guid
		        TypeGuid = guid,
		        // nullable guid
		        TypeGuidN = guid,
	        };

	        Action<DataTypesSimple> testValue = valueDb =>
	        {
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
		        Assert.InRange(valueDb.TypeFloat, float.MaxValue * (1 - 7e-1), float.MaxValue); // floats have 7 digits of precision
		        // nullable decimals
		        Assert.Equal(decimal.MaxValue, valueDb.TypeDecimalN);
		        Assert.Equal(double.MaxValue, valueDb.TypeDoubleN);
		        Assert.InRange(valueDb.TypeFloatN.GetValueOrDefault(), float.MaxValue * (1 - 7e-1), float.MaxValue); // floats have 7 digits of precision

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
		        // nullable DateTime
		        Assert.Equal(dateTime, valueDb.TypeDateTimeN);
		        Assert.Equal(dateTimeOffset, valueDb.TypeDateTimeOffsetN);

		        // guid
		        Assert.Equal(guid, valueDb.TypeGuid);
		        // nullable guid
		        Assert.Equal(guid, valueDb.TypeGuidN);
	        };

	        // create test data objects
	        var emptyMemAsync = new DataTypesSimple();
	        var emptyMemSync = new DataTypesSimple();
	        var valueMemAsync = newValueMem();
	        var valueMemSync = newValueMem();

	        // save them to the database
	        using (var db = new AppDb()){
		        db.DataTypesSimple.Add(emptyMemAsync);
		        db.DataTypesSimple.Add(valueMemAsync);
		        await db.SaveChangesAsync();

		        db.DataTypesSimple.Add(emptyMemSync);
		        db.DataTypesSimple.Add(valueMemSync);
		        db.SaveChanges();
	        }

	        // load them from the database and run tests
	        using (var db = new AppDb())
	        {
		        // ReSharper disable once AccessToDisposedClosure
		        Func<DataTypesSimple, Task<DataTypesSimple>> fromDbAsync =
			        async dt => await db.DataTypesSimple.FirstOrDefaultAsync(m => m.Id == dt.Id);
		        // ReSharper disable once AccessToDisposedClosure
		        Func<DataTypesSimple, DataTypesSimple> fromDbSync =
			        dt => db.DataTypesSimple.FirstOrDefault(m => m.Id == dt.Id);

		        testEmpty(await fromDbAsync(emptyMemAsync));
		        testEmpty(fromDbSync(emptyMemSync));
		        testValue(await fromDbAsync(valueMemAsync));
		        testValue(fromDbSync(valueMemSync));
	        }
        }

	    [Fact]
	    public async Task TestDataTypesVariable()
	    {
		    var emptyByteArray = new byte[0];
		    var emptyJsonArray = new JsonObject<List<string>>(new List<string>());
		    var emptyJsonObject = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string>());

		    // non-null data types must be initialized
		    Func<DataTypesVariable> newEmptyMem = () => new DataTypesVariable
		    {
			    TypeString = "",
			    TypeString255 = "",
			    TypeByteArray    = emptyByteArray,
			    TypeByteArray255 = emptyByteArray,
			    TypeJsonArray    = emptyJsonArray,
			    TypeJsonObject   = emptyJsonObject,
		    };

            Action<DataTypesVariable> testEmpty = valueDb =>
            {
                // string not null
                Assert.Equal("", valueDb.TypeString);
                Assert.Equal("", valueDb.TypeString255);
                // string null
                Assert.Equal(null, valueDb.TypeStringN);
                Assert.Equal(null, valueDb.TypeString255N);

                // binary not null
                Assert.Equal(emptyByteArray, valueDb.TypeByteArray);
                Assert.Equal(emptyByteArray, valueDb.TypeByteArray255);
                // binary null
                Assert.Equal(null, valueDb.TypeByteArrayN);
                Assert.Equal(null, valueDb.TypeByteArray255N);

                // json not null
                Assert.Equal(emptyJsonArray.Json, valueDb.TypeJsonArray.Json);
                Assert.Equal(emptyJsonObject.Json, valueDb.TypeJsonObject.Json);
                // json null
                Assert.Equal(null, valueDb.TypeJsonArrayN);
                Assert.Equal(null, valueDb.TypeJsonObjectN);
            };

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
            Func<DataTypesVariable> newValueMem = () => new DataTypesVariable{
                // string not null
                TypeString     = string10K,
                TypeString255  = string10K, // should be truncated by DBMS
                // string null
                TypeStringN    = string10K,
                TypeString255N = string10K, // should be truncated by DBMS

                // binary not null
                TypeByteArray     = byte10K,
                TypeByteArray255  = byte10K, // should be truncated by DBMS
                // binary null
                TypeByteArrayN    = byte10K,
                TypeByteArray255N = byte10K, // should be truncated by DBMS

                // json not null
                TypeJsonArray   = jsonArray,
                TypeJsonObject  = jsonObject,
                // json null
                TypeJsonArrayN  = jsonArray,
                TypeJsonObjectN = jsonObject,
            };

            Action<DataTypesVariable> testValue = valueDb =>
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
            };

		    // create test data objects
		    var emptyMemAsync = newEmptyMem();
		    var emptyMemSync = newEmptyMem();
		    var valueMemAsync = newValueMem();
		    var valueMemSync = newValueMem();

		    // save them to the database
		    using (var db = new AppDb()){
			    db.DataTypesVariable.Add(emptyMemAsync);
			    db.DataTypesVariable.Add(valueMemAsync);
			    await db.SaveChangesAsync();

			    db.DataTypesVariable.Add(emptyMemSync);
			    db.DataTypesVariable.Add(valueMemSync);
			    db.SaveChanges();
		    }

		    // load them from the database and run tests
		    using (var db = new AppDb())
		    {
			    // ReSharper disable once AccessToDisposedClosure
			    Func<DataTypesVariable, Task<DataTypesVariable>> fromDbAsync =
				    async dt => await db.DataTypesVariable.FirstOrDefaultAsync(m => m.Id == dt.Id);
			    // ReSharper disable once AccessToDisposedClosure
			    Func<DataTypesVariable, DataTypesVariable> fromDbSync =
				    dt => db.DataTypesVariable.FirstOrDefault(m => m.Id == dt.Id);

			    testEmpty(await fromDbAsync(emptyMemAsync));
			    testEmpty(fromDbSync(emptyMemSync));
			    testValue(await fromDbAsync(valueMemAsync));
			    testValue(fromDbSync(valueMemSync));
		    }

	    }

    }

}
