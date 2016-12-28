// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Expressions.Internal
{
    public class DateAddExpression : Expression
    {
        public DateAddExpression([NotNull] string datePart, [NotNull] Type type, [NotNull] IEnumerable<Expression> arguments)
        {
            DatePart = datePart;
            Type = type;
            Arguments = arguments;
        }

        public override Type Type { get; }

        public override ExpressionType NodeType => ExpressionType.Call;

        public virtual IEnumerable<Expression> Arguments { get; }

        public virtual string DatePart { get; }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var specificVisitor = visitor as MySqlQuerySqlGenerator;

            return specificVisitor != null
                ? specificVisitor.VisitDateAdd(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;
    }
}
