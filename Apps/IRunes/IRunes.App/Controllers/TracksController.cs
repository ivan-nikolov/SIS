namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IRunes.App.Extensions;
    using IRunes.Data;
    using IRunes.Models;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes.Http;

    public class TracksController : Controller
    {
        public IHttpResponse Create()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            string albumId = (string)this.Request.QueryData["albumId"];

            this.ViewData["AlbumId"] = albumId;

            return this.View();
        }

        [HttpPost(ActionName = "Create")]
        public IHttpResponse CreateConfirm()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            string albumId = (string)this.Request.QueryData["albumId"];

            using (var context = new RunesDbContext())
            {
                Album album = context.Albums.Find(albumId);

                if (album == null)
                {
                    return this.Redirect("/Albums/All");
                }

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

                album.Tracks.Add(track);
                album.Price = (album.Tracks.Select(t => t.Price).Sum() * 87) / 100;

                context.Update(album);
                context.SaveChanges();
            }


            return this.Redirect($"/Albums/Details?id={albumId}");
        }

        public IHttpResponse Details()
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }

            string trackId = (string)this.Request.QueryData["trackId"];
            string albumId = (string)this.Request.QueryData["albumId"];

            using (var context = new RunesDbContext())
            {
                Track track = context.Tracks.Find(trackId);

                if (track == null)
                {
                    return this.Redirect($"/Albums/Details?id={albumId}");
                }

                this.ViewData["Track"] = track.ToHtmlDetails(albumId);
                this.ViewData["AlbumId"] = albumId;
            }

            return this.View();
        }
    }
}
