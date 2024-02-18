using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryBaselineUpdater
{
    internal static class Program
    {
        private const string AssertSqlPattern = @"\s*Assert(?:ExecuteUpdate)?Sql\(\s*(?:(?:@?(""""""|"")).*?\1)?\);\r?\n";

        private static int Main(string[] args)
        {
            var queryBaselineFilePath = args[0];
            var testFileBasePath = args[1];

            if (!Directory.Exists(testFileBasePath))
            {
                throw new ArgumentException($"Path '{testFileBasePath}' does not exist or is not a directory.");
            }

            var notFound = new List<string>();

            foreach (var file in Regex.Matches(
                             File.ReadAllText(queryBaselineFilePath),
                             $@"(?:^|\n)(?<Id>(?<Name>(?:Pomelo|EntityFrameworkCore|Microsoft)[^\r\n]*)\([^\r\n]*\) : (?<Line>\d+))\r?\n(?<AssertSql>{AssertSqlPattern})\r?\n(?<Truncated>Output truncated.)?\r?\n--------------------(?=\r?\n)",
                             RegexOptions.IgnoreCase | RegexOptions.Singleline)
                         .Select(
                             match => new AssertSqlChunk(
                                 match.Groups["Id"].Value,
                                 Path.Combine(
                                     testFileBasePath,
                                     Regex.Replace(
                                         Regex.Replace(
                                             Regex.Replace(
                                                 Regex.Replace(
                                                     match.Groups["Name"].Value,
                                                     @"^Pomelo\.EntityFrameworkCore\.MySql\.FunctionalTests\.",
                                                     string.Empty),
                                                 @"\.[^.]+$",
                                                 string.Empty),
                                             @"\.",
                                             @"\"),
                                         "^.*$",
                                         "$0.cs")),
                                 int.Parse(match.Groups["Line"].Value),
                                 match.Groups["Name"].Value,
                                 match.Groups["AssertSql"].Value,
                                 match.Groups["Truncated"].Success))
                         .GroupBy(
                             c => c.File,
                             (_, cc) => cc
                                 .GroupBy(t => t.Line)
                                 .Select(g => g.First())
                                 .OrderByDescending(t => t.Line)
                                 .ToArray()))
            {
                var filePath = file.First().File;

                // If we didn't find the chunk in the original file, it is possible that the test class is partial and that the chunk exists
                // in a separate file that has the name `<ClassName>.MySql.cs`.
                var retryCustomized = new List<string>();

                File.WriteAllText(
                    filePath,
                    file.Aggregate(
                            File.ReadAllText(filePath),
                            (result, current) => ReplaceChunk(result, current, retryCustomized)));

                if (!retryCustomized.Any())
                {
                    continue;
                }

                var customizedFilePath = Path.Combine(
                    Path.GetDirectoryName(filePath) ?? string.Empty,
                    $"{Path.GetFileNameWithoutExtension(filePath)}.MySql{Path.GetExtension(filePath)}");

                if (File.Exists(customizedFilePath))
                {
                    File.WriteAllText(
                        customizedFilePath,
                        file.Join(
                                retryCustomized,
                                f => f.Id,
                                s => s,
                                (inner, _) => inner)
                            .OrderByDescending(t => t.Line)
                            .Aggregate(
                                File.ReadAllText(customizedFilePath),
                                (result, current) => ReplaceChunk(result, current, notFound)));
                }
                else
                {
                    notFound.AddRange(retryCustomized);
                }
            }

            if (notFound.Any())
            {
                Console.WriteLine("The following chunks where not found:");
                Console.WriteLine();

                foreach (var id in notFound)
                {
                    Console.WriteLine(id);
                }

                return -1;
            }

            return 0;
        }

        private static string ReplaceChunk(string result, AssertSqlChunk current, List<string> notFound)
        {
            var lines = Regex.Split(result, @"\r?\n");
            var before = lines.Take(current.Line - 1);
            var remaining = lines.Skip(current.Line - 1);
            var inputText = string.Join(Environment.NewLine, remaining);
            var replacedResult = Regex.Replace(
                inputText,
                $"^{AssertSqlPattern}",
                current.AssertSql,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (inputText == replacedResult)
            {
                notFound.Add(current.Id);
                return result;
            }

            var splitReplaced = Regex.Split(
                    replacedResult,
                    @"\r?\n")
                .AsEnumerable();

            if (current.Truncated)
            {
                splitReplaced = splitReplaced.Prepend("            #warning TRUNCATED: Add remaining baseline queries.");
            }

            return string.Join(Environment.NewLine, before.Concat(splitReplaced));
        }
    }

    public record AssertSqlChunk(string Id, string File, int Line, string Name, string AssertSql, bool Truncated);
}
