using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pomelo.EntityFrameworkCore.MySql.Functional.Models;

namespace Pomelo.EntityFrameworkCore.MySql.Functional.Tests.Models
{

    [TestClass]
    public class DataTypesTest
    {
        [TestMethod]
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
                    TypeJsonObj    = new JsonObject<List<string>>(new List<string>{ "test" }),     
                    TypeJsonArray  = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string>(){{ "test", "test" }})
                };
                db.DataTypes.Add(dataTypes);
                db.SaveChanges();
                Assert.IsTrue(dataTypes.Id > 0);
            }
        }
    }

}