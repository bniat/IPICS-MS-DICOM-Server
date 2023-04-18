// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.Health.Core.Features.Security.Authorization;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.Common;
using Microsoft.Health.Dicom.Core.Features.Security;
using Microsoft.Health.Dicom.Core.Messages.Update;

namespace Microsoft.Health.Dicom.Core.Features.Update;

internal class UpdateInstanceHandler : BaseHandler, IRequestHandler<UpdateInstanceRequest, UpdateInstanceResponse>
{
    private readonly IUpdateOperationInstanceService _updateOperationInstanceService;

    public UpdateInstanceHandler(IAuthorizationService<DataActions> authorizationService, IUpdateOperationInstanceService updateOperationInstanceService)
        : base(authorizationService)
        => _updateOperationInstanceService = EnsureArg.IsNotNull(updateOperationInstanceService, nameof(updateOperationInstanceService));

    public async Task<UpdateInstanceResponse> Handle(UpdateInstanceRequest request, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(request, nameof(request));
        EnsureArg.IsNotNull(request.UpdateSpec, nameof(request.UpdateSpec));

        if (await AuthorizationService.CheckAccess(DataActions.Write, cancellationToken) != DataActions.Write)
            throw new UnauthorizedDicomActionException(DataActions.Write);

        return await _updateOperationInstanceService.QueueUpdateOperationAsync(request.UpdateSpec, cancellationToken);
    }
}
