using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using BespokeFusion;
using VinylCollectionApplication.Helper;

namespace VinylCollectionApplication
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        // Get Users db instance
        public IMongoCollection<BsonDocument> getUsersDB()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://DadsThrobber:BrianSucks@vynl.4q0um.mongodb.net/VynLDatabase?retryWrites=true&w=majority");
           // var dbList = dbClient.ListDatabases().ToList();
            var database = dbClient.GetDatabase("VynLDatabase");
            return database.GetCollection<BsonDocument>("VynLCollection");
        }


        public SignUpWindow()
        {
            InitializeComponent();
        }

        //RegisterButton_Click
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var bcoll = getUsersDB();

            if (UserNameTextBox.Text != "" && PasswordBox.Password != "" &&
               FirstNameTextBox.Text != "" && LastNameTextBox.Text != "" && Helper.HelperFunctions.IsValidEmail(EmailTextBox.Text))
            {
                Account newUser = new Account(FirstNameTextBox.Text, LastNameTextBox.Text, EmailTextBox.Text, UserNameTextBox.Text, IsPrivateCheckBox.IsEnabled, StringCipher.Encrypt((PasswordBox.Password), "passwordCipher"));

                bcoll.InsertOne(newUser.ToBsonDocument());
                //Refresh the verifiedUsers list with the new user
                VerifiedAccounts.getAccountsAsync();
                MaterialMessageBox.Show("Account Created. Log in with your new account.");

                Close();
            }
            else if (Helper.HelperFunctions.IsValidEmail(EmailTextBox.Text) == false)
            {
                MaterialMessageBox.ShowError("Invalid email address");
            }
            else
            {
                MaterialMessageBox.ShowError("Fill all fields.");
            }
        }
    }

}
