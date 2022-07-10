using BespokeFusion;
using Newtonsoft.Json;
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
    /// Interaction logic for LoadJSONWindow.xaml
    /// </summary>
    public partial class LoadJSONWindow : Window
    {

        Account currentUser;
        public LoadJSONWindow()
        {
            InitializeComponent();
            currentUser = ((MainWindow)Application.Current.MainWindow).getCurrentUser();
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (JSONTextBox.Text != "") {
                try
                {
                    int added = 0;
                    string JSON = JSONTextBox.Text;
                    TempUser tempUser = JsonConvert.DeserializeObject<TempUser>(JSON);
                    foreach (Vinyl v in tempUser.Collection)
                    {
                        if (!currentUser.Collection.Contains(v))
                        {
                            currentUser.Add(v);
                            added++;
                        }
                    }
                    MaterialMessageBox.Show($"Added {added} Vinyl to the collection. \n" +
                                            $"{tempUser.Collection.Count - added} Vinyl were duplicate(s)");
                    currentUser.upload();
                    ((MainWindow)Application.Current.MainWindow).PopulateVinylList();

                    Close();

                }
                catch (Newtonsoft.Json.JsonReaderException)
                {
                    MaterialMessageBox.ShowError("JSON reader error");
                }
            }
        }
    }

    public class TempUser
    {
        public string Date { get; set; }
        public List<Vinyl> Collection { get; set; }
        public TempUser()
        {
            Date = "Feb 1st 2021";
            Collection = new List<Vinyl>();
        }
    }
}
