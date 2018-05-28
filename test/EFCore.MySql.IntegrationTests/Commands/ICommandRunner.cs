namespace EFCore.MySql.FunctionalTests.Commands
{

    public interface ICommandRunner
    {
        int Run(string[] args);
    }

}
