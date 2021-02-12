﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.CustomTag;
using Microsoft.Health.Dicom.SqlServer.Features.Schema;
using Microsoft.Health.Dicom.SqlServer.Features.Schema.Model;
using Microsoft.Health.SqlServer.Features.Client;
using Microsoft.Health.SqlServer.Features.Schema;
using Microsoft.Health.SqlServer.Features.Storage;

namespace Microsoft.Health.Dicom.SqlServer.Features.CustomTag
{
    public class SqlCustomTagStore : ICustomTagStore
    {
        private readonly SqlConnectionWrapperFactory _sqlConnectionWrapperFactory;
        private readonly SchemaInformation _schemaInformation;
        private readonly ILogger<SqlCustomTagStore> _logger;

        public SqlCustomTagStore(
           SqlConnectionWrapperFactory sqlConnectionWrapperFactory,
           SchemaInformation schemaInformation,
           ILogger<SqlCustomTagStore> logger)
        {
            EnsureArg.IsNotNull(sqlConnectionWrapperFactory, nameof(sqlConnectionWrapperFactory));
            EnsureArg.IsNotNull(schemaInformation, nameof(schemaInformation));
            EnsureArg.IsNotNull(logger, nameof(logger));

            _sqlConnectionWrapperFactory = sqlConnectionWrapperFactory;
            _schemaInformation = schemaInformation;
            _logger = logger;
        }

        public async Task AddCustomTagsAsync(IEnumerable<CustomTagEntry> customTagEntries, CancellationToken cancellationToken = default)
        {
            if (_schemaInformation.Current < SchemaVersionConstants.SupportCustomTagSchemaVersion)
            {
                throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
            }

            using (SqlConnectionWrapper sqlConnectionWrapper = await _sqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
            using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateSqlCommand())
            {
                IEnumerable<AddCustomTagsInputTableTypeV1Row> rows = customTagEntries.Select(ToAddCustomTagsInputTableTypeV1Row);
                V2.AddCustomTags.PopulateCommand(sqlCommandWrapper, new V2.AddCustomTagsTableValuedParameters(rows));

                try
                {
                    await sqlCommandWrapper.ExecuteNonQueryAsync(cancellationToken);
                }
                catch (SqlException ex)
                {
                    switch (ex.Number)
                    {
                        case SqlErrorCodes.Conflict:
                            throw new CustomTagsAlreadyExistsException();

                        default:
                            throw new DataStoreException(ex);
                    }
                }
            }
        }

        public async Task<IEnumerable<CustomTagEntry>> GetCustomTagsAsync(string path, CancellationToken cancellationToken = default)
        {
            if (_schemaInformation.Current < SchemaVersionConstants.SupportCustomTagSchemaVersion)
            {
                throw new BadRequestException(DicomSqlServerResource.SchemaVersionNeedsToBeUpgraded);
            }

            List<CustomTagEntry> results = new List<CustomTagEntry>();

            using (SqlConnectionWrapper sqlConnectionWrapper = await _sqlConnectionWrapperFactory.ObtainSqlConnectionWrapperAsync(cancellationToken))
            using (SqlCommandWrapper sqlCommandWrapper = sqlConnectionWrapper.CreateSqlCommand())
            {
                V2.GetCustomTag.PopulateCommand(sqlCommandWrapper, path);

                using (var reader = await sqlCommandWrapper.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        (string tagPath, string tagVR, int tagLevel, int tagStatus) = reader.ReadRow(
                           V2.CustomTag.TagPath,
                           V2.CustomTag.TagVR,
                           V2.CustomTag.TagLevel,
                           V2.CustomTag.TagStatus);

                        results.Add(new CustomTagEntry { Path = tagPath, VR = tagVR, Level = (CustomTagLevel)tagLevel, Status = (CustomTagStatus)tagStatus });
                    }
                }
            }

            return results;
        }

        public Task DeleteCustomTagAsync(long key, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        private static AddCustomTagsInputTableTypeV1Row ToAddCustomTagsInputTableTypeV1Row(CustomTagEntry entry)
        {
            return new AddCustomTagsInputTableTypeV1Row(entry.Path, entry.VR, (byte)entry.Level);
        }

        public Task<CustomTagEntry> GetCustomTagAsync(string path, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<CustomTagEntry>> GetAllCustomTagsAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCustomTagAsync(string tagPath, string vr, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
