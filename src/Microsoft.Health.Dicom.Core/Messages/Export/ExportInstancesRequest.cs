﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using MediatR;
using Microsoft.Health.Dicom.Core.Features.Partition;
using Microsoft.Health.Dicom.Core.Models.Export;

namespace Microsoft.Health.Dicom.Core.Messages.Export;

public sealed class ExportInstancesRequest : IRequest<ExportInstancesResponse>
{
    public ExportSpecification Specification { get; }

    public PartitionEntry Partition { get; }

    public ExportInstancesRequest(ExportSpecification spec, PartitionEntry partition)
    {
        Specification = EnsureArg.IsNotNull(spec, nameof(spec));
        Partition = EnsureArg.IsNotNull(partition, nameof(partition));
    }
}
