namespace SIS.MvcFramework.Results
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Responses;

    public class InlineResourceResult : ActionResult
    {
        private const string DefaultContentDisposition = "inline";

        public InlineResourceResult(byte[] content, HttpResponseStatusCode responseStatusCode = HttpResponseStatusCode.Ok)
            : base(responseStatusCode)
        {
            this.Headers.AddHeader(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Headers.AddHeader(new HttpHeader(HttpHeader.ContentDisposition, DefaultContentDisposition));
            this.Content = content;
        }
    }
}
