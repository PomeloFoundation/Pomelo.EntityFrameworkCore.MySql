using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Json.Microsoft.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Storage
{
    public class MySqlJsonMicrosoftTypeMappingTest
    {
        #region Json

        [Fact]
        public void GenerateSqlLiteral_returns_json_string_literal()
            => Assert.Equal(@"'{""a"":1}'", GetMapping("json").GenerateSqlLiteral(@"{""a"":1}"));

        [Fact]
        public void GenerateSqlLiteral_returns_json_object_literal()
        {
            var literal = Mapper.FindMapping(typeof(Customer), "json").GenerateSqlLiteral(SampleCustomer);
            Assert.Equal(@"'{""Name"":""Joe"",""Age"":25,""IsVip"":false,""Orders"":[" +
                         @"{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01T00:00:00""}," +
                         @"{""Price"":23.1,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10T00:00:00""}" +
                         @"]}'", literal);
        }

        [Fact]
        public void GenerateSqlLiteral_returns_json_document_literal()
        {
            var json = @"{""Name"":""Joe"",""Age"":25}";
            var literal = Mapper.FindMapping(typeof(JsonDocument), "json").GenerateSqlLiteral(JsonDocument.Parse(json));
            Assert.Equal($"'{json}'", literal);
        }

        [Fact]
        public void GenerateSqlLiteral_returns_json_element_literal()
        {
            var json = @"{""Name"":""Joe"",""Age"":25}";
            var literal = Mapper.FindMapping(typeof(JsonElement), "json").GenerateSqlLiteral(JsonDocument.Parse(json).RootElement);
            Assert.Equal($"'{json}'", literal);
        }

        [Fact]
        public void GenerateCodeLiteral_returns_json_document_literal()
            => Assert.Equal(
                @"System.Text.Json.JsonDocument.Parse(""{\""Name\"":\""Joe\"",\""Age\"":25}"", new System.Text.Json.JsonDocumentOptions())",
                CodeLiteral(JsonDocument.Parse(@"{""Name"":""Joe"",""Age"":25}")));

        [Fact]
        public void GenerateCodeLiteral_returns_json_element_literal()
            => Assert.Equal(
                @"System.Text.Json.JsonDocument.Parse(""{\""Name\"":\""Joe\"",\""Age\"":25}"", new System.Text.Json.JsonDocumentOptions()).RootElement",
                CodeLiteral(JsonDocument.Parse(@"{""Name"":""Joe"",""Age"":25}").RootElement));

        private static readonly Customer SampleCustomer = new Customer
        {
            Name = "Joe",
            Age = 25,
            IsVip = false,
            Orders = new[]
            {
                new Order
                {
                    Price = 99.5m,
                    ShippingAddress = "Some address 1",
                    ShippingDate = new DateTime(2019, 10, 1)
                },
                new Order
                {
                    Price = 23.1m,
                    ShippingAddress = "Some address 2",
                    ShippingDate = new DateTime(2019, 10, 10)
                }
            }
        };

        public class Customer
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsVip { get; set; }
            public Order[] Orders { get; set; }
        }

        public class Order
        {
            public decimal Price { get; set; }
            public string ShippingAddress { get; set; }
            public DateTime ShippingDate { get; set; }
        }

        #endregion Json

        #region Support

        private static readonly MySqlTypeMappingSource Mapper = new MySqlTypeMappingSource(
            new TypeMappingSourceDependencies(
                new ValueConverterSelector(new ValueConverterSelectorDependencies()),
                Array.Empty<ITypeMappingSourcePlugin>()),
            new RelationalTypeMappingSourceDependencies(
                new [] {new MySqlJsonMicrosoftTypeMappingSourcePlugin(new MySqlOptions())}),
            new MySqlOptions()
        );

        private static RelationalTypeMapping GetMapping(string storeType) => Mapper.FindMapping(storeType);

        private static RelationalTypeMapping GetMapping(Type clrType) => (RelationalTypeMapping)Mapper.FindMapping(clrType);

        private static readonly CSharpHelper CsHelper = new CSharpHelper(Mapper);

        private static string CodeLiteral(object value) => CsHelper.UnknownLiteral(value);

        #endregion Support
    }
}
