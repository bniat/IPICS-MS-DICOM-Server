﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.Health.Dicom.Core.Models.Operations;

namespace Microsoft.Health.Dicom.Core.Messages.Operations
{
    // TODO: Use record type once we're no longer multi-targeting 3.1 + 5

    /// <summary>
    /// Represents the metadata for a long-running DICOM operation.
    /// </summary>
    public class OperationStatusResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationStatusResponse"/> class.
        /// </summary>
        /// <param name="operationId">The unique ID for a particular DICOM operation.</param>
        /// <param name="type">The type of the operation.</param>
        /// <param name="createdTime">The date and time when the operation was created.</param>
        /// <param name="lastUpdatedTime">The date and time when the operation's status was last updated.</param>
        /// <param name="status">The runtime status of the operation.</param>
        /// <exception cref="ArgumentException"><paramref name="operationId"/> consists of white space characters.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="operationId"/> is <see langword="null"/>.</exception>
        public OperationStatusResponse(
            string operationId,
            OperationType type,
            DateTime createdTime,
            DateTime lastUpdatedTime,
            OperationRuntimeStatus status)
        {
            EnsureArg.IsNotNullOrWhiteSpace(operationId, nameof(operationId));
            EnsureArg.EnumIsDefined(type, nameof(type));
            EnsureArg.EnumIsDefined(status, nameof(status));

            OperationId = operationId;
            Type = type;
            CreatedTime = createdTime;
            LastUpdatedTime = lastUpdatedTime;
            Status = status;
        }

        /// <summary>
        /// Gets the operation ID.
        /// </summary>
        /// <value>The unique ID that denotes a particular operation.</value>
        public string OperationId { get; }

        /// <summary>
        /// Gets the category of the operation.
        /// </summary>
        /// <value>
        /// The <see cref="OperationType"/> if recognized; otherwise <see cref="OperationType.Unknown"/>.
        /// </value>
        public OperationType Type { get; }

        /// <summary>
        /// Gets the date and time the operation was started.
        /// </summary>
        /// <value>The <see cref="DateTime"/> when the operation was started.</value>
        public DateTime CreatedTime { get; }

        /// <summary>
        /// Gets the last date and time the operation's execution status was updated.
        /// </summary>
        /// <value>The last <see cref="DateTime"/> when the operation status was updated.</value>
        public DateTime LastUpdatedTime { get; }

        /// <summary>
        /// Gets the execution status of the operation.
        /// </summary>
        /// <value>
        /// The <see cref="OperationRuntimeStatus"/> if recognized; otherwise <see cref="OperationType.Unknown"/>.
        /// </value>
        public OperationRuntimeStatus Status { get; }
    }
}
