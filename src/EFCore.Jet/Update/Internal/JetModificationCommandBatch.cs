// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace EntityFrameworkCore.Jet.Update.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class JetModificationCommandBatch : AffectedCountModificationCommandBatch
    {
        private const int MaxRowCount = 1;
        private const int MaxParameterCount = 2100 - 2;
        private readonly List<IReadOnlyModificationCommand> _pendingBulkInsertCommands = new();
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public JetModificationCommandBatch(
            [NotNull] ModificationCommandBatchFactoryDependencies dependencies)
            : base(dependencies,1)
        {
            // See https://support.office.com/en-us/article/access-specifications-0cf3c66f-9cf2-4e32-9568-98c1025bb47c
            // for Access specifications and limits.
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected new virtual IJetUpdateSqlGenerator UpdateSqlGenerator
            => (IJetUpdateSqlGenerator)base.UpdateSqlGenerator;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void RollbackLastCommand(IReadOnlyModificationCommand modificationCommand)
        {
            if (_pendingBulkInsertCommands.Count > 0)
            {
                _pendingBulkInsertCommands.RemoveAt(_pendingBulkInsertCommands.Count - 1);
            }

            base.RollbackLastCommand(modificationCommand);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override bool IsValid()
        {
            return true;
        }

        private void ApplyPendingBulkInsertCommands()
        {
            if (_pendingBulkInsertCommands.Count == 0)
            {
                return;
            }

            var commandPosition = ResultSetMappings.Count;

            var wasCachedCommandTextEmpty = IsCommandTextEmpty;

            var resultSetMapping = UpdateSqlGenerator.AppendBulkInsertOperation(
                SqlBuilder, _pendingBulkInsertCommands, commandPosition, out var requiresTransaction);

            SetRequiresTransaction(!wasCachedCommandTextEmpty || requiresTransaction);

            for (var i = 0; i < _pendingBulkInsertCommands.Count; i++)
            {
                ResultSetMappings.Add(resultSetMapping);
            }

            if (resultSetMapping != ResultSetMapping.NoResults)
            {
                ResultSetMappings[^1] = ResultSetMapping.LastInResultSet;
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void AddCommand(IReadOnlyModificationCommand modificationCommand)
        {
            if (modificationCommand.EntityState == EntityState.Added && modificationCommand.StoreStoredProcedure is null)
            {
                if (_pendingBulkInsertCommands.Count > 0
                    && !CanBeInsertedInSameStatement(_pendingBulkInsertCommands[0], modificationCommand))
                {
                    // The new Add command cannot be added to the pending bulk insert commands (e.g. different table).
                    // Write out the pending commands before starting a new pending chain.
                    ApplyPendingBulkInsertCommands();
                    _pendingBulkInsertCommands.Clear();
                }

                _pendingBulkInsertCommands.Add(modificationCommand);
                AddParameters(modificationCommand);
            }
            else
            {
                // If we have any pending bulk insert commands, write them out before the next non-Add command
                if (_pendingBulkInsertCommands.Count > 0)
                {
                    // Note that we don't care about the transactionality of the bulk insert SQL, since there's the additional non-Add
                    // command coming right afterwards, and so a transaction is required in any case.
                    ApplyPendingBulkInsertCommands();
                    _pendingBulkInsertCommands.Clear();
                }

                base.AddCommand(modificationCommand);
            }
        }

        private static bool CanBeInsertedInSameStatement(
            IReadOnlyModificationCommand firstCommand,
            IReadOnlyModificationCommand secondCommand)
            => firstCommand.TableName == secondCommand.TableName
                && firstCommand.Schema == secondCommand.Schema
                && firstCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName).SequenceEqual(
                    secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
                && firstCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName).SequenceEqual(
                    secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void Complete(bool moreBatchesExpected)
        {
            ApplyPendingBulkInsertCommands();

            base.Complete(moreBatchesExpected);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override void Execute(IRelationalConnection connection)
        {
            base.Execute(connection);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override async Task ExecuteAsync(
            IRelationalConnection connection,
            CancellationToken cancellationToken = default)
        {
            await base.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
        }
    }
}
