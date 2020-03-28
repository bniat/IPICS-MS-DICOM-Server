﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Health.Dicom.Core.Features.Common
{
    public interface IDicomBlobDataStore
    {
        Task<Uri> AddFileAsStreamAsync(DicomInstanceIdentifier dicomInstanceIdentifier, Stream buffer, bool overwriteIfExists = false, CancellationToken cancellationToken = default);

        Task<Stream> GetFileAsStreamAsync(DicomInstanceIdentifier dicomInstanceIdentifier, CancellationToken cancellationToken = default);

        Task DeleteFileIfExistsAsync(DicomInstanceIdentifier dicomInstanceIdentifier, CancellationToken cancellationToken = default);
    }
}
