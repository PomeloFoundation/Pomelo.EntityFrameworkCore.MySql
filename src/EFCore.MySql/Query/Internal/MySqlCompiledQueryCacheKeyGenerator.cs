// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    public class MySqlCompiledQueryCacheKeyGenerator : RelationalCompiledQueryCacheKeyGenerator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public MySqlCompiledQueryCacheKeyGenerator(
            [NotNull] CompiledQueryCacheKeyGeneratorDependencies dependencies,
            [NotNull] RelationalCompiledQueryCacheKeyGeneratorDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override object GenerateCacheKey(Expression query, bool async)
        {
            var extensions = RelationalDependencies.ContextOptions.FindExtension<MySqlOptionsExtension>();
            return new MySqlCompiledQueryCacheKey(
                GenerateCacheKeyCore(query, async),
                extensions?.ServerVersion,
                extensions?.NoBackslashEscapes ?? false);
        }

        private readonly struct MySqlCompiledQueryCacheKey
        {
            private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;
            private readonly ServerVersion _serverVersion;
            private readonly bool _noBackslashEscapes;

            public MySqlCompiledQueryCacheKey(
                RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey,
                ServerVersion serverVersion,
                bool noBackslashEscapes)
            {
                _relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
                _serverVersion = serverVersion;
                _noBackslashEscapes = noBackslashEscapes;
            }

            public override bool Equals(object obj)
                => !(obj is null)
                   && obj is MySqlCompiledQueryCacheKey key
                   && Equals(key);

            private bool Equals(MySqlCompiledQueryCacheKey other)
                => _relationalCompiledQueryCacheKey.Equals(other._relationalCompiledQueryCacheKey)
                   && Equals(_serverVersion, other._serverVersion)
                   && _noBackslashEscapes == other._noBackslashEscapes
                ;

            public override int GetHashCode()
                => HashCode.Combine(_relationalCompiledQueryCacheKey,
                    _serverVersion,
                    _noBackslashEscapes);
        }
    }
}
