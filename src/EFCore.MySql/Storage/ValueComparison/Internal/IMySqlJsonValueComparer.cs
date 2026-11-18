// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microting.EntityFrameworkCore.MySql.Storage.Internal;

namespace Microting.EntityFrameworkCore.MySql.Storage.ValueComparison.Internal
{
    public interface IMySqlJsonValueComparer
    {
        ValueComparer Clone(MySqlJsonChangeTrackingOptions options);
    }
}
