﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Health.Dicom.Core.Web;
using Microsoft.Net.Http.Headers;

namespace Microsoft.Health.Dicom.Api.Features.Filters
{
    public class AcceptMultipartContentFilterAttribute : AcceptContentFilterAttribute
    {
        public AcceptMultipartContentFilterAttribute(params string[] mediaTypes)
            : base(mediaTypes)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IList<MediaTypeHeaderValue> acceptHeaders = context.HttpContext.Request.GetTypedHeaders().Accept;

            bool acceptable = false;

            // Validate the accept headers has one of the specified accepted media types.
            if (acceptHeaders != null && acceptHeaders.Count > 0)
            {
                var multipartHeaders = acceptHeaders.Where(x => StringSegment.Equals(x.MediaType, KnownContentTypes.MultipartRelated, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (multipartHeaders.Count > 0)
                {
                    IEnumerable<MediaTypeHeaderValue> prospectiveTypes = multipartHeaders.SelectMany(
                        x => x.Parameters.Where(p => StringSegment.Equals(p.Name, TypeParameter, StringComparison.InvariantCultureIgnoreCase))
                            .Select(p => MediaTypeHeaderValue.TryParse(p.Value.ToString().Trim('"'), out MediaTypeHeaderValue parsedValue) ? parsedValue : null));

                    if (prospectiveTypes.Any(x => MediaTypes.Contains(x)))
                    {
                        acceptable = true;
                    }
                }
            }

            if (!acceptable)
            {
                context.Result = new StatusCodeResult(NotAcceptableResponseCode);
            }

            base.OnActionExecuting(context);
        }
    }
}
