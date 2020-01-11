namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
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
            string albumId = (string)this.Request.QueryData["albumId"];

            return this.View(new TrackCreateViewModel() { AlbumId = albumId});
        }

        [Authorize]
        [HttpPost(ActionName = "Create")]
        public ActionResult CreateConfirm()
        {
            string albumId = (string)this.Request.QueryData["albumId"];

            string name = ((ISet<string>)this.Request.FormData["name"]).FirstOrDefault();
            string link = ((ISet<string>)this.Request.FormData["link"]).FirstOrDefault();
            string price = ((ISet<string>)this.Request.FormData["price"]).FirstOrDefault();

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
            string trackId = (string)this.Request.QueryData["trackId"];
            string albumId = (string)this.Request.QueryData["albumId"];


            Track track = this.trackService.GetTrackById(trackId);

            if (track == null)
            {
                return this.Redirect($"/Albums/Details?id={albumId}");
            }

            //this.ViewData["Track"] = track.ToHtmlDetails(albumId);
            //this.ViewData["AlbumId"] = albumId;

            var trackViewModel = ModelMapper.ProjectTo<TrackDetailsViewModel>(track);
            trackViewModel.AlbumId = albumId;

            return this.View(trackViewModel);
        }
    }
}
