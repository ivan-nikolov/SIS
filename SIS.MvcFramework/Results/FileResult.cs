namespace SIS.MvcFramework.Results
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;

    public class FileResult : ActionResult
    {
        private const string DefaultContentDisposition = "attachment";

        public FileResult(byte[] fileContent, HttpResponseStatusCode statusCode = HttpResponseStatusCode.Ok) 
            : base(statusCode)
        {
            this.Headers.AddHeader(new HttpHeader(HttpHeader.ContentLength, fileContent.Length.ToString()));
            this.Headers.AddHeader(new HttpHeader(HttpHeader.ContentDisposition, DefaultContentDisposition));
            this.Content = fileContent;
        }
    }
}
