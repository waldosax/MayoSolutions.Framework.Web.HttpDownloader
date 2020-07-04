﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MayoSolutions.Framework.Web
{
    public interface IHttpDownloader
    {
        Task<string> GetStringAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );

        Task<Stream> GetStreamAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );

        Task<byte[]> GetBytesAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );

        Task<string> PostStringAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );

        Task<Stream> PostStreamAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );

        Task<byte[]> PostBytesAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );
    }
}