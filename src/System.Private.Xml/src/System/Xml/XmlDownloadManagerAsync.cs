// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    using System;
    using System.IO;
    using System.Security;
    using System.Collections;
    using System.Net;
    using System.Net.Cache;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;
    using System.Net.Http;
    //
    // XmlDownloadManager
    //
    internal partial class XmlDownloadManager
    {
        internal Task<Stream> GetStreamAsync(Uri uri, ICredentials credentials, IWebProxy proxy,
            RequestCachePolicy cachePolicy)
        {
            if (uri.Scheme == "file")
            {
                return Task.Run<Stream>(() => { return new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, true); });
            }
            else
            {
                return GetNonFileStreamAsync(uri, credentials, proxy, cachePolicy);
            }
        }

        private async Task<Stream> GetNonFileStreamAsync(Uri uri, ICredentials credentials, IWebProxy proxy,
            RequestCachePolicy cachePolicy)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);

            lock (this)
            {
                if (_connections == null)
                {
                    _connections = new Hashtable();
                }
            }

            HttpResponseMessage resp = await client.SendAsync(req).ConfigureAwait(false);

            Stream respStream = new MemoryStream();
            await resp.Content.CopyToAsync(respStream);
            return respStream;
        }
    }
}
