using BespokeFusion;
using System;
using System.Collections.Generic;
using System.Linq;
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
using VinylCollectionApplication.FetchInfo;

namespace VinylCollectionApplication
{
    /// <summary>
    /// Interaction logic for AddFriendWindow.xaml
    /// </summary>
    public partial class AddFriendWindow : Window
    {
        Account currentUser;
        public AddFriendWindow()
        {
            InitializeComponent();
            currentUser = ((MainWindow)Application.Current.MainWindow).getCurrentUser();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            bool friendFound = false;
            string friendsName = "";
            string friendEmail = FriendsNameTextBox.Text;
            foreach(Account a in VerifiedAccounts.accounts)
            {
                if(a.email == friendEmail)
                {
                    bool exists = false;

                    foreach (Friend f in currentUser.friends)
                    {
                        if (f.email == a.email && !a.isPrivate)
                        {
                            exists = true;
                            break;
                        }
                    }
                    

                    if (!exists)
                    {
                        friendFound = true;
                        Friend newFriend = new Friend(a.firstName, a.lastName, a.email, a.Collection);
                        currentUser.friends.Add(newFriend);
                        friendsName = newFriend.firstName + " " + newFriend.lastName;
                        break;
                    }
                }
            }
            if (friendFound)
            {
                MaterialMessageBox.Show($"{friendsName} added to your friends list.");
                ((MainWindow)Application.Current.MainWindow).FriendsListView.SelectedItem = -1;
                Close();
            }
            else
            {
                MaterialMessageBox.ShowError("Wrong email, or this user has their profile set to Private.");
            }
        }
    }
}
