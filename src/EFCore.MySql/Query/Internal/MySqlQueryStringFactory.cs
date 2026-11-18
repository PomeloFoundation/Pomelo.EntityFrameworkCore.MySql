// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microting.EntityFrameworkCore.MySql.Query.Internal
{
    // Based on: System.Data.Jet.JetCommandParser (EntityFrameworkCore.Jet)
    public class MySqlQueryStringFactory : IRelationalQueryStringFactory
    {
        private static readonly Lazy<Regex> _limitExpressionParameterRegex = new Lazy<Regex>(
            () => new Regex(
                $@"(?<=\W)LIMIT\s+(?:(?<leading_offset>@?\w+),\s*)?(?<row_count>(?:LEAST|GREATEST)\s*\([^)]+\)|@?\w+)(?:\s*OFFSET\s*(?<trailing_offset>@?\w+))?",
                RegexOptions.Singleline | RegexOptions.IgnoreCase));

        private static readonly Lazy<Regex> _extractParameterRegex = new Lazy<Regex>(() => new Regex(@"@\w+"));

        private readonly IRelationalTypeMappingSource _typeMapper;

        public MySqlQueryStringFactory([NotNull] IRelationalTypeMappingSource typeMapper)
        {
            _typeMapper = typeMapper;
        }

        public virtual string Create(DbCommand command)
        {
            if (command.Parameters.Count == 0)
            {
                return command.CommandText;
            }

            // For parameter in LIMIT clauses and for string parameters, we need to inline the parameter values directly into the SQL.
            PrepareCommand(command);

            var builder = new StringBuilder();
            foreach (DbParameter parameter in command.Parameters)
            {
                builder
                    .Append("SET ")
                    .Append(parameter.ParameterName)
                    .Append(" = ")
                    .Append(GetParameterValue(parameter))
                    .AppendLine(";");
            }

            return builder
                .AppendLine()
                .Append(command.CommandText).ToString();
        }

        private string GetParameterValue(DbParameter parameter)
        {
            var typeMapping = _typeMapper.FindMapping(parameter.Value.GetType());

            return (parameter.Value == DBNull.Value
                    || parameter.Value == null)
                ? "NULL"
                : typeMapping != null
                    ? typeMapping.GenerateSqlLiteral(parameter.Value)
                    : parameter.Value.ToString();
        }

        private string EvaluateLeastGreatest(string functionCall, DbParameterCollection parameters)
        {
            // Extract function name (LEAST or GREATEST)
            var isLeast = functionCall.StartsWith("LEAST", StringComparison.OrdinalIgnoreCase);
            
            // Extract parameter names from the function call
            var paramMatches = _extractParameterRegex.Value.Matches(functionCall);
            if (paramMatches.Count == 0)
            {
                return functionCall; // No parameters, return as-is
            }

            // Get parameter values and evaluate
            var values = new List<decimal>();
            foreach (Match match in paramMatches)
            {
                var paramName = match.Value;
                if (parameters.Contains(paramName))
                {
                    var param = parameters[paramName];
                    if (param.Value != null && param.Value != DBNull.Value)
                    {
                        try
                        {
                            // Convert to decimal for comparison (handles int, long, decimal, double, float)
                            values.Add(Convert.ToDecimal(param.Value));
                        }
                        catch
                        {
                            // Skip invalid values
                        }
                    }
                }
            }

            if (values.Count == 0)
            {
                return functionCall; // No valid values
            }

            // Compute LEAST or GREATEST
            var result = isLeast ? values.Min() : values.Max();
            return result.ToString();
        }

        protected virtual void PrepareCommand(DbCommand command)
        {
            // MySQL does not support user variables in LIMIT statements.
            // (It does however support parameters in LIMIT statements since 2010. See https://bugs.mysql.com/bug.php?id=11918)
            //
            // Because of that, we need to inline the parameter values as constants into the SQL command, in cases where they appear in a
            // LIMIT clause.
            //
            // Also, the rules for applying collation from user variables are different than the once for parameters.
            // We therefore inline all parameter values of type string as well.

            var stringParameterNames = command.Parameters.Cast<DbParameter>()
                .Where(p => p.Value is string)
                .Select(p => p.ParameterName)
                .ToList();

            if (!command.CommandText.Contains("LIMIT", StringComparison.OrdinalIgnoreCase) &&
                !stringParameterNames.Any())
            {
                return;
            }

            var matches = _limitExpressionParameterRegex.Value.Matches(command.CommandText);
            if (matches.Count <= 0 &&
                !stringParameterNames.Any())
            {
                return;
            }

            var limitGroupsWithParameter = matches.SelectMany(m => m.Groups["row_count"].Captures)
                .Concat(matches.SelectMany(m => m.Groups["leading_offset"].Captures))
                .Concat(matches.SelectMany(m => m.Groups["trailing_offset"].Captures))
                .ToList();

            if (!limitGroupsWithParameter.Any() &&
                !stringParameterNames.Any())
            {
                return;
            }

            var parser = new MySqlCommandParser(command.CommandText);
            var parameterPositions = parser.GetStateIndices('@');
            var parameters = parameterPositions
                .Select(
                    i => new
                    {
                        Index = i,
                        ParameterName = _extractParameterRegex.Value.Match(command.CommandText.Substring(i)).Value,
                    })
                .Where(t => !string.IsNullOrEmpty(t.ParameterName))
                .ToList();

            // Handle LEAST/GREATEST function calls in LIMIT clauses by evaluating them
            // MariaDB 11.6.2 does not support function calls in LIMIT clauses
            var functionsToReplace = limitGroupsWithParameter
                .Where(c => c.Value.StartsWith("LEAST", StringComparison.OrdinalIgnoreCase) || 
                           c.Value.StartsWith("GREATEST", StringComparison.OrdinalIgnoreCase))
                .Select(c => new
                {
                    Index = c.Index,
                    FunctionCall = c.Value,
                    EvaluatedValue = EvaluateLeastGreatest(c.Value, command.Parameters)
                })
                .OrderByDescending(f => f.Index)
                .ToList();

            // Replace LEAST/GREATEST function calls with their evaluated values
            // Use StringBuilder to avoid multiple string allocations
            if (functionsToReplace.Count > 0)
            {
                var sb = new StringBuilder(command.CommandText.Length);
                var lastIndex = 0;
                
                // Process replacements from start to end (list is sorted descending, so reverse it)
                foreach (var func in functionsToReplace.OrderBy(f => f.Index))
                {
                    // Append text from last position to current function
                    sb.Append(command.CommandText, lastIndex, func.Index - lastIndex);
                    
                    // Append the evaluated value
                    sb.Append(func.EvaluatedValue);
                    
                    // Update last index to after the function call
                    lastIndex = func.Index + func.FunctionCall.Length;
                    
                    // Remove the parameters used in this function from the parameters collection
                    var paramMatches = _extractParameterRegex.Value.Matches(func.FunctionCall);
                    foreach (Match match in paramMatches)
                    {
                        var paramName = match.Value;
                        if (command.Parameters.Contains(paramName))
                        {
                            command.Parameters.Remove(command.Parameters[paramName]);
                        }
                    }
                }
                
                // Append remaining text after last replacement
                if (lastIndex < command.CommandText.Length)
                {
                    sb.Append(command.CommandText, lastIndex, command.CommandText.Length - lastIndex);
                }
                
                command.CommandText = sb.ToString();
            }

            var validParameters = (limitGroupsWithParameter
                    .Where(c => !c.Value.StartsWith("LEAST", StringComparison.OrdinalIgnoreCase) && 
                                !c.Value.StartsWith("GREATEST", StringComparison.OrdinalIgnoreCase) &&
                                parameterPositions.Contains(c.Index) &&
                                command.Parameters.Contains(c.Value))
                    .Select(c => new {Index = c.Index, ParameterName = c.Value}))
                .Concat(stringParameterNames.SelectMany(s => parameters.Where(p => p.ParameterName == s)))
                .Distinct()
                .OrderByDescending(c => c.Index)
                .ToList();

            foreach (var validParameter in validParameters)
            {
                var parameterIndex = validParameter.Index;
                var parameterName = validParameter.ParameterName;

                var paramIndex = parameters.FindIndex(
                    t => t.Index == parameterIndex &&
                         t.ParameterName == parameterName);
                
                if (paramIndex >= 0)
                {
                    parameters.RemoveAt(paramIndex);
                }

                var parameter = command.Parameters[parameterName];
                var parameterValue = GetParameterValue(parameter);

                command.CommandText = command.CommandText.Substring(0, parameterIndex) +
                                      parameterValue +
                                      command.CommandText.Substring(parameterIndex + validParameter.ParameterName.Length);
            }

            foreach (var parameterName in validParameters
                .Select(c => c.ParameterName)
                .Distinct()
                .Where(s => parameters.FindIndex(t => t.ParameterName == s) == -1))
            {
                command.Parameters.RemoveAt(parameterName);
            }
        }
    }
}
