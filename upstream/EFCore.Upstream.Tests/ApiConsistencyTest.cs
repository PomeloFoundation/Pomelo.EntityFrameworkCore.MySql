// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public class ApiConsistencyTest : ApiConsistencyTestBase
    {
        private static readonly Type[] _fluentApiTypes =
        {
            typeof(MySqlDbContextOptionsBuilder),
            typeof(MySqlDbContextOptionsExtensions),
            typeof(MySqlIndexBuilderExtensions),
            typeof(MySqlKeyBuilderExtensions),
            typeof(MySqlMetadataExtensions),
            typeof(MySqlModelBuilderExtensions),
            typeof(MySqlPropertyBuilderExtensions),
            typeof(MySqlReferenceOwnershipBuilderExtensions),
            typeof(MySqlServiceCollectionExtensions),
            typeof(MySqlEntityTypeBuilderExtensions)
        };

        protected override IEnumerable<Type> FluentApiTypes => _fluentApiTypes;

        protected override void AddServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkMySql();
        }

        protected override Assembly TargetAssembly => typeof(MySqlConnection).GetTypeInfo().Assembly;
    }
}
