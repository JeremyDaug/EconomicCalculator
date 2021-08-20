using EconomicCalculator;
using EconomicCalculator.Storage.Planet;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfAppTest.Maps
{
    /// <summary>
    /// Interaction logic for PlanetViewWindow.xaml
    /// </summary>
    public partial class PlanetViewWindow : Window
    {
        private Manager manager;

        private Planet Planet;
        
        public PlanetViewWindow()
        {
            InitializeComponent();
        }

        internal void LoadData(string dataName)
        {}

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapImage Bitmap2Image(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage result;

            try
            {
                result = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return result;
        }

        private void LoadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            string folder = "";
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                dialog.Filters.Add(new CommonFileDialogFilter("", ".png"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    MessageBox.Show("You Selected: " + dialog.FileName);
                    var image = new BitmapImage(new Uri(dialog.FileName));
                    PlanetMap.Source = image;
                }
            }
        }

        private void BackToWelcomeScreen(object sender, RoutedEventArgs e)
        {
            Window win = new MainWindow();

            win.Show();
            this.Close();
        }

        private void ExpandMap(object sender, RoutedEventArgs e)
        {
            // TODO expand to full scrollable/zoomable map
            // with additional options to swap between data layers
        }

        // 
        private void RedrawMap(object sender, RoutedEventArgs e)
        {
            // null out the image for updating.
            PlanetMap.Source = null;

            Planet.Frequency = double.Parse(Frequency.Text);
            Planet.Octaves = int.Parse(Octaves.Text);
            Planet.Lacunarity = double.Parse(Lacunarity.Text);
            Planet.Persistence = double.Parse(Persistence.Text);
            Planet.SeaLevel = (int)(double.Parse(SeaLevel.Text) * 65_536);

            Planet.GenerateSimpleTerrain();
        }
    }
}
