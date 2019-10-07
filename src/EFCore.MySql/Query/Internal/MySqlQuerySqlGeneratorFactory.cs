// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly IMySqlConnectionInfo _connectionInfo;

        public MySqlQuerySqlGeneratorFactory(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            IMySqlConnectionInfo connectionInfo)
        {
            _dependencies = dependencies;
            _connectionInfo = connectionInfo;
        }

        public virtual QuerySqlGenerator Create()
            => new MySqlQuerySqlGenerator(_dependencies, _connectionInfo);
    }
}
