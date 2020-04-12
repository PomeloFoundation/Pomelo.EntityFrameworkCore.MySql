﻿using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Extensions
{
    public static class MySqlTestDateTimeOffsetExtensions
    {
        public static DateTimeOffset SimulateDatabaseRoundtrip(this DateTimeOffset dateTimeOffset, MySqlTypeMappingSource typeMappingSource)
        {
            var dateTimeOffsetTypeMapping = (MySqlDateTimeOffsetTypeMapping)typeMappingSource.GetMappingForValue(dateTimeOffset);
            var formatString = dateTimeOffsetTypeMapping.GetFormatString();
            var value = DateTimeOffset.ParseExact(
                dateTimeOffsetTypeMapping.GenerateSqlLiteral(dateTimeOffset)
                    .Trim('\''),
                formatString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);
            return value;
        }
    }
}
