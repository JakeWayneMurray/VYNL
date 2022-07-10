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
using VinylCollectionApplication.Helper;

namespace VinylCollectionApplication
{
    /// <summary>
    /// Interaction logic for UserSettingsWindow.xaml
    /// </summary>
    public partial class UserSettingsWindow : Window
    {
        public Account currentUser;
        static bool hasBeenDeleted;

        public UserSettingsWindow()
        {
            InitializeComponent();
            currentUser = ((MainWindow)Application.Current.MainWindow).getCurrentUser();
            hasBeenDeleted = false;
            try
            {
                firstNameTxtBox.Text = currentUser.firstName;
                lastNameTxtBox.Text = currentUser.lastName;
                emailTxtBox.Text = currentUser.email;
                userNameTxtBox.Text = currentUser.userName;
                PremiumAccountToggleButton.IsChecked = currentUser.isPremium;
                PrivateAccountToggleButton.IsChecked = currentUser.isPrivate;

            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (firstNameTxtBox.Text != "" && lastNameTxtBox.Text != "" && Helper.HelperFunctions.IsValidEmail(emailTxtBox.Text))
            {
                var selectedOption = System.Windows.Forms.MessageBox.Show("Are you sure you want to save these changes?", "Save Changes", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (selectedOption == System.Windows.Forms.DialogResult.Yes)
                {
                    currentUser.firstName = firstNameTxtBox.Text;
                    currentUser.lastName = lastNameTxtBox.Text;
                    currentUser.isPremium = PremiumAccountToggleButton.IsChecked.Value;
                    currentUser.isPrivate = PrivateAccountToggleButton.IsChecked.Value;
                    currentUser.email = emailTxtBox.Text;
                    currentUser.upload();
                    Close();
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid Input", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PremiumAccountToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)PremiumAccountToggleButton.IsChecked)
            {
                var selectedOption = System.Windows.Forms.MessageBox.Show("Are you sure you want to remove your Premium Status?", "Remove Premium Status", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (selectedOption == System.Windows.Forms.DialogResult.Yes)
                {
                    PremiumAccountToggleButton.IsChecked = false;
                }
            }
            else
            {
                UpgradeToPremiumWindow window = new UpgradeToPremiumWindow();
                window.ShowDialog();
                PremiumAccountToggleButton.IsChecked = currentUser.isPremium;
            }
        }
        public bool getDeleteStatus()
        {
            return hasBeenDeleted;
        }
        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
                var selectedOption = System.Windows.Forms.MessageBox.Show("ARE YOU SURE YOU WANT TO DELETE THIS ACCOUNT?\nIT CANNOT BE REVERSED.", "Delete account", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (selectedOption == System.Windows.Forms.DialogResult.Yes)
                {
                    currentUser.delete();
                    hasBeenDeleted = true;
                    Close();
                }
        }
    }
}
