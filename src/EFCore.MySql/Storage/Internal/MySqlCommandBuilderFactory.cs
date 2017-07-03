/// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{

    public class MySqlCommandBuilderFactory : IRelationalCommandBuilderFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _logger;
        private readonly IRelationalTypeMapper _typeMapper;

        public MySqlCommandBuilderFactory(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
            [NotNull] IRelationalTypeMapper typeMapper)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(typeMapper, nameof(typeMapper));

            _logger = logger;
            _typeMapper = typeMapper;
        }

        public virtual IRelationalCommandBuilder Create() => CreateCore(_logger, _typeMapper);

        protected virtual IRelationalCommandBuilder CreateCore(
            [NotNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
            [NotNull] IRelationalTypeMapper relationalTypeMapper)
            => new MySqlCommandBuilder(
                logger,
                relationalTypeMapper);
    }
}
