using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindSelectQueryMySqlTest
    {
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.CrossApply))]
        [MemberData(nameof(IsAsyncData))]
        public virtual Task CrossApply_not_supported_throws(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_1(async));
        }

        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.OuterApply))]
        [MemberData(nameof(IsAsyncData))]
        public virtual Task OuterApply_not_supported_throws(bool async)
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => base.SelectMany_correlated_with_outer_3(async));
        }
    }
}
