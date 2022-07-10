using System;
using System.Security.Cryptography.X509Certificates;

namespace VinylCollectionApplication
{
    public class Vinyl
    {
        public string Album { get; set; } 
        public string Artist { get; set; }
        public Album APIAlbum { get; set; }

        public Vinyl()
        {

        }
        public Vinyl(string album, string artist)
        {
            Album = album;
            Artist = artist;
            APIAlbum = null;
        }

        public Vinyl(string album, string artist, Album api)
        {
            Album = album;
            Artist = artist;
            APIAlbum = api;
        }
    }
}