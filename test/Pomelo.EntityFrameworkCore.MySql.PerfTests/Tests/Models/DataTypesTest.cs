using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Tests.Models
{

    public class DataTypesTest
    {
        [Fact]
        public void TestDataTypes()
        {
            using (var db = new AppDb()){
                var dataTypes = new DataTypes{
                    TypeShort      = 1,
                    TypeUshort     = 2,
                    TypeInt        = 3,
                    TypeUint       = 4,
                    TypeLong       = 5,
                    TypeUlong      = 6,
                    TypeString     = "test",
                    TypeByte       = (byte) 'a',
                    TypeByteArray  = new byte[] { (byte) 'a', (byte) 'b' },
                    TypeJsonArray  = new JsonObject<List<string>>(new List<string>{ "test" }),
                    TypeJsonObject = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string>(){{ "test", "test" }})
                };
                db.DataTypes.Add(dataTypes);
                db.SaveChanges();
	            Assert.True(dataTypes.Id > 0);
            }
        }
    }

}
