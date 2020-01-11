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

    public class AlbumsController : Controller
    {
        private readonly IAlbumService albumService;

        public AlbumsController(IAlbumService albumService)
        {
            this.albumService = albumService;
        }

        [Authorize]
        public ActionResult All()
        {
            ICollection<Album> allAlbums = albumService.GetAllAlbums();

            if (allAlbums.Count != 0)
            {
                return this.View(allAlbums.Select(ModelMapper.ProjectTo<AlbumAllViewModel>).ToList());
            }

            return this.View();
        }

        [Authorize]
        public ActionResult Create()
        {
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(string name, string cover)
        {
            Album album = new Album
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Cover = cover,
                Price = 0
            };

            this.albumService.CreateAlbum(album);

            return this.Redirect("/Albums/All");
        }

        [Authorize]
        public ActionResult Details(string id)
        {
            Album albumFromDb = albumService
                .GetAlbumById(id);

            if (albumFromDb == null)
            {
                return this.Redirect("/Albums/All");
            }

            AlbumDetailslViewModel album = ModelMapper.ProjectTo<AlbumDetailslViewModel>(albumFromDb);

            return this.View(album);
        }
    }
}
