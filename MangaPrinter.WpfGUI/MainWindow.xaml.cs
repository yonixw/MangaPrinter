using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MangaPrinter.WpfGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtPageMaxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (!int.TryParse(txtPageMaxWidth.Text, out value))
            {
                MessageBox.Show("Can't convert " + txtPageMaxWidth.Text + " to integer, try again.");
                txtPageMaxWidth.Text = 900.ToString();
            }
        }

        private void menuImprtFolders_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlgSaveFile = new Microsoft.Win32.OpenFileDialog();
            dlgSaveFile.Filter = "Folder|_._";
            dlgSaveFile.FileName = "Open here";
            dlgSaveFile.CheckFileExists = false;
            dlgSaveFile.Multiselect = false;
            dlgSaveFile.ValidateNames = false;
            dlgSaveFile.Title = "Choose folder to import from:";

            if (dlgSaveFile.ShowDialog() == true)
            {
                Core.FileImporter fileImporter = new Core.FileImporter();

                var DirPath = new System.IO.FileInfo(dlgSaveFile.FileName).Directory.FullName;
                var subFolders = cbSubfolders.IsChecked ?? false;
                int cutoff = int.Parse(txtPageMaxWidth.Text);
                var rtl = rbRTL.IsChecked ?? false;

                List<Core.MangaChapter> chapters = winWorking.waitForTask((updateFunc) =>
                {
                    return fileImporter.getChapters(DirPath,subFolders,cutoff,rtl, updateFunc);
                }, 
                isProgressKnwon: false);

                tvFiles.ItemsSource = chapters;
                
            }
        }
    }
}
