using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlExecutionStrategyFactory : RelationalExecutionStrategyFactory
    {
        public MySqlExecutionStrategyFactory(
            ExecutionStrategyDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override IExecutionStrategy CreateDefaultStrategy(ExecutionStrategyDependencies dependencies)
            => new MySqlExecutionStrategy(dependencies);
    }
}