// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    class MySqlCreateIndexOperation : CreateIndexOperation
    {
        public virtual bool IsFullText { get; set; }
    }
}
