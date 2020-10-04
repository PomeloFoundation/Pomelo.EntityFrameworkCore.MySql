// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;

#nullable enable

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    internal static class TypeExtensions
    {
        internal static bool IsGenericList(this Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        internal static bool IsArrayOrGenericList(this Type type)
            => type.IsArray || type.IsGenericList();

        internal static bool TryGetElementType(this Type type, out Type? elementType)
        {
            elementType = type.IsArray
                ? type.GetElementType()
                : type.IsGenericList()
                    ? type.GetGenericArguments()[0]
                    : null;
            return elementType != null;
        }
    }
}
