using System;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands
{
    public class ConnectionStringCommand : IConnectionStringCommand
    {
        public void Run()
        {
            Console.Write(AppConfig.ConnectionString);
        }
    }
}
