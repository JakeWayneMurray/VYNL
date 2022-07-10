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
using BespokeFusion;
using VinylCollectionApplication.Helper;

namespace VinylCollectionApplication
{

    public partial class LoginWindow : Window
    {

        public Account currentUser;
        public Account TempUser;


        public LoginWindow()
        {
            InitializeComponent();
            VerifiedAccounts.getAccountsAsync();

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string currentUserName = UserNameTextBox.Text.ToLower();
            string currentPassword = PasswordTextBox.Password;
            bool validFlag = false;
            foreach (Account u in VerifiedAccounts.accounts)
            {
                if ((currentUserName == u.email.ToLower() || currentUserName == u.userName.ToLower()) && currentPassword == StringCipher.Decrypt(u.password, "passwordCipher"))
                {
                    ((MainWindow)Application.Current.MainWindow).setUser(u);
                    validFlag = true;
                    Close(); //window close else...
                    break;
                }
            }
            if (validFlag)
            {
                Console.WriteLine("Successfully logged in");
            }
            else
            {
                MaterialMessageBox.ShowError("Invalid Credentials Presented."); // ...else this message will pop

            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow window = new SignUpWindow();
            window.ShowDialog();
        }
    }
}
