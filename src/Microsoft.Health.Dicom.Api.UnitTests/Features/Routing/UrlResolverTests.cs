﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Health.Dicom.Api.Features.Routing;
using Microsoft.Health.Dicom.Core.Features.Model;
using Microsoft.Health.Dicom.Core.Models.Operations;
using NSubstitute;
using Xunit;

namespace Microsoft.Health.Dicom.Api.UnitTests.Features.Routing
{
    public class UrlResolverTests
    {
        private const string DefaultScheme = "http";
        private const string DefaultHost = "test";

        private readonly IUrlHelperFactory _urlHelperFactory = Substitute.For<IUrlHelperFactory>();
        private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        private readonly IActionContextAccessor _actionContextAccessor = Substitute.For<IActionContextAccessor>();

        private readonly UrlResolver _urlResolver;

        private readonly IUrlHelper _urlHelper = Substitute.For<IUrlHelper>();
        private readonly DefaultHttpContext _httpContext = new DefaultHttpContext();
        private readonly ActionContext _actionContext = new ActionContext();

        private UrlRouteContext _capturedUrlRouteContext;

        public UrlResolverTests()
        {
            _httpContext.Request.Scheme = DefaultScheme;
            _httpContext.Request.Host = new HostString(DefaultHost);

            _httpContextAccessor.HttpContext.Returns(_httpContext);

            _actionContextAccessor.ActionContext.Returns(_actionContext);

            _urlHelper.RouteUrl(
                Arg.Do<UrlRouteContext>(c => _capturedUrlRouteContext = c));

            _urlHelperFactory.GetUrlHelper(_actionContext).Returns(_urlHelper);

            _urlHelper.RouteUrl(Arg.Any<UrlRouteContext>()).Returns($"{DefaultScheme}://{DefaultHost}");

            _urlResolver = new UrlResolver(
                   _urlHelperFactory,
                   _httpContextAccessor,
                   _actionContextAccessor);
        }

        [Fact]
        public void GivenOperationId_WhenRetrieveOperationStatusUriIsResolved_ThenCorrectUrlShouldBeReturned()
        {
            Guid operationId = Guid.NewGuid();

            _urlResolver.ResolveOperationStatusUri(operationId);

            ValidateUrlRouteContext(
                KnownRouteNames.OperationStatus,
                routeValues =>
                {
                    Assert.Equal(OperationId.ToString(operationId), routeValues[KnownActionParameterNames.OperationId]);
                });
        }

        [Fact]
        public void GivenAStudy_WhenRetrieveStudyUriIsResolved_ThenCorrectUrlShouldBeReturned()
        {
            const string studyInstanceUid = "123.123";

            _urlResolver.ResolveRetrieveStudyUri(studyInstanceUid);

            ValidateUrlRouteContext(
                KnownRouteNames.RetrieveStudy,
                routeValues =>
                {
                    Assert.Equal(studyInstanceUid, routeValues[KnownActionParameterNames.StudyInstanceUid]);
                });
        }

        [Fact]
        public void GivenAStudy_WhenRetrieveStudyUriWithPartitionIdAndVersionIsResolved_ThenCorrectUrlShouldBeReturned()
        {
            const string studyInstanceUid = "123.123";
            const string partitionId = "partition1";
            const string version = "v1.0-prerelease";
            _httpContext.Request.RouteValues.Add(KnownActionParameterNames.PartitionId, partitionId);
            _httpContext.Request.RouteValues.Add(KnownActionParameterNames.Version, version);

            _urlResolver.ResolveRetrieveStudyUri(studyInstanceUid);

            ValidateUrlRouteContext(
                KnownRouteNames.PartitionVersionedRetrieveStudy,
                routeValues =>
                {
                    Assert.Equal(studyInstanceUid, routeValues[KnownActionParameterNames.StudyInstanceUid]);
                    Assert.Equal(partitionId, routeValues[KnownActionParameterNames.PartitionId]);
                });
        }

