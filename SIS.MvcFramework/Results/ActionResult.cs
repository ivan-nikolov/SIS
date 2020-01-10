namespace SIS.MvcFramework.Results
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses;

    public class ActionResult : HttpResponse
    {
        public ActionResult(HttpResponseStatusCode statusCode)
            : base(statusCode)
        {
        }
    }
}
