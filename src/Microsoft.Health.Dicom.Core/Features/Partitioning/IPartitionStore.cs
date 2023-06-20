﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Health.Dicom.Core.Features.Partitioning;

public interface IPartitionStore
{
    Task<Partitioning.Partition> AddPartitionAsync(string partitionName, CancellationToken cancellationToken = default);

    Task<IEnumerable<Partitioning.Partition>> GetPartitionsAsync(CancellationToken cancellationToken = default);

    Task<Partitioning.Partition> GetPartitionAsync(string partitionName, CancellationToken cancellationToken = default);
}