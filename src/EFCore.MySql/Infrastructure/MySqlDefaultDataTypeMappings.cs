// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure
{
    public class MySqlDefaultDataTypeMappings
    {
        public MySqlDefaultDataTypeMappings()
        {
        }

        protected MySqlDefaultDataTypeMappings(MySqlDefaultDataTypeMappings copyFrom)
        {
            ClrBoolean = copyFrom.ClrBoolean;
            ClrDateTime = copyFrom.ClrDateTime;
            ClrDateTimeOffset = copyFrom.ClrDateTimeOffset;
            ClrTimeSpan = copyFrom.ClrTimeSpan;
            ClrTimeOnlyPrecision = copyFrom.ClrTimeOnlyPrecision;
        }

        public virtual MySqlBooleanType ClrBoolean { get; private set; }
        public virtual MySqlDateTimeType ClrDateTime { get; private set; }
        public virtual MySqlDateTimeType ClrDateTimeOffset { get; private set; }
        public virtual MySqlTimeSpanType ClrTimeSpan { get; private set; }
        public virtual int ClrTimeOnlyPrecision { get; private set; } = -1;

        public virtual MySqlDefaultDataTypeMappings WithClrBoolean(MySqlBooleanType mysqlBooleanType)
        {
            var clone = Clone();
            clone.ClrBoolean = mysqlBooleanType;
            return clone;
        }

        public virtual MySqlDefaultDataTypeMappings WithClrDateTime(MySqlDateTimeType mysqlDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTime = mysqlDateTimeType;
            return clone;
        }

        public virtual MySqlDefaultDataTypeMappings WithClrDateTimeOffset(MySqlDateTimeType mysqlDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTimeOffset = mysqlDateTimeType;
            return clone;
        }

        // TODO: Remove Time6, add optional precision parameter for Time types.
        public virtual MySqlDefaultDataTypeMappings WithClrTimeSpan(MySqlTimeSpanType mysqlTimeSpanType)
        {
            var clone = Clone();
            clone.ClrTimeSpan = mysqlTimeSpanType;
            return clone;
        }

        /// <summary>
        /// Set the default precision for `TimeOnly` CLR type mapping to a MySQL TIME type.
        /// Set <paramref name="precision"/> to <see langword="null"/>, to use the highest supported precision.
        /// Otherwise, set <paramref name="precision"/> to a valid value between `0` and `6`.
        /// </summary>
        /// <param name="precision">The precision used for the MySQL TIME type.</param>
        /// <returns>The same instance, to allow chained method calls.</returns>
        public virtual MySqlDefaultDataTypeMappings WithClrTimeOnly(int? precision = null)
        {
            if (precision is < 0 or > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(precision));
            }

            var clone = Clone();
            clone.ClrTimeOnlyPrecision = precision ?? -1;
            return clone;
        }

        protected virtual MySqlDefaultDataTypeMappings Clone() => new MySqlDefaultDataTypeMappings(this);

        protected virtual bool Equals(MySqlDefaultDataTypeMappings other)
        {
            return ClrBoolean == other.ClrBoolean &&
                   ClrDateTime == other.ClrDateTime &&
                   ClrDateTimeOffset == other.ClrDateTimeOffset &&
                   ClrTimeSpan == other.ClrTimeSpan &&
                   ClrTimeOnlyPrecision == other.ClrTimeOnlyPrecision;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MySqlDefaultDataTypeMappings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ClrBoolean;
                hashCode = (hashCode * 397) ^ (int)ClrDateTime;
                hashCode = (hashCode * 397) ^ (int)ClrDateTimeOffset;
                hashCode = (hashCode * 397) ^ (int)ClrTimeSpan;
                return hashCode;
            }
        }
    }

    public enum MySqlBooleanType
    {
        /// <summary>
        /// TODO
        /// </summary>
        None = -1, // TODO: Remove in EF Core 5; see MySqlTypeMappingTest.Bool_with_MySqlBooleanType_None_maps_to_null()

        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        TinyInt1 = 1,

        /// <summary>
        /// TODO
        /// </summary>
        Bit1 = 2
    }

    public enum MySqlDateTimeType
    {
        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        DateTime = 1,

        /// <summary>
        /// TODO
        /// </summary>
        DateTime6 = 2,

        /// <summary>
        /// TODO
        /// </summary>
        Timestamp6 = 3,

        /// <summary>
        /// TODO
        /// </summary>
        Timestamp = 4,
    }

    public enum MySqlTimeSpanType
    {
        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        Time = 1,

        /// <summary>
        /// TODO
        /// </summary>
        Time6 = 2,
    }
}
