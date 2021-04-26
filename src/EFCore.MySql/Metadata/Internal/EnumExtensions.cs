// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Internal
{
    public static class ObjectToEnumConverter
    {
        /// <summary>
        /// Can be used to allow substitution of enum values with their underlying type in annotations, so that multi-provider models can
        /// be setup without provider specific dependencies.
        /// </summary>
        /// <remarks>
        /// See https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1205 for further information.
        /// </remarks>
        public static T? GetEnumValue<T>(object value)
            where T : struct
            => value != null &&
               Enum.IsDefined(typeof(T), value)
                ? (T?)(T)Enum.ToObject(typeof(T), value)
                : null;
    }
}
