namespace SIS.HTTP.Cookies
{
    using System.Collections;
    using System.Collections.Generic;
    using SIS.Common;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private readonly Dictionary<string, HttpCookie> httpCookies;

        public HttpCookieCollection()
        {
            this.httpCookies = new Dictionary<string, HttpCookie>();
        }

        public void AddCookie(HttpCookie httpCookie)
        {
            httpCookie.ThrowIfNull(nameof(httpCookie));

            if (!this.ContainsCookie(httpCookie.Key))
            {
                this.httpCookies.Add(httpCookie.Key, httpCookie);
            }
        }

        public bool ContainsCookie(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            return this.httpCookies.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            HttpCookie httpCookie = null;

            if (this.httpCookies.ContainsKey(key))
            {
                httpCookie = this.httpCookies[key];
            }

            return httpCookie;
        }

        public bool HasCookies()
        {
            return this.httpCookies.Count != 0;
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            return this.httpCookies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
