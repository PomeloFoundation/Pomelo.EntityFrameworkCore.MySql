// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDesignTimeProviderServicesTest : DesignTimeProviderServicesTest
    {
        protected override Assembly GetRuntimeAssembly()
            => typeof(MySqlConnection).GetTypeInfo().Assembly;

        protected override Type GetDesignTimeServicesType()
            => typeof(MySqlDesignTimeServices);
    }
}
