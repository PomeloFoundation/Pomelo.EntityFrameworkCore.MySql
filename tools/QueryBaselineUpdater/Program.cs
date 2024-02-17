using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryBaselineUpdater
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string assertSqlPattern = @"\s*Assert(?:ExecuteUpdate)?Sql\(\s*(?:(?:@?(""""""|"")).*?\1)?\);\r?\n";

            var queryBaselineFilePath = args[0];
            var testFileBasePath = args[1];

            if (!Directory.Exists(testFileBasePath))
            {
                throw new ArgumentException($"Path '{testFileBasePath}' does not exist or is not a directory.");
            }

            Regex.Matches(
                    File.ReadAllText(queryBaselineFilePath),
                    $@"(?:^|\n)(?<Name>(?:Pomelo|EntityFrameworkCore|Microsoft)[^\r\n]*)\([^\r\n]*\) : (?<Line>\d+)\r?\n(?<AssertSql>{assertSqlPattern})\r?\n(?<Truncated>Output truncated.)?\r?\n--------------------(?=\r?\n)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline)
                .Select(
                    match => new
                    {
                        File = Path.Combine(testFileBasePath,
                            Regex.Replace(
                                Regex.Replace(
                                    Regex.Replace(
                                        Regex.Replace(match.Groups["Name"].Value, @"^Pomelo\.EntityFrameworkCore\.MySql\.FunctionalTests\.",
                                            string.Empty), @"\.[^.]+$", string.Empty), @"\.", @"\"), "^.*$", "$0.cs")),
                        Line = int.Parse(match.Groups["Line"].Value),
                        Name = match.Groups["Name"].Value,
                        AssertSql = match.Groups["AssertSql"].Value,
                        Truncated = match.Groups["Truncated"].Success,
                    })
                .GroupBy(m => m.File)
                .ToList()
                .ForEach(
                    file => File.WriteAllText(
                        file.First().File,
                        file.GroupBy(t => t.Line)
                            .Select(g => g.First())
                            .OrderByDescending(t => t.Line)
                            .Aggregate(
                                File.ReadAllText(file.First().File),
                                (result, current) =>
                                {
                                    var lines = Regex.Split(result, @"\r?\n");
                                    var before = lines.Take(current.Line - 1);
                                    var remaining = lines.Skip(current.Line - 1);
                                    var replaced = Regex.Split(
                                            Regex.Replace(
                                                string.Join(Environment.NewLine, remaining),
                                                $"^{assertSqlPattern}",
                                                current.AssertSql,
                                                RegexOptions.IgnoreCase | RegexOptions.Singleline),
                                            @"\r?\n")
                                        .AsEnumerable();

                                    if (current.Truncated)
                                    {
                                        replaced = replaced.Prepend("            #warning TRUNCATED: Add remaining baseline queries.");
                                    }

                                    return string.Join(Environment.NewLine, before.Concat(replaced));
                                })));
        }
    }
}
