namespace IRunes.App.Controllers
{
    using System;
    using System.Linq;
    using IRunes.App.ViewModels;
    using IRunes.Models;
    using IRunes.Services;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes.Http;
    using SIS.MvcFramework.Attributes.Security;
    using SIS.MvcFramework.Mapping;
    using SIS.MvcFramework.Results;

    public class TracksController : Controller
    {
        private readonly ITrackService trackService;
        private readonly IAlbumService albumService;

        public TracksController(ITrackService trackService, IAlbumService albumService)
        {
            this.trackService = trackService;
            this.albumService = albumService;
        }

        [Authorize]
        public ActionResult Create()
        {
            string albumId = this.Request.QueryData["albumId"].FirstOrDefault();

            return this.View(new TrackCreateViewModel() { AlbumId = albumId});
        }

        [Authorize]
        [HttpPost(ActionName = "Create")]
        public ActionResult CreateConfirm()
        {
            string albumId = this.Request.QueryData["albumId"].FirstOrDefault();

            string name = this.Request.FormData["name"].FirstOrDefault();
            string link = this.Request.FormData["link"].FirstOrDefault();
            string price = this.Request.FormData["price"].FirstOrDefault();

            Track track = new Track()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Link = link,
                Price = decimal.Parse(price)
            };

            if (!this.albumService.AddTrackToAlbum(albumId, track))
            {
                return this.Redirect($"/Albums/Details?id={albumId}");
            }

            return this.Redirect("/Albums/All");
        }

        [Authorize]
        public ActionResult Details()
        {
            string trackId = this.Request.QueryData["trackId"].FirstOrDefault();
            string albumId = this.Request.QueryData["albumId"].FirstOrDefault();


            Track track = this.trackService.GetTrackById(trackId);

            if (track == null)
            {
                return this.Redirect($"/Albums/Details?id={albumId}");
            }

            var trackViewModel = ModelMapper.ProjectTo<TrackDetailsViewModel>(track);
            trackViewModel.AlbumId = albumId;

            return this.View(trackViewModel);
        }
    }
}
