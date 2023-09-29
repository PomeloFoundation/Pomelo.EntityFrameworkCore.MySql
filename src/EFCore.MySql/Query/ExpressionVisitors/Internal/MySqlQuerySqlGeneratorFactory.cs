// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly IRelationalTypeMappingSource _typeMappingSource;
        private readonly IMySqlOptions _options;

        public MySqlQuerySqlGeneratorFactory(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            IRelationalTypeMappingSource typeMappingSource,
            IMySqlOptions options)
        {
            _dependencies = dependencies;
            _typeMappingSource = typeMappingSource;
            _options = options;
        }

        public virtual QuerySqlGenerator Create()
            => new MySqlQuerySqlGenerator(_dependencies, _typeMappingSource, _options);
    }
}
