// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Utilities;

[DebuggerStepThrough]
internal static class Check
{
    [ContractAnnotation("value:null => halt")]
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    public static T NotNull<T>(
        [NoEnumeration, AllowNull, System.Diagnostics.CodeAnalysis.NotNull] T value,
        [InvokerParameterName, CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        if (value is null)
        {
            ThrowArgumentNull(parameterName);
        }

        return value;
    }

    [ContractAnnotation("value:null => halt")]
    public static IReadOnlyList<T> NotEmpty<T>(
        [System.Diagnostics.CodeAnalysis.NotNull] IReadOnlyList<T>? value,
        [InvokerParameterName, CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        NotNull(value, parameterName);

        if (value.Count == 0)
        {
            ThrowNotEmpty(parameterName);
        }

        return value;
    }

    [ContractAnnotation("value:null => halt")]
    public static string NotEmpty(
        [System.Diagnostics.CodeAnalysis.NotNull] string? value,
        [InvokerParameterName, CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        NotNull(value, parameterName);

        if (value.AsSpan().Trim().Length == 0)
        {
            ThrowStringArgumentEmpty(parameterName);
        }

        return value;
    }

    public static string? NullButNotEmpty(
        string? value,
        [InvokerParameterName, CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        if (value is not null && value.Length == 0)
        {
            ThrowStringArgumentEmpty(parameterName);
        }

        return value;
    }

    public static IReadOnlyList<T> HasNoNulls<T>(
        [System.Diagnostics.CodeAnalysis.NotNull] IReadOnlyList<T>? value,
        [InvokerParameterName, CallerArgumentExpression(nameof(value))] string parameterName = "")
        where T : class
    {
        NotNull(value, parameterName);

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < value.Count; i++)
        {
            if (value[i] is null)
            {
                ThrowArgumentException(parameterName, parameterName);
            }
        }

        return value;
    }

    public static IReadOnlyList<string> HasNoEmptyElements(
        [System.Diagnostics.CodeAnalysis.NotNull] IReadOnlyList<string>? value,
        [InvokerParameterName, CallerArgumentExpression(nameof(value))] string parameterName = "")
    {
        NotNull(value, parameterName);

        for (var i = 0; i < value.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(value[i]))
            {
                ThrowCollectionHasEmptyElements(parameterName);
            }
        }

        return value;
    }

    public static TEnum? EnumValue<TEnum>(
        TEnum? value,
        [InvokerParameterName] string parameterName)
        where TEnum : struct
    {
        NotNull(value, parameterName);

        return NullOrEnumValue(value, parameterName);
    }

    public static TEnum? NullOrEnumValue<TEnum>(
        TEnum? value,
        [InvokerParameterName] string parameterName)
        where TEnum : struct
    {
        if (value is not null)
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new ArgumentOutOfRangeException(parameterName, value, null);
            }
        }

        return value;
    }

    [Conditional("DEBUG")]
    public static void DebugAssert(
        [DoesNotReturnIf(false)] bool condition,
        [CallerArgumentExpression(nameof(condition))] string message = "")
    {
        if (!condition)
        {
            throw new UnreachableException($"Check.DebugAssert failed: {message}");
        }
    }

    [Conditional("DEBUG"), DoesNotReturn]
    public static void DebugFail(string message)
        => throw new UnreachableException($"Check.DebugFail failed: {message}");

    [DoesNotReturn]
    private static void ThrowArgumentNull(string parameterName)
        => throw new ArgumentNullException(parameterName);

    [DoesNotReturn]
    private static void ThrowNotEmpty(string parameterName)
        => throw new ArgumentException(AbstractionsStrings.CollectionArgumentIsEmpty, parameterName);

    [DoesNotReturn]
    private static void ThrowStringArgumentEmpty(string parameterName)
        => throw new ArgumentException(AbstractionsStrings.ArgumentIsEmpty, parameterName);

    [DoesNotReturn]
    private static void ThrowCollectionHasEmptyElements(string parameterName)
        => throw new ArgumentException(AbstractionsStrings.CollectionArgumentHasEmptyElements, parameterName);

    [DoesNotReturn]
    private static void ThrowArgumentException(string message, string parameterName)
        => throw new ArgumentException(message, parameterName);
}
