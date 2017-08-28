namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands
{

    public interface ITestPerformanceCommand
    {
        void Run(int iterations, int concurrency, int ops);
    }

}
