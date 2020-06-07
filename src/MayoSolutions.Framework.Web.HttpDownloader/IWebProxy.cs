namespace MayoSolutions.Framework.Web
{
    public interface IWebProxy
    {
        string Host { get; set; }
        int Port { get; set; }
    }
}