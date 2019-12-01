using System.Collections.Generic;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage
{
    public partial class ServerVersion
    {
        #region Individual version strings for tests

        public const string DateTimeCurrentTimestampMySqlSupportVersionString = "5.6.5-mysql";
        public const string DateTimeCurrentTimestampMariaDbSupportVersionString = "10.0.1-mariadb";

        public const string DateTime6MySqlSupportVersionString = "5.6.4-mysql";
        public const string DateTime6MariaDbSupportVersionString = "10.1.2-mariadb";

        public const string LargerKeyLengthMySqlSupportVersionString = "5.7.7-mysql";
        public const string LargerKeyLengthMariaDbSupportVersionString = "10.2.2-mariadb";

        public const string RenameIndexMySqlSupportVersionString = "5.7.0-mysql";
        // public const string RenameIndexMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string GeneratedColumnsMySqlSupportVersionString = "5.7.0-mysql";
        public const string GeneratedColumnsMariaDbSupportVersionString = "10.2.0-mariadb";

        public const string NullableGeneratedColumnsMySqlSupportVersionString = "5.7.0-mysql";
        // public const string NullableGeneratedColumnsMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string JsonMySqlSupportVersionString = "5.7.8-mysql";
        // public const string JsonMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string RenameColumnMySqlSupportVersionString = "8.0.0-mysql";
        // public const string RenameColumnMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string WindowFunctionsMySqlSupportVersionString = "8.0.0-mysql";
        public const string WindowFunctionsMariaDbSupportVersionString = "10.2.0-mariadb";

        public const string OuterApplyMySqlSupportVersionString = "8.0.14-mysql";
        // public const string OuterApplyMariaDbSupportVersionString = "?.?.?-mariadb"; // MDEV-19078, MDEV-6373

        public const string CrossApplyMySqlSupportVersionString = "8.0.14-mysql";
        // public const string CrossApplyMariaDbSupportVersionString = "?.?.?-mariadb"; // MDEV-19078, MDEV-6373

        // public const string FloatCastMySqlSupportVersionString = "8.0.17-mysql"; // The implemented support drops some decimal places and rounds.
        // public const string FloatCastMariaDbSupportVersionString = "10.4.5-mariadb"; // The implemented support drops some decimal places and rounds.

        public const string DoubleCastMySqlSupportVersionString = "8.0.17-mysql";
        public const string DoubleCastMariaDbSupportVersionString = "10.4.0-mariadb";

        public const string DefaultCharSetUtf8Mb4MySqlSupportVersionString = "8.0.0-mysql";
        // public const string DefaultCharSetUtf8Mb4MariaDbSupportVersionString = "?.?.?-mariadb";

        public const string DefaultExpressionMySqlSupportVersionString = "8.0.13-mysql";
        // public const string DefaultExpressionMariaDbSupportVersionString = "?.?.?-mariadb";

        #endregion

        #region SupportMap keys for test attributes

        public const string DateTimeCurrentTimestampSupportKey = nameof(DateTimeCurrentTimestampSupportKey);
        public const string DateTime6SupportKey = nameof(DateTime6SupportKey);
        public const string LargerKeyLengthSupportKey = nameof(LargerKeyLengthSupportKey);
        public const string RenameIndexSupportKey = nameof(RenameIndexSupportKey);
        public const string RenameColumnSupportKey = nameof(RenameColumnSupportKey);
        public const string WindowFunctionsSupportKey = nameof(WindowFunctionsSupportKey);
        public const string OuterApplySupportKey = nameof(OuterApplySupportKey);
        public const string CrossApplySupportKey = nameof(CrossApplySupportKey);
        public const string FloatCastSupportKey = nameof(FloatCastSupportKey);
        public const string DoubleCastSupportKey = nameof(DoubleCastSupportKey);
        public const string JsonSupportKey = nameof(JsonSupportKey);
        public const string GeneratedColumnsSupportKey = nameof(GeneratedColumnsSupportKey);
        public const string NullableGeneratedColumnsSupportKey = nameof(NullableGeneratedColumnsSupportKey);
        public const string DefaultCharSetUtf8Mb4SupportKey = nameof(DefaultCharSetUtf8Mb4SupportKey);
        public const string DefaultExpressionSupportKey = nameof(DefaultExpressionSupportKey);

        #endregion

        protected static readonly Dictionary<string, ServerVersionSupport> SupportMap = new Dictionary<string, ServerVersionSupport>()
        {
            { DateTimeCurrentTimestampSupportKey, new ServerVersionSupport(DateTimeCurrentTimestampMySqlSupportVersionString, DateTimeCurrentTimestampMariaDbSupportVersionString) },
            { DateTime6SupportKey, new ServerVersionSupport(DateTime6MySqlSupportVersionString, DateTime6MariaDbSupportVersionString) },
            { LargerKeyLengthSupportKey, new ServerVersionSupport(LargerKeyLengthMySqlSupportVersionString, LargerKeyLengthMariaDbSupportVersionString) },
            { RenameIndexSupportKey, new ServerVersionSupport(RenameIndexMySqlSupportVersionString/*, RenameIndexMariaDbSupportVersionString*/) },
            { RenameColumnSupportKey, new ServerVersionSupport(RenameColumnMySqlSupportVersionString/*, RenameColumnMariaDbSupportVersionString*/) },
            { WindowFunctionsSupportKey, new ServerVersionSupport(WindowFunctionsMySqlSupportVersionString, WindowFunctionsMariaDbSupportVersionString) },
            { OuterApplySupportKey, new ServerVersionSupport(OuterApplyMySqlSupportVersionString/*, OuterApplyMariaDbSupportVersionString*/) },
            { CrossApplySupportKey, new ServerVersionSupport(CrossApplyMySqlSupportVersionString/*, CrossApplyMariaDbSupportVersionString*/) },
            { DoubleCastSupportKey, new ServerVersionSupport(DoubleCastMySqlSupportVersionString, DoubleCastMariaDbSupportVersionString) },
            { JsonSupportKey, new ServerVersionSupport(JsonMySqlSupportVersionString/*, JsonMariaDbSupportVersionString*/) },
            { GeneratedColumnsSupportKey, new ServerVersionSupport(GeneratedColumnsMySqlSupportVersionString, GeneratedColumnsMariaDbSupportVersionString) },
            { NullableGeneratedColumnsSupportKey, new ServerVersionSupport(NullableGeneratedColumnsMySqlSupportVersionString/*, NullableGeneratedColumnsMariaDbSupportVersionString*/) },
            { DefaultCharSetUtf8Mb4SupportKey, new ServerVersionSupport(DefaultCharSetUtf8Mb4MySqlSupportVersionString/*, DefaultCharSetUtf8Mb4MariaDbSupportVersionString*/) },
            { DefaultExpressionSupportKey, new ServerVersionSupport(DefaultExpressionMySqlSupportVersionString/*, DefaultExpressionMariaDbSupportVersionString*/) },
        };

        #region Support checks for provider code

        public virtual bool SupportsDateTimeCurrentTimestamp => SupportMap[DateTimeCurrentTimestampSupportKey].IsSupported(this);
        public virtual bool SupportsDateTime6 => SupportMap[DateTime6SupportKey].IsSupported(this);
        public virtual bool SupportsLargerKeyLength => SupportMap[LargerKeyLengthSupportKey].IsSupported(this);
        public virtual bool SupportsRenameIndex => SupportMap[RenameIndexSupportKey].IsSupported(this);
        public virtual bool SupportsRenameColumn => SupportMap[RenameColumnSupportKey].IsSupported(this);
        public virtual bool SupportsWindowFunctions => SupportMap[WindowFunctionsSupportKey].IsSupported(this);
        public virtual bool SupportsFloatCast => false; // The implemented support drops some decimal places and rounds.
        public virtual bool SupportsDoubleCast => SupportMap[DoubleCastSupportKey].IsSupported(this);
        public virtual bool SupportsOuterApply => SupportMap[OuterApplySupportKey].IsSupported(this);
        public virtual bool SupportsCrossApply => SupportMap[CrossApplySupportKey].IsSupported(this);
        public virtual bool SupportsJson => SupportMap[JsonSupportKey].IsSupported(this);
        public virtual bool SupportsGeneratedColumns => SupportMap[GeneratedColumnsSupportKey].IsSupported(this);
        public virtual bool SupportsNullableGeneratedColumns => SupportMap[NullableGeneratedColumnsSupportKey].IsSupported(this);
        public virtual bool SupportsDefaultCharSetUtf8Mb4 => SupportMap[DefaultCharSetUtf8Mb4SupportKey].IsSupported(this);
        public virtual bool SupportsDefaultExpression => SupportMap[DefaultExpressionSupportKey].IsSupported(this);

        #endregion

        public virtual int MaxKeyLength => SupportsLargerKeyLength ? 3072 : 767;
        public virtual CharSet DefaultCharSet => SupportsDefaultCharSetUtf8Mb4 ? CharSet.Utf8Mb4 : CharSet.Latin1;
        
        /// <summary>
        /// Constructs a new <see cref="ServerVersionSupport"/> object containing the <see cref="ServerVersion"/> objects
        /// referenced by the provided version strings and/or support keys.
        /// </summary>
        /// <param name="supportKeysOrVersionStrings">A mix of server version strings and/or support keys.</param>
        /// <returns>Returns the constructed <see cref="ServerVersionSupport"/> object.</returns>
        /// <remarks>Used by test attributes. Provider code should use the `Supports` instance methods instead.</remarks>
        public static ServerVersionSupport GetSupport(params string[] supportKeysOrVersionStrings)
            => new ServerVersionSupport(supportKeysOrVersionStrings
                .SelectMany(s => SupportMap.ContainsKey(s) ? SupportMap[s].SupportedServerVersions : new[] { new ServerVersion(s) })
                .ToArray());
    }
}
