using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aspose.Pdf;
using VinylCollectionApplication.FetchInfo;
using BespokeFusion;
using Image = System.Drawing.Image;
using Page = Aspose.Pdf.Page;
using Aspose.Pdf.Text;
using System.Runtime.InteropServices;

namespace VinylCollectionApplication 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //MEMBERS
        public Account currentUser;

        private Vinyl selectedVinyl;
        private Aspose.Pdf.Document document;
        public bool rerollVinyl { get; set; }
        Friend selectedFriend;
        private int privateCounter;

        public MainWindow()
        {
            InitializeComponent();
            document = new Aspose.Pdf.Document();
            LoginWindow login = new LoginWindow();
            login.ShowDialog();
            privateCounter = 0;

            if (currentUser == null)
            {
                Close();
                return;
            }
            RefreshFriends();
            PopulateVinylList();
            PopulateFriendsList();
            rerollVinyl = false;
            FriendsListView.SelectedItem = -1;
            if (currentUser.friends.Count > 0)
            {
                selectedFriend = currentUser.friends[0];
            }

        }

        public Friend getSelectedFriend()
        {
            return selectedFriend;
        }

        public Account getCurrentUser()
        {
            return currentUser;
        }
        internal void setUser(Account u)
        {
            currentUser = u;
        }

        public void PopulateVinylList()
        {
            if (currentUser == null) return;
            VinylListView.Items.Clear();
            foreach (var vinyl in currentUser.Collection)
            {
                ListViewItem newItem = new ListViewItem();
                newItem.Tag = vinyl;
                VinylListView.Items.Add(vinyl.Album);
            }

        }

        public void PopulateFriendsList()
        {
            if (currentUser == null) return;

            FriendsListView.Items.Clear();
            foreach (Friend friend in currentUser.friends)
            {
                ListViewItem newItem = new ListViewItem();
                newItem.Tag = friend;
                FriendsListView.Items.Add(friend.firstName + " " + friend.lastName);
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

        private void VinylListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VinylListView.SelectedIndex == -1)
                return;

            selectedVinyl = currentUser.Collection.Where(r => r.Album == VinylListView.SelectedItem.ToString())
                .FirstOrDefault();
            if (selectedVinyl.APIAlbum != null)
            {
                if (currentUser.isPremium)
                {
                    var request = WebRequest.Create(selectedVinyl.APIAlbum.cover_medium);
                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        SelectedVinylImage.Source = ToImageSource(Bitmap.FromStream(stream), ImageFormat.Png);
                    }
                }
                else
                {
                    Image white = Image.FromFile("C:\\CodingApplications\\VinylCollectionApplicationNonSpotify\\VinylCollectionApplication\\Images\\blank.png");
                    SelectedVinylImage.Source = ToImageSource(white, ImageFormat.Png);
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
        private void SearchVinyl_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchVinyl.Text == "")
            {
                VinylListView.Items.Clear();
                PopulateVinylList();
            }
            else
            {
                VinylListView.Items.Clear();
                string textInput = SearchVinyl.Text;
                foreach (Vinyl vinyl in currentUser.Collection.Where(r =>
                    r.Album.ToLower().Contains(textInput.ToLower()) || r.Artist.ToLower().Contains(textInput.ToLower())))
                    VinylListView.Items.Add(vinyl.Album);
            }
        }

        private void SaveCollectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentUser.upload();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.L)
            {
                LoadJSONWindow window = new LoadJSONWindow();
                window.ShowDialog();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                //Collection.updateCollection();
                currentUser.upload();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                //  Collection.SaveListAs();
            }
            if (e.Key == Key.R)
            {
                do
                {
                    RandomVinylWindow window = new RandomVinylWindow();
                    window.ShowDialog();
                } while (rerollVinyl);
            }
        }

        private void PreviewRightClick_OnClick(object sender, RoutedEventArgs e)
        {
            if (currentUser.isPremium)
            {
                SongPreviewWindow window = new SongPreviewWindow(selectedVinyl, TrackListView.SelectedIndex);
                window.ShowDialog();
            }
            else
            {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
            }
        }

        private void SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            currentUser.upload();
        }
        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            int previousCount = currentUser.Collection.Count();
            LoadJSONWindow window = new LoadJSONWindow();
            window.ShowDialog();

        }

        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            //Collection.newCollection();
        }

        public void ClearFields()
        {
            SelectedVinylAlbum.Text = "";
            SelectedVinylArtist.Text = "";
            TrackListView.Items.Clear();
        }

        private void NewVinylButton_OnClick(object sender, RoutedEventArgs e)
        {
            NewVinylWindow window = new NewVinylWindow();
            window.ShowDialog();
            PopulateVinylList();
            currentUser.upload();
        }

        private void DeleteMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedVinyl != null) {
                for (int i = 0; i < currentUser.Collection.Count; i++)
                {
                    if (currentUser.Collection[i].Album == selectedVinyl.Album &&
                        currentUser.Collection[i].Artist == selectedVinyl.Artist)
                    {
                        currentUser.Collection.RemoveAt(i);
                        PopulateVinylList();
                    }
                }
            }
        }

        private void TrackListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentUser.isPremium)
            {
                SongPreviewWindow window = new SongPreviewWindow(selectedVinyl, TrackListView.SelectedIndex);
                window.ShowDialog();
            }
            else
            {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
            }
        }

        private void FindFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.isPremium && !currentUser.isPrivate)
            {
                AddFriendWindow window = new AddFriendWindow();
                window.ShowDialog();
                currentUser.upload();
                PopulateFriendsList();
            }
            else if (!currentUser.isPremium)
            {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
            }
            else if (currentUser.isPrivate)
            {
                System.Windows.Forms.MessageBox.Show("Your account is set to private.");
                privateCounter++;
                AskToChangePrivacySetting();
            }
        }

        private void AskToChangePrivacySetting()
        {
            if (privateCounter >= 3)
            {
                var selectedOption = System.Windows.Forms.MessageBox.Show("Do you want to change your privacy setting?", "Changing Privacy setting", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                if (selectedOption == System.Windows.Forms.DialogResult.Yes)
                {
                    currentUser.isPrivate = false;
                }
                privateCounter = 0;
            }
        }
        private void FriendsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selectedFriend = currentUser.friends.Where(r => r.firstName == FriendsListView.SelectedItem.ToString().Split(' ')[0] && r.lastName == FriendsListView.SelectedItem.ToString().Split(' ')[1])
                    .FirstOrDefault();
            }
            catch (NullReferenceException) { }
        }

        private void FriendsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (selectedFriend != null)
            {
                FriendProfileWindow window = new FriendProfileWindow();
                window.ShowDialog();
            }
        }

        private void RemoveFriendClick_Click(object sender, RoutedEventArgs e)
        {
            Friend deletableUser = currentUser.friends.Where(r => r.firstName == FriendsListView.SelectedItem.ToString().Split(' ')[0] && r.lastName == FriendsListView.SelectedItem.ToString().Split(' ')[1])
            .FirstOrDefault();
            currentUser.friends.Remove(deletableUser);
            if (currentUser.friends.Count > 0)
                selectedFriend = currentUser.friends[0];
            else
                selectedFriend = null;

            PopulateFriendsList();
            FriendsListView.SelectedItem = -1;
        }

        public void RefreshFriends()
        {
            List<Friend> refreshedFriends = new List<Friend>();
            try
            {
                foreach (Friend friend in currentUser.friends)
                {
                    foreach (Account a in VerifiedAccounts.accounts)
                    {
                        if (a.email == friend.email)
                        {
                            Friend newFriend = new Friend(a.firstName, a.lastName, a.email, a.Collection);
                            refreshedFriends.Add(newFriend);
                            break;
                        }
                    }
                }
                currentUser.friends = refreshedFriends;
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private void VinylListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SongPreviewWindow window = new SongPreviewWindow(selectedVinyl, 0);
            window.ShowDialog();
        }

        private void SelectedVinylImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
            window.ShowDialog();
        }
        private void ShareProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.isPremium && !currentUser.isPrivate)
            {
                Page page = document.Pages.Add();
                currentUser.Collection.Sort((x, y) => x.Artist.CompareTo(y.Artist));

                // -------------------------------------------------------------
                // Add Header
                var header = new TextFragment(currentUser.firstName + " " + currentUser.lastName + "'s Collection");
                header.TextState.Font = FontRepository.FindFont("Arial");
                header.TextState.FontSize = 24;
                header.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center;
                header.Position = new Position(130, 720);
                page.Paragraphs.Add(header);

                // Add description
                var descriptionText = currentUser.Collection.Count() + " Record(s) in collection";
                var description = new TextFragment(descriptionText);
                description.TextState.Font = FontRepository.FindFont("Times New Roman");
                description.TextState.FontSize = 14;
                description.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Left;
                page.Paragraphs.Add(description);


                // Add table
                var table = new Aspose.Pdf.Table
                {
                    ColumnWidths = "200",
                    Border = new BorderInfo(BorderSide.Box, 1f, Aspose.Pdf.Color.DarkSlateGray),
                    DefaultCellBorder = new BorderInfo(BorderSide.Box, 0.5f, Aspose.Pdf.Color.Black),
                    DefaultCellPadding = new MarginInfo(4.5, 4.5, 4.5, 4.5),
                    Margin =
                {
                    Bottom = 10
                },
                    DefaultCellTextState =
                {
                    Font =  FontRepository.FindFont("Helvetica")
                }
                };

                var headerRow = table.Rows.Add();
                headerRow.Cells.Add("Artist");
                headerRow.Cells.Add("Album");
                foreach (Cell headerRowCell in headerRow.Cells)
                {
                    headerRowCell.BackgroundColor = Aspose.Pdf.Color.Gray;
                    headerRowCell.DefaultCellTextState.ForegroundColor = Aspose.Pdf.Color.WhiteSmoke;
                }
                foreach (Vinyl v in currentUser.Collection)
                {
                    var musicRow = table.Rows.Add();
                    musicRow.Cells.Add(v.Artist);
                    musicRow.Cells.Add(v.Album);
                }

                page.Paragraphs.Add(table);
                try
                {
                    document.Save(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Collection.pdf"));
                    MessageBox.Show("Collection.pdf was saved to your desktop");

                }
                catch (IOException)
                {
                    MessageBox.Show("Please close your collection.pdf and try again...");
                }
                var selectedOption = System.Windows.Forms.MessageBox.Show("Do you want to share your profile via Email?", "Share Profile", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                if (selectedOption == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        var fileLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Collection.pdf");
                        Helper.MAPI mapi = new Helper.MAPI();

                        mapi.AddAttachment(fileLocation);
                        mapi.AddRecipientTo("testing@hotmail.com");
                        mapi.SendMailPopup("Sharing Your Collection", "Find the Collection.pdf in the attatchments");

                    }
                    catch (Exception ex)
                    {
                        MaterialMessageBox.ShowError("Error occured: " + ex.Message);
                    }
                }
                //Refresh the document incase they want to save or share it again in the session
                document = new Document();
            }
            else if(!currentUser.isPremium)
            {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
            }
            else if(currentUser.isPrivate)
            {
                System.Windows.Forms.MessageBox.Show("Your account is set to private.");
                privateCounter++;
                AskToChangePrivacySetting();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            UserSettingsWindow window = new UserSettingsWindow();
            window.ShowDialog();
            if (window.getDeleteStatus())
                Close();
        }

        private void SortByArtistToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(!(bool)SortByAlbumToggleButton.IsChecked))
            {
                currentUser.Collection.Sort((x, y) => x.Artist.CompareTo(y.Artist));
                PopulateVinylList();
                SortByAlbumToggleButton.IsChecked = false;
            }
        }

        private void SortByAlbumToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(!(bool)SortByArtistToggleButton.IsChecked))
            {
                currentUser.Collection.Sort((x, y) => x.Album.CompareTo(y.Album));
                PopulateVinylList();
                SortByArtistToggleButton.IsChecked = false;
            }
        }
    }
}
