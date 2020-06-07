using System.Collections.Generic;
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
        Task<string> PostStringAsync(
            string url,
            string body,
            IDictionary<string, string> headers = null,
            IWebProxy proxy = null
        );
    }
}