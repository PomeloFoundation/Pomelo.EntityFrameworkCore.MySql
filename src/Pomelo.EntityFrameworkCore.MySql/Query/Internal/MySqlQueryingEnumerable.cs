using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage.Internal;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore.Query.Internal
{

	/// Wraps an QueryingEnumerable in an enumerable that opens a RepeatableRead transaction when querying with Includes
	/// This way every statement within the Includes gets a consistent snapshot of the database
	public class MySqlQueryingEnumerable<T> : IEnumerable<T>
	{
		private readonly MySqlQueryContext _queryContext;
		private readonly IEnumerable<T> _source;

		public MySqlQueryingEnumerable(MySqlQueryContext queryContext, IEnumerable<T> source)
		{
			_queryContext = queryContext;
			_source = source;
		}

		public IEnumerator<T> GetEnumerator()
			=> new MySqlEnumerator(_queryContext, _source.GetEnumerator());

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private sealed class MySqlEnumerator : IEnumerator<T>
		{
			private readonly IEnumerator<T> _enumerator;
			private readonly MySqlQueryContext _queryContext;
			private MySqlRelationalTransaction _transaction;
			private bool _tryInitTransaction;

			public MySqlEnumerator(MySqlQueryContext queryContext, IEnumerator<T> enumerator)
			{
				_queryContext = queryContext;
				_enumerator = enumerator;
			}

			public bool MoveNext()
			{
				if (!_tryInitTransaction)
				{
					if (_queryContext.HasInclude && _queryContext.Connection.CurrentTransaction == null)
					{
						_transaction = _queryContext.Connection.BeginTransaction(IsolationLevel.RepeatableRead) as MySqlRelationalTransaction;
					}
					_tryInitTransaction = true;
				}
				if (!_enumerator.MoveNext())
				{
					_transaction?.Commit();
					return false;
				}
				return true;
			}

			public void Reset() => _enumerator.Reset();

			public T Current => _enumerator.Current;

			object IEnumerator.Current => _enumerator.Current;

			public void Dispose()
			{
				_enumerator.Dispose();
				_transaction?.Dispose();
			}
		}

	}
}
