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
            const string assertSqlPattern = @"\s*AssertSql\(\s*@"".*?""\);\r?\n";

            var queryBaselineFilePath = args[0];
            var testFilePath = args[1];

            File.WriteAllText(
                testFilePath,
                Regex.Matches(
                        File.ReadAllText(queryBaselineFilePath),
                        $@"(?:^|\n)(?<Name>(?:Pomelo|EntityFrameworkCore|Microsoft)[^\r\n]*)\([^\r\n]*\) : (?<Line>\d+)\r?\n(?<AssertSql>{assertSqlPattern})\r?\n(?<Truncated>Output truncated.)?\r?\n--------------------(?=\r?\n)",
                        RegexOptions.IgnoreCase | RegexOptions.Singleline)
                    .Select(
                        match => new
                        {
                            Line = int.Parse(match.Groups["Line"].Value),
                            Name = match.Groups["Name"].Value,
                            AssertSql = match.Groups["AssertSql"].Value,
                            Truncated = match.Groups["Truncated"].Success,
                        })
                    .GroupBy(t => t.Line)
                    .Select(g => g.First())
                    .OrderByDescending(t => t.Line)
                    .Aggregate(
                        File.ReadAllText(testFilePath),
                        (current, next) =>
                        {
                            var lines = Regex.Split(current, @"\r?\n");
                            var before = lines.Take(next.Line - 1);
                            var remaining = lines.Skip(next.Line - 1);
                            var replaced = Regex.Split(
                                Regex.Replace(
                                    string.Join(Environment.NewLine, remaining),
                                    $"^{assertSqlPattern}",
                                    next.AssertSql,
                                    RegexOptions.IgnoreCase | RegexOptions.Singleline),
                                @"\r?\n")
                                .AsEnumerable();

                            if (next.Truncated)
                            {
                                replaced = replaced.Prepend("            #warning TRUNCATED: Add remaining baseline queries.");
                            }

                            return string.Join(Environment.NewLine, before.Concat(replaced));
                        }));
        }
    }
}
