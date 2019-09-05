using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped"/>. This means that each
    ///         <see cref="DbContext"/> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
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
                extensions?.ServerVersion ?? null,
                extensions?.NullableCharSetBehavior ?? null,
                extensions?.AnsiCharSetInfo ?? null,
                extensions?.UnicodeCharSetInfo ?? null,
                extensions?.NoBackslashEscapes ?? false);
        }

        private readonly struct MySqlCompiledQueryCacheKey
        {
            private readonly RelationalCompiledQueryCacheKey _relationalCompiledQueryCacheKey;
            private readonly ServerVersion _serverVersion;
            private readonly CharSetBehavior? _nullableCharSetBehavior;
            private readonly CharSetInfo _ansiCharSetInfo;
            private readonly CharSetInfo _unicodeCharSetInfo;
            private readonly bool _noBackslashEscapes;

            public MySqlCompiledQueryCacheKey(
                RelationalCompiledQueryCacheKey relationalCompiledQueryCacheKey,
                ServerVersion serverVersion,
                CharSetBehavior? nullableCharSetBehavior,
                CharSetInfo ansiCharSetInfo,
                CharSetInfo unicodeCharSetInfo,
                bool noBackslashEscapes)
            {
                _relationalCompiledQueryCacheKey = relationalCompiledQueryCacheKey;
                _serverVersion = serverVersion;
                _nullableCharSetBehavior = nullableCharSetBehavior;
                _ansiCharSetInfo = ansiCharSetInfo;
                _unicodeCharSetInfo = unicodeCharSetInfo;
                _noBackslashEscapes = noBackslashEscapes;
            }

            public override bool Equals(object obj)
                => !(obj is null)
                   && obj is MySqlCompiledQueryCacheKey
                   && Equals((MySqlCompiledQueryCacheKey)obj);

            private bool Equals(MySqlCompiledQueryCacheKey other)
                => _relationalCompiledQueryCacheKey.Equals(other._relationalCompiledQueryCacheKey)
                   && _serverVersion == other._serverVersion
                   && _nullableCharSetBehavior.Value == other._nullableCharSetBehavior.Value
                   && _ansiCharSetInfo == other._ansiCharSetInfo
                   && _unicodeCharSetInfo == other._unicodeCharSetInfo
                   && _noBackslashEscapes == other._noBackslashEscapes
                ;

            public override int GetHashCode()
                => HashCode.Combine(_relationalCompiledQueryCacheKey,
                    _serverVersion,
                    _nullableCharSetBehavior,
                    _ansiCharSetInfo,
                    _unicodeCharSetInfo,
                    _noBackslashEscapes);
        }
    }
}
