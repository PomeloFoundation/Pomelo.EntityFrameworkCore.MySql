using System;
using System.Text.Json;
using JetBrains.Annotations;
using Pomelo.EntityFrameworkCore.MySql.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides methods for supporting translation to MySQL JSON operators and functions.
    /// </summary>
    public static class MySqlJsonDbFunctionsExtensions
    {
        /// <summary>
        /// Checks if <paramref name="json"/> contains <paramref name="contained"/> as top-level entries.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string property mapped to JSON,
        /// or a user POCO mapped to JSON.
        /// </param>
        /// <param name="contained">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <remarks>
        /// This operation is only supported with MySQL <c>json</c>, not <c>json</c>.
        ///
        /// See https://www.TODO.org/docs/current/functions-json.html.
        /// </remarks>
        public static bool JsonContains(
            [CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] object candidate)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContains)));

        /// <summary>
        /// Checks if <paramref name="json"/> contains <paramref name="contained"/> as top-level entries.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string property mapped to JSON,
        /// or a user POCO mapped to JSON.
        /// </param>
        /// <param name="contained">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <remarks>
        /// This operation is only supported with MySQL <c>json</c>, not <c>json</c>.
        ///
        /// See https://www.TODO.org/docs/current/functions-json.html.
        /// </remarks>
        public static bool JsonContains(
            [CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] object candidate, [CanBeNull] string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContains)));

        /// <summary>
        /// Checks if <paramref name="key"/> exists as a top-level key within <paramref name="json"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="key">A key to be checked inside <paramref name="json"/>.</param>
        /// <remarks>
        /// This operation is only supported with MySQL <c>json</c>, not <c>json</c>.
        ///
        /// See https://www.TODO.org/docs/current/functions-json.html.
        /// </remarks>
        public static bool JsonContainsPath([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContainsPath)));

        /// <summary>
        /// Checks if any of the given <paramref name="keys"/> exist as top-level keys within <paramref name="json"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="keys">A set of keys to be checked inside <paramref name="json"/>.</param>
        /// <remarks>
        /// This operation is only supported with MySQL <c>json</c>, not <c>json</c>.
        ///
        /// See https://www.TODO.org/docs/current/functions-json.html.
        /// </remarks>
        public static bool JsonContainsPathAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] params string[] paths)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContainsPathAny)));

        /// <summary>
        /// Checks if all of the given <paramref name="keys"/> exist as top-level keys within <paramref name="json"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="keys">A set of keys to be checked inside <paramref name="json"/>.</param>
        /// <remarks>
        /// This operation is only supported with MySQL <c>json</c>, not <c>json</c>.
        ///
        /// See https://www.TODO.org/docs/current/functions-json.html.
        /// </remarks>
        public static bool JsonContainsPathAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] params string[] paths)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContainsPathAll)));

        /// <summary>
        /// Returns the type of the outermost JSON value as a text string.
        /// Possible types are object, array, string, number, boolean, and null.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a <see cref="JsonDocument"/>, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <remarks>
        /// See https://www.TODO.org/docs/current/functions-json.html.
        /// </remarks>
        public static string JsonType([CanBeNull] this DbFunctions _, [NotNull] object json)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonType)));

        // JSON_SEARCH(json_doc, one_or_all, search_str[, escape_char[, path] ...])
        public static bool JsonSearchAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAny)));

        public static bool JsonSearchAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAny)));

        public static bool JsonSearchAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path, string escapeChar)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAny)));

        public static bool JsonSearchAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAll)));

        public static bool JsonSearchAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAll)));

        public static bool JsonSearchAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path, string escapeChar)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAll)));
    }
}
