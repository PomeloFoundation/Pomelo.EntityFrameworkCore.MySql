namespace Pomelo.EntityFrameworkCore.MySql.Internal
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

        public MySqlBooleanType ClrBoolean { get; private set; }
        public MySqlDateTimeType ClrDateTime { get; private set; }
        public MySqlDateTimeType ClrDateTimeOffset { get; private set; }
        public MySqlTimeSpanType ClrTimeSpan { get; private set; }

        public MySqlDefaultDataTypeMappings WithClrBoolean(MySqlBooleanType mysqlBooleanType)
        {
            var clone = Clone();
            clone.ClrBoolean = mysqlBooleanType;
            return clone;
        }

        public MySqlDefaultDataTypeMappings WithClrDateTime(MySqlDateTimeType mysqlDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTime = mysqlDateTimeType;
            return clone;
        }

        public MySqlDefaultDataTypeMappings WithClrDateTimeOffset(MySqlDateTimeType mysqlDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTimeOffset = mysqlDateTimeType;
            return clone;
        }

        public MySqlDefaultDataTypeMappings WithClrTimeSpan(MySqlTimeSpanType mysqlTimeSpanType)
        {
            var clone = Clone();
            clone.ClrTimeSpan = mysqlTimeSpanType;
            return clone;
        }

        protected virtual MySqlDefaultDataTypeMappings Clone() => new MySqlDefaultDataTypeMappings(this);

        protected bool Equals(MySqlDefaultDataTypeMappings other)
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
        None = -1,
        Default = 0,
        TinyInt1 = 1,
        Bit1 = 2
    }

    public enum MySqlDateTimeType
    {
        Default = 0,
        DateTime = 1,
        DateTime6 = 2,
        Timestamp6 = 3
    }

    public enum MySqlTimeSpanType
    {
        Default = 0,
        Time = 1,
        Time6 = 2,
    }
}
