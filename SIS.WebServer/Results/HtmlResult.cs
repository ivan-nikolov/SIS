namespace SIS.MvcFramework.Results
{
    using System.Text;

    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class HtmlResult : HttpResponse
    {
        private const string DefaultContentType = "text/html; charset=UTF-8";
        private const string HttpHeaderKey = "Content-Type";

        public HtmlResult(string content, HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            this.Headers.AddHeader(new HttpHeader(HttpHeaderKey, DefaultContentType));
            this.Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
