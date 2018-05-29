namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands
{

    public interface ITestPerformanceCommand
    {
        void Run(int iterations, int concurrency, int ops);
    }

}
