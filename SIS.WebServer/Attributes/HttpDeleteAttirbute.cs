namespace SIS.MvcFramework.Attributes
{
    using SIS.HTTP.Enums;

    public class HttpDeleteAttirbute : BaseHttpAttribute
    {
        public override HttpRequestMethod Method => HttpRequestMethod.Delete;
    }
}
