﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;
using Microsoft.Health.Dicom.Core.Features.Model;
using Microsoft.Health.Dicom.Core.Models;
using Microsoft.Health.Dicom.Functions.Indexing.Models;
using Microsoft.Health.Dicom.Tests.Common;
using NSubstitute;
using Xunit;

namespace Microsoft.Health.Dicom.Functions.UnitTests.Indexing
{
    public partial class ReindexDurableFunctionTests
    {
        [Fact]
        public async Task GivenTagKeys_WhenGettingQueryTagsForReindexing_ThenShouldPassArguments()
        {
            string operationId = Guid.NewGuid().ToString();
            var expectedInput = new List<int> { 1, 2, 3, 4, 5 };
            var expectedOutput = new List<ExtendedQueryTagStoreEntry>
            {
                new ExtendedQueryTagStoreEntry(1, "01010101", "AS", null, QueryTagLevel.Instance, ExtendedQueryTagStatus.Adding)
            };

            // Set up input
            IDurableActivityContext context = Substitute.For<IDurableActivityContext>();
            context.InstanceId.Returns(operationId);
            context.GetInput<IReadOnlyCollection<int>>().Returns(expectedInput);

            _extendedQueryTagStore
                .ConfirmReindexingAsync(
                    expectedInput,
                    operationId,
                    CancellationToken.None)
                .Returns(Task.FromResult<IReadOnlyCollection<ExtendedQueryTagStoreEntry>>(expectedOutput));

            // Call the activity
            IReadOnlyCollection<ExtendedQueryTagStoreEntry> actual = await _reindexDurableFunction.GetQueryTagsAsync(
                context,
                NullLogger.Instance);

            // Assert behavior
            Assert.Same(expectedOutput, actual);
            await _extendedQueryTagStore.Received(1).ConfirmReindexingAsync(expectedInput, operationId, CancellationToken.None);
        }

        [Fact]
        public async Task GivenInstances_WhenGettingMaxInstanceWatermark_ThenShouldInvokeCorrectMethod()
        {
            _instanceStore.GetMaxInstanceWatermarkAsync(CancellationToken.None).Returns(Task.FromResult<long?>(12345));

            long? actual = await _reindexDurableFunction.GetMaxInstanceWatermarkAsync(
                Substitute.For<IDurableActivityContext>(),
                NullLogger.Instance);

            Assert.Equal(12345, actual);
            await _instanceStore.Received(1).GetMaxInstanceWatermarkAsync(CancellationToken.None);
        }

        [Fact]
        public async Task GivenBatch_WhenReindexing_ThenShouldReindexEachInstance()
        {
            var batch = new ReindexBatch
            {
                QueryTags = new List<ExtendedQueryTagStoreEntry>
                {
                    new ExtendedQueryTagStoreEntry(1, "01", "DT", "foo", QueryTagLevel.Instance, ExtendedQueryTagStatus.Adding),
                    new ExtendedQueryTagStoreEntry(2, "02", "DT", null, QueryTagLevel.Series, ExtendedQueryTagStatus.Adding),
                    new ExtendedQueryTagStoreEntry(3, "03", "AS", "bar", QueryTagLevel.Study, ExtendedQueryTagStatus.Adding),
                },
                WatermarkRange = new WatermarkRange(5, 5),
            };

            // Set up input
            _instanceStore
                .GetInstanceIdentifiersByWatermarkRangeAsync(new WatermarkRange(5, 5), IndexStatus.Created, CancellationToken.None)
                .Returns(
                    new List<VersionedInstanceIdentifier>
                    {
                        new VersionedInstanceIdentifier(TestUidGenerator.Generate(), TestUidGenerator.Generate(), TestUidGenerator.Generate(), 5),
                        new VersionedInstanceIdentifier(TestUidGenerator.Generate(), TestUidGenerator.Generate(), TestUidGenerator.Generate(), 6),
                        new VersionedInstanceIdentifier(TestUidGenerator.Generate(), TestUidGenerator.Generate(), TestUidGenerator.Generate(), 7),
                        new VersionedInstanceIdentifier(TestUidGenerator.Generate(), TestUidGenerator.Generate(), TestUidGenerator.Generate(), 8),
                        new VersionedInstanceIdentifier(TestUidGenerator.Generate(), TestUidGenerator.Generate(), TestUidGenerator.Generate(), 9),
                    });

            // Call the activity
            await _reindexDurableFunction.ReindexBatchAsync(batch, NullLogger.Instance);

            // Assert behavior
            await _instanceStore
                .Received(1)
                .GetInstanceIdentifiersByWatermarkRangeAsync(new WatermarkRange(5, 5), IndexStatus.Created, CancellationToken.None);

            await _instanceReindexer.Received(1).ReindexInstanceAsync(batch.QueryTags, 5);
            await _instanceReindexer.Received(1).ReindexInstanceAsync(batch.QueryTags, 6);
            await _instanceReindexer.Received(1).ReindexInstanceAsync(batch.QueryTags, 7);
            await _instanceReindexer.Received(1).ReindexInstanceAsync(batch.QueryTags, 8);
            await _instanceReindexer.Received(1).ReindexInstanceAsync(batch.QueryTags, 9);
        }

        [Fact]
        public async Task GivenTagKeys_WhenCompletingReindexing_ThenShouldPassArguments()
        {
            string operationId = Guid.NewGuid().ToString();
            var expectedInput = new List<int> { 1, 2, 3, 4, 5 };
            var expectedOutput = new List<int> { 1, 2, 4, 5 };

            // Set up input
            IDurableActivityContext context = Substitute.For<IDurableActivityContext>();
            context.InstanceId.Returns(operationId);
            context.GetInput<IReadOnlyCollection<int>>().Returns(expectedInput);

            _extendedQueryTagStore
                .CompleteReindexingAsync(
                    expectedInput,
                    CancellationToken.None)
                .Returns(Task.FromResult<IReadOnlyCollection<int>>(expectedOutput));

            // Call the activity
            IReadOnlyCollection<int> actual = await _reindexDurableFunction.CompleteReindexingAsync(
                context,
                NullLogger.Instance);

            // Assert behavior
            Assert.Same(expectedOutput, actual);
            await _extendedQueryTagStore.Received(1).CompleteReindexingAsync(expectedInput, CancellationToken.None);
        }
    }
}
