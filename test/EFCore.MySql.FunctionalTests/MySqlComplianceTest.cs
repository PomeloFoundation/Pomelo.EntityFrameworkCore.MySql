using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Associations;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting;
using Microsoft.EntityFrameworkCore.Query.Associations.Navigations;
using Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson;
using Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations;
using Microsoft.EntityFrameworkCore.Query.Associations.OwnedTableSplitting;
using Microsoft.EntityFrameworkCore.Query.Translations;
using Microsoft.EntityFrameworkCore.Query.Translations.Operators;
using Microsoft.EntityFrameworkCore.Query.Translations.Temporal;
using Microsoft.EntityFrameworkCore.Types;
using Microsoft.EntityFrameworkCore.Update;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
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
            
            // TODO: 10.0.2 - AdHocComplexTypeQueryRelationalTestBase requires NonSharedFixture infrastructure
            typeof(AdHocComplexTypeQueryRelationalTestBase),

            // TODO: Reenable LoggingMySqlTest once its issue has been fixed in EF Core upstream.
            typeof(LoggingTestBase),
            typeof(LoggingRelationalTestBase<,>),

            // We have our own JSON support for now
            // AdHocJsonQueryTestBase and AdHocJsonQueryRelationalTestBase are now enabled (public class)
            // JsonTypesRelationalTestBase is now enabled (public class JsonTypesRelationalMySqlTest)
            typeof(JsonQueryRelationalTestBase<>),
            typeof(JsonQueryTestBase<>),
            typeof(JsonTypesTestBase),
            typeof(JsonUpdateTestBase<>),
            typeof(OptionalDependentQueryTestBase<>),

            // TODO: 10.0 - Type tests
            typeof(TypeTestBase<,>),
            typeof(RelationalTypeTestBase<,>),

            // TODO: 10.0 - Translation tests
            typeof(ByteArrayTranslationsTestBase<>),
            typeof(EnumTranslationsTestBase<>),
            typeof(GuidTranslationsTestBase<>),
            typeof(MathTranslationsTestBase<>),
            typeof(MiscellaneousTranslationsTestBase<>),
            typeof(StringTranslationsTestBase<>),
            typeof(MiscellaneousTranslationsRelationalTestBase<>),
            typeof(StringTranslationsRelationalTestBase<>),

            // TODO: 10.0 - Temporal translation tests
            typeof(DateOnlyTranslationsTestBase<>),
            typeof(DateTimeOffsetTranslationsTestBase<>),
            typeof(DateTimeTranslationsTestBase<>),
            typeof(TimeOnlyTranslationsTestBase<>),
            typeof(TimeSpanTranslationsTestBase<>),

            // TODO: 10.0 - Operator translation tests
            typeof(ArithmeticOperatorTranslationsTestBase<>),
            typeof(BitwiseOperatorTranslationsTestBase<>),
            typeof(ComparisonOperatorTranslationsTestBase<>),
            typeof(LogicalOperatorTranslationsTestBase<>),
            typeof(MiscellaneousOperatorTranslationsTestBase<>),

            // TODO: 10.0 - Association tests
            typeof(AssociationsBulkUpdateTestBase<>),
            typeof(AssociationsCollectionTestBase<>),
            typeof(AssociationsMiscellaneousTestBase<>),
            typeof(AssociationsPrimitiveCollectionTestBase<>),
            typeof(AssociationsProjectionTestBase<>),
            typeof(AssociationsSetOperationsTestBase<>),
            typeof(AssociationsStructuralEqualityTestBase<>),

            // TODO: 10.0 - Owned navigation tests
            typeof(OwnedNavigationsCollectionTestBase<>),
            typeof(OwnedNavigationsMiscellaneousTestBase<>),
            typeof(OwnedNavigationsPrimitiveCollectionTestBase<>),
            typeof(OwnedNavigationsProjectionTestBase<>),
            typeof(OwnedNavigationsSetOperationsTestBase<>),
            typeof(OwnedNavigationsStructuralEqualityTestBase<>),
            typeof(OwnedNavigationsCollectionRelationalTestBase<>),
            typeof(OwnedNavigationsMiscellaneousRelationalTestBase<>),
            typeof(OwnedNavigationsPrimitiveCollectionRelationalTestBase<>),
            typeof(OwnedNavigationsProjectionRelationalTestBase<>),
            typeof(OwnedNavigationsSetOperationsRelationalTestBase<>),
            typeof(OwnedNavigationsStructuralEqualityRelationalTestBase<>),

            // TODO: 10.0 - Navigation tests
            typeof(NavigationsCollectionTestBase<>),
            typeof(NavigationsIncludeTestBase<>),
            typeof(NavigationsMiscellaneousTestBase<>),
            typeof(NavigationsPrimitiveCollectionTestBase<>),
            typeof(NavigationsProjectionTestBase<>),
            typeof(NavigationsSetOperationsTestBase<>),
            typeof(NavigationsStructuralEqualityTestBase<>),
            typeof(NavigationsCollectionRelationalTestBase<>),
            typeof(NavigationsIncludeRelationalTestBase<>),
            typeof(NavigationsMiscellaneousRelationalTestBase<>),
            typeof(NavigationsPrimitiveCollectionRelationalTestBase<>),
            typeof(NavigationsProjectionRelationalTestBase<>),
            typeof(NavigationsSetOperationsRelationalTestBase<>),
            typeof(NavigationsStructuralEqualityRelationalTestBase<>),

            // TODO: 10.0 - Association tests
            typeof(AssociationsBulkUpdateTestBase<>),
            typeof(AssociationsCollectionTestBase<>),
            typeof(AssociationsMiscellaneousTestBase<>),
            typeof(AssociationsPrimitiveCollectionTestBase<>),
            typeof(AssociationsProjectionTestBase<>),
            typeof(AssociationsSetOperationsTestBase<>),
            typeof(AssociationsStructuralEqualityTestBase<>),
            typeof(ComplexTypesTrackingRelationalTestBase<>),
            typeof(LazyLoadProxyRelationalTestBase<>),
            typeof(PropertyValuesRelationalTestBase<>),

            // TODO: 10.0 - Update tests
            typeof(ComplexCollectionJsonUpdateTestBase<>),

            // TODO: 10.0 - Owned table splitting tests
            typeof(OwnedTableSplittingMiscellaneousRelationalTestBase<>),
            typeof(OwnedTableSplittingPrimitiveCollectionRelationalTestBase<>),
            typeof(OwnedTableSplittingProjectionRelationalTestBase<>),
            typeof(OwnedTableSplittingStructuralEqualityRelationalTestBase<>),

            // TODO: 10.0 - Owned JSON tests
            typeof(OwnedJsonBulkUpdateRelationalTestBase<>),
            typeof(OwnedJsonCollectionRelationalTestBase<>),
            typeof(OwnedJsonMiscellaneousRelationalTestBase<>),
            typeof(OwnedJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(OwnedJsonProjectionRelationalTestBase<>),
            typeof(OwnedJsonStructuralEqualityRelationalTestBase<>),

            // TODO: 10.0 - Complex table splitting tests
            typeof(ComplexTableSplittingBulkUpdateRelationalTestBase<>),
            typeof(ComplexTableSplittingMiscellaneousRelationalTestBase<>),
            typeof(ComplexTableSplittingPrimitiveCollectionRelationalTestBase<>),
            typeof(ComplexTableSplittingProjectionRelationalTestBase<>),
            typeof(ComplexTableSplittingStructuralEqualityRelationalTestBase<>),

            // TODO: 10.0 - Complex properties tests
            typeof(ComplexPropertiesBulkUpdateTestBase<>),
            typeof(ComplexPropertiesCollectionTestBase<>),
            typeof(ComplexPropertiesMiscellaneousTestBase<>),
            typeof(ComplexPropertiesPrimitiveCollectionTestBase<>),
            typeof(ComplexPropertiesProjectionTestBase<>),
            typeof(ComplexPropertiesSetOperationsTestBase<>),
            typeof(ComplexPropertiesStructuralEqualityTestBase<>),

            // BadDataJsonDeserialization test is now enabled (public class)

            // Complex JSON tests are now supported for MySQL 5.7.8+ and MariaDB 10.2.4+
            // These tests should use [SupportedServerVersionCondition("Json")] to skip on older versions
        };

        protected override Assembly TargetAssembly { get; } = typeof(MySqlComplianceTest).Assembly;
    }
}
