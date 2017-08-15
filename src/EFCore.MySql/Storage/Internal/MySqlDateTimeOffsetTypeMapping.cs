// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     <para>
    ///         Represents the mapping between a .NET <see cref="DateTimeOffset" /> type and a database type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class MySqlDateTimeOffsetTypeMapping : RelationalTypeMapping
    {
        private const string DateTimeOffsetFormatConst6 = @"{0:yyyy-MM-dd HH\:mm\:ss.ffffff}";
        private const string DateTimeOffsetFormatConst = @"{0:yyyy-MM-dd HH\:mm\:ss}";
        private readonly string _storeType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DateTimeOffsetTypeMapping" /> class.
        /// </summary>
        /// <param name="storeType"> The name of the database type. </param>
        /// <param name="dbType"> The <see cref="DbType" /> to be used. </param>
        public MySqlDateTimeOffsetTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] DbType? dbType = null)
            : base(storeType, typeof(DateTimeOffset), dbType, unicode: false, size: null)
        {
            _storeType = storeType;
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="storeType"> The name of the database type. </param>
        /// <param name="size"> The size of data the property is configured to store, or null if no size is configured. </param>
        /// <returns> The newly created mapping. </returns>
        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new MySqlDateTimeOffsetTypeMapping(
                storeType,
                DbType);

        /// <summary>
        ///     Gets the string format to be used to generate SQL literals of this type.
        /// </summary>
        protected override string SqlLiteralFormatString => "'" + (_storeType.EndsWith("(6)") ? DateTimeOffsetFormatConst6 : DateTimeOffsetFormatConst) + "'";
    }
}
