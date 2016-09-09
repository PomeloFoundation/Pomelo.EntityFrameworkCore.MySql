using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlCommandBuilderFactory : RelationalCommandBuilderFactory
    {
        private readonly ISensitiveDataLogger _logger;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly IRelationalTypeMapper _typeMapper;

        public MySqlCommandBuilderFactory(
            [NotNull] ISensitiveDataLogger<RelationalCommandBuilderFactory> logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] IRelationalTypeMapper typeMapper)
            : base(logger, diagnosticSource, typeMapper)
        {
            _logger = logger;
            _diagnosticSource = diagnosticSource;
            _typeMapper = typeMapper;
        }

        public override IRelationalCommandBuilder Create()
        {
            return new MySqlCommandBuilder(
                _logger,
                _diagnosticSource,
                _typeMapper);
        }

    }
}