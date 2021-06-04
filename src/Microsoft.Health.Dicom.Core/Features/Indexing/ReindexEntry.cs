﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Dicom.Core.Features.Indexing
{
    /// <summary>
    /// Entry of TagReindexOperationStore.
    /// </summary>
    public class ReindexEntry
    {
        /// <summary>
        /// The tag key.
        /// </summary>
        public int TagKey { get; set; }

        /// <summary>
        /// The operation id.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// The reindex status.
        /// </summary>
        public ReindexStatus Status { get; set; }

        /// <summary>
        /// The start watermark.
        /// </summary>
        public long StartWatermark { get; set; }

        /// <summary>
        /// The end watermark.
        /// </summary>
        public long EndWatermark { get; set; }
    }
}
