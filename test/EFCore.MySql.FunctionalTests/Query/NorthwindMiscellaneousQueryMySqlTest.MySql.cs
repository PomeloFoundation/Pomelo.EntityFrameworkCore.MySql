using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindMiscellaneousQueryMySqlTest
    {
        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public virtual Task RowNumberOverPartitionBy_not_supported_throws(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_Joined_Take(async));
        }
    }
}
