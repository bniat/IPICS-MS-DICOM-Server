﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Health.Dicom.Core.Configs;
using Microsoft.Health.Dicom.Core.Web;

namespace Microsoft.Health.Dicom.Api.Web
{
    /// <summary>
    /// Provides functionality to convert stream into a seekable stream.
    /// </summary>
    internal class SeekableStreamConverter : ISeekableStreamConverter
    {
        private const int DefaultBufferThreshold = 1024 * 30000; // 30MB
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<StoreConfiguration> _storeConfiguration;

        public SeekableStreamConverter(IHttpContextAccessor httpContextAccessor, IOptions<StoreConfiguration> storeConfiguration)
        {
            EnsureArg.IsNotNull(httpContextAccessor, nameof(httpContextAccessor));
            EnsureArg.IsNotNull(storeConfiguration?.Value, nameof(storeConfiguration));

            _httpContextAccessor = httpContextAccessor;
            _storeConfiguration = storeConfiguration;
        }

        /// <inheritdoc />
        public async Task<Stream> ConvertAsync(Stream stream, bool streamContainsLimit, CancellationToken cancellationToken = default)
        {
            EnsureArg.IsNotNull(stream, nameof(stream));

            int bufferThreshold = DefaultBufferThreshold;
            long? bufferLimit = null;
            Stream seekableStream = null;

            if (!streamContainsLimit)
            {
                stream = new LimitStream(stream, _storeConfiguration.Value.MaxAllowedDicomFileSize);
            }

            if (!stream.CanSeek)
            {
                try
                {
                    seekableStream = new FileBufferingReadStream(stream, bufferThreshold, bufferLimit, AspNetCoreTempDirectory.TempDirectoryFactory);
                    _httpContextAccessor.HttpContext?.Response.RegisterForDisposeAsync(seekableStream);
                    await seekableStream.DrainAsync(cancellationToken);
                }
                catch (InvalidDataException)
                {
                    // This will result in bad request, we need to handle this differently when we make the processing serial.
                    throw new DicomFileLengthLimitExceededException(_storeConfiguration.Value.MaxAllowedDicomFileSize);
                }
                catch (IOException ex)
                {
                    throw new InvalidMultipartBodyPartException(ex);
                }
            }
            else
            {
                seekableStream = stream;
            }

            seekableStream.Seek(0, SeekOrigin.Begin);

            if(seekableStream.Length > _storeConfiguration.Value.MaxAllowedDicomFileSize)
            {
                throw new DicomFileLengthLimitExceededException(_storeConfiguration.Value.MaxAllowedDicomFileSize);
            }

            return seekableStream;
        }

        private static class AspNetCoreTempDirectory
        {
            private static string _tempDirectory;

            public static string TempDirectory
            {
                get
                {
                    if (_tempDirectory == null)
                    {
                        // Look for folders in the following order.
                        string temp = Environment.GetEnvironmentVariable("ASPNETCORE_TEMP") ?? // ASPNETCORE_TEMP - User set temporary location.
                                      Path.GetTempPath();                                      // Fall back.

                        if (!Directory.Exists(temp))
                        {
                            throw new DirectoryNotFoundException(temp);
                        }

                        _tempDirectory = temp;
                    }

                    return _tempDirectory;
                }
            }

            public static Func<string> TempDirectoryFactory => () => TempDirectory;
        }
    }
}
