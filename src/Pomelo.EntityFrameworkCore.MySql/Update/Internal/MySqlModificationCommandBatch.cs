﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Update.Internal
{
  using RelationalStrings = EntityFrameworkCore.Internal.RelationalStrings;

  public class MySqlModificationCommandBatch : AffectedCountModificationCommandBatch
  {

    private const int DefaultNetworkPacketSizeBytes = 4096;
    private const int MaxScriptLength = 65536 * DefaultNetworkPacketSizeBytes / 2;
    private const int MaxParameterCount = 2100;
    private const int MaxRowCount = 1000;
    private int _parameterCount = 1; // Implicit parameter for the command text
    private readonly int _maxBatchSize;
    private readonly List<ModificationCommand> _bulkInsertCommands = new List<ModificationCommand>();
    private int _commandsLeftToLengthCheck = 50;

    public MySqlModificationCommandBatch(
      [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
      [NotNull] ISqlGenerationHelper sqlGenerationHelper,
      [NotNull] IUpdateSqlGenerator updateSqlGenerator,
      [NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory,
      [CanBeNull] int? maxBatchSize)
      : base(commandBuilderFactory, sqlGenerationHelper, updateSqlGenerator, valueBufferFactoryFactory)
    {
      if (maxBatchSize.HasValue
          && (maxBatchSize.Value <= 0))
      {
        throw new ArgumentOutOfRangeException(nameof(maxBatchSize), RelationalStrings.InvalidMaxBatchSize);
      }

      _maxBatchSize = Math.Min(maxBatchSize ?? int.MaxValue, MaxRowCount);
    }

    protected new virtual IMySqlUpdateSqlGenerator UpdateSqlGenerator
      => (IMySqlUpdateSqlGenerator) base.UpdateSqlGenerator;


    protected override bool CanAddCommand(ModificationCommand modificationCommand)
    {
      if (_maxBatchSize <= ModificationCommands.Count)
      {
        return false;
      }

      var additionalParameterCount = CountParameters(modificationCommand);

      if (_parameterCount + additionalParameterCount >= MaxParameterCount)
      {
        return false;
      }

      _parameterCount += additionalParameterCount;
      return true;
    }

    private static int CountParameters(ModificationCommand modificationCommand)
    {
      var parameterCount = 0;
      foreach (var columnModification in modificationCommand.ColumnModifications)
      {
        if (columnModification.ParameterName != null)
        {
          parameterCount++;
        }

        if (columnModification.OriginalParameterName != null)
        {
          parameterCount++;
        }
      }

      return parameterCount;
    }

    protected override void ResetCommandText()
    {
      base.ResetCommandText();
      _bulkInsertCommands.Clear();
    }

    protected override bool IsCommandTextValid()
    {
      if (--_commandsLeftToLengthCheck < 0)
      {
        var commandTextLength = GetCommandText().Length;
        if (commandTextLength >= MaxScriptLength)
        {
          return false;
        }

        var avarageCommandLength = commandTextLength / ModificationCommands.Count;
        var expectedAdditionalCommandCapacity = (MaxScriptLength - commandTextLength) / avarageCommandLength;
        _commandsLeftToLengthCheck = Math.Max(1, expectedAdditionalCommandCapacity / 4);
      }

      return true;
    }

    protected override string GetCommandText()
      => base.GetCommandText() + GetBulkInsertCommandText(ModificationCommands.Count);

    private string GetBulkInsertCommandText(int lastIndex)
    {
      if (_bulkInsertCommands.Count == 0)
      {
        return string.Empty;
      }

      var stringBuilder = new StringBuilder();
      var grouping = UpdateSqlGenerator.AppendBulkInsertOperation(stringBuilder, _bulkInsertCommands, lastIndex);
      for (var i = lastIndex - _bulkInsertCommands.Count; i < lastIndex; i++)
      {
        CommandResultSet[i] = grouping;
      }

      if (grouping != ResultSetMapping.NoResultSet)
      {
        CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
      }

      return stringBuilder.ToString();
    }

    protected override void UpdateCachedCommandText(int commandPosition)
    {
      var newModificationCommand = ModificationCommands[commandPosition];

      if (newModificationCommand.EntityState == EntityState.Added)
      {
        if ((_bulkInsertCommands.Count > 0)
            && !CanBeInsertedInSameStatement(_bulkInsertCommands[0], newModificationCommand))
        {
          CachedCommandText.Append(GetBulkInsertCommandText(commandPosition));
          _bulkInsertCommands.Clear();
        }
        _bulkInsertCommands.Add(newModificationCommand);

        LastCachedCommandIndex = commandPosition;
      }
      else
      {
        CachedCommandText.Append(GetBulkInsertCommandText(commandPosition));
        _bulkInsertCommands.Clear();

        base.UpdateCachedCommandText(commandPosition);
      }
    }

    private static bool CanBeInsertedInSameStatement(ModificationCommand firstCommand, ModificationCommand secondCommand)
      => string.Equals(firstCommand.TableName, secondCommand.TableName, StringComparison.Ordinal)
         && string.Equals(firstCommand.Schema, secondCommand.Schema, StringComparison.Ordinal)
         && firstCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName).SequenceEqual(
           secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
         && firstCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName).SequenceEqual(
           secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

      public override void Execute(IRelationalConnection connection)
      {
          var storeCommand = CreateStoreCommand();
          try
          {
              using (var relationalDataReader = storeCommand.RelationalCommand.ExecuteReader(connection, storeCommand.ParameterValues, false))
              {
                  Consume(relationalDataReader.DbDataReader);
              }
          }
          catch (DbUpdateException)
          {
              throw;
          }
          catch (Exception ex)
          {
              throw new DbUpdateException(RelationalStrings.UpdateStoreException, ex);
          }
      }

      public override async Task ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken = default(CancellationToken))
      {
          var storeCommand = CreateStoreCommand();
          try
          {
              var dataReader = await storeCommand.RelationalCommand.ExecuteReaderAsync(connection, storeCommand.ParameterValues, false, cancellationToken).ConfigureAwait(false);
              try
              {
                  await ConsumeAsync(dataReader.DbDataReader, cancellationToken).ConfigureAwait(false);
              }
              finally
              {
	                while (await dataReader.DbDataReader.NextResultAsync(cancellationToken).ConfigureAwait(false))
                  dataReader.Dispose();
              }
          }
          catch (DbUpdateException)
          {
              throw;
          }
          catch (Exception ex)
          {
              throw new DbUpdateException(RelationalStrings.UpdateStoreException, ex);
          }
      }

  }
}
