using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace VinylCollectionApplication
{
    static class Fetch
    {
        public static string FetchBody { get; set; } = "";
        public static string FetchTracklist { get; set; } = "";


        public static async Task findArtistInfo(string artist)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://deezerdevs-deezer.p.rapidapi.com/search?q=" + artist),
                Headers =
                {
                    {"x-rapidapi-key", "d0ad3111f2msh4c67c4f184bb966p182954jsnbdc878e499a7"},
                    {"x-rapidapi-host", "deezerdevs-deezer.p.rapidapi.com"},
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                FetchBody = body;
            }

        }

        public static void findTracklist(string tracklistLink)
        {

            FetchTracklist = new WebClient().DownloadString(tracklistLink);

        }

    }
}