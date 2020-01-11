namespace IRunes.App.Controllers
{
    using System;
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
        public ActionResult Create(string albumId)
        {
            return this.View(new TrackCreateViewModel() { AlbumId = albumId});
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreateTrackInputModel model)
        {
            Track track = new Track()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Link = model.Link,
                Price = model.Price
            };

            if (!this.albumService.AddTrackToAlbum(model.AlbumId, track))
            {
                return this.Redirect($"/Albums/Details?id={model.AlbumId}");
            }

            return this.Redirect("/Albums/All");
        }

        [Authorize]
        public ActionResult Details(string trackId, string albumId)
        {
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
