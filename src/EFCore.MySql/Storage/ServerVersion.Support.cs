// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;

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
        public const string JsonMariaDbSupportVersionString = "10.2.4-mariadb";

        public const string RenameColumnMySqlSupportVersionString = "8.0.0-mysql";
        // public const string RenameColumnMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string WindowFunctionsMySqlSupportVersionString = "8.0.0-mysql";
        public const string WindowFunctionsMariaDbSupportVersionString = "10.2.0-mariadb";

        public const string OuterApplyMySqlSupportVersionString = "8.0.14-mysql";
        // public const string OuterApplyMariaDbSupportVersionString = "?.?.?-mariadb"; // MDEV-19078, MDEV-6373

        public const string CrossApplyMySqlSupportVersionString = "8.0.14-mysql";
        // public const string CrossApplyMariaDbSupportVersionString = "?.?.?-mariadb"; // MDEV-19078, MDEV-6373

        public const string OuterReferenceInMultiLevelSubqueryMySqlSupportVersionString = "8.0.14-mysql"; // Exact version has not been verified yet
        // public const string OuterReferenceInMultiLevelSubqueryMariaDbSupportVersionString = "?.?.?-mariadb";

        // public const string FloatCastMySqlSupportVersionString = "8.0.17-mysql"; // The implemented support drops some decimal places and rounds.
        // public const string FloatCastMariaDbSupportVersionString = "10.4.5-mariadb"; // The implemented support drops some decimal places and rounds.

        public const string DoubleCastMySqlSupportVersionString = "8.0.17-mysql";
        public const string DoubleCastMariaDbSupportVersionString = "10.4.0-mariadb";

        public const string DefaultCharSetUtf8Mb4MySqlSupportVersionString = "8.0.0-mysql";
        // public const string DefaultCharSetUtf8Mb4MariaDbSupportVersionString = "?.?.?-mariadb";

        public const string DefaultExpressionMySqlSupportVersionString = "8.0.13-mysql";
        // public const string DefaultExpressionMariaDbSupportVersionString = "?.?.?-mariadb";

        // public const string AlternativeDefaultExpressionMySqlSupportVersionString = "?.?.?-mysql";
        public const string AlternativeDefaultExpressionMariaDbSupportVersionString = "10.2.7-mariadb"; // MDEV-13132

        public const string SpatialReferenceSystemRestrictedColumnsMySqlSupportVersionString = "8.0.3-mysql";
        // public const string SpatialReferenceSystemRestrictedColumnsMariaDbSupportVersionString = "?.?.?-mariadb";

        // public const string SpatialFunctionAdditionsMySqlSupportVersionString = "?.?.?-mysql";
        public const string SpatialFunctionAdditionsMariaDbSupportVersionString = "10.1.2-mariadb";

        // public const string SpatialBoundaryFunctionMySqlSupportVersionString = SpatialFunctionAdditionsMySqlSupportVersionString;
        public const string SpatialBoundaryFunctionMariaDbSupportVersionString = SpatialFunctionAdditionsMariaDbSupportVersionString;

        // public const string SpatialIsRingFunctionMySqlSupportVersionString = SpatialFunctionAdditionsMySqlSupportVersionString;
        public const string SpatialIsRingFunctionMariaDbSupportVersionString = SpatialFunctionAdditionsMariaDbSupportVersionString;

        // public const string SpatialPointOnSurfaceFunctionMySqlSupportVersionString = SpatialFunctionAdditionsMySqlSupportVersionString;
        public const string SpatialPointOnSurfaceFunctionMariaDbSupportVersionString = SpatialFunctionAdditionsMariaDbSupportVersionString;

        // public const string SpatialRelateFunctionMySqlSupportVersionString = SpatialFunctionAdditionsMySqlSupportVersionString;
        public const string SpatialRelateFunctionMariaDbSupportVersionString = SpatialFunctionAdditionsMariaDbSupportVersionString;

        public const string SpatialSupportFunctionAdditionsMySqlSupportVersionString = "5.7.6-mysql";
        // public const string SpatialSupportFunctionAdditionsMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string SpatialIsValidFunctionMySqlSupportVersionString = SpatialSupportFunctionAdditionsMySqlSupportVersionString;
        // public const string SpatialIsValidFunctionMariaDbSupportVersionString = SpatialSupportFunctionAdditionsMariaDbSupportVersionString;

        public const string SpatialDistanceSphereFunctionMySqlSupportVersionString = SpatialSupportFunctionAdditionsMySqlSupportVersionString;
        // public const string SpatialDistanceSphereFunctionMariaDbSupportVersionString = SpatialSupportFunctionAdditionsMariaDbSupportVersionString;

        public const string SpatialSetSridFunctionMySqlSupportVersionString = "8.0.0-mysql";
        // public const string SpatialSetSridFunctionMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string SpatialDistanceFunctionImplementsAndoyerMySqlSupportVersionString = "8.0.0-mysql";
        // public const string SpatialDistanceFunctionImplementsAndoyerMariaDbSupportVersionString = "?.?.?-mariadb";

        public const string SpatialGeographicMySqlSupportVersionString = "8.0.0-mysql";
        // public const string SpatialGeographicMariaDbSupportVersionString = "?.?.?-mariadb";

        // public const string ExceptInterceptMySqlSupportVersionString = "?.?.?-mysql";
        public const string ExceptInterceptMariaDbSupportVersionString = "10.3.0-mariadb";

        // public const string ExceptInterceptPrecedenceMySqlSupportVersionString = "?.?.?-mysql";
        public const string ExceptInterceptPrecedenceMariaDbSupportVersionString = "10.4.0-mariadb";

        // public const string JsonDataTypeEmulationMySqlSupportVersionString = "5.7.8-mysql";
        public const string JsonDataTypeEmulationMariaDbSupportVersionString = "10.2.4-mariadb"; // JSON_COMPACT was added in 10.2.4, though most other functions where added in 10.2.3

        public const string ImplicitBoolCheckUsesIndexMySqlSupportVersionString = "8.0.0-mysql"; // Exact version has not been verified yet
        public const string ImplicitBoolCheckUsesIndexMariaDbSupportVersionString = "10.0.0-mariadb"; // Exact version has not been verified yet

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
        public const string OuterReferenceInMultiLevelSubquerySupportKey = nameof(OuterReferenceInMultiLevelSubquerySupportKey);
        public const string DoubleCastSupportKey = nameof(DoubleCastSupportKey);
        public const string JsonSupportKey = nameof(JsonSupportKey);
        public const string GeneratedColumnsSupportKey = nameof(GeneratedColumnsSupportKey);
        public const string NullableGeneratedColumnsSupportKey = nameof(NullableGeneratedColumnsSupportKey);
        public const string DefaultCharSetUtf8Mb4SupportKey = nameof(DefaultCharSetUtf8Mb4SupportKey);
        public const string DefaultExpressionSupportKey = nameof(DefaultExpressionSupportKey);
        public const string AlternativeDefaultExpressionSupportKey = nameof(AlternativeDefaultExpressionSupportKey);
        public const string SpatialReferenceSystemRestrictedColumnsSupportKey = nameof(SpatialReferenceSystemRestrictedColumnsSupportKey);
        public const string SpatialFunctionAdditionsSupportKey = nameof(SpatialFunctionAdditionsSupportKey);
        public const string SpatialBoundaryFunctionSupportKey = nameof(SpatialBoundaryFunctionSupportKey);
        public const string SpatialIsRingFunctionSupportKey = nameof(SpatialIsRingFunctionSupportKey);
        public const string SpatialPointOnSurfaceFunctionSupportKey = nameof(SpatialPointOnSurfaceFunctionSupportKey);
        public const string SpatialRelateFunctionSupportKey = nameof(SpatialRelateFunctionSupportKey);
        public const string SpatialSupportFunctionAdditionsSupportKey = nameof(SpatialSupportFunctionAdditionsSupportKey);
        public const string SpatialIsValidFunctionSupportKey = nameof(SpatialIsValidFunctionSupportKey);
        public const string SpatialDistanceSphereFunctionSupportKey = nameof(SpatialDistanceSphereFunctionSupportKey);
        public const string SpatialSetSridFunctionSupportKey = nameof(SpatialSetSridFunctionSupportKey);
        public const string SpatialDistanceFunctionImplementsAndoyerSupportKey = nameof(SpatialDistanceFunctionImplementsAndoyerSupportKey);
        public const string SpatialGeographicSupportKey = nameof(SpatialGeographicSupportKey);
        public const string ExceptInterceptSupportKey = nameof(ExceptInterceptSupportKey);
        public const string ExceptInterceptPrecedenceSupportKey = nameof(ExceptInterceptPrecedenceSupportKey);
        public const string JsonDataTypeEmulationSupportKey = nameof(JsonDataTypeEmulationSupportKey);
        public const string ImplicitBoolCheckUsesIndexSupportKey = nameof(ImplicitBoolCheckUsesIndexSupportKey);

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
            { OuterReferenceInMultiLevelSubquerySupportKey, new ServerVersionSupport(OuterReferenceInMultiLevelSubqueryMySqlSupportVersionString/*, OuterReferenceInMultiLevelSubqueryMariaDbSupportVersionString*/) },
            { DoubleCastSupportKey, new ServerVersionSupport(DoubleCastMySqlSupportVersionString, DoubleCastMariaDbSupportVersionString) },
            { JsonSupportKey, new ServerVersionSupport(JsonMySqlSupportVersionString/*, JsonMariaDbSupportVersionString*/) },
            { GeneratedColumnsSupportKey, new ServerVersionSupport(GeneratedColumnsMySqlSupportVersionString, GeneratedColumnsMariaDbSupportVersionString) },
            { NullableGeneratedColumnsSupportKey, new ServerVersionSupport(NullableGeneratedColumnsMySqlSupportVersionString/*, NullableGeneratedColumnsMariaDbSupportVersionString*/) },
            { DefaultCharSetUtf8Mb4SupportKey, new ServerVersionSupport(DefaultCharSetUtf8Mb4MySqlSupportVersionString/*, DefaultCharSetUtf8Mb4MariaDbSupportVersionString*/) },
            { DefaultExpressionSupportKey, new ServerVersionSupport(DefaultExpressionMySqlSupportVersionString/*, DefaultExpressionMariaDbSupportVersionString*/) },
            { AlternativeDefaultExpressionSupportKey, new ServerVersionSupport(/*AlternativeDefaultExpressionMySqlSupportVersionString, */AlternativeDefaultExpressionMariaDbSupportVersionString) },
            { SpatialReferenceSystemRestrictedColumnsSupportKey, new ServerVersionSupport(SpatialReferenceSystemRestrictedColumnsMySqlSupportVersionString/*, SpatialReferenceSystemRestrictedColumnsMariaDbSupportVersionString*/)},
            { SpatialFunctionAdditionsSupportKey, new ServerVersionSupport(/*SpatialFunctionAdditionsMySqlSupportVersionString, */SpatialFunctionAdditionsMariaDbSupportVersionString)},
            { SpatialBoundaryFunctionSupportKey, new ServerVersionSupport(/*SpatialBoundaryFunctionMySqlSupportVersionString, */SpatialBoundaryFunctionMariaDbSupportVersionString)},
            { SpatialIsRingFunctionSupportKey, new ServerVersionSupport(/*SpatialIsRingFunctionMySqlSupportVersionString, */SpatialIsRingFunctionMariaDbSupportVersionString)},
            { SpatialPointOnSurfaceFunctionSupportKey, new ServerVersionSupport(/*SpatialPointOnSurfaceFunctionMySqlSupportVersionString, */SpatialPointOnSurfaceFunctionMariaDbSupportVersionString)},
            { SpatialRelateFunctionSupportKey, new ServerVersionSupport(/*SpatialRelateFunctionMySqlSupportVersionString, */SpatialRelateFunctionMariaDbSupportVersionString)},
            { SpatialSupportFunctionAdditionsSupportKey, new ServerVersionSupport(SpatialSupportFunctionAdditionsMySqlSupportVersionString/*, SpatialSupportFunctionAdditionsMariaDbSupportVersionString*/)},
            { SpatialIsValidFunctionSupportKey, new ServerVersionSupport(SpatialIsValidFunctionMySqlSupportVersionString/*, SpatialIsValidFunctionMariaDbSupportVersionString*/)},
            { SpatialDistanceSphereFunctionSupportKey, new ServerVersionSupport(SpatialDistanceSphereFunctionMySqlSupportVersionString/*, SpatialDistanceSphereFunctionMariaDbSupportVersionString*/)},
            { SpatialSetSridFunctionSupportKey, new ServerVersionSupport(SpatialSetSridFunctionMySqlSupportVersionString/*, SpatialSetSridFunctionMariaDbSupportVersionString*/)},
            { SpatialDistanceFunctionImplementsAndoyerSupportKey, new ServerVersionSupport(SpatialDistanceFunctionImplementsAndoyerMySqlSupportVersionString/*, SpatialDistanceFunctionImplementsAndoyerMariaDbSupportVersionString*/)},
            { SpatialGeographicSupportKey, new ServerVersionSupport(SpatialGeographicMySqlSupportVersionString/*, SpatialGeographicMariaDbSupportVersionString*/)},
            { ExceptInterceptSupportKey, new ServerVersionSupport(/*ExceptInterceptMySqlSupportVersionString, */ExceptInterceptMariaDbSupportVersionString)},
            { ExceptInterceptPrecedenceSupportKey, new ServerVersionSupport(/*ExceptInterceptPrecedenceMySqlSupportVersionString, */ExceptInterceptPrecedenceMariaDbSupportVersionString)},
            { JsonDataTypeEmulationSupportKey, new ServerVersionSupport(/*JsonDataTypeEmulationMySqlSupportVersionString, */JsonDataTypeEmulationMariaDbSupportVersionString)},
            { ImplicitBoolCheckUsesIndexSupportKey, new ServerVersionSupport(ImplicitBoolCheckUsesIndexMySqlSupportVersionString, ImplicitBoolCheckUsesIndexMariaDbSupportVersionString)},
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
        public virtual bool SupportsOuterReferenceInMultiLevelSubquery => SupportMap[OuterReferenceInMultiLevelSubquerySupportKey].IsSupported(this);
        public virtual bool SupportsJson => SupportMap[JsonSupportKey].IsSupported(this);
        public virtual bool SupportsGeneratedColumns => SupportMap[GeneratedColumnsSupportKey].IsSupported(this);
        public virtual bool SupportsNullableGeneratedColumns => SupportMap[NullableGeneratedColumnsSupportKey].IsSupported(this);
        public virtual bool SupportsDefaultCharSetUtf8Mb4 => SupportMap[DefaultCharSetUtf8Mb4SupportKey].IsSupported(this);
        public virtual bool SupportsDefaultExpression => SupportMap[DefaultExpressionSupportKey].IsSupported(this);
        public virtual bool SupportsAlternativeDefaultExpression => SupportMap[AlternativeDefaultExpressionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialReferenceSystemRestrictedColumns => SupportMap[SpatialReferenceSystemRestrictedColumnsSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialFunctionAdditions => SupportMap[SpatialFunctionAdditionsSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialBoundaryFunction => SupportMap[SpatialBoundaryFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialIsRingFunction => SupportMap[SpatialIsRingFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialPointOnSurfaceFunction => SupportMap[SpatialPointOnSurfaceFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialRelateFunction => SupportMap[SpatialRelateFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialSupportFunctionAdditions => SupportMap[SpatialSupportFunctionAdditionsSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialIsValidFunction => SupportMap[SpatialIsValidFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialDistanceSphereFunction => SupportMap[SpatialDistanceSphereFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialSetSridFunction => SupportMap[SpatialSetSridFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialDistanceFunctionImplementsAndoyer => SupportMap[SpatialSetSridFunctionSupportKey].IsSupported(this);
        public virtual bool SupportsSpatialGeographic => SupportMap[SpatialGeographicSupportKey].IsSupported(this);
        public virtual bool SupportsExceptIntercept => SupportMap[ExceptInterceptSupportKey].IsSupported(this);
        public virtual bool SupportsExceptInterceptPrecedence => SupportMap[ExceptInterceptPrecedenceSupportKey].IsSupported(this);
        public virtual bool SupportsJsonDataTypeEmulation => SupportMap[JsonDataTypeEmulationSupportKey].IsSupported(this);
        public virtual bool SupportsImplicitBoolCheckUsesIndex => SupportMap[ImplicitBoolCheckUsesIndexSupportKey].IsSupported(this);

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