        [Fact]
        public void GivenAnInstance_WhenRetrieveInstanceUriIsResolved_ThenCorrectUrlShouldBeReturned()
        {
            const string studyInstanceUid = "123.123";
            const string seriesInstanceUid = "456.456";
            const string sopInstanceUid = "789.789";

            var instance = new InstanceIdentifier(studyInstanceUid, seriesInstanceUid, sopInstanceUid);

            _urlResolver.ResolveRetrieveInstanceUri(instance);

            ValidateUrlRouteContext(
                KnownRouteNames.RetrieveInstance,
                routeValues =>
                {
                    Assert.Equal(studyInstanceUid, routeValues[KnownActionParameterNames.StudyInstanceUid]);
                    Assert.Equal(seriesInstanceUid, routeValues[KnownActionParameterNames.SeriesInstanceUid]);
                    Assert.Equal(sopInstanceUid, routeValues[KnownActionParameterNames.SopInstanceUid]);
                });
        }

        [Fact]
        public void GivenAnInstance_WhenRetrieveInstanceUriWithPartitionIdIsResolved_ThenCorrectUrlShouldBeReturned()
        {
            const string studyInstanceUid = "123.123";
            const string seriesInstanceUid = "456.456";
            const string sopInstanceUid = "789.789";
            const string partitionId = "partition1";

            var instance = new InstanceIdentifier(studyInstanceUid, seriesInstanceUid, sopInstanceUid, partitionId);
            _httpContext.Request.RouteValues.Add(KnownActionParameterNames.PartitionId, "partition1");

            _urlResolver.ResolveRetrieveInstanceUri(instance);

            ValidateUrlRouteContext(
                KnownRouteNames.PartitionRetrieveInstance,
                routeValues =>
                {
                    Assert.Equal(studyInstanceUid, routeValues[KnownActionParameterNames.StudyInstanceUid]);
                    Assert.Equal(seriesInstanceUid, routeValues[KnownActionParameterNames.SeriesInstanceUid]);
                    Assert.Equal(sopInstanceUid, routeValues[KnownActionParameterNames.SopInstanceUid]);
                    Assert.Equal(partitionId, routeValues[KnownActionParameterNames.PartitionId]);
                });
        }

        [Fact]
        public void GivenAnInstance_WhenRetrieveInstanceUriWithPartitionAndVersionIdIsResolved_ThenCorrectUrlShouldBeReturned()
        {
            const string studyInstanceUid = "123.123";
            const string seriesInstanceUid = "456.456";
            const string sopInstanceUid = "789.789";
            const string partitionId = "partition1";
            const string version = "v1.0-prerelease";

            var instance = new InstanceIdentifier(studyInstanceUid, seriesInstanceUid, sopInstanceUid, partitionId);
            _httpContext.Request.RouteValues.Add(KnownActionParameterNames.PartitionId, partitionId);
            _httpContext.Request.RouteValues.Add(KnownActionParameterNames.Version, version);

            _urlResolver.ResolveRetrieveInstanceUri(instance);

            ValidateUrlRouteContext(
                KnownRouteNames.PartitionVersionedRetrieveInstance,
                routeValues =>
                {
                    Assert.Equal(studyInstanceUid, routeValues[KnownActionParameterNames.StudyInstanceUid]);
                    Assert.Equal(seriesInstanceUid, routeValues[KnownActionParameterNames.SeriesInstanceUid]);
                    Assert.Equal(sopInstanceUid, routeValues[KnownActionParameterNames.SopInstanceUid]);
                    Assert.Equal(partitionId, routeValues[KnownActionParameterNames.PartitionId]);
                });
        }

        private void ValidateUrlRouteContext(string routeName, Action<RouteValueDictionary> routeValuesValidator = null)
        {
            Assert.NotNull(_capturedUrlRouteContext);

            Assert.Equal(routeName, _capturedUrlRouteContext.RouteName);
            Assert.Equal(DefaultScheme, _capturedUrlRouteContext.Protocol);
            Assert.Equal(DefaultHost, _capturedUrlRouteContext.Host);

            RouteValueDictionary routeValues = Assert.IsType<RouteValueDictionary>(_capturedUrlRouteContext.Values);

            routeValuesValidator(routeValues);
        }
    }
}
