﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using Dicom;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.Common;
using Microsoft.Health.Dicom.Core.Features.CustomTag;

namespace Microsoft.Health.Dicom.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="CustomTagEntry"/>.
    /// </summary>
    internal static class CustomTagEntryExtensions
    {
        /// <summary>
        /// Normalize custom tag entry before saving to CustomTagStore.
        /// </summary>
        /// <param name="customTagEntry">The custom tag entry.</param>
        /// <returns>Normalize custom tag entry.</returns>
        public static CustomTagEntry Normalize(this CustomTagEntry customTagEntry)
        {
            DicomTagParser dicomTagParser = new DicomTagParser();
            DicomTag[] tags;
            if (!dicomTagParser.TryParse(customTagEntry.Path, out tags, supportMultiple: false))
            {
                // not a valid dicom tag path
                throw new CustomTagEntryValidationException(
                    string.Format(CultureInfo.InvariantCulture, DicomCoreResource.InvalidCustomTag, customTagEntry));
            }

            DicomTag tag = tags[0];
            string path = tag.GetPath();
            string vr = customTagEntry.VR;

            // when VR is not specified for standard tag,
            if (!tag.IsPrivate && tag.DictionaryEntry != DicomDictionary.UnknownTag)
            {
                if (string.IsNullOrWhiteSpace(vr))
                {
                    vr = tag.GetDefaultVR()?.Code;
                }
            }

            vr = vr.ToUpperInvariant();

            return new CustomTagEntry { Path = path, VR = vr, Level = customTagEntry.Level, Status = customTagEntry.Status };
        }

        public static void SplitStandardAndPrivateTags(this IEnumerable<CustomTagEntry> customTagEntries, out IDictionary<string, CustomTagEntry> standardTags, out IDictionary<string, CustomTagEntry> privateTags)
        {
            standardTags = new Dictionary<string, CustomTagEntry>();
            privateTags = new Dictionary<string, CustomTagEntry>();
            foreach (var customTagEntry in customTagEntries)
            {
                DicomTag dicomTag = DicomTag.Parse(customTagEntry.Path);
                if (dicomTag.IsPrivate)
                {
                    privateTags.Add(customTagEntry.Path, customTagEntry);
                }
                else
                {
                    standardTags.Add(customTagEntry.Path, customTagEntry);
                }
            }
        }
    }
}
