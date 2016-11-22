using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class MySqlScaffoldingModelFactory : RelationalScaffoldingModelFactory
    {
        public MySqlScaffoldingModelFactory(
            ILoggerFactory loggerFactory,
            IRelationalTypeMapper typeMapper,
            IDatabaseModelFactory databaseModelFactory,
            CandidateNamingService candidateNamingService)
            : base(loggerFactory, typeMapper, databaseModelFactory, candidateNamingService)
        {
        }

        public override IModel Create(string connectionString, TableSelectionSet tableSelectionSet)
        {
            var model = base.Create(connectionString, tableSelectionSet);
            model.Scaffolding().UseProviderMethodName = nameof(MySqlDbContextOptionsExtensions.UseMySql);
            return model;
        }

        protected override KeyBuilder VisitPrimaryKey(EntityTypeBuilder builder, TableModel table)
        {
            var keyBuilder = base.VisitPrimaryKey(builder, table);

            if (keyBuilder == null)
            {
                return null;
            }

            // If this property is the single integer primary key on the EntityType then
            // KeyConvention assumes ValueGeneratedOnAdd(). If the underlying column does
            // not have Serial set then we need to set to ValueGeneratedNever() to
            // override this behavior.

            // TODO use KeyConvention directly to detect when it will be applied
            var pkColumns = table.Columns.Where(c => c.PrimaryKeyOrdinal.HasValue).ToList();
            if (pkColumns.Count != 1 || pkColumns[0].MySql().IsSerial)
            {
                return keyBuilder;
            }

            // TODO 
            var property = builder.Metadata.FindProperty(GetPropertyName(pkColumns[0]));
            var propertyType = property?.ClrType?.UnwrapNullableType();

            if ((propertyType?.IsIntegerForSerial() == true || propertyType == typeof(Guid)) &&
                !pkColumns[0].ValueGenerated.HasValue)
            {
                property.ValueGenerated = ValueGenerated.Never;
            }

            return keyBuilder;
        }
        
        protected override IndexBuilder VisitIndex(EntityTypeBuilder builder, IndexModel index)
        {
            var expression = index.MySql().Expression;
            if (expression != null)
            {
                Logger.LogWarning($"Ignoring unsupported index {index.Name} which contains an expression ({expression})");
                return null;
            }

            return base.VisitIndex(builder, index);
        }
    }
}
