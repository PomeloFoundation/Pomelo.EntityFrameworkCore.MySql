using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlCommandBuilder : RelationalCommandBuilder, IInfrastructure<IndentedStringBuilder>
    {
        private readonly ISensitiveDataLogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        private readonly IndentedStringBuilder _commandTextBuilder = new IndentedStringBuilder();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MySqlCommandBuilder(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] DiagnosticSource diagnosticSource,
            [NotNull] IRelationalTypeMapper typeMapper)
            :base(logger, diagnosticSource, typeMapper)
        {
            _logger = logger;
            _diagnosticSource = diagnosticSource;
            ParameterBuilder = new RelationalParameterBuilder(typeMapper);
        }

        IndentedStringBuilder IInfrastructure<IndentedStringBuilder>.Instance
            => _commandTextBuilder;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override IRelationalParameterBuilder ParameterBuilder { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override IRelationalCommand Build()
            => new MySqlRelationalCommand(
                _logger,
                _diagnosticSource,
                _commandTextBuilder.ToString(),
                ParameterBuilder.Parameters);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override string ToString() => _commandTextBuilder.ToString();
    }
}