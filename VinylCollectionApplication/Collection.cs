using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using NAudio.Wave;
using System.Linq;
using Newtonsoft.Json;
using VinylCollectionApplication;

namespace VinylCollectionApplication

{
    static class Collection
    {
        //Data Members
        private static Account collection;
        private static string fileLocation;


        static Collection()
        {
            if (File.Exists("collectionNEW.collection") && File.ReadAllText("collectionNEW.collection") != "")
            {
                string json = File.ReadAllText("collectionNEW.collection");
                collection = JsonConvert.DeserializeObject<Account>(json);
            }
            else
            {
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter("collectionNEW.collection", false))
                {
                    collection = new Account();
                    file.Write("");
                    ((MainWindow)Application.Current.MainWindow).ClearFields();
                }
            }

            fileLocation = "collectionNEW.collection";

        }

        public static void addVinyl(string albumName, string artistName)
        {
            collection.Add(new Vinyl(albumName, artistName));
        }

        public static void addVinyl(Vinyl vinyl)
        {
            collection.Add(vinyl);
        }

        public static void updateCollection()
        {
            ((MainWindow)Application.Current.MainWindow).getCurrentUser().Collection = collection.Collection;
            /*var jsonString = JsonConvert.SerializeObject(collection);
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(fileLocation, false))
            {
                file.Write(jsonString);
            }
            */
        }

        public static void saveAs()
        {
            /*
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Files|*.collection;*.out";
                saveFileDialog.Title = "Select a File";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(collection));
                    fileLocation = saveFileDialog.FileName;
                }
            */
        }

        public static void SaveListAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Files|*.txt;*.out";
            saveFileDialog.Title = "Select a File";
            string list = "";
            foreach(var c in collection.Collection.OrderBy(x=> x.Artist).ToList())
            {
                list += $"{c.Artist} - {c.Album}\n";
            }
            list += $"\n {collection.Collection.Count} record(s) in the collection.";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, list);
                fileLocation = saveFileDialog.FileName;
            }

        }

        public static void newCollection()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Files|*.collection;*.out";
            saveFileDialog.Title = "Select a File";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "");
                fileLocation = saveFileDialog.FileName;
            }

            collection = new Account();
            ((MainWindow)Application.Current.MainWindow).PopulateVinylList();
            ((MainWindow)Application.Current.MainWindow).ClearFields();

        }

        public static void loadCollection()
        {
            try
            {
                string file = "";
                // Displays an OpenFileDialog so the user can select a file.  
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Files|*.collection;*.out";
                openFileDialog1.Title = "Select a File";

                // Show the Dialog.  
                // If the user clicked OK in the dialog and  
                // a file was selected, open it.  
                if (openFileDialog1.ShowDialog() == true)
                {
                    file = openFileDialog1.FileName;
                    fileLocation = file;
                    string json = File.ReadAllText(file);
                    if (json != "")
                        collection = JsonConvert.DeserializeObject<Account>(json);
                    else
                        collection = new Account();

                }
                openFileDialog1 = null;

            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid File selected.");
            }
            ((MainWindow)Application.Current.MainWindow).ClearFields();
            ((MainWindow)Application.Current.MainWindow).PopulateVinylList();
        }

        public static List<Vinyl> getCollection()
        {
            return collection.Collection;
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

        public static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        {
            try
            {
                using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
                {
                    using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                    {
                        WaveFileWriter.CreateWaveFile(_outPath_, pcm);
                    }
                }
            }
            catch (IOException)
            {

            }
        }
    }
}