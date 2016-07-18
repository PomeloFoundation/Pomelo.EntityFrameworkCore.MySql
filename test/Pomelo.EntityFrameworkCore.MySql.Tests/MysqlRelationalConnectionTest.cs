using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using Pomelo.Data.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests
{

    public class MysqlRelationalConnectionTest
    {
        [Fact]
        public void Creates_Mysql_Server_connection_string()
        {
            using (var connection = new MySqlRelationalConnection(CreateOptions(), new Logger<MySqlConnection>(new LoggerFactory())))
            {
                Assert.IsType<MySqlConnection>(connection.DbConnection);
            }
        }

        [Fact]
        public void Can_create_master_connection_string()
        {
            using (var connection = new MySqlRelationalConnection(CreateOptions(), new Logger<MySqlConnection>(new LoggerFactory())))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(@"server=127.0.0.1;port=3306;database=mysql;user id=root;password=Password12!;pooling=False", master.ConnectionString);
                }
            }
        }

        public static IDbContextOptions CreateOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql(@"Server=127.0.0.1;Port=3306;Database=blogs;Uid=root;Password=Password12!");

            return optionsBuilder.Options;
        }
    }
}
