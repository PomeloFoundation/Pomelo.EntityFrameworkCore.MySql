using System;

//ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    internal static class MySqlTableSelectionSetExtensions
    {
        public static bool Allows(this TableSelectionSet _tableSelectionSet, /* [NotNull] */ string schemaName, /* [NotNull] */ string tableName)
        {
            if (_tableSelectionSet == null
                || (_tableSelectionSet.Schemas.Count == 0
                && _tableSelectionSet.Tables.Count == 0))
            {
                return true;
            }

            var result = false;

            foreach (var schemaSelection in _tableSelectionSet.Schemas)
                if (EqualsWithQuotes(schemaSelection.Text, schemaName))
                {
                    schemaSelection.IsMatched = true;
                    result = true;
                }

            foreach (var tableSelection in _tableSelectionSet.Tables)
            {
                var components = tableSelection.Text.Split('.');
                if (components.Length == 1
                    ? EqualsWithQuotes(components[0], tableName)
                    : EqualsWithQuotes(components[0], schemaName) && EqualsWithQuotes(components[1], tableName))
                {
                    tableSelection.IsMatched = true;
                    result = true;
                }
            }

            return result;
        }

        static bool EqualsWithQuotes(string expr, string name) =>
            expr[0] == '"' && expr[expr.Length - 1] == '"'
                ? expr.Substring(0, expr.Length - 2).Equals(name)
                : expr.Equals(name, StringComparison.OrdinalIgnoreCase);
    }
}
