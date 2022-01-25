using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MayoSolutions.Framework.Web
{
    public interface IHttpDownloader
    {
        Task<string> GetStringAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<Stream> GetStreamAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<byte[]> GetBytesAsync(
            string url,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<string> PostStringAsync(
            string url,
            IDictionary<string, string> form,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<Stream> PostStreamAsync(
            string url,
            IDictionary<string, string> form,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<byte[]> PostBytesAsync(
            string url,
            IDictionary<string, string> form,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<string> PostStringAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<Stream> PostStreamAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );

        Task<byte[]> PostBytesAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null,
            CancellationToken cancellationToken = default
        );
    }
}