using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ModelBuilding;
using Microsoft.EntityFrameworkCore.Query;
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
            typeof(AdHocJsonQueryRelationalTestBase),
            typeof(JsonQueryRelationalTestBase<>),
            typeof(JsonQueryTestBase<>),
            typeof(JsonTypesRelationalTestBase),
            typeof(JsonTypesTestBase),
            typeof(JsonUpdateTestBase<>),
            typeof(OptionalDependentQueryTestBase<>),

            // TODO: 10.0 — New test bases introduced in EF Core 10 that need MySQL implementations.
            // Associations suite (many new test bases for query testing)
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsBulkUpdateTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsMiscellaneousTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsPrimitiveCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsProjectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsSetOperationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.AssociationsStructuralEqualityTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesBulkUpdateTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesMiscellaneousTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesPrimitiveCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesProjectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesSetOperationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties.ComplexPropertiesStructuralEqualityTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsIncludeTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsMiscellaneousTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsPrimitiveCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsProjectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsSetOperationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsStructuralEqualityTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsMiscellaneousTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsPrimitiveCollectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsProjectionTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsSetOperationsTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsStructuralEqualityTestBase<>),

            // Associations relational variants
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonBulkUpdateRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonSetOperationsRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson.ComplexJsonStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting.ComplexTableSplittingBulkUpdateRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting.ComplexTableSplittingMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting.ComplexTableSplittingPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting.ComplexTableSplittingProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.ComplexTableSplitting.ComplexTableSplittingStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsIncludeRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsSetOperationsRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.Navigations.NavigationsStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonBulkUpdateRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedJson.OwnedJsonStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsSetOperationsRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedNavigations.OwnedNavigationsStructuralEqualityRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedTableSplitting.OwnedTableSplittingMiscellaneousRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedTableSplitting.OwnedTableSplittingPrimitiveCollectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedTableSplitting.OwnedTableSplittingProjectionRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Query.Associations.OwnedTableSplitting.OwnedTableSplittingStructuralEqualityRelationalTestBase<>),

            // Other new 10.0 test bases
            typeof(Microsoft.EntityFrameworkCore.ComplexTypesTrackingRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.LazyLoadProxyRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.PropertyValuesRelationalTestBase<>),
            typeof(Microsoft.EntityFrameworkCore.Types.TypeTestBase<,>),
            typeof(Microsoft.EntityFrameworkCore.Types.RelationalTypeTestBase<,>),
            typeof(Microsoft.EntityFrameworkCore.Update.ComplexCollectionJsonUpdateTestBase<>),
            typeof(ModelBuilderTest.ComplexCollectionTestBase),
            typeof(RelationalModelBuilderTest.RelationalComplexCollectionTestBase),
        };

        protected override Assembly TargetAssembly { get; } = typeof(MySqlComplianceTest).Assembly;
    }
}
