﻿using System.Diagnostics;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    [DebuggerStepThrough]
    internal static class MethodInfoExtensions
    {
        internal static bool IsClosedFormOf(
            [NotNull] this MethodInfo methodInfo, [NotNull] MethodInfo genericMethod)
            => methodInfo.IsGenericMethod
               && Equals(
                   methodInfo.GetGenericMethodDefinition(),
                   genericMethod);
    }
}
