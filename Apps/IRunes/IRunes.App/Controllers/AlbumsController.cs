using System;
using System.Collections.Generic;
using System.Linq;
using IRunes.App.Extensions;
using IRunes.Data;
using IRunes.Models;
using Microsoft.EntityFrameworkCore;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace IRunes.App.Controllers
{
    public class AlbumsController : BaseController
    {
        public IHttpResponse All(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
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

        public IHttpResponse Create(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
            {
                return this.Redirect("/Users/Login");
            }

            return this.View();
        }

        public IHttpResponse CreateConfirm(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
            {
                return this.Redirect("/Users/Login");
            }

            string name = ((ISet<string>)httpRequest.FormData["name"]).FirstOrDefault();
            string cover = ((ISet<string>)httpRequest.FormData["cover"]).FirstOrDefault();

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

        public IHttpResponse Details(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
            {
                return this.Redirect("/Users/Login");
            }

            string albumId = (string)httpRequest.QueryData["id"];

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
