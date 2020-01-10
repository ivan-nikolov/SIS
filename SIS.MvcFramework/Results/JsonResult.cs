namespace SIS.MvcFramework.Results
{
    using System.Text;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;

    public class JsonResult : ActionResult
    {
        public JsonResult(string jsonContent, HttpResponseStatusCode statusCode = HttpResponseStatusCode.Ok) 
            : base(statusCode)
        {
            this.AddHeader(new HttpHeader(HttpHeader.ContentType, "application/json"));
            this.Content = Encoding.UTF8.GetBytes(jsonContent);
        }
    }
}
