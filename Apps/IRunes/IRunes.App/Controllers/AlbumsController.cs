﻿namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IRunes.App.Extensions;
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
        private IAlbumService albumService;

        public AlbumsController()
        {
            this.albumService = new AlbumService();
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
        [HttpPost(ActionName = "Create")]
        public ActionResult CreateConfirm()
        {
            string name = ((ISet<string>)this.Request.FormData["name"]).FirstOrDefault();
            string cover = ((ISet<string>)this.Request.FormData["cover"]).FirstOrDefault();

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
        public ActionResult Details()
        {
            string albumId = (string)this.Request.QueryData["id"];


            Album albumFromDb = albumService
                .GetAlbumById(albumId);

            if (albumFromDb == null)
            {
                return this.Redirect("/Albums/All");
            }

            AlbumDetailslViewModel album = ModelMapper.ProjectTo<AlbumDetailslViewModel>(albumFromDb);


            return this.View(album);
        }
    }
}
