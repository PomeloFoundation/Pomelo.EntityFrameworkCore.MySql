using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Tests.Models
{

    public class ExpressionTest : IDisposable
    {

        private readonly AppDbScope _scope;
        private AppDb _db => _scope.AppDb;
        private readonly DataTypesSimple _simple;
        private readonly DataTypesSimple _simple2;
        private readonly DataTypesVariable _variable;

        public ExpressionTest()
        {
            _scope = new AppDbScope();

            // initialize simple data types
            _simple = new DataTypesSimple
            {
                TypeDateTime = new DateTime(2017, 1, 1, 0, 0, 0),
                TypeDouble = 3.1415,
                TypeDoubleN = -3.1415
            };
            _db.DataTypesSimple.Add(_simple);

            // initialize simple data types
            _simple2 = new DataTypesSimple
            {
                TypeDouble = 1,
                TypeDoubleN = -1
            };
            _db.DataTypesSimple.Add(_simple2);

            // initialize variable data types
            _variable = DataTypesVariable.CreateEmpty();
            _variable.TypeString = "EntityFramework";

            _db.DataTypesVariable.Add(_variable);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            try
            {
                _db.DataTypesSimple.Remove(_simple);
                _db.DataTypesVariable.Remove(_variable);
                _db.SaveChanges();
            }
            finally
            {
                _scope.Dispose();
            }
        }


        [Fact]
        public async Task MySqlContainsOptimizedTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Contains = m.TypeString.Contains("Fram"),
                    NotContains = m.TypeString.Contains("asdf")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.Contains);
            Assert.False(result.NotContains);
        }

        [Fact]
        public async Task MySqlDateAddTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    FutureYear = m.TypeDateTime.AddYears(1),
                    FutureMonth = m.TypeDateTime.AddMonths(1),
                    FutureDay = m.TypeDateTime.AddDays(1),
                    FutureHour = m.TypeDateTime.AddHours(1),
                    FutureMinute = m.TypeDateTime.AddMinutes(1),
                    FutureSecond = m.TypeDateTime.AddSeconds(1),
                    PastYear = m.TypeDateTime.AddYears(-1),
                    PastMonth = m.TypeDateTime.AddMonths(-1),
                    PastDay = m.TypeDateTime.AddDays(-1),
                    PastHour = m.TypeDateTime.AddHours(-1),
                    PastMinute = m.TypeDateTime.AddMinutes(-1),
                    PastSecond = m.TypeDateTime.AddSeconds(-1),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(_simple.TypeDateTime.AddYears(1), result.FutureYear);
            Assert.Equal(_simple.TypeDateTime.AddMonths(1), result.FutureMonth);
            Assert.Equal(_simple.TypeDateTime.AddDays(1), result.FutureDay);
            Assert.Equal(_simple.TypeDateTime.AddHours(1), result.FutureHour);
            Assert.Equal(_simple.TypeDateTime.AddMinutes(1), result.FutureMinute);
            Assert.Equal(_simple.TypeDateTime.AddSeconds(1), result.FutureSecond);
            Assert.Equal(_simple.TypeDateTime.AddYears(-1), result.PastYear);
            Assert.Equal(_simple.TypeDateTime.AddMonths(-1), result.PastMonth);
            Assert.Equal(_simple.TypeDateTime.AddDays(-1), result.PastDay);
            Assert.Equal(_simple.TypeDateTime.AddHours(-1), result.PastHour);
            Assert.Equal(_simple.TypeDateTime.AddMinutes(-1), result.PastMinute);
            Assert.Equal(_simple.TypeDateTime.AddSeconds(-1), result.PastSecond);
        }

        [Fact]
        public async Task MySqlDatePartTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Year = m.TypeDateTime.Year,
                    Month = m.TypeDateTime.Month,
                    Day = m.TypeDateTime.Day,
                    Hour = m.TypeDateTime.Hour,
                    Minute = m.TypeDateTime.Minute,
                    Second = m.TypeDateTime.Second,
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(_simple.TypeDateTime.Year, result.Year);
            Assert.Equal(_simple.TypeDateTime.Month, result.Month);
            Assert.Equal(_simple.TypeDateTime.Day, result.Day);
            Assert.Equal(_simple.TypeDateTime.Hour, result.Hour);
            Assert.Equal(_simple.TypeDateTime.Minute, result.Minute);
            Assert.Equal(_simple.TypeDateTime.Second, result.Second);
        }

        [Fact]
        public async Task MySqlDateTimeNowTranslator()
        {
            var utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            var utcOffsetStr = (utcOffset.TotalHours >= 0 ? "+" : "") + utcOffset.TotalHours.ToString("00") + ":" + utcOffset.Minutes.ToString("00");

            await _db.Database.OpenConnectionAsync();
            await _db.Database.ExecuteSqlCommandAsync($"SET @@session.time_zone = {utcOffsetStr}");

            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Now = DateTime.Now,
                    UtcNow = DateTime.UtcNow
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            _db.Database.CloseConnection();

            Assert.InRange(result.Now, DateTime.Now - TimeSpan.FromSeconds(5), DateTime.Now + TimeSpan.FromSeconds(5));
            Assert.InRange(result.UtcNow, DateTime.UtcNow - TimeSpan.FromSeconds(5), DateTime.UtcNow + TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task MySqlEndsWithOptimizedTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    EndsWith = m.TypeString.EndsWith("Framework"),
                    NotEndsWith = m.TypeString.EndsWith("Entity")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.EndsWith);
            Assert.False(result.NotEndsWith);
        }

        [Fact]
        public async Task MySqlMathAbsTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Abs = Math.Abs(m.TypeDoubleN.Value),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Abs(_simple.TypeDoubleN.Value), result.Abs);
        }

        [Fact]
        public async Task MySqlMathCeilingTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Ceiling = Math.Ceiling(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Ceiling(_simple.TypeDouble), result.Ceiling);
        }

        [Fact]
        public async Task MySqlMathFloorTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Floor = Math.Floor(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Floor(_simple.TypeDouble), result.Floor);
        }

        [Fact]
        public async Task MySqlMathPowerTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Pow = Math.Pow(m.TypeDouble, m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Pow(_simple.TypeDouble, _simple.TypeDouble), result.Pow);
        }

        [Fact]
        public async Task MySqlMathRoundTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Round = Math.Round(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Round(_simple.TypeDouble), result.Round);
        }

        [Fact]
        public async Task MySqlMathTruncateTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Truncate = Math.Truncate(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Truncate(_simple.TypeDouble), result.Truncate);
        }

        [Fact]
        public async Task MySqlNewGuidTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Guid = Guid.NewGuid(),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.NotEqual(Guid.Empty, result.Guid);
        }

        [Fact]
        public async Task MySqlRegexIsMatchTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Match = Regex.IsMatch(m.TypeString, @"^Entity[a-zA-Z]{9}$"),
                    NotMatch = Regex.IsMatch(m.TypeString, @"^Entity[a-zA-Z]{8}$")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.Match);
            Assert.False(result.NotMatch);
        }

        [Fact]
        public async Task MySqlStringEqualsTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Equals = m.TypeString.Equals("EntityFramework"),
                    NotEquals = m.TypeString.Equals("asdf")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.Equals);
            Assert.False(result.NotEquals);
        }

        [Fact]
        public async Task MySqlStartsWithOptimizedTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    StartsWith = m.TypeString.StartsWith("Entity"),
                    NotStartsWith = m.TypeString.StartsWith("Framework")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.StartsWith);
            Assert.False(result.NotStartsWith);
        }

        [Fact]
        public async Task MySqlStringLengthTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Length = m.TypeString.Length,
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);
            Assert.Equal(_variable.TypeString.Length, result.Length);
        }

        [Fact]
        public async Task MySqlStringReplaceTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Replaced = m.TypeString.Replace("Entity", "Pomelo.Entity")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal("Pomelo.EntityFramework", result.Replaced);
        }

        [Fact]
        public async Task MySqlStringSubstringTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    First3Chars = m.TypeString.Substring(0, 3),
                    Last3Chars = m.TypeString.Substring(m.TypeString.Length - 4, 3),
                    MiddleChars = m.TypeString.Substring(1, m.TypeString.Length - 2)
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal(_variable.TypeString.Substring(0, 3), result.First3Chars);
            Assert.Equal(_variable.TypeString.Substring(_variable.TypeString.Length - 4, 3), result.Last3Chars);
            Assert.Equal(_variable.TypeString.Substring(1, _variable.TypeString.Length - 2), result.MiddleChars);
        }

        [Fact]
        public async Task MySqlStringToLowerTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Lower = m.TypeString.ToLower()
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal("entityframework", result.Lower);
        }

        [Fact]
        public async Task MySqlStringToUpperTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Upper = m.TypeString.ToUpper()
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal("ENTITYFRAMEWORK", result.Upper);
        }

        [Fact]
        public async Task MySqlMathAcosTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Acos = Math.Acos(m.TypeDoubleN.Value),
                }).FirstOrDefaultAsync(m => m.Id == _simple2.Id);

            Assert.Equal(Math.Acos(_simple2.TypeDoubleN.Value), result.Acos);
        }

        [Fact]
        public async Task MySqlMathCosTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Cos = Math.Cos(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple2.Id);

            Assert.Equal(Math.Cos(_simple2.TypeDouble), result.Cos);
        }

        [Fact]
        public async Task MySqlMathSinTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Sin = Math.Sin(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple2.Id);

            Assert.Equal(Math.Sin(_simple2.TypeDouble), result.Sin);
        }

        [Fact]
        public async Task MySqlDateToDateTimeConvertTranslator()
        {
            var result = await _db.DataTypesSimple.CountAsync(m => m.TypeDateTimeN <= DateTime.Now.Date);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task MySqlToStringConvertTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m => new {
                ConvertedInt32 = m.Id.ToString(),
                ConvertedLong = m.TypeLong.ToString(),
                ConvertedByte = m.TypeByte.ToString(),
                ConvertedSByte = m.TypeSbyte.ToString(),
                ConvertedBool = m.TypeBool.ToString(),
                ConvertedNullBool = m.TypeBoolN.ToString(),
                ConvertedDecimal = m.TypeDecimal.ToString(),
                ConvertedDouble = m.TypeDouble.ToString(),
                ConvertedFloat = m.TypeFloat.ToString(),
                ConvertedGuid = m.TypeGuid.ToString(),
                Text = m.TypeChar
            }
            ).FirstOrDefaultAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task MySqlCastTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m => new {
                IntToUlong = (ulong) m.TypeInt,
                IntToDecimal = (decimal) m.TypeInt,
                IntToString = "test" + m.TypeInt,
            }
            ).FirstOrDefaultAsync();

            Assert.NotNull(result);
        }

    }

}
