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
    }
}
