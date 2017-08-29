namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands
{

    public interface ICommandRunner
    {
        int Run(string[] args);
    }

}
