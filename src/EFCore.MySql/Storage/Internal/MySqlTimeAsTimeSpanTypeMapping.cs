// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    ///     Maps the MySQL "time" store type to <see cref="TimeSpan" />.
    /// </summary>
    public class MySqlTimeAsTimeSpanTypeMapping : MySqlTimeTypeMapping<TimeSpan>
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static MySqlTimeAsTimeSpanTypeMapping Default { get; } = new("time");

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlTimeAsTimeSpanTypeMapping(
            [NotNull] string storeType,
            int? precision = null,
            bool isDefaultValueCompatible = false)
            : base(storeType, JsonTimeSpanReaderWriter.Instance, precision, isDefaultValueCompatible)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected MySqlTimeAsTimeSpanTypeMapping(RelationalTypeMappingParameters parameters, bool isDefaultValueCompatible)
            : base(parameters, isDefaultValueCompatible)
        {
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlTimeAsTimeSpanTypeMapping(parameters, IsDefaultValueCompatible);

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="isDefaultValueCompatible"> Use a default value compatible syntax, or not. </param>
        /// <returns> The newly created mapping. </returns>
        public override RelationalTypeMapping Clone(bool isDefaultValueCompatible = false)
            => new MySqlTimeAsTimeSpanTypeMapping(Parameters, isDefaultValueCompatible);
    }
}
