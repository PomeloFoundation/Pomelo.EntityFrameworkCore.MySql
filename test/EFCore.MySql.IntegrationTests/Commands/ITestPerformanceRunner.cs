using System;
using System.Threading.Tasks;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands
{

    public interface ITestPerformanceRunner
    {
        Task ConnectionTask(Func<AppDb, Task> cb, int ops);
    }

}
