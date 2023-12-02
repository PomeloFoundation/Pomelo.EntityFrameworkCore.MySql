// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlConnectionStringOptionsValidator : IMySqlConnectionStringOptionsValidator
{
    public static MySqlConnectionStringOptionsValidator Instance { get; } = new MySqlConnectionStringOptionsValidator();

    public virtual void EnsureMandatoryOptions(ref string connectionString, IDbContextOptions options = null)
    {
        if (connectionString is null)
        {
            return;
        }

        if (options is not null)
        {
            connectionString = new NamedConnectionStringResolver(options)
                .ResolveConnectionString(connectionString);
        }

        var csb = new MySqlConnectionStringBuilder(connectionString);

        if (!ValidateMandatoryOptions(csb))
        {
            csb.AllowUserVariables = true;
            csb.UseAffectedRows = false;
        }

        connectionString = csb.ConnectionString;
    }

    public virtual void EnsureMandatoryOptions([NotNull] DbConnection connection, IDbContextOptions options = null)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var connectionString = options is not null
            ? connection.ConnectionString is not null
                ? new NamedConnectionStringResolver(options)
                    .ResolveConnectionString(connection.ConnectionString)
                : null
            : connection.ConnectionString;

        var csb = new MySqlConnectionStringBuilder(connectionString);

        if (!ValidateMandatoryOptions(csb))
        {
            try
            {
                csb.AllowUserVariables = true;
                csb.UseAffectedRows = false;

                connection.ConnectionString = csb.ConnectionString;
                return;
            }
            catch (Exception e)
            {
                ThrowException(e);
            }
        }

        // The connection string could have changed by the NamedConnectionStringResolver call earlier.
        if (connectionString != connection.ConnectionString)
        {
            connection.ConnectionString = connectionString;
        }
    }

    public virtual void EnsureMandatoryOptions([NotNull] DbDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);

        var csb = new MySqlConnectionStringBuilder(dataSource.ConnectionString);

        if (!ValidateMandatoryOptions(csb))
        {
            // We can't alter the connection string of a DbDataSource/MySqlDataSource as we do for DbConnection/MySqlConnection in cases
            // where the necessary connection string options have not been set.
            // We can only throw.
            ThrowException();
        }
    }

    public virtual void ThrowOnInvalidMandatoryOptions(
        string connectionString,
        DbConnection connection,
        DbDataSource dataSource)
    {
        bool valid;

        // If we don't have an explicitly-configured data source, try to get one from the application service provider.
        if (dataSource is not null)
        {
            valid = ValidateMandatoryOptions(dataSource.ConnectionString);
        }
        else if (connection is not null)
        {
            valid = ValidateMandatoryOptions(connection.ConnectionString);
        }
        else
        {
            valid = ValidateMandatoryOptions(connectionString);
        }

        if (!valid)
        {
            ThrowException();
        }
    }

    protected virtual bool ValidateMandatoryOptions(string connectionString)
        => ValidateMandatoryOptions(new MySqlConnectionStringBuilder(connectionString));

    protected virtual bool ValidateMandatoryOptions(MySqlConnectionStringBuilder csb)
        => csb.AllowUserVariables &&
           !csb.UseAffectedRows;

    protected virtual void ThrowException(Exception innerException = null)
        => throw new InvalidOperationException(
            @"The connection string of a connection used by Pomelo.EntityFrameworkCore.MySql must contain ""AllowUserVariables=True;UseAffectedRows=False"".",
            innerException);
}
