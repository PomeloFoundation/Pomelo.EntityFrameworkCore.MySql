using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Translations;
using Microsoft.EntityFrameworkCore.Types;
using Microsoft.EntityFrameworkCore.Update;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlComplianceTest : RelationalComplianceTestBase
    {
        // TODO: Implement remaining 3.x tests.
        protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>
        {
            // There are two classes that can lead to a MySqlEndOfStreamException, if *both* test classes are included in the run:
            //     - RelationalModelBuilderTest.RelationalComplexTypeTestBase
            //     - RelationalModelBuilderTest.RelationalOwnedTypesTestBase
            //
            // The exception is thrown for MySQL most of the time, though in rare cases also for MariaDB.
            // We disable `RelationalModelBuilderTest.RelationalOwnedTypesTestBase` for now.

            // typeof(RelationalModelBuilderTest.RelationalNonRelationshipTestBase),
            // typeof(RelationalModelBuilderTest.RelationalComplexTypeTestBase),
            // typeof(RelationalModelBuilderTest.RelationalInheritanceTestBase),
            // typeof(RelationalModelBuilderTest.RelationalOneToManyTestBase),
            // typeof(RelationalModelBuilderTest.RelationalManyToOneTestBase),
            // typeof(RelationalModelBuilderTest.RelationalOneToOneTestBase),
            // typeof(RelationalModelBuilderTest.RelationalManyToManyTestBase),
            typeof(RelationalModelBuilderTest.RelationalOwnedTypesTestBase),
            typeof(ModelBuilderTest.OwnedTypesTestBase), // base class of RelationalModelBuilderTest.RelationalOwnedTypesTestBase

            typeof(UdfDbFunctionTestBase<>),
            typeof(TransactionInterceptionTestBase),
            typeof(CommandInterceptionTestBase),
            typeof(NorthwindQueryTaggingQueryTestBase<>),

            // TODO: 9.0
            typeof(AdHocComplexTypeQueryTestBase),
            typeof(AdHocPrecompiledQueryRelationalTestBase),
            typeof(PrecompiledQueryRelationalTestBase),
            typeof(PrecompiledSqlPregenerationQueryRelationalTestBase),

            // TODO: Reenable LoggingMySqlTest once its issue has been fixed in EF Core upstream.
            typeof(LoggingTestBase),
            typeof(LoggingRelationalTestBase<,>),

            // We have our own JSON support for now
            typeof(AdHocJsonQueryTestBase),
            typeof(JsonQueryRelationalTestBase<>),
            typeof(JsonQueryTestBase<>),
            typeof(JsonTypesRelationalTestBase),
            typeof(JsonTypesTestBase),
            typeof(JsonUpdateTestBase<>),
            typeof(OptionalDependentQueryTestBase<>),

            // TODO: EF Core 10 added new specification test suites (JSON-based Associations [provider has its own JSON
            // support], ComplexProperties, dedicated Translations/Temporal/Operators/Type suites). These are deferred;
            // the relational Associations suites (Navigations, OwnedNavigations, *TableSplitting) ARE implemented.
            typeof(Microsoft.EntityFrameworkCore.LazyLoadProxyRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.ModelBuilding.ModelBuilderTest.ComplexCollectionTestBase),
            typeof(Microsoft.EntityFrameworkCore.ModelBuilding.RelationalModelBuilderTest.RelationalComplexCollectionTestBase),
            typeof(Microsoft.EntityFrameworkCore.Query.AdHocJsonQueryRelationalTestBase),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonBulkUpdateRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonSetOperationsRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesSetOperationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonBulkUpdateRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.MiscellaneousTranslationsRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Operators.ArithmeticOperatorTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Operators.BitwiseOperatorTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Operators.ComparisonOperatorTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Operators.LogicalOperatorTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Operators.MiscellaneousOperatorTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.StringTranslationsRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.StringTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Temporal.DateOnlyTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Temporal.DateTimeOffsetTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Temporal.DateTimeTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Temporal.TimeOnlyTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Translations.Temporal.TimeSpanTranslationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Types.RelationalTypeTestBase<,>),
            typeof(Microsoft.EntityFrameworkCore.Update.ComplexCollectionJsonUpdateTestBase<>),

            // TODO: EF Core 10 split type/translation specification tests into dedicated base classes. The underlying
            // translations are already exercised by the existing Northwind* query tests; implementing these dedicated
            // suites is pending.
            typeof(TypeTestBase<,>),
            typeof(ByteArrayTranslationsTestBase<>),
            typeof(EnumTranslationsTestBase<>),
            typeof(GuidTranslationsTestBase<>),
            typeof(MathTranslationsTestBase<>),
            typeof(MiscellaneousTranslationsTestBase<>),
        };

        protected override Assembly TargetAssembly { get; } = typeof(MySqlComplianceTest).Assembly;
    }
}
