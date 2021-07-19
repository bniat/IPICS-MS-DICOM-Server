﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Options;
using Microsoft.Health.Dicom.Core.Configs;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Extensions;
using Microsoft.Health.Dicom.Core.Features.Common;
using Microsoft.Health.Dicom.Core.Features.Operations;
using Microsoft.Health.Dicom.Core.Features.Routing;
using Microsoft.Health.Dicom.Core.Messages.ExtendedQueryTag;
using Microsoft.Health.Dicom.Core.Models.Operations;

namespace Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag
{
    public class AddExtendedQueryTagService : IAddExtendedQueryTagService
    {
        private readonly IStoreFactory<IExtendedQueryTagStore> _extendedQueryTagStoreFactory;
        private readonly IDicomOperationsClient _client;
        private readonly IExtendedQueryTagEntryValidator _extendedQueryTagEntryValidator;
        private readonly IUrlResolver _uriResolver;
        private readonly int _maxAllowedCount;

        public AddExtendedQueryTagService(
            IStoreFactory<IExtendedQueryTagStore> extendedQueryTagStoreFactory,
            IDicomOperationsClient client,
            IExtendedQueryTagEntryValidator extendedQueryTagEntryValidator,
            IUrlResolver uriResolver,
            IOptions<ExtendedQueryTagConfiguration> extendedQueryTagConfiguration)
        {
            EnsureArg.IsNotNull(extendedQueryTagStoreFactory, nameof(extendedQueryTagStoreFactory));
            EnsureArg.IsNotNull(client, nameof(client));
            EnsureArg.IsNotNull(extendedQueryTagEntryValidator, nameof(extendedQueryTagEntryValidator));
            EnsureArg.IsNotNull(uriResolver, nameof(uriResolver));
            EnsureArg.IsNotNull(extendedQueryTagConfiguration?.Value, nameof(extendedQueryTagConfiguration));

            _extendedQueryTagStoreFactory = extendedQueryTagStoreFactory;
            _client = client;
            _extendedQueryTagEntryValidator = extendedQueryTagEntryValidator;
            _uriResolver = uriResolver;
            _maxAllowedCount = extendedQueryTagConfiguration.Value.MaxAllowedCount;
        }

        public async Task<AddExtendedQueryTagResponse> AddExtendedQueryTagsAsync(
            IEnumerable<AddExtendedQueryTagEntry> extendedQueryTags,
            CancellationToken cancellationToken = default)
        {
            _extendedQueryTagEntryValidator.ValidateExtendedQueryTags(extendedQueryTags);
            var normalized = extendedQueryTags
                .Select(item => item.Normalize())
                .ToList();

            // TODO: Handle tags that have already been added
            // Add the extended query tags to the DB
            IExtendedQueryTagStore extendedQueryTagStore = await _extendedQueryTagStoreFactory.GetInstanceAsync(cancellationToken);
            IReadOnlyCollection<int> expected = await extendedQueryTagStore.AddExtendedQueryTagsAsync(
                normalized,
                _maxAllowedCount,
                ready: false,
                cancellationToken: cancellationToken);

            // Start re-indexing
            string operationId = await _client.StartQueryTagIndexingAsync(expected, cancellationToken);

            // Associate the tags to the operation and confirm their processing
            if ((await extendedQueryTagStore.ConfirmReindexingAsync(expected, operationId, cancellationToken)).Count == 0)
            {
                throw new ExtendedQueryTagsAlreadyExistsException();
            }

            return new AddExtendedQueryTagResponse(
                new OperationReference(operationId, _uriResolver.ResolveOperationStatusUri(operationId)));
        }
    }
}
