namespace SIS.HTTP.Headers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SIS.Common;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void AddHeader(HttpHeader header)
        {
            header.ThrowIfNull(nameof(HttpHeader));

            this.headers[header.Key] = header;
        }

        public bool ContainsHeader(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            return this.headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            key.ThrowIfNullOrEmpty(nameof(key));

            HttpHeader header = null;

            if (this.headers.ContainsKey(key))
            {
                header = this.headers[key];
            }

            return header;
        }

        public override string ToString() => string.Join(Environment.NewLine, this.headers.Select(v => v.Value.ToString()));
    }
}
