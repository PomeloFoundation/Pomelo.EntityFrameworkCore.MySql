// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Globalization;
using System.Text;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    internal static class TypeNameBuilder
    {
        private static StringBuilder AppendSize(this StringBuilder builder, DbParameter parameter)
        {
            if (parameter.Size > 0)
            {
                builder
                    .Append('(')
                    .Append(parameter.Size.ToString(CultureInfo.InvariantCulture))
                    .Append(')');
            }

            return builder;
        }

        private static StringBuilder AppendPrecision(this StringBuilder builder, DbParameter parameter)
        {
            if (parameter.Precision > 0)
            {
                builder
                    .Append('(')
                    .Append(parameter.Precision.ToString(CultureInfo.InvariantCulture))
                    .Append(')');
            }

            return builder;
        }

        private static StringBuilder AppendPrecisionAndScale(this StringBuilder builder, DbParameter parameter)
        {
            if (parameter.Precision > 0
                && parameter.Scale > 0)
            {
                builder
                    .Append('(')
                    .Append(parameter.Precision.ToString(CultureInfo.InvariantCulture))
                    .Append(',')
                    .Append(parameter.Scale.ToString(CultureInfo.InvariantCulture))
                    .Append(')');
            }

            return builder.AppendPrecision(parameter);
        }

        public static string CreateTypeName(DbParameter parameter)
        {
            if (!(parameter is MySqlParameter mySqlParameter))
            {
                throw new ArgumentException($"The parameter is not of type '{nameof(MySqlParameter)}'.", nameof(parameter));
            }

            var builder = new StringBuilder();
            return (mySqlParameter.MySqlDbType switch
            {
                MySqlDbType.Bool => builder.Append("bool"),
                MySqlDbType.Decimal => builder.Append("decimal").AppendPrecisionAndScale(parameter),
                MySqlDbType.Byte => builder.Append("tinyint"),
                MySqlDbType.Int16 => builder.Append("smallint"),
                MySqlDbType.Int32 => builder.Append("int"),
                MySqlDbType.Float => builder.Append("float"),
                MySqlDbType.Double => builder.Append("double"),
                MySqlDbType.Null => builder.Append("null"),
                MySqlDbType.Timestamp => builder.Append("timestamp"),
                MySqlDbType.Int64 => builder.Append("bigint"),
                MySqlDbType.Int24 => builder.Append("mediumint"),
                MySqlDbType.Date => builder.Append("date").AppendPrecision(parameter),
                MySqlDbType.Time => builder.Append("time").AppendPrecision(parameter),
                MySqlDbType.DateTime => builder.Append("datetime").AppendPrecision(parameter),
                MySqlDbType.Year => builder.Append("year"),
                MySqlDbType.Newdate => builder.Append("date").AppendPrecision(parameter),
                MySqlDbType.VarString => builder.Append("varchar").AppendSize(parameter),
                MySqlDbType.Bit => builder.Append("bit"),
                MySqlDbType.JSON => builder.Append("json"),
                MySqlDbType.NewDecimal => builder.Append("decimal"),
                MySqlDbType.Enum => builder.Append("enum"),
                MySqlDbType.Set => builder.Append("set"),
                MySqlDbType.TinyBlob => builder.Append("tinyblob"),
                MySqlDbType.MediumBlob => builder.Append("mediumblob"),
                MySqlDbType.LongBlob => builder.Append("longblob"),
                MySqlDbType.Blob => builder.Append("blob"),
                MySqlDbType.VarChar => builder.Append("varchar").AppendSize(parameter),
                MySqlDbType.String => builder.Append("char").AppendSize(parameter),
                MySqlDbType.Geometry => builder.Append("geometry"),
                MySqlDbType.UByte => builder.Append("tinyint"),
                MySqlDbType.UInt16 => builder.Append("smallint"),
                MySqlDbType.UInt32 => builder.Append("int"),
                MySqlDbType.UInt64 => builder.Append("bigint"),
                MySqlDbType.UInt24 => builder.Append("mediumint"),
                MySqlDbType.Binary => builder.Append("binary").AppendSize(parameter),
                MySqlDbType.VarBinary => builder.Append("varbinary").AppendSize(parameter),
                MySqlDbType.TinyText => builder.Append("tinytext"),
                MySqlDbType.MediumText => builder.Append("mediumtext"),
                MySqlDbType.LongText => builder.Append("longtext"),
                MySqlDbType.Text => builder.Append("text"),
                MySqlDbType.Guid => builder.Append("char"),
                _ => throw new InvalidOperationException($"Unknown {nameof(MySqlDbType)} value of '{mySqlParameter.MySqlDbType}'.")
            }).ToString();
        }
    }
}
