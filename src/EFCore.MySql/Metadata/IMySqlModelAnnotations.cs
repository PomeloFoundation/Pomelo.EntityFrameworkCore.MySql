// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMySqlModelAnnotations : IRelationalModelAnnotations
    {
        MySqlValueGenerationStrategy? ValueGenerationStrategy { get; }
    }
}
