namespace SIS.MvcFramework.Results
{
    using System.Text;
    using SIS.HTTP.Enums;

    public class NotFoundResult : ActionResult
    {
        public NotFoundResult(string message, HttpResponseStatusCode statusCode = HttpResponseStatusCode.NotFound) 
            : base(statusCode)
        {
            this.Content = Encoding.UTF8.GetBytes(message);
        }
    }
}
