using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace VinylCollectionApplication
{
    /// <summary>
    /// Interaction logic for NewVinylWindow.xaml
    /// </summary>
    public partial class NewVinylWindow : Window
    {
        public Vinyl newVinyl;
        public string uploadedImage;
        public List<Album> albums;
        public Album selectedAlbum;
        public Account currentUser;
        public Vinyl highlightedVinyl;
        public static SnackbarMessageQueue snackbarMessageQueue { get; set; }
        public NewVinylWindow()
        {
            InitializeComponent();
            currentUser = ((MainWindow)Application.Current.MainWindow).getCurrentUser();
            snackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            messageSnackBar.MessageQueue = snackbarMessageQueue;
            newVinyl = new Vinyl();
            albums = new List<Album>();

        }
        
        //private void btnLoad_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog fd = new OpenFileDialog();
        //    if (fd.ShowDialog() == true)
        //    {
        //        imgPhoto.Source = new BitmapImage(new Uri(fd.FileName));
        //        Stream stream = File.OpenRead(fd.FileName);
        //        stream = File.OpenRead(fd.FileName);
        //        byte[] binaryImage = new byte[stream.Length];
        //        stream.Read(binaryImage, 0, (int)stream.Length);
        //        uploadedImage = Convert.ToBase64String(binaryImage, 0, (int) stream.Length);
        //    }
        //}

        //private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    newVinyl.Album = AlbumNameTextBox.Text;
        //    newVinyl.Artist = ArtistNameTextBox.Text;

        //    Collection.addVinyl(newVinyl);
            
        //    Close();
        //}
        public static ImageSource ToImageSource(System.Drawing.Image image, ImageFormat imageFormat)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save to the stream
                image.Save(stream, imageFormat);

                // Rewind the stream
                stream.Seek(0, SeekOrigin.Begin);

                // Tell the WPF BitmapImage to use this stream
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }

            return bitmap;
        }
        


        private void APIListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (APIListView.SelectedIndex == -1)
            {
                return;
            }
            Album highlightedAlbum = new Album();
            if (albums.Count != 0 && APIListView.SelectedIndex != -1)
            {
                highlightedAlbum = albums[APIListView.SelectedIndex];
                highlightedVinyl = new Vinyl(albums[APIListView.SelectedIndex].title, albums[APIListView.SelectedIndex].artist.name, albums[APIListView.SelectedIndex]);
                selectedAlbum = highlightedAlbum;

                WebRequest request = WebRequest.Create(selectedAlbum.cover_medium);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                     APIVinylImage.Source = ToImageSource(Bitmap.FromStream(stream), ImageFormat.Png);
                }

                SelectedAPIVinylAlbum.Text = highlightedAlbum.title;
                SelectedAPIVinylArtist.Text = highlightedAlbum.artist.name;
                Fetch.findTracklist(highlightedAlbum.tracklist);
                FetchTrackList trackListData = JsonConvert.DeserializeObject<FetchTrackList>(Fetch.FetchTracklist);
                while (trackListData == null)
                {

                }

                highlightedAlbum.tracks = trackListData.data;
                TrackListView.Items.Clear();
                for(int i =0; i < highlightedAlbum.tracks.Length; i++)
                {
                    TrackListView.Items.Add(i + 1 + ". " + highlightedAlbum.tracks[i].title);
                }
                AddToCollectionButton.IsEnabled = true;
            }
        }

        private void AddToCollectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (currentUser.isAbleToAdd())
            {
                if (APIListView.SelectedIndex == -1)
                {
                    return;
                }
                bool isUnique = true;
                foreach (Vinyl v in currentUser.Collection)
                {
                    if (v.Album == selectedAlbum.title && v.APIAlbum.artist.name == selectedAlbum.artist.name)
                    {
                        isUnique = false;
                    }
                }

                if (selectedAlbum != null && isUnique)
                {
                    currentUser.Collection.Add(new Vinyl(selectedAlbum.title, selectedAlbum.artist.name, selectedAlbum));
                    if (currentUser.isPremium)
                    {
                        snackbarMessageQueue.Clear();
                        snackbarMessageQueue.Enqueue($"{selectedAlbum.title} by {selectedAlbum.artist.name} has been added to your collection.");
                    }
                    if (!currentUser.isPremium)
                    {
                        snackbarMessageQueue.Clear();
                        snackbarMessageQueue.Enqueue($"{selectedAlbum.title} by {selectedAlbum.artist.name} has been added to your collection. {25 - currentUser.Collection.Count} Spots left in your Free Collection");
                    }

                    }
                    else
                {
                    snackbarMessageQueue.Enqueue("This album is already in your collection.");
                }
            }
            else
            {
                snackbarMessageQueue.Enqueue("You have reached the maximum number of records for a free account.");
            }
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            SearchAPI();
        }

        private void PreviewRightClick_OnClick(object sender, RoutedEventArgs e)
        {
            if (currentUser.isPremium)
            {
                SongPreviewWindow window = new SongPreviewWindow(highlightedVinyl, TrackListView.SelectedIndex);
                window.ShowDialog();
            }
            else {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
             }
        }

        private void APISearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SearchAPI();

            }
        }

        private async void SearchAPI()
        {
            if (APISearchBox.Text != "")
            {
                APIListView.Items.Clear();
                albums.Clear();
                await Fetch.findArtistInfo(APISearchBox.Text.ToLower());
                if (Fetch.FetchBody != "")
                {
                    FetchData vinylCollection = JsonConvert.DeserializeObject<FetchData>(Fetch.FetchBody);
                    foreach (Track track in vinylCollection.data)
                    {
                        bool contains = false;
                        foreach (Album album in albums)
                        {
                            if (album.title == track.album.title)
                                contains = true;
                        }
                        if (!contains)
                        {
                            Album tempAlbum = track.album;
                            tempAlbum.artist = track.artist;
                            albums.Add(track.album);
                        }
                    }
                }

                if (albums.Count != 0)
                {
                    foreach (Album album in albums)
                    {
                        APIListView.Items.Add(album.title);
                    }
                }

            }
        }

        private void TrackListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TrackListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.isPremium)
            {
                SongPreviewWindow window = new SongPreviewWindow(highlightedVinyl, TrackListView.SelectedIndex);
                window.ShowDialog();
            }
            else
            {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
            }
        }
    }
}
