// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MySqlCodeGenerator : ProviderCodeGenerator
    {
        private static readonly MethodInfo _useMySqlMethodInfo = typeof(MySqlDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(MySqlDbContextOptionsBuilderExtensions.UseMySql),
            typeof(DbContextOptionsBuilder),
            typeof(string),
            typeof(ServerVersion),
            typeof(Action<MySqlDbContextOptionsBuilder>));

        private readonly IMySqlOptions _options;

        public MySqlCodeGenerator(
            [NotNull] ProviderCodeGeneratorDependencies dependencies,
            IMySqlOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override MethodCallCodeFragment GenerateUseProvider(
            string connectionString,
            MethodCallCodeFragment providerOptions)
        {
            // Strip scaffolding specific connection string options first.
            connectionString = new MySqlScaffoldingConnectionSettings(connectionString).GetProviderCompatibleConnectionString();

            return new MethodCallCodeFragment(
                _useMySqlMethodInfo,
                providerOptions == null
                    ? new object[] { connectionString, new MySqlCodeGenerationServerVersionCreation(_options.ServerVersion) }
                    : new object[] { connectionString, new MySqlCodeGenerationServerVersionCreation(_options.ServerVersion), new NestedClosureCodeFragment("x", providerOptions) });
        }
    }
}
