// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMySqlPropertyAnnotations : IRelationalPropertyAnnotations
    {

        MySqlValueGenerationStrategy? ValueGenerationStrategy { get; }
    }
}
