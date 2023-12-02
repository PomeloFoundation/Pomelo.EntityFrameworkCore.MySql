// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public interface IMySqlConnectionStringOptionsValidator
{
    void EnsureMandatoryOptions([NotNull] ref string connectionString, IDbContextOptions options = null);
    void EnsureMandatoryOptions([NotNull] DbConnection connection, IDbContextOptions options = null);
    void EnsureMandatoryOptions([NotNull] DbDataSource dataSource);

    void ThrowOnInvalidMandatoryOptions(
        string connectionString,
        DbConnection connection,
        DbDataSource dataSource);
}
