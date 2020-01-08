namespace SIS.MvcFramework.Results
{
    using System.Text;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;

    public class XmlResult : ActionResult
    {
        public XmlResult(string xmlContent, HttpResponseStatusCode statusCode = HttpResponseStatusCode.Ok) 
            : base(statusCode)
        {
            this.AddHeader(new HttpHeader(HttpHeader.ContentType, "application/xml"));
            this.Content = Encoding.UTF8.GetBytes(xmlContent);
        }
    }
}
