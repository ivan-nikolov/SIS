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
        public IHttpResponse Create(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
            {
                return this.Redirect("/Users/Login");
            }

            string albumId = (string)httpRequest.QueryData["albumId"];

            this.ViewData["AlbumId"] = albumId;

            return this.View();
        }

        [HttpPost(ActionName = "Create")]
        public IHttpResponse CreateConfirm(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
            {
                return this.Redirect("/Users/Login");
            }

            string albumId = (string)httpRequest.QueryData["albumId"];

            using (var context = new RunesDbContext())
            {
                Album album = context.Albums.Find(albumId);

                if (album == null)
                {
                    return this.Redirect("/Albums/All");
                }

                string name = ((ISet<string>)httpRequest.FormData["name"]).FirstOrDefault();
                string link = ((ISet<string>)httpRequest.FormData["link"]).FirstOrDefault();
                string price = ((ISet<string>)httpRequest.FormData["price"]).FirstOrDefault();

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

        public IHttpResponse Details(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn(httpRequest))
            {
                return this.Redirect("/Users/Login");
            }

            string trackId = (string)httpRequest.QueryData["trackId"];
            string albumId = (string)httpRequest.QueryData["albumId"];

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
