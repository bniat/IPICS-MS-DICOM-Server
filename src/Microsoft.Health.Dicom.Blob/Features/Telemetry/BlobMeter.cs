// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Metrics;

namespace Microsoft.Health.Dicom.Blob.Features.Telemetry;
public sealed class BlobMeter : IDisposable
{
    private readonly Meter _meter;

    public BlobMeter()
    {
        _meter = new Meter("Microsoft.Health.Dicom.Blob.Features.Storage", "1.0");
        JsonSerializationException = _meter.CreateCounter<double>(nameof(JsonSerializationException));
        JsonDeserializationException = _meter.CreateCounter<double>(nameof(JsonDeserializationException));
    }

    public Counter<double> JsonSerializationException { get; }
    public Counter<double> JsonDeserializationException { get; }


    public void Dispose()
        => _meter.Dispose();
}
