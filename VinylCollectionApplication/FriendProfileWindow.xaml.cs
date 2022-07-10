using BespokeFusion;
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
using System.Windows.Shapes;

namespace VinylCollectionApplication
{
    /// <summary>
    /// Interaction logic for FriendProfileWindow.xaml
    /// </summary>
    public partial class FriendProfileWindow : Window
    {
        //MEMBERS
        public Friend selectedFriend;
        private Vinyl selectedVinyl;
        private MediaPlayer player { get; set; } = new MediaPlayer();
        public string fileName { get; set; } = "mp3preview.wav";

        public FriendProfileWindow()
        {
            InitializeComponent();
            selectedFriend = ((MainWindow)Application.Current.MainWindow).getSelectedFriend();
            ((MainWindow)Application.Current.MainWindow).RefreshFriends();
            PopulateFriendsVinylList();
            player = new MediaPlayer();

            if (selectedFriend == null)
            {
                Close();
            }
            //change the temp spots
            FriendsNameTextBlock.Text = selectedFriend.firstName +" "+ selectedFriend.lastName;
            VinylInCollectionTextBlock.Text = selectedFriend.Collection.Count().ToString();

        }
        public void PopulateFriendsVinylList()
       {
            if (selectedFriend.Collection != null)
            {
                foreach (var vinyl in selectedFriend.Collection)
                {
                    ListViewItem newItem = new ListViewItem();
                    newItem.Tag = vinyl;
                    FriendsCollectionListView.Items.Add(vinyl.Album);
                }
            }
            else
            {
                MaterialMessageBox.ShowWarning($"{selectedFriend.firstName} {selectedFriend.lastName} does not have any vinyl in their collection yet.");
            }
        }

        private void SortByArtistRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            selectedFriend.Collection.Sort((x, y) => x.Artist.CompareTo(y.Artist));
            PopulateFriendsVinylList();
        }

        private void SortByAlbumRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            selectedFriend.Collection.Sort((x, y) => x.Artist.CompareTo(y.Artist));
            PopulateFriendsVinylList();
        }

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

        private void FriendsCollectionListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FriendsCollectionListView.SelectedIndex == -1)
                return;

            selectedVinyl = selectedFriend.Collection.Where(r => r.Album == FriendsCollectionListView.SelectedItem.ToString())
                .FirstOrDefault();
            if (selectedVinyl.APIAlbum != null)
            {
                var request = WebRequest.Create(selectedVinyl.APIAlbum.cover_medium);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    SelectedVinylImage.Source = ToImageSource(Bitmap.FromStream(stream), ImageFormat.Png);
                }
            }

            TrackListView.Items.Clear();
            for (int i = 0; i < selectedVinyl.APIAlbum.tracks.Length; i++)
            {
                TrackListView.Items.Add(i + 1 + ". " + selectedVinyl.APIAlbum.tracks[i].title);
            }

            SelectedVinylAlbum.Text = selectedVinyl.Album;
            SelectedVinylArtist.Text = selectedVinyl.Artist;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void PreviewRightClick_OnClick(object sender, RoutedEventArgs e)
        {
            SongPreviewWindow window = new SongPreviewWindow(selectedVinyl, TrackListView.SelectedIndex);
            window.ShowDialog();
        }
        public void ClearFields()
        {
            SelectedVinylAlbum.Text = "";
            SelectedVinylArtist.Text = "";
            System.Drawing.Image white = System.Drawing.Image.FromFile("C:\\CodingApplications\\VinylCollectionApplication\\VinylCollectionApplication\\Gifs\\blank.jpg");
            SelectedVinylImage.Source = ToImageSource(white, ImageFormat.Png);
            TrackListView.Items.Clear();
        }
       
        private void SearchVinyl_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (SearchVinyl.Text == "")
            {
                FriendsCollectionListView.Items.Clear();
                PopulateFriendsVinylList();
            }
            else
            {
                FriendsCollectionListView.Items.Clear();
                string textInput = SearchVinyl.Text;
                foreach (Vinyl vinyl in selectedFriend.Collection.Where(r =>
                    r.Album.ToLower().Contains(textInput.ToLower()) || r.Artist.ToLower().Contains(textInput.ToLower())))
                    FriendsCollectionListView.Items.Add(vinyl.Album);
            }
        }

        private void TrackListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SongPreviewWindow window = new SongPreviewWindow(selectedVinyl, TrackListView.SelectedIndex);
            window.ShowDialog();
        }

        private void FriendsCollectionListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SongPreviewWindow window = new SongPreviewWindow(selectedVinyl, 0);
            window.ShowDialog();
        }
    }
}
