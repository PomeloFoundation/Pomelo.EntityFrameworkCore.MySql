using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure
{
    public class ServerVersionSupportTest
    {
        [Fact]
        public void Latest_MySql_version_feature_support()
        {
            var supports = MySqlServerVersion.LatestSupportedServerVersion.Supports;

            // Supported features:
            Assert.True(supports.DateTimeCurrentTimestamp);
            Assert.True(supports.DateTime6);
            Assert.True(supports.LargerKeyLength);
            Assert.True(supports.RenameIndex);
            Assert.True(supports.RenameColumn);
            Assert.True(supports.WindowFunctions);
            Assert.True(supports.DoubleCast);
            Assert.True(supports.OuterApply);
            Assert.True(supports.CrossApply);
            Assert.True(supports.OuterReferenceInMultiLevelSubquery);
            Assert.True(supports.Json);
            Assert.True(supports.GeneratedColumns);
            Assert.True(supports.NullableGeneratedColumns);
            Assert.True(supports.ParenthesisEnclosedGeneratedColumnExpressions);
            Assert.True(supports.DefaultCharSetUtf8Mb4);
            Assert.True(supports.DefaultExpression);
            Assert.True(supports.SpatialIndexes);
            Assert.True(supports.SpatialReferenceSystemRestrictedColumns);
            Assert.True(supports.SpatialSupportFunctionAdditions);
            Assert.True(supports.SpatialSetSridFunction);
            Assert.True(supports.SpatialDistanceFunctionImplementsAndoyer);
            Assert.True(supports.SpatialDistanceSphereFunction);
            Assert.True(supports.SpatialGeographic);
            Assert.True(supports.ExceptIntercept);
            Assert.True(supports.ExceptInterceptPrecedence);
            Assert.True(supports.ImplicitBoolCheckUsesIndex);
            Assert.True(supports.MySqlBug104294Workaround);
            Assert.True(supports.FullTextParser);
            Assert.True(supports.InformationSchemaCheckConstraintsTable);
            Assert.True(supports.MySqlBugLimit0Offset0ExistsWorkaround);
            Assert.True(supports.DescendingIndexes);
            Assert.True(supports.CommonTableExpressions);
            Assert.True(supports.JsonTable);
            Assert.True(supports.JsonValue);
            Assert.True(supports.JsonOverlaps);
            Assert.True(supports.ValuesWithRows);
            Assert.True(supports.FieldReferenceInTableValueConstructor);
            Assert.True(supports.JsonTableImplementationWithoutMariaDbBugs);
            Assert.True(supports.JsonTableImplementationWithAggregate);

            // Unsupported features (update these assertions when MySQL adds support):
            Assert.False(supports.FloatCast);
            Assert.False(supports.AlternativeDefaultExpression);
            Assert.False(supports.SpatialFunctionAdditions);
            Assert.False(supports.JsonDataTypeEmulation);
            Assert.False(supports.Sequences);
            Assert.False(supports.MySqlBug96947Workaround);
            Assert.False(supports.IdentifyJsonColumsByCheckConstraints);
            Assert.False(supports.Returning);
            Assert.False(supports.LimitWithinInAllAnySomeSubquery);
            Assert.False(supports.LimitWithNonConstantValue);
            Assert.False(supports.Values);
            Assert.False(supports.WhereSubqueryReferencesOuterQuery);
            Assert.False(supports.CollationCharacterSetApplicabilityWithFullCollationNameColumn);
            Assert.False(supports.JsonTableImplementationStable);
            Assert.False(supports.JsonTableImplementationWithoutMySqlBugs);
            Assert.False(supports.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash);
        }

        [Fact]
        public void Latest_MariaDb_version_feature_support()
        {
            var supports = MariaDbServerVersion.LatestSupportedServerVersion.Supports;

            // Supported features:
            Assert.True(supports.DateTimeCurrentTimestamp);
            Assert.True(supports.DateTime6);
            Assert.True(supports.LargerKeyLength);
            Assert.True(supports.RenameIndex);
            Assert.True(supports.RenameColumn);
            Assert.True(supports.WindowFunctions);
            Assert.True(supports.DoubleCast);
            Assert.True(supports.Json);
            Assert.True(supports.GeneratedColumns);
            Assert.True(supports.DefaultCharSetUtf8Mb4);
            Assert.True(supports.AlternativeDefaultExpression);
            Assert.True(supports.SpatialIndexes);
            Assert.True(supports.SpatialFunctionAdditions);
            Assert.True(supports.ExceptIntercept);
            Assert.True(supports.ExceptInterceptPrecedence);
            Assert.True(supports.JsonDataTypeEmulation);
            Assert.True(supports.ImplicitBoolCheckUsesIndex);
            Assert.True(supports.Sequences);
            Assert.True(supports.InformationSchemaCheckConstraintsTable);
            Assert.True(supports.IdentifyJsonColumsByCheckConstraints);
            Assert.True(supports.Returning);
            Assert.True(supports.CommonTableExpressions);
            Assert.True(supports.JsonTable);
            Assert.True(supports.JsonValue);
            Assert.True(supports.JsonOverlaps);
            Assert.True(supports.Values);
            Assert.True(supports.CollationCharacterSetApplicabilityWithFullCollationNameColumn);
            Assert.True(supports.JsonTableImplementationWithoutMySqlBugs);
            Assert.True(supports.JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash);

            // Unsupported features (update these assertions when MariaDB adds support):
            Assert.False(supports.FloatCast);
            Assert.False(supports.OuterApply);
            Assert.False(supports.CrossApply);
            Assert.False(supports.OuterReferenceInMultiLevelSubquery);
            Assert.False(supports.NullableGeneratedColumns);
            Assert.False(supports.ParenthesisEnclosedGeneratedColumnExpressions);
            Assert.False(supports.DefaultExpression);
            Assert.False(supports.SpatialReferenceSystemRestrictedColumns);
            Assert.False(supports.SpatialSupportFunctionAdditions);
            Assert.False(supports.SpatialSetSridFunction);
            Assert.False(supports.SpatialDistanceFunctionImplementsAndoyer);
            Assert.False(supports.SpatialDistanceSphereFunction);
            Assert.False(supports.SpatialGeographic);
            Assert.False(supports.MySqlBug96947Workaround);
            Assert.False(supports.MySqlBug104294Workaround);
            Assert.False(supports.FullTextParser);
            Assert.False(supports.MySqlBugLimit0Offset0ExistsWorkaround);
            Assert.False(supports.DescendingIndexes);
            Assert.False(supports.LimitWithinInAllAnySomeSubquery);
            Assert.False(supports.LimitWithNonConstantValue);
            Assert.False(supports.ValuesWithRows);
            Assert.False(supports.WhereSubqueryReferencesOuterQuery);
            Assert.False(supports.FieldReferenceInTableValueConstructor);
            Assert.False(supports.JsonTableImplementationStable);
            Assert.False(supports.JsonTableImplementationWithoutMariaDbBugs);
            Assert.False(supports.JsonTableImplementationWithAggregate);
        }
    }
}
