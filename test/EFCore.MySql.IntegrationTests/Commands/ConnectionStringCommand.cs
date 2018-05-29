using System;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands{

    public class ConnectionStringCommand : IConnectionStringCommand
    {

        public void Run()
        {
            Console.Write(AppConfig.Config["Data:ConnectionString"]);
        }

    }

}
