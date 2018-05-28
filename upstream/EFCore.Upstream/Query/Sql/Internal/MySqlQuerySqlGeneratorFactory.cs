// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Sql.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlQuerySqlGeneratorFactory : QuerySqlGeneratorFactoryBase
    {
        private readonly IMySqlOptions _mySqlOptions;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlQuerySqlGeneratorFactory(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [NotNull] IMySqlOptions mySqlOptions)
            : base(dependencies)
        {
            _mySqlOptions = mySqlOptions;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
            => new MySqlQuerySqlGenerator(
                Dependencies,
                Check.NotNull(selectExpression, nameof(selectExpression)),
                _mySqlOptions.RowNumberPagingEnabled);
    }
}
