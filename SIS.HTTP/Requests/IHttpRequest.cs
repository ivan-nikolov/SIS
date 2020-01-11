namespace SIS.HTTP.Requests
{
    using System.Collections.Generic;

    using Enums;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Sessions;

    public interface IHttpRequest
    {
        string Path { get; }

        string Url { get; }

        Dictionary<string, ISet<string>> FormData { get; }

        Dictionary<string, ISet<string>> QueryData { get; }

        IHttpHeaderCollection Headers { get; }

        IHttpCookieCollection Cookies { get; }

        HttpRequestMethod RequestMethod { get; }

        IHttpSession Session { get; set; }
    }
}
