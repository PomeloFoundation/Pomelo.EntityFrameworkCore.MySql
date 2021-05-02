// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

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
        }

        public virtual MySqlBooleanType ClrBoolean { get; private set; }
        public virtual MySqlDateTimeType ClrDateTime { get; private set; }
        public virtual MySqlDateTimeType ClrDateTimeOffset { get; private set; }
        public virtual MySqlTimeSpanType ClrTimeSpan { get; private set; }

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

        public virtual MySqlDefaultDataTypeMappings WithClrTimeSpan(MySqlTimeSpanType mysqlTimeSpanType)
        {
            var clone = Clone();
            clone.ClrTimeSpan = mysqlTimeSpanType;
            return clone;
        }

        protected virtual MySqlDefaultDataTypeMappings Clone() => new MySqlDefaultDataTypeMappings(this);

        protected virtual bool Equals(MySqlDefaultDataTypeMappings other)
        {
            return ClrBoolean == other.ClrBoolean &&
                   ClrDateTime == other.ClrDateTime &&
                   ClrDateTimeOffset == other.ClrDateTimeOffset &&
                   ClrTimeSpan == other.ClrTimeSpan;
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
