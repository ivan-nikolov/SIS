namespace IRunes.Services
{
    using System.Collections.Generic;
    using IRunes.Models;

    public interface IAlbumService 
    {
        List<Album> GetAllAlbums();

        Album CreateAlbum(Album album);

        Album GetAlbumById(string id);

        bool AddTrackToAlbum(string albumId, Track track);
    }
}
