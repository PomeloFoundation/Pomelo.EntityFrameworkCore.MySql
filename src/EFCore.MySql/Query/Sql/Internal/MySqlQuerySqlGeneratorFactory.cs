// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal
{
    public class MySqlQuerySqlGeneratorFactory : QuerySqlGeneratorFactoryBase
    {
        private readonly IMySqlOptions _options;

        public MySqlQuerySqlGeneratorFactory([NotNull] QuerySqlGeneratorDependencies dependencies, IMySqlOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
            => new MySqlQuerySqlGenerator(
                Dependencies,
                Check.NotNull(selectExpression, nameof(selectExpression)), _options);
    }
}
