using System;
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
        /// Checks if <paramref name="json"/> contains <paramref name="candidate"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a JSON DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="candidate">
        /// A JSON column or value. Can be a JSON DOM object, a string, or a user POCO mapped to JSON.
        /// </param>
        public static bool JsonContains(
            [CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] object candidate)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContains)));

        /// <summary>
        /// Checks if <paramref name="json"/> contains <paramref name="candidate"/> at a specific <paramref name="path"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="candidate">
        /// A JSON column or value. Can be a JSON DOM object, a string, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="path">
        /// A string containing a valid JSON path (staring with `$`).
        /// </param>
        public static bool JsonContains(
            [CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] object candidate, [CanBeNull] string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContains)));

        /// <summary>
        /// Checks if <paramref name="path"/> exists within <paramref name="json"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="path">A path to be checked inside <paramref name="json"/>.</param>
        public static bool JsonContainsPath([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContainsPath)));

        /// <summary>
        /// Checks if any of the given <paramref name="paths"/> exist within <paramref name="json"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="paths">A set of paths to be checked inside <paramref name="json"/>.</param>
        public static bool JsonContainsPathAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] params string[] paths)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContainsPathAny)));

        /// <summary>
        /// Checks if all of the given <paramref name="paths"/> exist within <paramref name="json"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="paths">A set of paths to be checked inside <paramref name="json"/>.</param>
        public static bool JsonContainsPathAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] params string[] paths)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonContainsPathAll)));

        /// <summary>
        /// Returns the type of the outermost JSON value as a text string.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <returns> The JSON type as a text string. </returns>
        /// <remarks> For possible return values see: https://dev.mysql.com/doc/refman/8.0/en/json-attribute-functions.html#function_json-type </remarks>
        public static string JsonType([CanBeNull] this DbFunctions _, [NotNull] object json)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonType)));

        /// <summary>
        /// Checks if <paramref name="json"/> contains <paramref name="searchString"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a JSON DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="searchString">
        /// The string to search for.
        /// </param>
        public static bool JsonSearchAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAny)));

        /// <summary>
        /// Checks if <paramref name="json"/> contains <paramref name="searchString"/> under <paramref name="path"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a JSON DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="searchString">
        /// The string to search for.
        /// </param>
        /// <param name="path">
        /// A string containing a valid JSON path (staring with `$`).
        /// </param>
        public static bool JsonSearchAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAny)));

        /// <summary>
        /// Checks if <paramref name="json"/> contains <paramref name="searchString"/> under <paramref name="path"/>.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a JSON DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="searchString">
        /// The string to search for.
        /// </param>
        /// <param name="path">
        /// A string containing a valid JSON path (staring with `$`).
        /// </param>
        /// <param name="escapeChar">
        /// Can be `null`, an empty string or a one character wide string used for escaping characters in <paramref name="searchString"/>.
        /// </param>
        public static bool JsonSearchAny([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path, string escapeChar)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAny)));

        // These methods make no sense as long as they only return true or false, because they would return
        // the same result as JsonSearchAny would.

        [Obsolete("Use 'JsonSearchAny()' instead.")]
        public static bool JsonSearchAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAll)));

        [Obsolete("Use 'JsonSearchAny()' instead.")]
        public static bool JsonSearchAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAll)));

        [Obsolete("Use 'JsonSearchAny()' instead.")]
        public static bool JsonSearchAll([CanBeNull] this DbFunctions _, [NotNull] object json, [NotNull] string searchString, string path, string escapeChar)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonSearchAll)));
    }
}
