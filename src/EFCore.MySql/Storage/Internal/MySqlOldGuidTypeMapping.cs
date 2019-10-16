// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlOldGuidTypeMapping : MySqlGuidTypeMapping
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MySqlOldGuidTypeMapping" /> class.
        /// </summary>
        public MySqlOldGuidTypeMapping() : base("binary", System.Data.DbType.Guid, size: 16) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MySqlOldGuidTypeMapping" />
        ///     class from a <see cref="RelationalTypeMappingParameters"/> instance.
        /// </summary>
        /// <param name="parmeters"></param>
        protected MySqlOldGuidTypeMapping(RelationalTypeMappingParameters parameters) : base(parameters) { }

        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected sealed override string GenerateNonNullSqlLiteral([CanBeNull] object value)
            => ByteArrayFormatter.ToHex(((Guid)value).ToByteArray());

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlOldGuidTypeMapping(parameters);
    }
}
