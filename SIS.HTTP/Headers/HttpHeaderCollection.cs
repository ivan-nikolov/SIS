namespace SIS.HTTP.Headers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;
    using Contracts;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(HttpHeader));

            this.headers[header.Key] = header;
        }

        public bool ContainsHeader(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            return this.headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));

            if (!this.headers.ContainsKey(key))
            {
                return null;
            }

            return this.headers[key];
        }

        public override string ToString() => string.Join(Environment.NewLine, this.headers.Select(v => v.Value.ToString()));
    }
}
