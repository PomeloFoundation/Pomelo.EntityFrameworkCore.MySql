using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Internal
{

	/// Wraps an AsyncQueryingEnumerable in an enumerable that opens a RepeatableRead transaction when querying with Includes
	/// This way every statement within the Includes gets a consistent snapshot of the database
	public class MySqlAsyncQueryingEnumerable<T> : IAsyncEnumerable<T>
	{
		private readonly MySqlQueryContext _queryContext;
		private readonly IAsyncEnumerable<T> _source;

		public MySqlAsyncQueryingEnumerable(MySqlQueryContext queryContext, IAsyncEnumerable<T> source)
        {
	        _queryContext = queryContext;
	        _source = source;
        }

        public IAsyncEnumerator<T> GetEnumerator()
            => new MySqlAsyncEnumerator(_queryContext, _source.GetEnumerator());

        private sealed class MySqlAsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IAsyncEnumerator<T> _enumerator;
	        private readonly MySqlQueryContext _queryContext;
	        private MySqlRelationalTransaction _transaction;
	        private bool _tryInitTransaction;

	        public MySqlAsyncEnumerator(MySqlQueryContext queryContext, IAsyncEnumerator<T> enumerator)
            {
	            _queryContext = queryContext;
	            _enumerator = enumerator;
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
	            if (!_tryInitTransaction)
	            {
		            if (_queryContext.HasInclude && _queryContext.Connection.CurrentTransaction == null)
		            {
			            _transaction = await _queryContext.Connection.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken).ConfigureAwait(false) as MySqlRelationalTransaction;
		            }
		            _tryInitTransaction = true;
	            }
                if (!await _enumerator.MoveNext(cancellationToken).ConfigureAwait(false))
                {
	                if (_transaction != null)
	                {
		                await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
	                }
                    return false;
                }
                return true;
            }

            public T Current => _enumerator.Current;

	        public void Dispose()
	        {
		        _enumerator.Dispose();
		        _transaction?.Dispose();
	        }
		}

	}
}
