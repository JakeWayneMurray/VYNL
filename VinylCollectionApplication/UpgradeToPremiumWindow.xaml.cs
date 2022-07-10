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

namespace VinylCollectionApplication
{
    /// <summary>
    /// Interaction logic for UpgradeToPremiumWindow.xaml
    /// </summary>
    public partial class UpgradeToPremiumWindow : Window
    {
        public Account currentUser;
        public UpgradeToPremiumWindow()
        {
            InitializeComponent();
            currentUser = ((MainWindow)Application.Current.MainWindow).getCurrentUser();

        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            currentUser.isPremium = true;
            MaterialMessageBox.Show("Congrats, you are now a premium user!");
            currentUser.upload();
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
