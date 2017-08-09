using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

            Assert.Equal(@"SELECT `x`.`ISBN`, `x`.`AuthorId`, `x`.`PressId`, `x`.`SaleCount`, `x`.`SinglePrice`, `x`.`Title`
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
FROM `Books` AS `x`;
", sql, false, true, true);
        }

        [Fact(Skip = "Regex translator not work")]
        public void Select_match_regex_test()
        {
            var regex = new Regex("^[a-zA-Z ]{4,8}$");
            var db = new BookContext();
            var sql = db.Books
                .Where(x => regex.IsMatch(x.Title))
                .ToSql();

            Assert.Equal("", sql);
        }

        [Fact]
        public void Select_to_upper_case_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Select(x => new { Title = x.Title.ToUpper() })
                .ToSql();

            Assert.Equal(@"SELECT UPPER(`x`.`Title`) AS `Title`
FROM `Books` AS `x`;
", sql, false, true, true);
        }

        [Fact]
        public void Select_to_lower_case_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Select(x => new { Title = x.Title.ToLower() })
                .ToSql();

            Assert.Equal(@"SELECT LOWER(`x`.`Title`) AS `Title`
FROM `Books` AS `x`;
", sql, false, true, true);
        }

        [Fact]
        public void Select_where_string_is_null_or_whitespace_test()
        {
            var db = new BookContext();
            var sql = db.Presses
                .Where(x => string.IsNullOrWhiteSpace(x.Tel))
                .ToSql();

            Assert.Equal(@"SELECT `x`.`Id`, `x`.`Name`, `x`.`Tel`
FROM `Presses` AS `x`
WHERE `x`.`Tel` IS NULL OR (LTRIM(RTRIM(`x`.`Tel`)) = '');
", sql, false, true, true);
        }

        [Fact]
        public void Select_where_string_is_null_or_empty_test()
        {
            var db = new BookContext();
            var sql = db.Presses
                .Where(x => string.IsNullOrWhiteSpace(x.Tel))
                .ToSql();

            Assert.Equal(@"SELECT `x`.`Id`, `x`.`Name`, `x`.`Tel`
FROM `Presses` AS `x`
WHERE `x`.`Tel` IS NULL OR (LTRIM(RTRIM(`x`.`Tel`)) = '');
", sql, false, true, true);
        }

        [Fact]
        public void Select_datetime_now_test()
        {
            var db = new BookContext();
            var sql = db.Authors
                .Select(x => new
                {
                    Name = x.FirstName + " " + x.LastName,
                    Timestamp = DateTime.Now
                })
                .ToSql();

            Assert.Equal(@"SELECT CONCAT(CONCAT(`x`.`FirstName`,' '),`x`.`LastName`) AS `Name`, CURRENT_TIMESTAMP() AS `Timestamp`
FROM `Authors` AS `x`;
", sql, false, true, true);
        }

        [Fact]
        public void Select_convert_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Where(x => x.SaleCount >= 100)
                .Select(x => new
                {
                    Title = x.Title,
                    TotalMoney = Convert.ToInt64(x.SinglePrice * x.SaleCount * 100) / 100.0
                })
                .ToSql();

            Assert.Equal(@"SELECT `x`.`Title`, CONVERT(bigint, (`x`.`SinglePrice` * `x`.`SaleCount`) * 100E0) / 100E0 AS `TotalMoney`
FROM `Books` AS `x`
WHERE `x`.`SaleCount` >= 100;
", sql, false, true, true);
        }

        [Fact]
        public void Select_math_floor_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Where(x => x.SaleCount >= 100)
                .Select(x => new
                {
                    Title = x.Title,
                    TotalMoney = Math.Floor(Convert.ToInt64(x.SinglePrice * x.SaleCount * 100) / 100.0)
                })
                .ToSql();

            Assert.Equal(@"SELECT `x`.`Title`, FLOOR(CONVERT(bigint, (`x`.`SinglePrice` * `x`.`SaleCount`) * 100E0) / 100E0) AS `TotalMoney`
FROM `Books` AS `x`
WHERE `x`.`SaleCount` >= 100;
", sql, false, true, true);
        }

        [Fact]
        public void Select_string_length_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Where(x => x.SaleCount >= 100)
                .Select(x => new
                {
                    TitleLength = x.Title.Length
                })
                .ToSql();

            Assert.Equal(@"SELECT CHAR_LENGTH(`x`.`Title`) AS `TitleLength`
FROM `Books` AS `x`
WHERE `x`.`SaleCount` >= 100;
", sql, false, true, true);
        }

        [Fact]
        public void Select_string_replace_test()
        {
            var db = new BookContext();
            var sql = db.Books
                .Where(x => x.SaleCount >= 100)
                .Select(x => new
                {
                    ReplacedTitle = x.Title.Replace("Oracle", "Some Company")
                })
                .ToSql();

            Assert.Equal(@"SELECT REPLACE(`x`.`Title`, 'Oracle', 'Some Company') AS `ReplacedTitle`
FROM `Books` AS `x`
WHERE `x`.`SaleCount` >= 100;
", sql, false, true, true);
        }

        [Fact]
        public void Select_string_trim_test()
        {
            var db = new BookContext();
            var sql = db.Presses
                .Select(x => new
                {
                    Name = x.Name.Trim()
                })
                .ToSql();

            Assert.Equal(@"SELECT LTRIM(RTRIM(`x`.`Name`)) AS `Name`
FROM `Presses` AS `x`;
", sql, false, true, true);
        }
    }
}
