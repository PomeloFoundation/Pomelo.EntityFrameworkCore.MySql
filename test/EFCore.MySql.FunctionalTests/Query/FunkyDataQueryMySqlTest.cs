using System.Linq;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.FunkyDataModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FunkyDataQueryMySqlTest : FunkyDataQueryTestBase<FunkyDataQueryMySqlTest.FunkyDataQueryMySqlFixture>
    {
        public FunkyDataQueryMySqlTest(FunkyDataQueryMySqlFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task String_contains_on_argument_with_wildcard_column(bool isAsync)
        {
            // See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-607844082

            return AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Select(c => c.FirstName)
                    .SelectMany(c => ss.Set<FunkyCustomer>().Select(c2 => c2.LastName), (fn, ln) => new { fn, ln })
                    .Where(r => r.fn.Contains(r.ln)),
                ss => ss.Set<FunkyCustomer>().Select(c => c.FirstName)
                    .SelectMany(c => ss.Set<FunkyCustomer>().Select(c2 => c2.LastName), (fn, ln) => new { fn, ln })
                    .Where(r => MaybeScalar(r.fn, () => MaybeScalar<bool>(r.ln, () => r.fn.Contains(r.ln))) == true),
                elementSorter: e => (e.fn, e.ln),
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.fn, a.fn);
                    Assert.Equal(e.ln, a.ln);
                });
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override Task String_contains_on_argument_with_wildcard_column_negated(bool isAsync)
        {
            // See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-607844082

            return AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Select(c => c.FirstName)
                    .SelectMany(c => ss.Set<FunkyCustomer>().Select(c2 => c2.LastName), (fn, ln) => new { fn, ln })
                    .Where(r => !r.fn.Contains(r.ln)),
                ss => ss.Set<FunkyCustomer>().Select(c => c.FirstName)
                    .SelectMany(c => ss.Set<FunkyCustomer>().Select(c2 => c2.LastName), (fn, ln) => new { fn, ln })
                    .Where(r => !MaybeScalar(r.fn, () => MaybeScalar<bool>(r.ln, () => r.fn.Contains(r.ln))) == true));
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public override async Task String_contains_on_argument_with_wildcard_parameter(bool isAsync)
        {
            // See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/996#issuecomment-607844082

            var prm1 = "%B";
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => c.FirstName.Contains(prm1)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => MaybeScalar<bool>(c.FirstName, () => c.FirstName.Contains(prm1)) == true)
                    .Select(c => c.FirstName));

            var prm2 = "a_";
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => c.FirstName.Contains(prm2)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => MaybeScalar<bool>(c.FirstName, () => c.FirstName.Contains(prm2)) == true)
                    .Select(c => c.FirstName));

            var prm3 = (string)null;
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => c.FirstName.Contains(prm3)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => false).Select(c => c.FirstName));

            var prm4 = "";
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => c.FirstName.Contains(prm4)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => MaybeScalar<bool>(c.FirstName, () => c.FirstName.Contains(prm4)) == true).Select(c => c.FirstName));

            var prm5 = "_Ba_";
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => c.FirstName.Contains(prm5)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => MaybeScalar<bool>(c.FirstName, () => c.FirstName.Contains(prm5)) == true)
                    .Select(c => c.FirstName));

            var prm6 = "%B%a%r";
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => !c.FirstName.Contains(prm6)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => !MaybeScalar<bool>(c.FirstName, () => c.FirstName.Contains(prm6)) == true)
                    .Select(c => c.FirstName));

            var prm7 = "";
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => !c.FirstName.Contains(prm7)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => false).Select(c => c.FirstName));

            var prm8 = (string)null;
            await AssertQuery(
                isAsync,
                ss => ss.Set<FunkyCustomer>().Where(c => !c.FirstName.Contains(prm8)).Select(c => c.FirstName),
                ss => ss.Set<FunkyCustomer>().Where(c => false).Select(c => c.FirstName));
        }

        public class FunkyDataQueryMySqlFixture : FunkyDataQueryFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
