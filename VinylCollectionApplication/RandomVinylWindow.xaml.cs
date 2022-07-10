using BespokeFusion;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SongPreviewWindow.xaml
    /// </summary>
    public partial class RandomVinylWindow : Window
    {
        public Account currentUser;
        public Vinyl selectedVinyl;
        public TrackItem selectedSong;
        public int currentIndex;
        ISimpleAudioPlayer player;

        public RandomVinylWindow()
        {
            InitializeComponent();
            currentUser = ((MainWindow)Application.Current.MainWindow).getCurrentUser();
            Random random = new Random();
            selectedVinyl = currentUser.Collection[random.Next(currentUser.Collection.Count)];
            currentIndex = random.Next(selectedVinyl.APIAlbum.tracks.Count());
            selectedSong = selectedVinyl.APIAlbum.tracks[currentIndex];
            player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Volume = 0.3;
            VolumeSlider.Value = 0.3;
            VinylPopulation();
        }

        private void VinylPopulation()
        {
            PreviewVinylArtist.Text = selectedVinyl.Artist;
            PreviewVinylAlbum.Text = selectedVinyl.Album;
            PreviewViynlSong.Text = selectedSong.title;
            PreviewTrackListVinylAlbum.Text = selectedVinyl.Album;
            if (selectedVinyl.APIAlbum != null)
            {
                var request = WebRequest.Create(selectedVinyl.APIAlbum.cover_medium);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    RandomVinylImage.Source = ToImageSource(Bitmap.FromStream(stream), ImageFormat.Png);
                }
            }

            PreviewTrackListView.Items.Clear();
            for (int i = 0; i < selectedVinyl.APIAlbum.tracks.Length; i++)
            {
                int songIndex = i + 1;
                if (selectedVinyl.APIAlbum.tracks[i].title == selectedSong.title)
                    PreviewTrackListView.Items.Add("• " + songIndex + ". " + selectedVinyl.APIAlbum.tracks[i].title);
                else
                    PreviewTrackListView.Items.Add(songIndex + ". " + selectedVinyl.APIAlbum.tracks[i].title);
            }
            try
            {
                WebClient wc = new WebClient();
                Stream fileStream = wc.OpenRead(selectedSong.preview);
                player.Load(fileStream);
                player.Play();
                player.Volume = .3;
            }
            catch (WebException)
            {
                MaterialMessageBox.ShowError("Resource not found");
            }

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

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            player.Stop();
            ((MainWindow)Application.Current.MainWindow).rerollVinyl = false;
            Close();

        }
        private void StopMusic_OnClick(object sender, RoutedEventArgs e)
        {
            player.Stop();

        }

        private void PlayMusic_OnClick(object sender, RoutedEventArgs e)
        {
            player.Play();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = VolumeSlider.Value;
        }

        private void PauseMusic_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void PreviewTrackListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                player.Stop();
                player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                currentIndex = PreviewTrackListView.SelectedIndex;
                selectedSong = selectedVinyl.APIAlbum.tracks[currentIndex];
                VinylPopulation();
            }
            catch (IndexOutOfRangeException) { }
        }

        private void PreviousMusic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                player.Stop();
                player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                currentIndex--;
                selectedSong = selectedVinyl.APIAlbum.tracks[currentIndex];
                VinylPopulation();
            }
            catch (IndexOutOfRangeException)
            {
                System.Media.SystemSounds.Hand.Play();
                currentIndex++;
            }

        }

        private void GetNewRandomVinyl_OnClick(object sender, RoutedEventArgs e)
        {
            player.Stop();
            ((MainWindow)Application.Current.MainWindow).rerollVinyl = true;
            Close();
        }

        private void NextMusic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                player.Stop();
                player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                currentIndex++;
                selectedSong = selectedVinyl.APIAlbum.tracks[currentIndex];
                VinylPopulation();
            }
            catch (IndexOutOfRangeException)
            {
                System.Media.SystemSounds.Hand.Play();
                currentIndex--;
            }
        }
    }
}


