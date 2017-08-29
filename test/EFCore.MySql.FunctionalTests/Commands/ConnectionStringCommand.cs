using System;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands{

    public class ConnectionStringCommand : IConnectionStringCommand
    {

        public void Run()
        {
            Console.Write(AppConfig.Config["Data:ConnectionString"]);
        }

    }

}
