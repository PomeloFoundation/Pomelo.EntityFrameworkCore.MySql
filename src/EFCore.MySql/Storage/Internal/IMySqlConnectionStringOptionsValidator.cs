// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;

namespace Microting.EntityFrameworkCore.MySql.Storage.Internal;

public interface IMySqlConnectionStringOptionsValidator
{
    bool EnsureMandatoryOptions(ref string connectionString);
    bool EnsureMandatoryOptions(DbConnection connection);
    bool EnsureMandatoryOptions(DbDataSource dataSource);

    void ThrowException(Exception innerException = null);
}
