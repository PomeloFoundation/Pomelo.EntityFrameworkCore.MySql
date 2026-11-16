// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         Represents the mapping between the MySQL "date" store type and a .NET type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     Starting with EF Core 11, RelationalTypeMapping.Clone() no longer accepts a CLR type and always preserves the CLR
    ///     type of the instance being cloned. Compiled models are generated as "TypeMappingClass.Default.Clone(...)", so a
    ///     single mapping class can only ever represent the CLR type of its own static Default instance. The "date" store type
    ///     maps to both DateOnly and DateTime, so each CLR type needs its own mapping class with its own Default.
    /// </remarks>
    public abstract class MySqlDateTypeMapping<T> : RelationalTypeMapping<T>, IDefaultValueCompatibilityAware
    {
        private readonly bool _isDefaultValueCompatible;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected MySqlDateTypeMapping(
            [NotNull] string storeType,
            [NotNull] JsonValueReaderWriter jsonValueReaderWriter,
            bool isDefaultValueCompatible)
            : this(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        typeof(T),
                        jsonValueReaderWriter: jsonValueReaderWriter),
                    storeType,
                    dbType: System.Data.DbType.Date),
                isDefaultValueCompatible)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlDateTypeMapping(RelationalTypeMappingParameters parameters, bool isDefaultValueCompatible)
            : base(parameters)
        {
            _isDefaultValueCompatible = isDefaultValueCompatible;
        }

        /// <summary>
        ///     Whether this mapping generates literals using a default value compatible syntax.
        /// </summary>
        protected virtual bool IsDefaultValueCompatible => _isDefaultValueCompatible;

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="isDefaultValueCompatible"> Use a default value compatible syntax, or not. </param>
        /// <returns> The newly created mapping. </returns>
        public abstract RelationalTypeMapping Clone(bool isDefaultValueCompatible = false);

        /// <summary>
        ///     Gets the string format to be used to generate SQL literals of this type.
        /// </summary>
        protected override string SqlLiteralFormatString => $@"{(_isDefaultValueCompatible ? null : "DATE ")}'{{0:yyyy-MM-dd}}'";
    }
}
