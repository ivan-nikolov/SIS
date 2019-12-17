namespace SIS.WebServer.Results
{
    using System.Text;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Responses;

    public class TextResult : HttpResponse
    {
        private const string DefaultContentType = "text/plain; charset=utf-8";
        private const string HttpHeaderKey = "Content-Type";

        public TextResult(string content, HttpResponseStatusCode responseStatusCode, string contentType = DefaultContentType)
            : base(responseStatusCode)
        {
            this.Headers.AddHeader(new HttpHeader(HttpHeaderKey, contentType));
            this.Content = Encoding.UTF8.GetBytes(content);
        }

        public TextResult(byte[] content, HttpResponseStatusCode responseStatusCode, string contentType = DefaultContentType)
            : base(responseStatusCode)
        {
            this.Headers.AddHeader(new HttpHeader(HttpHeaderKey, contentType));
            this.Content = content;
        }
    }
}
