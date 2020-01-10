namespace SIS.MvcFramework.Attributes.Http
{
    using SIS.HTTP.Enums;

    public class HttpDeleteAttirbute : BaseHttpAttribute
    {
        public override HttpRequestMethod Method => HttpRequestMethod.Delete;
    }
}
