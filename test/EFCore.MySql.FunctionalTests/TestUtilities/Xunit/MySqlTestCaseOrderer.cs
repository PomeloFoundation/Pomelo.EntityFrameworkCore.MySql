using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities.Xunit
{
    public class MySqlTestCaseOrderer : ITestCaseOrderer
    {
#if SPECIFIC_TEST_ORDER
        private static readonly bool _isSpecificTestCaseOrderingEnabled = true;
#else
        private static readonly bool _isSpecificTestCaseOrderingEnabled = false;
#endif

        private static string[] _specificTestCaseDisplayNamesInOrder;
        public static string[] SpecificTestCaseDisplayNamesInOrder
            => _specificTestCaseDisplayNamesInOrder ??= _isSpecificTestCaseOrderingEnabled &&
                                                        Path.GetFullPath(@"..\..\..\TestResults\SpecificTestOrder.txt") is var path &&
                                                        File.Exists(path)
                ? File.ReadLines(path)
                    .Select(s => Regex.Match(s, @"^(?:\W*)([^\u200B]+)").Groups[1].Value)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .ToArray()
                : [];

        private readonly Dictionary<string, int> _specificTestCaseDisplayNamesWithIndex;

        public MySqlTestCaseOrderer()
        {
            _specificTestCaseDisplayNamesWithIndex = SpecificTestCaseDisplayNamesInOrder
                .Select((s, i) => (s, i))
                .ToDictionary(t => t.s, t => t.i);
        }

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            var orderedTestCases = testCases
                .OrderBy(
                    c => _specificTestCaseDisplayNamesWithIndex.TryGetValue(c.DisplayName, out var index) &&
                         index > -1
                        ? index
                        : int.MaxValue)
                .ThenBy(c => c.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(c => c.DisplayName, StringComparer.Ordinal)
                .ThenBy(c => c.UniqueID)
                .ToArray();

            return orderedTestCases;
        }
    }
}
