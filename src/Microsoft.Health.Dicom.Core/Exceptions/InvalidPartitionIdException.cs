﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Dicom.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the DICOM instance is invalid.
    /// </summary>
    public class InvalidPartitionIdException : ValidationException
    {
        public InvalidPartitionIdException(string value)
            : base(string.Format(DicomCoreResource.InvalidPartitionId, value))
        {
        }
    }
}
