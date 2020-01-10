namespace SIS.MvcFramework.ViewEngine
{
    using SIS.MvcFramework.Identity;

    public interface IView
    {
        string GetHtml(object Model, Principal user);
    }
}
