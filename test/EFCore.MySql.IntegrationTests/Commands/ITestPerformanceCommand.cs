namespace EFCore.MySql.FunctionalTests.Commands
{

    public interface ITestPerformanceCommand
    {
        void Run(int iterations, int concurrency, int ops);
    }

}
