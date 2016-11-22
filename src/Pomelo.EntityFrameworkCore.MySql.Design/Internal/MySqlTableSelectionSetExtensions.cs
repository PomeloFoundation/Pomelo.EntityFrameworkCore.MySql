#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The MySql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Linq;
using JetBrains.Annotations;

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
