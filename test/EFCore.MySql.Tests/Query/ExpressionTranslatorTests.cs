using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Tests.Query.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.Query
{
    public class ExpressionTranslatorTests
    {
        [Fact]
        public void Order_by_rand_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .OrderByRandom()
                .ToSql();

            Assert.Equal(@"SELECT `x`.`ISBN`, `x`.`AuthorId`, `x`.`PressId`, `x`.`Title`
FROM `Books` AS `x`
ORDER BY RAND();
", sql, false, true, true);
        }

        [Fact]
        public void Select_new_guid_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Select(x =>
                new
                {
                    Guid = Guid.NewGuid(),
                    ISBN = x.ISBN
                })
                .ToSql();

            Assert.Equal(@"SELECT UUID() AS `Guid`, `x`.`ISBN`
FROM `Books` AS `x`;", sql, false, true, true);
        }
    }
}
