// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class MySqlRelationalConnection : IRelationalConnection
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;
        private readonly bool _connectionOwned;
        private int _openedCount;
        private bool _openedInternally;
        private int? _commandTimeout;
	    private readonly ILogger<MySqlRelationalConnection> _logger;

	    public readonly SemaphoreSlim Lock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1);

        public MySqlRelationalConnection([NotNull] IDbContextOptions options, [NotNull] ILogger<MySqlRelationalConnection> logger)
        {
	        Check.NotNull(options, nameof(options));
	        Check.NotNull(logger, nameof(logger));

	        _logger = logger;

	        var relationalOptions = RelationalOptionsExtension.Extract(options);
            _commandTimeout = relationalOptions.CommandTimeout;
            if (relationalOptions.Connection != null)
            {
                if (!string.IsNullOrWhiteSpace(relationalOptions.ConnectionString))
                {
                    throw new InvalidOperationException(RelationalStrings.ConnectionAndConnectionString);
                }
                _connection = relationalOptions.Connection as MySqlConnection;
                _connectionOwned = false;
            }
            else if (!string.IsNullOrWhiteSpace(relationalOptions.ConnectionString))
            {
                _connectionString = relationalOptions.ConnectionString;
                _connectionOwned = true;
            }
            else
            {
                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
            }
        }

	    protected virtual ILogger Logger => _logger;

	    public DbConnection DbConnection => _connection ?? (_connection = new MySqlConnection(ConnectionString));

	    private MySqlConnection MySqlDbConnection => DbConnection as MySqlConnection;

        public MySqlRelationalConnection CreateMasterConnection()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString)
            {
                Database = "mysql",
                Pooling = false
            };
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql(csb.ConnectionString);
            return new MySqlRelationalConnection(optionsBuilder.Options, _logger);
        }

	    private MySqlConnectionStringBuilder _connectionStringBuilder;

	    protected MySqlConnectionStringBuilder ConnectionStringBuilder
		    => _connectionStringBuilder ?? (_connectionStringBuilder = new MySqlConnectionStringBuilder(ConnectionString));

        public string ConnectionString => _connectionString ?? _connection.ConnectionString;

        public IDbContextTransaction CurrentTransaction { get; [param: CanBeNull] protected set; }

        public virtual int? CommandTimeout
        {
            get { return _commandTimeout; }
            set
            {
                if (value.HasValue
                    && (value < 0))
                {
                    throw new ArgumentException(RelationalStrings.InvalidCommandTimeout);
                }

                _commandTimeout = value;
            }
        }

        public IDbContextTransaction BeginTransaction() => BeginTransaction(IsolationLevel.Unspecified);

        public async Task<IDbContextTransaction> BeginTransactionAsync(
                CancellationToken cancellationToken = default(CancellationToken))
            => await BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken).ConfigureAwait(false);

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }
            DoOpen();
            return BeginTransactionWithNoPreconditions(isolationLevel);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }
            await DoOpenAsync(cancellationToken).ConfigureAwait(false);
            return await BeginTransactionWithNoPreconditionsAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        }

        private IDbContextTransaction BeginTransactionWithNoPreconditions(IsolationLevel isolationLevel)
        {
	        Check.NotNull(_logger, nameof(_logger));
	        _logger.LogDebug(
		        RelationalEventId.BeginningTransaction,
		        isolationLevel,
		        il => RelationalStrings.RelationalLoggerBeginningTransaction(il.ToString("G")));

	        // ReSharper disable once AssignNullToNotNullAttribute
	        CurrentTransaction = new MySqlRelationalTransaction(this, MySqlDbConnection.BeginTransaction(isolationLevel) as MySqlTransaction, _logger, true);
	        return CurrentTransaction;
        }

	    private async Task<IDbContextTransaction> BeginTransactionWithNoPreconditionsAsync(
		    IsolationLevel isolationLevel,
		    CancellationToken cancellationToken = default(CancellationToken)
	    )
	    {
		    Check.NotNull(_logger, nameof(_logger));
		    _logger.LogDebug(
			    RelationalEventId.BeginningTransaction,
			    isolationLevel,
			    il => RelationalStrings.RelationalLoggerBeginningTransaction(il.ToString("G")));

		    CurrentTransaction = new MySqlRelationalTransaction(this, await MySqlDbConnection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false), _logger, true);
		    return CurrentTransaction;
	    }

	    public IDbContextTransaction UseTransaction(DbTransaction transaction)
	    {
		    if (transaction == null)
		    {
			    if (CurrentTransaction != null)
			    {
				    CurrentTransaction = null;
				    DoClose();
			    }
		    }
            else
            {
	            var mySqlTransaction = transaction as MySqlTransaction;
	            if (mySqlTransaction == null)
	            {
		            throw new InvalidCastException("transaction must be of the type MySqlTransaction");
	            }
	            if (CurrentTransaction != null)
                {
                    throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
                }
                DoOpen();
                CurrentTransaction = new MySqlRelationalTransaction(this, mySqlTransaction, _logger, false);
            }
            return CurrentTransaction;
        }

        public void CommitTransaction()
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            CurrentTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            CurrentTransaction.Rollback();
        }

	    protected void DoOpen()
	    {
		    _connectionLock.Wait();
		    try
		    {
			    if (_openedCount == 0 && DbConnection.State != ConnectionState.Open)
			    {
				    _logger.LogDebug(
					    RelationalEventId.OpeningConnection,
					    new
					    {
						    _connection.Database,
						    _connection.DataSource
					    },
					    state =>
						    RelationalStrings.RelationalLoggerOpeningConnection(
							    state.Database,
							    state.DataSource));

				    DbConnection.Open();
				    _openedInternally = true;
			    }
			    _openedCount++;
		    }
		    finally
		    {
			    _connectionLock.Release();
		    }
	    }

	    protected async Task DoOpenAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    cancellationToken.ThrowIfCancellationRequested();
		    await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
		    try
		    {
			    if (_openedCount == 0 && DbConnection.State != ConnectionState.Open)
			    {
				    _logger.LogDebug(
					    RelationalEventId.OpeningConnection,
					    new
					    {
						    _connection.Database,
						    _connection.DataSource
					    },
					    state =>
						    RelationalStrings.RelationalLoggerOpeningConnection(
							    state.Database,
							    state.DataSource));

				    await DbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
				    _openedInternally = true;
			    }
			    _openedCount++;
		    }
		    finally
		    {
			    _connectionLock.Release();
		    }
	    }

	    protected void DoClose()
	    {
		    _connectionLock.Wait();
		    try
		    {
			    if (_openedCount > 0 && --_openedCount == 0 && _openedInternally)
			    {
				    _logger.LogDebug(
					    RelationalEventId.ClosingConnection,
					    new
					    {
						    _connection.Database,
						    _connection.DataSource
					    },
					    state =>
						    RelationalStrings.RelationalLoggerClosingConnection(
							    state.Database,
							    state.DataSource));

				    DbConnection.Close();
				    _openedInternally = false;
			    }
		    }
		    finally
		    {
			    _connectionLock.Release();
		    }
	    }

	    // Optomizations have been added to return connections to the pool faster
	    // Prefer PoolingOpen/Close functions when Connection Pooling is enabled

	    public bool Pooling => ConnectionStringBuilder.Pooling;

	    public void PoolingOpen()
	    {
		    if (Pooling)
		    {
			    DoOpen();
		    }
	    }

	    public async Task PoolingOpenAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    if (Pooling)
		    {
			    await DoOpenAsync(cancellationToken).ConfigureAwait(false);
		    }
	    }

	    public void PoolingClose()
	    {
		    if (Pooling)
		    {
			    DoClose();
		    }
	    }

	    // Use normal Open/Close functions when Connection Pooling is disabled
	    // These calls are used by Microsoft.EntityFrameworkCore

	    public void Open()
        {
	        if (!Pooling)
	        {
		        DoOpen();
	        }
	        else
	        {
		        // extarnal libraries can still try to open.  execute it but don't count it
		        _connectionLock.Wait();
		        try
		        {
			        if (DbConnection.State != ConnectionState.Open)
			        {
				        DbConnection.Open();
				        _openedInternally = true;
			        }
		        }
		        finally
		        {
			        _connectionLock.Release();
		        }
	        }
        }

        public async Task OpenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
	        if (!Pooling)
	        {
		        await DoOpenAsync(cancellationToken).ConfigureAwait(false);
	        }
	        else
	        {
		        // extarnal libraries can still try to open.  execute it but don't count it
		        await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
		        try
		        {
			        if (DbConnection.State != ConnectionState.Open)
			        {
				        await DbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
				        _openedInternally = true;
			        }
		        }
		        finally
		        {
			        _connectionLock.Release();
		        }
	        }
        }

        public void Close()
        {
	        if (!Pooling)
	        {
		        DoClose();
	        }
	        else
	        {
		        // extarnal libraries can still try to close.  close it if we manage it and count == 0
		        _connectionLock.Wait();
		        try
		        {
			        if (_openedCount == 0 && _openedInternally)
			        {
				        DbConnection.Close();
				        _openedInternally = false;
			        }
		        }
		        finally
		        {
			        _connectionLock.Release();
		        }
	        }
        }

        public bool IsMultipleActiveResultSetsEnabled => false;

        public IValueBufferCursor ActiveCursor { get; set; }

	    public virtual void Dispose()
	    {
		    CurrentTransaction?.Dispose();
		    if (_connectionOwned && _connection != null)
		    {
			    _connection.Dispose();
			    _connection = null;
			    _openedCount = 0;
		    }
	    }

    }
}
