using MangaPrinter.Core;
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
using MangaPrinter.WpfGUI.ExtendedClasses;
using MangaPrinter.Core.TemplateBuilders;
using System.IO;

namespace MangaPrinter.WpfGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<SelectableMangaChapter> mangaChapters = new ObservableCollection<SelectableMangaChapter>();
       

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstFileChapters.ItemsSource = mangaChapters;
            lstFileChaptersBinding.ItemsSource = mangaChapters;
            rtbInfo.AppendText(" " + Properties.Resources.GitInfo.Split(';')[0]);
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
                    return fileImporter.getChapters(DirPath, subFolders, cutoff, rtl,  orderFunc, updateFunc);
                },
                isProgressKnwon: false)
                .ForEach(ch => mangaChapters.Add(MangaChapter.Extend<SelectableMangaChapter>(ch)));
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
            ListBoxAction<Core.MangaPage>(lstFilePages, page => { page.IsDouble = false; page.Chapter.updatePageNumber(); });
        }

        private void mnuToDouble_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaPage>(lstFilePages, page => { page.IsDouble = true; page.Chapter.updatePageNumber(); });
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
                mangaChapters.Add(MangaChapter.Extend<SelectableMangaChapter>( new Core.MangaChapter()
                {
                    IsRTL = rbRTL.IsChecked ?? false,
                    Pages = new ObservableCollection<Core.MangaPage>(),
                    Name = dlgName.StringData
                }));
            }
        }

        private void mnuDeleteCh_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<SelectableMangaChapter>(lstFileChapters, (ch) =>
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


                    ch.autoPageNumbering = false;

                    winWorking.waitForTask((updateFunc) =>
                    {
                        return fileImporter.importImages(dlgOpenImages.FileNames, cutoff, orderFunc, updateFunc);
                    },
                    isProgressKnwon: true)
                    .ForEach(page => ch.Pages.Add(page));

                    ch.autoPageNumbering = true;
                    ch.updatePageNumber();
                }
            });
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
        #endregion

        private void txtSpoilerPgNm_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtSpoilerPgNm, "25");
        }

        private void TxtPrintWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPrintWidth, "794");
        }

        private void TxtPrintHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPrintHeight, "1123");
        }

        private void TxtPrintPadding_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPrintPadding, "25");
        }

        ObservableCollection<SelectablePrintPage> allPrintPages = new ObservableCollection<SelectablePrintPage>();
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool startPage = cbAddStart.IsChecked ?? false;
            bool endPage = cbAddEnd.IsChecked ?? false;
            int antiSpoiler = (cbUseAntiSpoiler.IsChecked ?? false) ? int.Parse(txtSpoilerPgNm.Text) : 0;

            var allChapters = mangaChapters.ToList();

            allPrintPages = winWorking.waitForTask<ObservableCollection<SelectablePrintPage>>((updateFunc) =>
            {
                ObservableCollection<SelectablePrintPage> result = new ObservableCollection<SelectablePrintPage>();

                (new Core.ChapterBuilders.DuplexBuilder())
                    .Build(allChapters.Cast<MangaChapter>().ToList(), startPage, endPage, antiSpoiler)
                    .ForEach((p) => result.Add(PrintPage.Extend<SelectablePrintPage>(p)));

                return result;
            },
            isProgressKnwon: false);

            lstPrintPages.ItemsSource = allPrintPages;
        }

        bool selectPrintPages = true, selectPrintChapters = true;

        private void LstFileChaptersBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!selectPrintChapters) return;
            if (e.AddedItems.Count == 0) return;


            SelectableMangaChapter c = (SelectableMangaChapter)e.AddedItems[0] ?? (SelectableMangaChapter)e.RemovedItems[0];
            selectPrintChapters = false;
            lstFileChaptersBinding.SelectedItem = c;
            selectPrintChapters = true;

            //unselect all
            selectPrintPages = false;
            foreach (var p in allPrintPages) p.Selected = false;

            bool pSelected = false;
            foreach (SelectablePrintPage p in allPrintPages)
            {

                if (
                      p.Front.Left.MangaPageSource?.Chapter.Name == c.Name ||
                     p.Front.Right.MangaPageSource?.Chapter.Name == c.Name ||
                     p.Back.Left.MangaPageSource?.Chapter.Name == c.Name ||
                     p.Back.Right.MangaPageSource?.Chapter.Name == c.Name
                    )
                {
                    p.Selected = true;
                    if (!pSelected)
                    {
                        pSelected = true;
                        lstPrintPages.ScrollIntoView(p);
                    }
                }
            }
            selectPrintPages = true;
        }

        private void LstPrintPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!selectPrintPages) return;
            if (e.AddedItems.Count == 0) return;

            SelectablePrintPage p = (SelectablePrintPage)e.AddedItems[0] ?? (SelectablePrintPage)e.RemovedItems;
            selectPrintPages = false;
            lstPrintPages.SelectedItem = p;
            selectPrintPages = true;

            //unselect all
            selectPrintChapters = false;
            foreach (var c in mangaChapters) c.Selected = false;

            bool cSelected = false;
            foreach(SelectableMangaChapter c in mangaChapters)
            {
                if (
                     p.Front.Left.MangaPageSource?.Chapter.Name == c.Name ||
                     p.Front.Right.MangaPageSource?.Chapter.Name == c.Name ||
                     p.Back.Left.MangaPageSource?.Chapter.Name == c.Name ||
                     p.Back.Right.MangaPageSource?.Chapter.Name == c.Name
                    )
                {
                    c.Selected = true;
                    if (!cSelected)
                    {
                        cSelected = true;
                        lstFileChaptersBinding.ScrollIntoView(c);
                    }
                }
            }
            selectPrintChapters = true;
        }

        FileInfo tempImage = new FileInfo("_tmp_.png");
        private void MnuPrvwFront_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<SelectablePrintPage>(lstPrintPages, (p) =>
            {
                var b = (new DuplexTemplates()).BuildFace(p.Front, p.Back,
                    int.Parse(txtPrintWidth.Text), int.Parse(txtPrintHeight.Text), int.Parse(txtPrintPadding.Text));

                if (tempImage.Exists)
                    tempImage.Delete();

                b.Save(tempImage.FullName);

                Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(tempImage.FullName,
                        "Front face of page: " + p.PageNumber);
                dlgImage.ShowDialog();

            });
        }

        private void MnuPrvwBack_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<SelectablePrintPage>(lstPrintPages, (p) =>
            {
                var b = (new DuplexTemplates()).BuildFace(p.Back, null,
                    int.Parse(txtPrintWidth.Text), int.Parse(txtPrintHeight.Text), int.Parse(txtPrintPadding.Text));

                if (tempImage.Exists)
                    tempImage.Delete();

                b.Save(tempImage.FullName);

                Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(tempImage.FullName,
                        "Back face of page: " + p.PageNumber);
                dlgImage.ShowDialog();

            });
        }

        private void LstPrintPages_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                SelectablePrintPage page = lstPrintPages.SelectedItem as SelectablePrintPage;
                if (page != null)
                {
                    
                    if (tempImage.Exists)
                        tempImage.Delete();

                    //var b = GraphicsUtils.loadFileZoomed(page.Back.Right.MangaPageSource.ImagePath, 100, 100);
                    var b = GraphicsUtils.createImageWithText("Hello\nאבגד\nWhat is up?\n",
                        500, 500);
                    b.Save(tempImage.FullName, System.Drawing.Imaging.ImageFormat.Png);
                    b.Dispose();

                    Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(tempImage.FullName,
                        "Print Page: " + page.PageNumber);
                    dlgImage.ShowDialog();
                }
            }
        }
    }
}
