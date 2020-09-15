// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlCommonJsonChangeTrackingOptionsExtensions
    {
        public static MySqlJsonChangeTrackingOptions ToJsonChangeTrackingOptions(this MySqlCommonJsonChangeTrackingOptions options)
            => options switch
            {
                MySqlCommonJsonChangeTrackingOptions.RootPropertyOnly => MySqlJsonChangeTrackingOptions.CompareRootPropertyOnly,
                MySqlCommonJsonChangeTrackingOptions.FullHierarchyOptimizedFast => MySqlJsonChangeTrackingOptions.CompareStringRootPropertyByEquals |
                                                                                   MySqlJsonChangeTrackingOptions.CompareDomRootPropertyByEquals |
                                                                                   MySqlJsonChangeTrackingOptions.SnapshotCallsDeepClone |
                                                                                   MySqlJsonChangeTrackingOptions.SnapshotCallsClone,
                MySqlCommonJsonChangeTrackingOptions.FullHierarchyOptimizedSemantically => MySqlJsonChangeTrackingOptions.CompareStringRootPropertyByEquals |
                                                                                           MySqlJsonChangeTrackingOptions.CompareDomSemantically |
                                                                                           MySqlJsonChangeTrackingOptions.HashDomSemantiallyOptimized |
                                                                                           MySqlJsonChangeTrackingOptions.SnapshotCallsDeepClone |
                                                                                           MySqlJsonChangeTrackingOptions.SnapshotCallsClone,
                MySqlCommonJsonChangeTrackingOptions.FullHierarchySemantically => MySqlJsonChangeTrackingOptions.None,
                _ => throw new ArgumentOutOfRangeException(nameof(options)),
            };
    }
}
