﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MayoSolutions.Framework.Web
{
    public class HttpDownloader : IHttpDownloader
    {
        private readonly HttpDownloaderConfig _config;

        public HttpDownloader()
            : this(new HttpDownloaderConfig())
        {

        }

        public HttpDownloader(HttpDownloaderConfig config)
        {
            _config = config ?? new HttpDownloaderConfig();
        }

        public async Task<string> GetStringAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsStringAsync(),
                null, null, headers, proxy, cancellationToken);
        }

        public async Task<Stream> GetStreamAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                async response =>
                {
                    MemoryStream copy = new MemoryStream();
                    var stream = await response.Content.ReadAsStreamAsync();
                    await stream.CopyToAsync(copy, 1024, cancellationToken);
                    copy.Position = 0L;
                    return copy;
                },
                null, null, headers, proxy, cancellationToken);
        }

        public async Task<byte[]> GetBytesAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsByteArrayAsync(),
                null, null, headers, proxy, cancellationToken);
        }

        public async Task<string> PostStringAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
                response => response.Content.ReadAsStringAsync(),
                body, null, headers, proxy, cancellationToken);
        }

        public async Task<Stream> PostStreamAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
               async response =>
               {
                   MemoryStream copy = new MemoryStream();
                   var stream = await response.Content.ReadAsStreamAsync();
                   await stream.CopyToAsync(copy, 1024, cancellationToken);
                   copy.Position = 0L;
                   return copy;
               },
               body, null, headers, proxy, cancellationToken);
        }

        public async Task<byte[]> PostBytesAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
                response => response.Content.ReadAsByteArrayAsync(),
                body, null, headers, proxy, cancellationToken);
        }

        public async Task<string> PostStringAsync(
            string url,
            IDictionary<string, string> form,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
                response => response.Content.ReadAsStringAsync(),
                null, form, headers, proxy, cancellationToken);
        }

        public async Task<Stream> PostStreamAsync(
            string url,
            IDictionary<string, string> form,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
                async response =>
                {
                    MemoryStream copy = new MemoryStream();
                    var stream = await response.Content.ReadAsStreamAsync();
                    await stream.CopyToAsync(copy, 1024, cancellationToken);
                    copy.Position = 0L;
                    return copy;
                },
                null, form, headers, proxy, cancellationToken);
        }

        public async Task<byte[]> PostBytesAsync(
            string url,
            IDictionary<string, string> form,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
                response => response.Content.ReadAsByteArrayAsync(),
                null, form, headers, proxy, cancellationToken);
        }

        private async Task<T> MakeRequestAsync<T>(
            HttpMethod method,
            string url,
            Func<HttpResponseMessage, Task<T>> handleResponse,
            string body = null,
            IDictionary<string, string> form = null,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
            )
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (proxy != null) httpClientHandler.Proxy = new WebProxy($"{proxy.Host}:{proxy.Port}", false);
            using (HttpClient httpClient = new HttpClient(httpClientHandler))
            {
                if (_config != null && _config.Timeout != null)
                    httpClient.Timeout = _config.Timeout.Value;

                using (HttpRequestMessage request = new HttpRequestMessage(method, url))
                {
                    ConvertHeaders(request, headers);
                    HttpContent content = null;
                    if (method == HttpMethod.Post || method == HttpMethod.Put)
                    {
                        string contentTypeKey = headers?.Keys.FirstOrDefault(k => string.Equals("Content-Type", k));
                        string contentType = !string.IsNullOrEmpty(contentTypeKey)
                            ? headers[contentTypeKey]
                            : null;
                        if (contentType?.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0)
                            content = new StringContent(body, null, contentType);
                        else
                            content = new FormUrlEncodedContent(form);
                    }
                    request.Content = content;

                    using (HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken))
                    {
                        return await handleResponse(response);
                    }
                }
            }
        }


        private void ConvertHeaders(HttpRequestMessage request, IDictionary<string, string> headers)
        {
            if (headers == null) return;
            foreach (var header in headers)
            {
                switch (header.Key.ToLowerInvariant())
                {
                    // TODO: Account for ALL headers
                    case "accept":
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header.Value));
                        break;
                    case "authorization":
                        string authorizationScheme = header.Value;
                        string authorizationParameter = null;
                        if (header.Value?.IndexOf(" ") > 0)
                        {
                            authorizationScheme = header.Value.Substring(0, header.Value.IndexOf(" "));
                            authorizationParameter = header.Value.Substring(header.Value.IndexOf(" ") + 1);
                        }
                        request.Headers.Authorization = new AuthenticationHeaderValue(authorizationScheme, authorizationParameter);
                        break;
                    case "content-type": break;
                    case "referrer":
                        request.Headers.Referrer = new Uri(header.Value);
                        break;
                    case "user-agent":
                        request.Headers.UserAgent.Add(new ProductInfoHeaderValue(header.Value));
                        break;
                    default: request.Headers.Add(header.Key, header.Value); break;
                }
            }
        }
    }
}
