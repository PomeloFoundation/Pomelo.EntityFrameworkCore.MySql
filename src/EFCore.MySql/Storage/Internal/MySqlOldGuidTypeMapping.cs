// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    internal sealed class MySqlOldGuidTypeMapping : GuidTypeMapping
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MySqlOldGuidTypeMapping" /> class.
        /// </summary>
        public MySqlOldGuidTypeMapping() : base("binary(16)", System.Data.DbType.Guid) { }

        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected sealed override string GenerateNonNullSqlLiteral([CanBeNull] object value) => ByteArrayFormatter.ToHex(((Guid)value).ToByteArray());
    }
}
