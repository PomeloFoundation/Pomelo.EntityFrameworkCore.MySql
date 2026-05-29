// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microting.EntityFrameworkCore.MySql.Infrastructure.Internal
{
    /// <summary>
    /// Helper class to provide version-agnostic patterns for EF Core compatibility.
    /// This class helps manage breaking changes between EF Core versions, particularly
    /// for migration to EF Core 10 where ExecuteUpdate API changes from Expression to Action.
    /// </summary>
    public static class EFCoreCompatibilityHelper
    {
        private static readonly Lazy<Version> _efCoreVersion = new(() => GetEFCoreVersion());
        
        /// <summary>
        /// Gets the current EF Core version.
        /// </summary>
        public static Version EFCoreVersion => _efCoreVersion.Value;
        
        /// <summary>
        /// Indicates if the current EF Core version is 10.0 or greater.
        /// </summary>
        public static bool IsEFCore10OrGreater => EFCoreVersion.Major >= 10;
        
        /// <summary>
        /// Indicates if the current EF Core version is 9.0 or greater.
        /// </summary>
        public static bool IsEFCore9OrGreater => EFCoreVersion.Major >= 9;
        
        /// <summary>
        /// Indicates if the current EF Core version is 8.0 or greater.
        /// </summary>
        public static bool IsEFCore8OrGreater => EFCoreVersion.Major >= 8;

        /// <summary>
        /// Creates a compatible ExecuteUpdate expression/action based on the EF Core version.
        /// In EF Core 10, ExecuteUpdate changed from Expression-based to Action-based API.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="setterExpression">The setter expression for pre-EF Core 10</param>
        /// <param name="setterAction">The setter action for EF Core 10+</param>
        /// <returns>The appropriate setter for the current EF Core version</returns>
        public static object CreateExecuteUpdateSetter<TEntity>(
            Expression<Func<TEntity, TEntity>> setterExpression = null,
            Action<TEntity> setterAction = null)
        {
            // In a real implementation, this would detect the EF Core version
            // and return the appropriate type. For now, we prepare the infrastructure.
            
            if (IsEFCore10OrGreater)
            {
                // EF Core 10+ uses Action<T>
                return setterAction ?? throw new ArgumentNullException(nameof(setterAction), 
                    "Action-based setter is required for EF Core 10+");
            }
            else
            {
                // EF Core 9 and earlier use Expression<Func<T, T>>
                return setterExpression ?? throw new ArgumentNullException(nameof(setterExpression), 
                    "Expression-based setter is required for EF Core 9 and earlier");
            }
        }

        /// <summary>
        /// Provides an example pattern for handling ExecuteUpdate API differences.
        /// This method demonstrates how to handle the breaking change in EF Core 10
        /// where ExecuteUpdate moved from Expression-based to Action-based setters.
        /// </summary>
        public static class ExecuteUpdatePatterns
        {
            /// <summary>
            /// Example: Update a single property with version-specific handling
            /// </summary>
            public static string GetUpdateSinglePropertyPattern()
            {
                return @"
// EF Core 9 and earlier pattern:
#if EFCORE9_OR_EARLIER
    await context.MyEntities
        .Where(e => e.Id == targetId)
        .ExecuteUpdateAsync(setters => setters.SetProperty(e => e.Name, newName));
#endif

// EF Core 10+ pattern:
#if EFCORE10_OR_GREATER
    await context.MyEntities
        .Where(e => e.Id == targetId)
        .ExecuteUpdateAsync(e => e.Name = newName);
#endif
";
            }

            /// <summary>
            /// Example: Update multiple properties with version-specific handling
            /// </summary>
            public static string GetUpdateMultiplePropertiesPattern()
            {
                return @"
// EF Core 9 and earlier pattern:
#if EFCORE9_OR_EARLIER
    await context.MyEntities
        .Where(e => e.Active)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(e => e.LastModified, DateTime.UtcNow)
            .SetProperty(e => e.Status, ""Updated""));
#endif

// EF Core 10+ pattern:
#if EFCORE10_OR_GREATER
    await context.MyEntities
        .Where(e => e.Active)
        .ExecuteUpdateAsync(e => new MyEntity
        {
            LastModified = DateTime.UtcNow,
            Status = ""Updated""
        });
#endif
";
            }
        }

        private static Version GetEFCoreVersion()
        {
            try
            {
                // Try to get version from Microsoft.EntityFrameworkCore assembly
                var efCoreAssembly = Assembly.Load("Microsoft.EntityFrameworkCore");
                var version = efCoreAssembly.GetName().Version;
                return version ?? new Version(8, 0, 0); // Fallback to 8.0.0
            }
            catch
            {
                // Fallback if assembly cannot be loaded
                return new Version(8, 0, 0);
            }
        }
    }
}