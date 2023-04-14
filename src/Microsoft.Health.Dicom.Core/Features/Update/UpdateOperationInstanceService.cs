// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.Common;
using Microsoft.Health.Dicom.Core.Features.Context;
using Microsoft.Health.Dicom.Core.Features.Operations;
using Microsoft.Health.Dicom.Core.Features.Routing;
using Microsoft.Health.Dicom.Core.Models.Operations;
using Microsoft.Health.Dicom.Core.Models.Update;
using Microsoft.Health.Operations;

namespace Microsoft.Health.Dicom.Core.Features.Update;

public class UpdateOperationInstanceService : IUpdateOperationInstanceService
{
    private readonly IGuidFactory _guidFactory;
    private readonly IDicomOperationsClient _client;
    private readonly IUrlResolver _urlResolver;
    private readonly IDicomRequestContextAccessor _contextAccessor;
    private readonly ILogger<UpdateInstanceOperationService> _logger;

    private static readonly OperationQueryCondition<DicomOperation> Query = new OperationQueryCondition<DicomOperation>
    {
        Operations = new DicomOperation[] { DicomOperation.Update },
        Statuses = new OperationStatus[]
        {
            OperationStatus.NotStarted,
            OperationStatus.Running,
        }
    };

    public UpdateOperationInstanceService(
        IGuidFactory guidFactory,
        IDicomOperationsClient client,
        IUrlResolver iUrlResolver,
        IDicomRequestContextAccessor contextAccessor,
        ILogger<UpdateInstanceOperationService> logger)
    {
        EnsureArg.IsNotNull(guidFactory, nameof(guidFactory));
        EnsureArg.IsNotNull(client, nameof(client));
        EnsureArg.IsNotNull(iUrlResolver, nameof(iUrlResolver));
        EnsureArg.IsNotNull(contextAccessor, nameof(contextAccessor));

        _guidFactory = guidFactory;
        _client = client;
        _urlResolver = iUrlResolver;
        _contextAccessor = contextAccessor;
    }

    public async Task<OperationReference> QueueUpdateOperationAsync(
        UpdateSpecification updateSpecification,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(updateSpecification, nameof(updateSpecification));
        EnsureArg.IsNotNull(updateSpecification.ChangeDataset, nameof(updateSpecification.ChangeDataset));

        UpdateRequestValidator.ValidateRequest(updateSpecification);
        UpdateRequestValidator.ValidateDicomDataset(updateSpecification.ChangeDataset);

        OperationReference activeOperation = await _client
            .FindOperationsAsync(Query, cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeOperation != null)
            throw new ExistingUpdateOperationException(activeOperation);

        var operationId = _guidFactory.Create();

        EnsureArg.IsNotNull(updateSpecification, nameof(updateSpecification));
        EnsureArg.IsNotNull(updateSpecification.ChangeDataset, nameof(updateSpecification.ChangeDataset));

        var partitionKey = 1;

        try
        {
            return await _client.StartUpdateOperationAsync(operationId, updateSpecification, partitionKey, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start update operation");
            throw;
        }
    }
}
