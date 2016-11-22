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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using MySql.Data.MySqlClient;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
    public class MySqlIndexModelAnnotations
    {
        private readonly IndexModel _index;

        public MySqlIndexModelAnnotations(/* [NotNull] */ IndexModel index)
        {
            // Check.NotNull(index, nameof(index));

            _index = index;
        }

        /// <summary>
        /// If the index contains an expression (rather than simple column references), the expression is contained here.
        /// This is currently unsupported and will be ignored.
        /// </summary>
        public string Expression
        {
            get { return _index[MySqlDatabaseModelAnnotationNames.Expression] as string; }
            /* [param: CanBeNull] */
            set { _index[MySqlDatabaseModelAnnotationNames.Expression] = value; }
        }
    }
}
