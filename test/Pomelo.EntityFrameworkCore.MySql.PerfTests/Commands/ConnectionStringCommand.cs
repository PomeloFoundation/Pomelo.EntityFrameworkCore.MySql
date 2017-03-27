using System;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests.Commands{

    public static class ConnectionStringCommand{

        public static void Run(){
            Console.Write(AppConfig.Config["Data:ConnectionString"]);
        }

    }

}
