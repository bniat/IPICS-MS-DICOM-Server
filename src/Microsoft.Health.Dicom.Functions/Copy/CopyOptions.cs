﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Health.Operations.Functions.DurableTask;

namespace Microsoft.Health.Dicom.Functions.Copy;

/// <summary>
/// Represents the options for a copy function.
/// </summary>
public class CopyOptions
{
    internal const string SectionName = "Copy";

    /// <summary>
    /// Gets or sets the number of DICOM instances processed by a single activity.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Gets or sets the number of threads available for each batch.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int BatchThreadCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum number of concurrent batches processed at a given time.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int MaxParallelBatches { get; set; } = 10;

    /// <summary>
    /// Gets or sets the <see cref="ActivityRetryOptions"/> for copy activities.
    /// </summary>
    public ActivityRetryOptions RetryOptions { get; set; }
}
