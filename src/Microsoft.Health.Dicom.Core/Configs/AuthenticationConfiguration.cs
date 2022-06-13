﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Health.Dicom.Core.Configs;

public class AuthenticationConfiguration
{
    public string Audience { get; set; }

    public IEnumerable<string> Audiences { get; set; }

    public string Authority { get; set; }

    /// <summary>
    /// Gets or sets the clientID used by MISE.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// True to disable JwtBearer Authentication so that could use MISE, false otherwise.
    /// </summary>
    public bool DisableJwtBearer { get; set; }
}
