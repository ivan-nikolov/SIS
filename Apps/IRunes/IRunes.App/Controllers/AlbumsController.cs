namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IRunes.App.Extensions;
    using IRunes.Data;
    using IRunes.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes.Http;
    using SIS.MvcFramework.Results;

    public class AlbumsController : Controller
    {
        public ActionResult All()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            using (var context = new RunesDbContext())
            {
                ICollection<Album> allAlbums = context.Albums.ToList();

                if (allAlbums.Count == 0)
                {
                    this.ViewData["Albums"] = "There are currently no albums.";
                }
                else
                {
                    this.ViewData["Albums"] =
                        string.Join(string.Empty, allAlbums.Select(album => album.ToHtmlAll()).ToList());
                }
            }

            return this.View();
        }

        public ActionResult Create()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            return this.View();
        }

        [HttpPost(ActionName = "Create")]
        public ActionResult CreateConfirm()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            string name = ((ISet<string>)this.Request.FormData["name"]).FirstOrDefault();
            string cover = ((ISet<string>)this.Request.FormData["cover"]).FirstOrDefault();

            Album album = new Album
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Cover = cover,
                Price = 0
            };

            using (var context = new RunesDbContext())
            {
                context.Albums.Add(album);
                context.SaveChanges();
            }

            return this.Redirect("/Albums/All");
        }

        public ActionResult Details()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            string albumId = (string)this.Request.QueryData["id"];

            using (var context = new RunesDbContext())
            {
                Album album = context.Albums
                    .Include(a => a.Tracks)
                    .SingleOrDefault(a => a.Id == albumId);

                if (album == null)
                {
                    return this.Redirect("/Albums/All");
                }

                this.ViewData["Album"] = album.ToHtmlDetails();
            }

            return this.View();
        }
    }
}
