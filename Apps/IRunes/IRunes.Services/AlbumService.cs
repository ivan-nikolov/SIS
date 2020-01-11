namespace IRunes.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IRunes.Data;
    using IRunes.Models;
    using Microsoft.EntityFrameworkCore;

    public class AlbumService : IAlbumService
    {
        private readonly RunesDbContext context;

        public AlbumService(RunesDbContext context)
        {
            this.context = context;
        }

        public bool AddTrackToAlbum(string albumId, Track track)
        {
            Album albumFromDb = this.GetAlbumById(albumId);

            if (albumFromDb == null)
            {
                return false;
            }

            albumFromDb.Tracks.Add(track);
            albumFromDb.Price = (albumFromDb.Tracks.Select(t => t.Price).Sum() * 87) / 100;

            this.context.Update(albumFromDb);
            this.context.SaveChanges();

            return true;
        }

        public Album CreateAlbum(Album album)
        {
            album = this.context.Albums.Add(album).Entity;
            this.context.SaveChanges();

            return album;
        }

        public Album GetAlbumById(string id)
        {
            return context.Albums
                .Include(album => album.Tracks)
                .SingleOrDefault(album => album.Id == id);
        }

        public List<Album> GetAllAlbums()
        {
            return context.Albums.ToList();
        }

        
    }
}
