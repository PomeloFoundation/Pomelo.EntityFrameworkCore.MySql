using System;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Commands{

    public static class CommandRunner
    {
        public static void Help()
        {
            Console.Error.WriteLine(@"dotnet run
    connectionString   print connection string
    testMigrate        test dbContext.Database functions: ensureCreate, ensureDelete, migrate
    -h, --help         show this message
            ");
        }

        public static int Run(string[] args)
        {
            var cmd = args[0];

            try
            {
	            switch (cmd)
	            {
		            case "connectionString":
						ConnectionStringCommand.Run();
			            break;
		            case "testMigrate":
						TestMigrateCommand.Run();
			            break;
		            case "-h":
		            case "--help":
			            Help();
			            break;
		            default:
			            Help();
			            return 1;
	            }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return 1;
            }
            return 0;
        }
    }
}
