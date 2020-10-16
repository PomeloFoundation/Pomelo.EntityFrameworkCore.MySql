using System;
using System.Globalization;
using System.Linq;
using System.Text;
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
        /// Quotes a string as a JSON value by wrapping it with double quote characters and escaping interior quote and
        /// other characters, then returning the result as a `utf8mb4` string. Returns `null` if the argument is `null`.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="value">
        /// The string value.
        /// </param>
        public static string JsonQuote(
            [CanBeNull] this DbFunctions _,
            [NotNull] string value)
        {
            var quotedString = new StringBuilder(value.Length * 2 + 2);
            var length = value.Length;

            quotedString.Append('"');

            for (var i = 0; i < length; i++)
            {
                quotedString.Append(
                    value[i] switch
                    {
                        '"' => @"\""",
                        '\b' => @"\b",
                        '\f' => @"\f",
                        '\n' => @"\n",
                        '\r' => @"\r",
                        '\t' => @"\t",
                        '\\' => @"\\",
                        _ => value[i]
                    });
            }

            return quotedString
                .Append('"')
                .ToString();
        }

        /// <summary>
        /// Unquotes JSON value and returns the result as a `utf8mb4` string. Returns `null` if the argument is `null`.
        /// An error occurs if the value starts and ends with double quotes but is not a valid JSON string literal.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        public static string JsonUnquote(
            [CanBeNull] this DbFunctions _,
            [NotNull] object json)
        {
            if (json is string jsonString)
            {
                var length = jsonString.Length;

                if (length < 2 ||
                    jsonString[0] != '"' ||
                    jsonString[length - 1] != '"')
                {
                    return jsonString;
                }

                var unquotedString = new StringBuilder(length - 2);
                var isEscapeSequence = false;
                var unicodeCharNum = -1;
                var unicodeChars = new char[4];

                for (var i = 1; i < length - 1; i++)
                {
                    var c = jsonString[i];

                    if (isEscapeSequence)
                    {
                        if (unicodeCharNum > -1)
                        {
                            if (c >= '0' && c <= '9' ||
                                c >= 'A' && c <= 'F' ||
                                c >= 'a' && c <= 'f')
                            {
                                unicodeChars[unicodeCharNum++] = c;

                                if (unicodeCharNum >= 4)
                                {
                                    var utf8Value = ushort.Parse(new string(unicodeChars), NumberStyles.AllowHexSpecifier);
                                    unquotedString.Append(
                                        Encoding.UTF8.GetChars(
                                            utf8Value <= 255
                                                ? new[] {(byte)utf8Value}
                                                : BitConverter.GetBytes(utf8Value)));
                                    unicodeCharNum = -1;
                                    isEscapeSequence = false;
                                }
                            }
                            else
                            {
                                throw new ArgumentException("The JSON string is not well formed.");
                            }
                        }
                        else if (c == '"')
                        {
                            unquotedString.Append('\"');
                            isEscapeSequence = false;
                        }
                        else if (c == 'b')
                        {
                            unquotedString.Append('\b');
                            isEscapeSequence = false;
                        }
                        else if (c == 'f')
                        {
                            unquotedString.Append('\f');
                            isEscapeSequence = false;
                        }
                        else if (c == 'n')
                        {
                            unquotedString.Append('\n');
                            isEscapeSequence = false;
                        }
                        else if (c == 'r')
                        {
                            unquotedString.Append('\r');
                            isEscapeSequence = false;
                        }
                        else if (c == 't')
                        {
                            unquotedString.Append('\t');
                            isEscapeSequence = false;
                        }
                        else if (c == '\\')
                        {
                            unquotedString.Append('\\');
                            isEscapeSequence = false;
                        }
                        else if (c == 'u' &&
                                 unicodeCharNum == -1)
                        {
                            unicodeCharNum = 0;
                        }
                        else
                        {
                            unquotedString.Append(c);
                            isEscapeSequence = false;
                        }
                    }
                    else if (c == '\\')
                    {
                        isEscapeSequence = true;
                    }
                    else
                    {
                        unquotedString.Append(c);
                    }
                }

                if (isEscapeSequence)
                {
                    throw new ArgumentException("The JSON string is not well formed.");
                }

                return unquotedString.ToString();
            }

            return json.ToString();
        }

        /// <summary>
        /// Returns data from a JSON document, selected from the parts of the document matched by the path arguments.
        /// Returns `null` if any argument is `null` or no paths locate a value in the document. An error occurs if the
        /// `json` argument is not a valid JSON document or any path argument is not a valid path expression.
        /// </summary>
        /// <param name="_">DbFunctions instance</param>
        /// <param name="json">
        /// A JSON column or value. Can be a DOM object, a string property mapped to JSON, or a user POCO mapped to JSON.
        /// </param>
        /// <param name="paths">
        /// A set of paths to extract from <paramref name="json"/>.
        /// </param>
        public static T JsonExtract<T>(
            [CanBeNull] this DbFunctions _,
            [NotNull] object json,
            [NotNull] params string[] paths)
            => throw new InvalidOperationException(MySqlStrings.FunctionOnClient(nameof(JsonExtract)));

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
