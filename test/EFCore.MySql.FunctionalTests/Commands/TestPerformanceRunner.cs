using System;
using System.Threading.Tasks;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands{

    public class TestPerformanceRunner : ITestPerformanceRunner
    {

        private AppDb _db;

        public TestPerformanceRunner(AppDb db)
        {
            _db = db;
        }

        public async Task ConnectionTask(Func<AppDb, Task> cb, int ops)
        {
            for (var op = 0; op < ops; op++)
            {
                await cb(_db);
            }
        }

    }
}
