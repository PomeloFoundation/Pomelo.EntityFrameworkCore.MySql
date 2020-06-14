// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal
{
    public class MySqlQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;
        private readonly IMySqlOptions _options;

        public MySqlQuerySqlGeneratorFactory(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            ISqlExpressionFactory sqlExpressionFactory,
            IMySqlOptions options)
        {
            _dependencies = dependencies;
            _sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            _options = options;
        }

        public virtual QuerySqlGenerator Create()
            => new MySqlQuerySqlGenerator(_dependencies, _sqlExpressionFactory, _options);
    }
}
