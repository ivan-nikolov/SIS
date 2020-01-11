using SIS.MvcFramework.Identity;

namespace SIS.MvcFramework.ViewEngine
{
    public class ErrorView : IView
    {
        private readonly string errors;

        public ErrorView(string errors)
        {
            this.errors = errors;
        }

        public string GetHtml(object Model, Principal user)
        {
            return errors;
        }
    }
}
