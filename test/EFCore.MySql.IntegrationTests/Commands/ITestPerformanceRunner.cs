using System;
using System.Threading.Tasks;

namespace EFCore.MySql.FunctionalTests.Commands
{

    public interface ITestPerformanceRunner
    {
        Task ConnectionTask(Func<AppDb, Task> cb, int ops);
    }

}
