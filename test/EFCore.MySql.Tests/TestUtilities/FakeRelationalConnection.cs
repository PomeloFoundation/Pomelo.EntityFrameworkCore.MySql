using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities
{
    public class FakeRelationalConnection : IRelationalConnection
    {
        public IDbContextTransaction BeginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new System.NotImplementedException();
        }

        IDbContextTransaction IRelationalConnection.CurrentTransaction
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Open()
        {
            throw new System.NotImplementedException();
        }

        public Task OpenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public string ConnectionString { get; }
        public DbConnection DbConnection { get; }
        public int? CommandTimeout { get; set; }
        public bool IsMultipleActiveResultSetsEnabled { get; }
        public IValueBufferCursor ActiveCursor { get; set; }

        IDbContextTransaction IDbContextTransactionManager.CurrentTransaction
        {
            get { throw new System.NotImplementedException(); }
        }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public IDbContextTransaction UseTransaction(DbTransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
