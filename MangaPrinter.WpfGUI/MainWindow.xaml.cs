using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<Core.MangaChapter> mangaChapters = new ObservableCollection<Core.MangaChapter>();
       

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstFileChapters.ItemsSource = mangaChapters;
        }

        void verifyInteger(TextBox textBox, string fallbackValue)
        {
            int value = 0;
            if (!int.TryParse(textBox.Text, out value))
            {
                MessageBox.Show("Can't convert \"" + textBox.Text + "\" to integer, try again.");
                textBox.Text = fallbackValue;
            }
        }

        #region FilesTab

        private void txtPageMaxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPageMaxWidth, "900");
        }

        private void menuImprtFolders_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlgSaveFile = new Microsoft.Win32.OpenFileDialog();
            dlgSaveFile.Filter = "Folder|_Choose.Here_";
            dlgSaveFile.FileName = "_Choose.Here_";
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

                Func<System.IO.FileSystemInfo, object> orderFunc = (si) => si.CreationTime;
                if (rbByName.IsChecked ?? false)
                    orderFunc = (si) => si.Name;

                winWorking.waitForTask((updateFunc) =>
                {
                    return fileImporter.getChapters(DirPath, subFolders, cutoff, rtl, orderFunc, updateFunc);
                },
                isProgressKnwon: false)
                .ForEach(ch => mangaChapters.Add(ch));
            }
        }


        public static void ListBoxAction<T>(ListBox tree, Action<T> action) where T : class
        {
            T obj = tree.SelectedItem as T;
            if (obj != null)
            {
                action(obj);
            }
        }

        private void mnuToSingle_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaPage>(lstFilePages, page => { page.IsDouble = false; page.Chapter.updateMeta(); });
        }

        private void mnuToDouble_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaPage>(lstFilePages, page => { page.IsDouble = true; page.Chapter.updateMeta(); });
        }

        private void mnuToRTL_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaChapter>(lstFileChapters, ch => ch.IsRTL = true);
        }

        private void mnuToLTR_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaChapter>(lstFileChapters, ch => ch.IsRTL = false);
        }

        private void mnuRenameChapter_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaChapter>(lstFileChapters, (ch) =>
            {
                Dialogs.dlgString dlgName = new Dialogs.dlgString()
                {
                    Title = "Enter name",
                    Caption = "Enter new chapter title:",
                    StringData = ch.Name
                };
                if (dlgName.ShowDialog() ?? false)
                {
                    ch.Name = dlgName.StringData;
                }
            });
        }

        private void mnuAddEmptyChapter_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.dlgString dlgName = new Dialogs.dlgString()
            {
                Title = "Enter name",
                Caption = "Enter empty chapter title:",
                StringData = "Chapter Name"
            };
            if (dlgName.ShowDialog() ?? false)
            {
                mangaChapters.Add(new Core.MangaChapter()
                {
                    IsRTL = rbRTL.IsChecked ?? false,
                    Pages = new ObservableCollection<Core.MangaPage>(),
                    Name = dlgName.StringData
                });
            }
        }

        private void mnuDeleteCh_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaChapter>(lstFileChapters, (ch) =>
            {
                mangaChapters.Remove(ch);
            });
        }

        private void mnuAddChapterPages_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaChapter>(lstFileChapters, (ch) =>
            {
                Microsoft.Win32.OpenFileDialog dlgOpenImages = new Microsoft.Win32.OpenFileDialog();
                dlgOpenImages.Filter = "Images|" + Core.FileImporter.ImagesExtensions;
                dlgOpenImages.FileName = "Open supported images";
                dlgOpenImages.CheckFileExists = true;
                dlgOpenImages.Multiselect = true;
                dlgOpenImages.Title = "Choose images to import:";

                if (dlgOpenImages.ShowDialog() == true)
                {
                    Core.FileImporter fileImporter = new Core.FileImporter();

                    int cutoff = int.Parse(txtPageMaxWidth.Text);

                    Func<System.IO.FileSystemInfo, object> orderFunc = (si) => si.CreationTime;
                    if (rbByName.IsChecked ?? false)
                        orderFunc = (si) => si.Name;


                    ch.autoUpdateMeta = false;

                    winWorking.waitForTask((updateFunc) =>
                    {
                        return fileImporter.importImages(dlgOpenImages.FileNames, cutoff, orderFunc, updateFunc);
                    },
                    isProgressKnwon: true)
                    .ForEach(page => ch.Pages.Add(page));

                    ch.autoUpdateMeta = true;
                    ch.updateMeta();
                }
            });
        }

        #endregion

        private void txtSpoilerPgNm_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtSpoilerPgNm, "25");
        }

        private void stackFilePage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Core.MangaPage page = lstFilePages.SelectedValue as Core.MangaPage;
                if (page != null)
                {
                    Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(page.ImagePath, "File: " + page.Name);
                    dlgImage.ShowDialog();
                }
            }
        }
    }
}
