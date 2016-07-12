// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMySqlExtension
    {
        IModel Model { get; }
        string Name { get; }
        string Schema { get; }
        string Version { get; }
    }
}