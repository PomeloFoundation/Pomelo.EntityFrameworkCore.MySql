// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    /// <summary>
    ///     Options set at the <see cref="IServiceProvider" /> singleton level to control MySql specific options.
    /// </summary>
    public interface IMySqlOptions : ISingletonOptions
    {
        /// <summary>
        ///     Reflects the option set by <see cref="MySqlDbContextOptionsBuilder.UseRowNumberForPaging" />.
        /// </summary>
        bool RowNumberPagingEnabled { get; }
    }
}
