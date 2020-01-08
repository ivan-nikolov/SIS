namespace SIS.MvcFramework.Attributes.Http
{
    using SIS.HTTP.Enums;

    public class HttpPutAttribute : BaseHttpAttribute
    {
        public override HttpRequestMethod Method => HttpRequestMethod.Put;
    }
}
