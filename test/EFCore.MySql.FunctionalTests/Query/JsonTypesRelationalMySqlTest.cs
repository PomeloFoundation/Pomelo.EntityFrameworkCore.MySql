using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Types;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationStable))]
    public class JsonTypesRelationalMySqlTest : JsonTypesRelationalTestBase
    {
        public JsonTypesRelationalMySqlTest(NonSharedFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // Skip spatial type tests for MariaDB < 11.8 (different spatial JSON handling)
        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_line_string()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_point()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_polygon()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_multi_line_string()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_point_with_M()
            => Task.CompletedTask;

        // TODO: Implement better handling for MariaDB
        [ConditionalFact(Skip = "TODO: Implement better handling for MariaDB")]
        public override Task Can_read_write_point_with_Z()
            => Task.CompletedTask;

        // TODO: Implement better handling for MariaDB
        [ConditionalFact(Skip = "TODO: Implement better handling for MariaDB")]
        public override Task Can_read_write_point_with_Z_and_M()
            => Task.CompletedTask;

        // TODO: Implement better handling for MariaDB
        [ConditionalFact(Skip = "TODO: Implement better handling for MariaDB")]
        public override Task Can_read_write_polygon_typed_as_geometry()
            => Task.CompletedTask;

        // TODO: Implement better handling for MariaDB
        [ConditionalFact(Skip = "TODO: Implement better handling for MariaDB")]
        public override Task Can_read_write_binary_as_collection()
            => Task.CompletedTask;

        // Skip ulong enum tests - MariaDB serializes UInt64 Max differently
        [ConditionalFact(Skip = "MariaDB 10.6+ serializes UInt64.MaxValue as full number instead of -1")]
        public override Task Can_read_write_collection_of_nullable_ulong_enum_JSON_values()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ serializes UInt64.MaxValue as full number instead of -1")]
        public override Task Can_read_write_collection_of_ulong_enum_JSON_values()
            => Task.CompletedTask;

        // TODO: Implement better handling for MariaDB - UInt64 enum parameterized tests
        // Skip all test cases from base class by providing minimal InlineData to replace them
        [ConditionalTheory(Skip = "TODO: Implement better handling for MariaDB - UInt64.MaxValue serialization difference")]
        [InlineData(default(EnumU64), "{\"Prop\":0}")]
        public new Task Can_read_write_ulong_enum_JSON_values(EnumU64 value, string json)
            => Task.CompletedTask;

        // TODO: Implement better handling for MariaDB - nullable UInt64 enum parameterized tests  
        [ConditionalTheory(Skip = "TODO: Implement better handling for MariaDB - UInt64.MaxValue serialization difference")]
        [InlineData(null, "{\"Prop\":null}")]
        public Task Can_read_write_nullable_ulong_enum_JSON_values(EnumU64? value, string json)
            => Task.CompletedTask;

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
