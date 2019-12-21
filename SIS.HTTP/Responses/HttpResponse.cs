namespace SIS.HTTP.Responses
{
    using System.Text;
    using Enums;

    using Headers;
    using Headers.Contracts;

    using Responses.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Cookies.Contracts;
    using SIS.HTTP.Extensions;

    public class HttpResponse : IHttpResponse
    {
        public HttpResponse()
        {
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.Content = new byte[0];
        }

        public HttpResponse(HttpResponseStatusCode statusCode)
            : this()
        {
            CoreValidator.ThrowIfNull(statusCode, nameof(statusCode));
            this.StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; }
        
        public IHttpCookieCollection Cookies { get; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            CoreValidator.ThrowIfNull(header, nameof(header));
            this.Headers.AddHeader(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));

            this.Cookies.AddCookie(cookie);
        }

        public byte[] GetBytes()
        {
            byte[] httpResponseWithoutBody = Encoding.UTF8.GetBytes(this.ToString());
            byte[] httpResponseWithBody = new byte[httpResponseWithoutBody.Length + this.Content.Length];

            for (int i = 0; i < httpResponseWithoutBody.Length; i++)
            {
                httpResponseWithBody[i] = httpResponseWithoutBody[i];
            }

            int counter = httpResponseWithoutBody.Length;
            for (int i = 0; i < this.Content.Length; i++)
            {
                httpResponseWithBody[counter++] = this.Content[i];
            }

            string responseHeaders = Encoding.UTF8.GetString(Content);
            string responseString = Encoding.UTF8.GetString(httpResponseWithBody);

            return httpResponseWithBody;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result
                .Append($"{GlobalConstants.HttpOneProtocolFragment} {this.StatusCode.GetStatusLine()}")
                .Append(GlobalConstants.HttpNewLine)
                .Append($"{this.Headers}")
                .Append(GlobalConstants.HttpNewLine);

            if (this.Cookies.HasCookies())
            {
                foreach (var cookie in this.Cookies)
                {
                    result.Append($"Set-Cookie: {cookie}").Append(GlobalConstants.HttpNewLine);
                }
            }

            result.Append(GlobalConstants.HttpNewLine);

            return result.ToString();
        }
    }
}
