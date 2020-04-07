﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using Microsoft.Health.Dicom.Core.Messages;
using Microsoft.Health.Dicom.Core.Messages.Retrieve;
using Xunit;

namespace Microsoft.Health.Dicom.Core.UnitTests.Messages.Retrieve
{
    public class DicomRetrieveMetadataRequestTests
    {
        [Fact]
        public void GivenDicomRetrieveStudyMetadataRequest_WhenConstructed_ThenResourceTypeAndInstanceUidIsSetCorrectly()
        {
            string studyInstanceUid = Guid.NewGuid().ToString();
            var request = new DicomRetrieveMetadataRequest(studyInstanceUid);

            Assert.Equal(ResourceType.Study, request.ResourceType);
            Assert.Equal(studyInstanceUid, request.StudyInstanceUid);
        }

        [Fact]
        public void GivenDicomRetrieveSeriesMetadataRequest_WhenConstructed_ThenResourceTypeAndInstanceUidsAreSetCorrectly()
        {
            string studyInstanceUid = Guid.NewGuid().ToString();
            string seriesInstanceUid = Guid.NewGuid().ToString();
            var request = new DicomRetrieveMetadataRequest(studyInstanceUid, seriesInstanceUid);

            Assert.Equal(ResourceType.Series, request.ResourceType);
            Assert.Equal(studyInstanceUid, request.StudyInstanceUid);
            Assert.Equal(seriesInstanceUid, request.SeriesInstanceUid);
        }

        [Fact]
        public void GivenDicomRetrieveSopInstanceMetadataRequest_WhenConstructed_ThenResourceTypeAndInstanceUidsAreSetCorrectly()
        {
            string studyInstanceUid = Guid.NewGuid().ToString();
            string seriesInstanceUid = Guid.NewGuid().ToString();
            string sopInstanceUid = Guid.NewGuid().ToString();

            var request = new DicomRetrieveMetadataRequest(studyInstanceUid, seriesInstanceUid, sopInstanceUid);

            Assert.Equal(ResourceType.Instance, request.ResourceType);
            Assert.Equal(studyInstanceUid, request.StudyInstanceUid);
            Assert.Equal(seriesInstanceUid, request.SeriesInstanceUid);
            Assert.Equal(sopInstanceUid, request.SopInstanceUid);
        }
    }
}
