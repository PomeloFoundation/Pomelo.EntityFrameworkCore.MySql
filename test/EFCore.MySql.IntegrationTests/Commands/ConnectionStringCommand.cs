using System;
using Microting.EntityFrameworkCore.MySql.Tests;

namespace Microting.EntityFrameworkCore.MySql.IntegrationTests.Commands
{
    public class ConnectionStringCommand : IConnectionStringCommand
    {
        public void Run()
        {
            Console.Write(AppConfig.ConnectionString);
        }
    }
}
