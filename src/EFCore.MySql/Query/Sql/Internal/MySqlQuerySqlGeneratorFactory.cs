// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Sql.Internal
{
    public class MySqlQuerySqlGenerationHelperFactory : QuerySqlGeneratorFactoryBase
    {
        
        public MySqlQuerySqlGenerationHelperFactory(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper,
            [NotNull] IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] IRelationalTypeMapper relationalTypeMapper)
            : base(Check.NotNull(commandBuilderFactory, nameof(commandBuilderFactory)),
                Check.NotNull(sqlGenerationHelper, nameof(sqlGenerationHelper)),
                Check.NotNull(parameterNameGeneratorFactory, nameof(parameterNameGeneratorFactory)),
                Check.NotNull(relationalTypeMapper, nameof(relationalTypeMapper)))

        {
           
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
            => new MySqlQuerySqlGenerator(
                CommandBuilderFactory,
                SqlGenerationHelper,
                ParameterNameGeneratorFactory,
                RelationalTypeMapper,
                Check.NotNull(selectExpression, nameof(selectExpression)));
        
    }
}
