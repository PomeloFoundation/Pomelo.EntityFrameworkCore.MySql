// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.ValueComparison.Internal
{
    public interface IMySqlJsonValueComparer
    {
        ValueComparer Clone(MySqlJsonChangeTrackingOptions options);
    }
}
