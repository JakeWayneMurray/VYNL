namespace VinylCollectionApplication
{
    public class FetchTrackList
    {
        public TrackItem[] data { get; set; }
    }

    public class TrackItem
    {
        public int id { get; set; }
        public bool readable { get; set; }
        public string title { get; set; }
        public string title_short { get; set; }
        public string title_version { get; set; }
        public string isrc { get; set; }
        public string link { get; set; }
        public int duration { get; set; }
        public int track_position { get; set; }
        public int disk_number { get; set; }
        public int rank { get; set; }
        public bool explicit_lyrics { get; set; }
        public int explicit_content_lyrics { get; set; }
        public int explicit_content_cover { get; set; }
        public string preview { get; set; }
        public string md5_image { get; set; }
        public TrackArtist artist { get; set; }
        public string type { get; set; }
    }

    public class TrackArtist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tracklist { get; set; }
        public string type { get; set; }
    }
}