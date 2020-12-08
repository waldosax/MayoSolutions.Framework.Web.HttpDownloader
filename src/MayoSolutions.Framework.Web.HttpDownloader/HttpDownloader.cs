using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MayoSolutions.Framework.Web
{
    public class HttpDownloader : IHttpDownloader
    {

        public async Task<string> GetStringAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsStringAsync(),
                null, headers, proxy);
        }

        public async Task<Stream> GetStreamAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsStreamAsync(),
                null, headers, proxy);
        }

        public async Task<byte[]> GetBytesAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsByteArrayAsync(),
                null, headers, proxy);
        }

        public async Task<string> PostStringAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        )
        {
            return await MakeRequestAsync(HttpMethod.Post, url,
                response => response.Content.ReadAsStringAsync(),
                body, headers, proxy);
        }

        public async Task<Stream> PostStreamAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsStreamAsync(),
                body, headers, proxy);
        }

        public async Task<byte[]> PostBytesAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        )
        {
            return await MakeRequestAsync(HttpMethod.Get, url,
                response => response.Content.ReadAsByteArrayAsync(),
                body, headers, proxy);
        }

        private async Task<T> MakeRequestAsync<T>(
            HttpMethod method,
            string url,
            Func<HttpResponseMessage, Task<T>> handleResponse,
            string body = null,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
            )
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (proxy != null) httpClientHandler.Proxy = new WebProxy($"{proxy.Host}:{proxy.Port}", false);
            using (HttpClient httpClient = new HttpClient(httpClientHandler))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(method, url))
                {
                    ConvertHeaders(request, headers);
                    if (method == HttpMethod.Post || method == HttpMethod.Put)
                    {
                        string contentType = headers?.ContainsKey("content-type") == true
                            ? headers["content-type"]
                            : null;
                        StringContent content = new StringContent(body, null, contentType);
                        request.Content = content;
                    }

                    // TODO: httpClient.PostAsync
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
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
