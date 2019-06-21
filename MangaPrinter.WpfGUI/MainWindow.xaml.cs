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
using Microsoft.Win32;
using MangaPrinter.WpfGUI.Utils;

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

            // Load Settings:
            var Config = MangaPrinter.WpfGUI.Properties.Settings.Default;
            txtPageMaxWidth.Text = Config.doublePageWidth.ToString();
            rbByName.IsChecked = Config.orderImportByName;
            cbSubfolders.IsChecked = Config.importSubfolders;
            rbLTR.IsChecked = !Config.RTL;
            cbAddStart.IsChecked = Config.addStartPage;
            cbAddEnd.IsChecked = Config.addEndPage;
            cbUseAntiSpoiler.IsChecked = Config.addAntiSpoiler;
            txtSpoilerPgNm.Text = Config.antiSpoilerStep.ToString();
            txtPrintWidth.Text = Config.exportPageWidth.ToString();
            txtPrintHeight.Text = Config.exportPageHeight.ToString();
            txtPrintPadding.Text = Config.exportPagePadding.ToString();
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

        void verifyFloat(TextBox textBox, string fallbackValue)
        {
            float value = 0;
            if (!float.TryParse(textBox.Text, out value))
            {
                MessageBox.Show("Can't convert \"" + textBox.Text + "\" to float, try again.");
                textBox.Text = fallbackValue;
            }
        }

        #region FilesTab

        private void txtPageMaxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyFloat(txtPageMaxWidth,
                MangaPrinter.WpfGUI.Properties.Settings.Default.doublePageWidth.ToString());
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
                float cutoff = float.Parse(txtPageMaxWidth.Text);
                var rtl = rbRTL.IsChecked ?? false;

                Func<System.IO.FileSystemInfo, object> orderFunc = (si) => si.CreationTime;
                if (rbByName.IsChecked ?? false)
                    orderFunc = (si) => si.Name;

                winWorking.waitForTask((updateFunc) =>
                {
                    return fileImporter.getChapters(DirPath, subFolders, cutoff, rtl, orderFunc, updateFunc);
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
                mangaChapters.Add(MangaChapter.Extend<SelectableMangaChapter>(new Core.MangaChapter()
                {
                    IsRTL = rbRTL.IsChecked ?? false,
                    Pages = new ObservableCollection<Core.MangaPage>(),
                    Name = dlgName.StringData
                }));
            }
        }


        private void MnuClearAllData_Click(object sender, RoutedEventArgs e)
        {
            mangaChapters.Clear();
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

                    float cutoff = float.Parse(txtPageMaxWidth.Text);

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

        private void MnPreviewManga_Click(object sender, RoutedEventArgs e)
        {
            Core.MangaPage page = lstFilePages.SelectedValue as Core.MangaPage;
            if (page != null)
            {
                Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(page.ImagePath, "File: " + page.Name);
                dlgImage.ShowDialog();
            }
        }

        private IEnumerable<BucketInfo> plotData(IEnumerable<MangaChapter> chapters, int numOfBuckets = 100)
        {
            if (chapters == null || chapters.Count() == 0)
                return new List<BucketInfo>();

            IEnumerable<MangaPage> allPages = chapters.SelectMany((ch) => ch.Pages);

            float min = allPages.Min((p) => p.AspectRatio);
            float max = allPages.Max((p) => p.AspectRatio);

            List<BucketInfo> buckets = Enumerable.Range(0, numOfBuckets)
                .Select(i => new BucketInfo()
                {
                    index = i,
                    value = min + (max - min) * (i * 1.0 / (numOfBuckets)),
                    count = 0
                }).ToList();

            foreach (MangaPage page in allPages)
            {
                var b = buckets.Last((bucket) => page.AspectRatio >= bucket.value);
                Console.WriteLine("{0}.{1}: {2}", page.Chapter.Name, page.Name, page.AspectRatio);
                b.count++;
            }

            return buckets;
        }

        private void BtnNewCutoffRatio_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.dlgChooseCutoffRatio dlg = new Dialogs.dlgChooseCutoffRatio();
            dlg.InputBuckets = winWorking.waitForTask((updateFunc) =>
            {
                return plotData(mangaChapters).ToList();
            },
            isProgressKnwon: false);
            dlg.ShowDialog();
            if ((dlg.DialogResult ?? false) && (dlg.InputBuckets != null) && (dlg.InputBuckets.Count > 0))
            {
                double newAspectCutoff = Math.Round(dlg.InputBuckets[dlg.BucketIndex].value, 2);
                txtPageMaxWidth.Text = newAspectCutoff.ToString();
                // Update all pages:
                winWorking.waitForTask((updateFunc) =>
                {
                    if (mangaChapters.Count < 1)
                        return 0;

                    List<MangaPage> allPages = mangaChapters.SelectMany((ch) => ch.Pages).ToList();
                    if (allPages.Count < 1)
                        return 0;

                    int counter = 0;
                    foreach (MangaPage page in allPages)
                    {
                        updateFunc(page.Name, (int)(counter * 100.0f / allPages.Count));
                        page.IsDouble = page.AspectRatio >= newAspectCutoff;
                    }
                    return 1;
                },
                isProgressKnwon: false);
            }
        }

        #endregion

        #region Binding Tab

        private void txtSpoilerPgNm_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtSpoilerPgNm,
                MangaPrinter.WpfGUI.Properties.Settings.Default.antiSpoilerStep.ToString());
        }

        private void TxtPrintWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPrintWidth,
                MangaPrinter.WpfGUI.Properties.Settings.Default.exportPageWidth.ToString());
        }

        private void TxtPrintHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPrintHeight,
                MangaPrinter.WpfGUI.Properties.Settings.Default.exportPageHeight.ToString());
        }

        private void TxtPrintPadding_TextChanged(object sender, TextChangedEventArgs e)
        {
            verifyInteger(txtPrintPadding,
                MangaPrinter.WpfGUI.Properties.Settings.Default.exportPagePadding.ToString());
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
            foreach (SelectableMangaChapter c in mangaChapters)
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
                var b = (new DuplexTemplates(Properties.Resources.GitInfo.Split(' ')[0])).BuildFace(p.Front,
                    int.Parse(txtPrintWidth.Text), int.Parse(txtPrintHeight.Text), int.Parse(txtPrintPadding.Text));

                if (tempImage.Exists)
                    tempImage.Delete();

                b.Save(tempImage.FullName);
                b.Dispose();

                Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(tempImage.FullName,
                        "Front face of page: " + p.PageNumber);
                dlgImage.ShowDialog();

            });
        }

        private void MnuPrvwBack_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<SelectablePrintPage>(lstPrintPages, (p) =>
            {
                var b = (new DuplexTemplates(Properties.Resources.GitInfo.Split(' ')[0])).BuildFace(p.Back,
                    int.Parse(txtPrintWidth.Text), int.Parse(txtPrintHeight.Text), int.Parse(txtPrintPadding.Text));

                if (tempImage.Exists)
                    tempImage.Delete();

                b.Save(tempImage.FullName);
                b.Dispose();

                Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(tempImage.FullName,
                        "Back face of page: " + p.PageNumber);
                dlgImage.ShowDialog();

            });
        }


        private void MnuExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.Title = "Export book as PDF";
            dlgSave.Filter = "PDF |*.pdf";
            if (dlgSave.ShowDialog() == true)
            {
                FileInfo fi = new FileInfo(dlgSave.FileName);
                int saveCounter = 0;
                int pW = int.Parse(txtPrintWidth.Text);
                int pH = int.Parse(txtPrintHeight.Text);
                int pad = int.Parse(txtPrintPadding.Text);
                int pagesCount = allPrintPages.Count;
                List<string> filesToDelete = new List<string>();

                Exception ex = winWorking.waitForTask<Exception>((updateFunc) =>
                {

                    DuplexTemplates dt = new DuplexTemplates(Properties.Resources.GitInfo.Split(' ')[0]);
                    foreach (SelectablePrintPage page in allPrintPages)
                    {

                        try
                        {
                            updateFunc("Export page " + page.PageNumber, (int)(100.0f * saveCounter / 2 / pagesCount));

                            var b = dt.BuildFace(page.Front, pW, pH, pad);
                            var bName = System.IO.Path.Combine(
                                    fi.Directory.FullName,
                                    "_temp_" + String.Format("{0:000000000}", saveCounter++) + ".jpg"
                                    );
                            b.Save(bName);
                            filesToDelete.Add(bName);
                            b.Dispose();

                            b = dt.BuildFace(page.Back, pW, pH, pad);
                            bName = System.IO.Path.Combine(
                                   fi.Directory.FullName,
                                   "_temp_" + String.Format("{0:000000000}", saveCounter++) + ".jpg"
                                   );
                            b.Save(bName);
                            filesToDelete.Add(bName);
                            b.Dispose();

                            b = null;
                        }
                        catch (Exception ex1)
                        {
                            return new Exception("Exception exporting page " + page.PageNumber, ex1);
                        }
                    }

                    return null;
                },
                isProgressKnwon: true);

                if (ex != null)
                {
                    MessageBox.Show("Error occured while exporting pdf (image step).\n" + ex.ToString());
                }
                else
                {
                    ex = winWorking.waitForTask<Exception>((updateFunc) =>
                    {
                        try
                        {
                            updateFunc("Converting images to PDF...", 0);
                            Core.MagickImaging.Convert(filesToDelete, fi.FullName, fi.Directory.FullName);
                        }
                        catch (Exception ex2)
                        {
                            return ex2;
                        }

                        return null;
                    }, false);

                    if (ex != null)
                    {
                        MessageBox.Show("Error occured while exporting pdf (convert step).\n" + ex.ToString());
                    }
                    else
                    {
                        foreach (var f in filesToDelete)
                            File.Delete(f);

                        MessageBox.Show("Export done successfully!");
                    }

                }

            }
        }

        #endregion

    }
}
