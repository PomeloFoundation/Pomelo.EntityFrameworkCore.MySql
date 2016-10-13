// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendJoin(
            this StringBuilder stringBuilder, IEnumerable<string> values, string separator = ", ")
            => stringBuilder.AppendJoin(values, (sb, value) => sb.Append(value), separator);

        public static StringBuilder AppendJoin<T>(
            this StringBuilder stringBuilder, IEnumerable<T> values, Action<StringBuilder, T> joinAction,
            string separator)
        {
            var appended = false;

            foreach (var value in values)
            {
                joinAction(stringBuilder, value);
                stringBuilder.Append(separator);
                appended = true;
            }

            if (appended)
            {
                stringBuilder.Length -= separator.Length;
            }

            return stringBuilder;
        }
    }
}
