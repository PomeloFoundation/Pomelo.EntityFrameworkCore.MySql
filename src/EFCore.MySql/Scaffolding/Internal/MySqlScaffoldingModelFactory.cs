// using Microsoft.EntityFrameworkCore.Metadata;
// using Microsoft.EntityFrameworkCore.Storage;
// using Microsoft.Extensions.Logging;

// namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
// {
//     public class MySqlScaffoldingModelFactory : RelationalScaffoldingModelFactory
//     {
//         private readonly MySqlDesignTimeScopedTypeMapper _typeMapper;

//         public MySqlScaffoldingModelFactory(
//             ILoggerFactory loggerFactory,
//             IRelationalTypeMapper typeMapper,
//             IDatabaseModelFactory databaseModelFactory,
//             CandidateNamingService candidateNamingService)
//             : base(loggerFactory, typeMapper, databaseModelFactory, candidateNamingService)
//         {
//             _typeMapper = typeMapper as MySqlDesignTimeScopedTypeMapper;
//         }

//         public override IModel Create(string connectionString, TableSelectionSet tableSelectionSet)
//         {
//             _typeMapper.ConnectionString = connectionString;
//             var model = base.Create(connectionString, tableSelectionSet);
//             model.Scaffolding().UseProviderMethodName = nameof(MySqlDbContextOptionsExtensions.UseMySql);
//             return model;
//         }
//     }
// }
