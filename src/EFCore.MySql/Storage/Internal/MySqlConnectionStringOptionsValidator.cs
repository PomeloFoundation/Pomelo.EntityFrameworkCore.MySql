// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

public class MySqlConnectionStringOptionsValidator : IMySqlConnectionStringOptionsValidator
{
    public virtual bool EnsureMandatoryOptions(ref string connectionString)
    {
        if (connectionString is not null)
        {
            var csb = new MySqlConnectionStringBuilder(connectionString);

            if (!ValidateMandatoryOptions(csb))
            {
                csb.AllowUserVariables = true;
                csb.UseAffectedRows = false;

                connectionString = csb.ConnectionString;

                return true;
            }
        }

        return false;
    }

    public virtual bool EnsureMandatoryOptions(DbConnection connection)
    {
        if (connection is not null)
        {
            var csb = new MySqlConnectionStringBuilder(connection.ConnectionString);

            if (!ValidateMandatoryOptions(csb))
            {
                try
                {
                    csb.AllowUserVariables = true;
                    csb.UseAffectedRows = false;

                    connection.ConnectionString = csb.ConnectionString;

                    return true;
                }
                catch (Exception e)
                {
                    ThrowException(e);
                }
            }
        }

        return false;
    }

    public virtual bool EnsureMandatoryOptions(DbDataSource dataSource)
    {
        if (dataSource is null)
        {
            return false;
        }

        var csb = new MySqlConnectionStringBuilder(dataSource.ConnectionString);

        if (!ValidateMandatoryOptions(csb))
        {
            // We can't alter the connection string of a DbDataSource/MySqlDataSource as we do for DbConnection/MySqlConnection in cases
            // where the necessary connection string options have not been set.
            // We can only throw.
            ThrowException();
        }

        return true;
    }

    public virtual void ThrowException(Exception innerException = null)
        => throw new InvalidOperationException(
            @"The connection string of a connection used by Pomelo.EntityFrameworkCore.MySql must contain ""AllowUserVariables=True;UseAffectedRows=False"".",
            innerException);

    protected virtual bool ValidateMandatoryOptions(MySqlConnectionStringBuilder csb)
        => csb.AllowUserVariables &&
           !csb.UseAffectedRows;
}
