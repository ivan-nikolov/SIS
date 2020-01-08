namespace SIS.MvcFramework.Results
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Responses;

    public class InlineResourceResult : HttpResponse
    {
        private const string DefaultContentDisposition = "inline";

        public InlineResourceResult(byte[] content, HttpResponseStatusCode responseStatusCode)
            : base(responseStatusCode)
        {
            this.Headers.AddHeader(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Headers.AddHeader(new HttpHeader(HttpHeader.ContentDisposition, DefaultContentDisposition));
            this.Content = content;
        }
    }
}
