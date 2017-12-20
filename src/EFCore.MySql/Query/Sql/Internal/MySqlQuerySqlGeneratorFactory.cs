// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Sql.Internal
{
    public class MySqlQuerySqlGeneratorFactory : QuerySqlGeneratorFactoryBase
    {
        private readonly IMySqlOptions options;

        public MySqlQuerySqlGeneratorFactory([NotNull] QuerySqlGeneratorDependencies dependencies,
                                             [NotNull] IMySqlOptions options)
            : base(dependencies)
        {
            this.options = options;
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
            => new MySqlQuerySqlGenerator(
                Dependencies,
                Check.NotNull(selectExpression, nameof(selectExpression)),
                options);
    }
}
