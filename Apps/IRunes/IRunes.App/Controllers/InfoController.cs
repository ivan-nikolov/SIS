namespace IRunes.App.Controllers
{
    using SIS.MvcFramework;
    using SIS.MvcFramework.Results;

    public class InfoController : Controller
    {
        public ActionResult About()
        {
            return this.View();
        }
    }
}
