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
using MangaPrinter.WpfGUI.Dialogs;
using System.Drawing;
using System.IO.Packaging;
using System.ComponentModel;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp;
using MangaPrinter.Conf;
using MangaPrinter.Core.ChapterBuilders;

namespace MangaPrinter.WpfGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<MangaChapter> mangaChapters = new BindingList<MangaChapter>();


        public MainWindow()
        {
            InitializeComponent();
        }

        bool winLoaded = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstFileChapters.ItemsSource = mangaChapters;
            lstFileChaptersBinding.ItemsSource = new List<MangaChapter>();
            
            mangaChapters.ListChanged += MangaChapters_ListChanged;

            
            if (CoreConfLoader.JsonConfigInstance != null)
            {
                ConfigChanged(CoreConfLoader.JsonConfigInstance);
            }
            CoreConfLoader.onConfigFinishUpdate += ConfigChanged;

            winLoaded = true;
        }

        void ConfigChanged(JsonConfig config)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(()=>ConfigChanged(config));
                return;
            }


            var locationSize = CoreConf.I.Window_LocationSize.Get();
            Height = locationSize.Height;
            Width = locationSize.Width;

            if (CoreConf.I.Window_StartMode == "fixed")
            {
                Left = locationSize.X;
                Left = locationSize.Y;
            }

            txtGitInfo.Text = CoreConf.I.Info_GitVersion;
            txtConfigInfo.Text = String.Format("{0}.{1}",
                CoreConf.CURR_CONFIG_MAJOR_VERSION, CoreConf.CURR_CONFIG_MINOR_VERSION);

            txtPageMaxWidth.Text = CoreConf.I.Chapters_DblPgRatioCuttof.Get().ToString();

            rbByName.IsChecked = CoreConf.I.Chapters_SortImportBy == "by_name";
            rbByDate.IsChecked = CoreConf.I.Chapters_SortImportBy == "by_create_date";

            cbSubfolders.IsChecked = CoreConf.I.Chapters_ImportSubfolders;

            cbNumberFix.IsChecked = CoreConf.I.Chapters_SmartNumberImport;

            rbRTL.IsChecked = CoreConf.I.Chapters_ChapDir == "rtl";
            rbLTR.IsChecked = CoreConf.I.Chapters_ChapDir != "rtl";

            cbAddStart.IsChecked = CoreConf.I.Binding_AddStartPage;
            cbAddEnd.IsChecked = CoreConf.I.Binding_AddEndPage;

            cbUseAntiSpoiler.IsChecked = CoreConf.I.Binding_AniSpoilerBatch > 0;
            txtSpoilerPgNm.Text = CoreConf.I.Binding_AniSpoilerBatch.Get().ToString();

            //todo txtPrintPadding.Text = Config.exportPagePadding.ToString();

            cbExportMinimal.IsChecked = CoreConf.I.Binding_ExportImages;

            cbKeepColors.IsChecked = CoreConf.I.Binding_KeepColors;

            cbIncludeParent.IsChecked = CoreConf.I.Binding_AddParentFolder;

            int lastcbPageSizeSelected = cbPageSize.SelectedIndex;
            if (cbPageSize.ItemsSource == null)
            {
                cbPageSize.ItemsSource = new ObservableCollection<JPage>(CoreConf.I.Binding_PageSizeList.Get().ToList());
            }
            else
            {
                ((ObservableCollection<JPage>)cbPageSize.ItemsSource).Clear();
                CoreConf.I.Binding_PageSizeList.Get().ForEach(
                    (p) => ((ObservableCollection<JPage>)cbPageSize.ItemsSource).Add(p)
                );
            }
            cbPageSize.SelectedIndex = Math.Max(0, lastcbPageSizeSelected);

            rbBindDuplex.IsChecked = CoreConf.I.Binding_Type == "duplex";
            rbBindBookletStack.IsChecked = CoreConf.I.Binding_Type == "booklet_print";

            txtNonWindows.Text = CoreConf.I.Info_IsNotWindows.Get().ToString();
        }

        bool shouldUpdateStats = true;
        private void MangaChapters_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (shouldUpdateStats)
            {
                lblChCount.DataContext = null;
                lblChCount.DataContext = mangaChapters;
            }
        }

        int? verifyInteger(TextBox textBox, string fallbackValue)
        {
            int value = 0;
            if (!int.TryParse(textBox.Text, out value))
            {
                MessageBox.Show(this,"Can't convert \"" + textBox.Text + "\" to integer, try again.");
                textBox.Text = fallbackValue;
                return null;
            }

            return value;
        }

        float? verifyFloat(TextBox textBox, string fallbackValue)
        {
            float value = 0;
            if (!float.TryParse(textBox.Text, out value))
            {
                MessageBox.Show(this,"Can't convert \"" + textBox.Text + "\" to float, try again.");
                textBox.Text = fallbackValue;

                return null;
            }

            return value;
        }

        #region FilesTab

        private void txtPageMaxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            float? doubleAspect = verifyFloat(txtPageMaxWidth,
                CoreConf.I.Chapters_DblPgRatioCuttof.Get().ToString());
            if (doubleAspect != null && winLoaded)
            {
                UpdateApectCutoff((float)doubleAspect);
            }
        }

        
        // Use save dialog as a trick to get folder
        Microsoft.Win32.OpenFileDialog dlgSaveTrick = new Microsoft.Win32.OpenFileDialog();


        private void menuImprtFolders_Click(object sender, RoutedEventArgs e)
        {
            if (CoreConf.I.Info_IsNotWindows.Get())
            {
                MessageBox.Show("We detected you are not running under Windows.\nPlease choose a import path with no special characters.\nThanks.");
            }


            dlgSaveTrick.Filter = "Folder|_Choose.Here_";
            dlgSaveTrick.FileName = "_Choose.Here_";
            dlgSaveTrick.InitialDirectory = Environment.ExpandEnvironmentVariables(CoreConf.I.Chapters_ImportDir);
            dlgSaveTrick.CheckFileExists = false;
            dlgSaveTrick.Multiselect = false;
            dlgSaveTrick.ValidateNames = false;
            dlgSaveTrick.Title = "Choose folder to import from:";

            if (dlgSaveTrick.ShowDialog() == true)
            {
                Core.FileImporter fileImporter = new Core.FileImporter();

                var DirPath = new System.IO.FileInfo(dlgSaveTrick.FileName).Directory.FullName;
                var subFolders = cbSubfolders.IsChecked ?? false;
                float cutoff = float.Parse(txtPageMaxWidth.Text);
                var rtl = rbRTL.IsChecked ?? false;
                var numFix = cbNumberFix.IsChecked ?? false;

                Func<System.IO.FileSystemInfo, object> orderFunc = (si) => si.CreationTime;
                if (rbByName.IsChecked ?? false)
                    orderFunc = (si) => numFix ?  FileImporter.pad0AllNumbers(si.Name): si.Name;

                List<FileImporterError> importErrors = new List<FileImporterError>();
                winWorking.waitForTask(this, (updateFunc) =>
                {
                    return fileImporter.getChapters(DirPath, subFolders, cutoff, rtl, orderFunc, importErrors, updateFunc);
                },
                isProgressKnwon: false)
                .ForEach(ch => mangaChapters.Add(ch));

                if (importErrors.Count >0)
                {
                    (new dlgImportErrors() { DataErrors = importErrors }).ShowDialog();
                }

                if (mangaChapters.Count > 0 )
                {
                    StartWhiteRatioScan();
                }
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
            //ListBoxAction<Core.MangaPage>(lstFilePages, page => { page.IsDouble = false; page.Chapter.updatePageNumber(); });
            if (lstFileChapters.SelectedValue!=null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                Chapter.Pages
                    .Where(p => p.IsChecked)
                    .ForEach(p => p.IsDouble = false);
                Chapter.updateChapterStats();
            }
        }

        private void mnuToDouble_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChapters.SelectedValue != null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                Chapter.Pages
                    .Where(p => p.IsChecked)
                    .ForEach(p => p.IsDouble = true);
                Chapter.updateChapterStats();
            }
        }

        private void mnuToRTL_Click(object sender, RoutedEventArgs e)
        {
            mangaChapters.Where(ch => ch.IsChecked).ForEach(ch => ch.IsRTL = true);
        }

        private void mnuToLTR_Click(object sender, RoutedEventArgs e)
        {
            mangaChapters.Where(ch => ch.IsChecked).ForEach(ch => ch.IsRTL = false);
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
            Dialogs.dlgNewChapter dlgName = new Dialogs.dlgNewChapter()
            {
                Name = "Chapter. 000",
                Folder = "Book-Name"
            };
            if (dlgName.ShowDialog() ?? false)
            {
                mangaChapters.Add(MangaChapter.Extend<MangaChapter>(new Core.MangaChapter()
                {
                    IsRTL = rbRTL.IsChecked ?? false,
                    Pages = new ObservableCollection<Core.MangaPage>(),
                    Name = dlgName.Name,
                    ParentName = dlgName.Folder
                }));
            }
        }


        private void MnuClearAllData_Click(object sender, RoutedEventArgs e)
        {
            mangaChapters.Clear();
        }

        private void mnuDeleteCh_Click(object sender, RoutedEventArgs e)
        {
            List<MangaChapter> tempList =  mangaChapters.Where(ch => ch.IsChecked).ToList();
            tempList.ForEach(ch => mangaChapters.Remove(ch));
        }

        private void mnuAddChapterPages_Click(object sender, RoutedEventArgs e)
        {
            ListBoxAction<Core.MangaChapter>(lstFileChapters, (ch) =>
            {
                Microsoft.Win32.OpenFileDialog dlgOpenImages = new Microsoft.Win32.OpenFileDialog();
                dlgOpenImages.Filter = "Supported Images|" + Core.FileImporter.BitmapSupportedImagesExtensions;
                dlgOpenImages.FileName = "Open supported images";
                dlgOpenImages.CheckFileExists = true;
                dlgOpenImages.Multiselect = true;
                dlgOpenImages.Title = "Choose images to import:";

                if (dlgOpenImages.ShowDialog() == true)
                {
                    Core.FileImporter fileImporter = new Core.FileImporter();

                    float cutoff = float.Parse(txtPageMaxWidth.Text);
                    var numFix = cbNumberFix.IsChecked ?? false;

                    Func<System.IO.FileSystemInfo, object> orderFunc = (si) => si.CreationTime;
                    if (rbByName.IsChecked ?? false)
                        orderFunc = (si) => numFix ? FileImporter.pad0AllNumbers(si.Name) : si.Name; 


                    ch.autoPageNumbering = false;

                    List<FileImporterError> importErrors = new List<FileImporterError>();
                    winWorking.waitForTask(this, (updateFunc) =>
                    {
                        return fileImporter.importImages(dlgOpenImages.FileNames, cutoff, orderFunc, importErrors, updateFunc);
                    },
                    isProgressKnwon: true)
                    .ForEach(page => ch.Pages.Add(page));

                    ch.autoPageNumbering = true;
                    ch.updateChapterStats();

                    
                    if (importErrors.Count > 0)
                    {
                        (new dlgImportErrors() { DataErrors = importErrors }).ShowDialog();
                    }
                }
            });
        }

        private void MnPreviewManga_Click(object sender, RoutedEventArgs e)
        {
            Core.MangaPage page = lstFilePages.SelectedValue as Core.MangaPage;
            if (page != null)
            {
                Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(page, "File: " + page.Name);
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
                //Console.WriteLine("{0}.{1}: {2}", page.Chapter.Name, page.Name, page.AspectRatio);
                b.count++;
            }

            return buckets;
        }

        private void BtnNewCutoffRatio_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.dlgChooseCutoffRatio dlg = new Dialogs.dlgChooseCutoffRatio();
            dlg.InputBuckets = winWorking.waitForTask(this, (updateFunc) =>
            {
                return plotData(mangaChapters).ToList();
            },
            isProgressKnwon: false);
            dlg.ShowDialog();
            if ((dlg.DialogResult ?? false) && (dlg.InputBuckets != null) && (dlg.InputBuckets.Count > 0))
            {
                double newAspectCutoff = Math.Round(dlg.InputBuckets[dlg.BucketIndex].value, 2);
                txtPageMaxWidth.Text = newAspectCutoff.ToString();
                UpdateApectCutoff(newAspectCutoff);
            }
        }

        private void UpdateApectCutoff(double newAspectCutoff)
        {
            // Update all pages:
            winWorking.waitForTask(this, (updateFunc) =>
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

        #endregion

        #region Binding Tab

        private void txtSpoilerPgNm_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? batch = verifyInteger(txtSpoilerPgNm,
                CoreConf.I.Binding_AniSpoilerBatch.Get().ToString());

            if (batch != null)
            {
                cbUseAntiSpoiler.IsChecked = batch > 0;
            }
            else
            {
                cbUseAntiSpoiler.IsChecked = CoreConf.I.Binding_AniSpoilerBatch > 0;
            }
        }


        ObservableCollection<SelectablePrintPage> allPrintPages = new ObservableCollection<SelectablePrintPage>();
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (mangaChapters.Where(p => p.IsChecked).Count() == 0)
            {
                MessageBox.Show(this,"Please check at least one chapter!");
                return;
            }

            BookletOptions _boptions = new BookletOptions() {
                isBookletRTL = rbBookRTL.IsChecked ?? false
            };

            if (cbCover.IsChecked ?? false)
            {
                // Already assuming more than 1 chapter checked

                if (mangaChapters.Where(ch => ch.IsChecked).First().Pages.Count > 0)
                {
                    _boptions.bookletCoverFirst =
                        mangaChapters.Where(ch => ch.IsChecked).First().Pages.First();
                }

                if (mangaChapters.Where(ch => ch.IsChecked).Last().Pages.Count > 0)
                {
                    _boptions.bookletCoverLast =
                        mangaChapters.Where(ch => ch.IsChecked).Last().Pages.Last();
                }
            }

            IBindBuilder bindBuilder = 
                ( rbBindDuplex.IsChecked ?? false ) ? 
                (IBindBuilder)new DuplexBuilder() :
                (IBindBuilder)new BookletBinder();

            bool startPage = cbAddStart.IsChecked ?? false;
            bool endPage = cbAddEnd.IsChecked ?? false;
            int antiSpoiler = (cbUseAntiSpoiler.IsChecked ?? false) ? int.Parse(txtSpoilerPgNm.Text) : 0;

            var allSelectedChapters = mangaChapters.Where(ch=>ch.IsChecked).ToList();

            allPrintPages = winWorking.waitForTask<ObservableCollection<SelectablePrintPage>>(this, (updateFunc) =>
            {
                ObservableCollection<SelectablePrintPage> result = new ObservableCollection<SelectablePrintPage>();

                bindBuilder
                    .Build(allSelectedChapters.Cast<MangaChapter>().ToList(), startPage, endPage, antiSpoiler, _boptions)
                    .ForEach((p) => result.Add(PrintPage.Extend<SelectablePrintPage>(p)));

                return result;
            },
            isProgressKnwon: false);

            lstFileChaptersBinding.ItemsSource = allSelectedChapters;
            lstPrintPages.ItemsSource = allPrintPages;
        }

        bool selectPrintPages = true, selectPrintChapters = true;

        private void LstFileChaptersBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!selectPrintChapters) return;
            if (e.AddedItems.Count == 0) return;


            MangaChapter c = (MangaChapter)e.AddedItems[0] ?? (MangaChapter)e.RemovedItems[0];
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
            foreach (MangaChapter c in mangaChapters)
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

        class PhysicalPageInfo
        {
            public int singlePageWidth = 0;
            public int singlePageHeight = 0;
            public int paddingPx = 0;
            public int pageDepth = 0;

            public PhysicalPageInfo(JPage page, float paddingPercent)
            {
                if (page == null)
                    throw new Exception("Can't find page!!!");
                pageDepth = page.TargetDensity;
                paddingPx = Math.Max(0, (int)( page.HeightPixels * paddingPercent / 100.0f) -1);
                singlePageHeight = page.HeightPixels - 2 * paddingPx;
                singlePageWidth = (page.WidthPixels - 3 * paddingPx) / 2 ;
            }
        }

        private void MnuPrvwFront_Click(object sender, RoutedEventArgs e)
        {
            SelectablePrintPage p = (SelectablePrintPage)(((System.Windows.FrameworkElement)sender).DataContext);

            var faces = new PrintFace[] { p.Front };
            var sides = new PrintSide[] { p.Front.Right, p.Front.Left };

            var page = new PhysicalPageInfo(((JPage)cbPageSize.SelectedItem), CoreConf.I.Templates_PaddingPrcnt);

            var b = (new DuplexTemplates()).BuildFace(faces, sides,
                    page.singlePageWidth,page.singlePageHeight,
                    page.paddingPx, cbKeepColors.IsChecked ?? false, cbIncludeParent.IsChecked ?? false);


            Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(null,
                    "Front face of page: " + p.PageNumber,b);
            dlgImage.ShowDialog();
        }

        private void MnuPrvwBack_Click(object sender, RoutedEventArgs e)
        {
            SelectablePrintPage p = (SelectablePrintPage)(((System.Windows.FrameworkElement)sender).DataContext);
            var page = new PhysicalPageInfo(((JPage)cbPageSize.SelectedItem), CoreConf.I.Templates_PaddingPrcnt);

            var faces = new PrintFace[] { p.Back };
            var sides = new PrintSide[] { p.Back.Right, p.Back.Left };


            var b = (new DuplexTemplates()).BuildFace(faces, sides,
                    page.singlePageWidth, page.singlePageHeight,
                    page.paddingPx, cbKeepColors.IsChecked ?? false, cbIncludeParent.IsChecked ?? false);

            Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(null,
                    "Back face of page: " + p.PageNumber, b);
            dlgImage.ShowDialog();
        }

        private void LblHelpExportMinimal_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage =
                "Normally, We bundled a pdf converter to use without any user interaction.\n" +
                "But, every now and then comes an image format that causes bugs. (invalid format etc.)\n\n" +
                "By cheking \"" + cbExportMinimal.Content.ToString() + "\"," +
                    "the program will only export the book in seperage JPEGs (one per page side).\n" +
                "And you will have to combine them in another software.\n\n" +
                "We recommend using ImageMagick-7.\n" +
                "After install, try to run (In the output folder):\n\n" +
                "\"C:\\Program Files\\ImageMagick - 7.0.8 - Q16\\magick.exe\" *.jpg -monitor output.pdf";

            MessageBox.Show(this,helpMessage, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        SaveFileDialog dlgSave = new SaveFileDialog();
        void resetDlgSaveName()
        {
            if (dlgSave.FileName != "")
            {
                FileInfo fi = new FileInfo(dlgSave.FileName);
                dlgSave.FileName = System.IO.Path.Combine(fi.Directory.FullName, "Enter New Name!");
            }
        }
       

        private void lblKeepColorsHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpMessage =
               "Normally, we convert all pages to grayscale. This option lets you keep the color. \n\n" +
               "* Colorful PDFs are usually bigger (x3+). \n" +
               "* Colorful PDFs even sometimes cannot be printed by some industrial printers (maybe due to temp memory size). \n\n" +
               "So, It is not recommended to relay on the printer to convert to grayscale.";

            MessageBox.Show(this,helpMessage, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCalcWhiteRatio_Click(object sender, RoutedEventArgs e)
        {
            StartWhiteRatioScan(true);
        }

        private void StartWhiteRatioScan(bool checkedOnly = false)
        {
            if (checkedOnly && mangaChapters.Where(p=>p.IsChecked).Count() == 0 )
            {
                MessageBox.Show(this,"Please check at least one chapter!");
                return;
            }

            Dialogs.dlgEmptyInk dlgScan = new Dialogs.dlgEmptyInk();
            bool isSkip = ! ( dlgScan.ShowDialog() ?? false );
            bool isQuick = dlgScan.isQuick;

            if (isSkip) return;

            int TotlaPageCount = mangaChapters.Where(p=>checkedOnly? p.IsChecked : true)
                .Sum(p => isQuick ? Math.Min(6, p.Pages.Count) : p.Pages.Count);
            DateTime start = DateTime.Now;

            shouldUpdateStats = false; // Because we are updating the stats which will update ui in the middle

            Exception ex = winWorking.waitForTask<Exception>(this, (updateFunc) =>
            {
                try
                {
                    int pageCounter = 0;
                    foreach (var ch in mangaChapters)
                    {
                        if (checkedOnly && !ch.IsChecked)
                            continue;

                        for (int i = 0; i < ch.Pages.Count; i++)
                        {
                            var page = ch.Pages[i];

                            if (!isQuick || i < 3 || i > ch.Pages.Count - 1 - 3)
                            {
                                pageCounter++;
                                updateFunc(
                                         "Processing: " + ch.Name + " -> " + page.Name,
                                         (int)(100.0f * pageCounter / TotlaPageCount)
                                     );
                                using (Bitmap b1 = MagickImaging.BitmapFromUrlExt(page))
                                {
                                    using (Bitmap b2 =
                                        CoreConf.I.Info_IsNotWindows.Get() ?
                                        // MakeGrayscale3 slower than MakeBW1 but supported in linux, i think
                                        GraphicsUtils.MakeGrayscale3(b1) : GraphicsUtils.MakeBW1(b1) )
                                    {
                                        page.WhiteBlackRatio = MagickImaging.WhiteRatio(b2);
                                        page.AspectRatio =
                                            b2.Height > 0 ?
                                                1.0f * b2.Width / b2.Height :
                                                MangaPage.MinRatio;
                                    }
                                }
                            }
                        }
                        ch.updateChapterStats();
                    }
                }
                catch (Exception ex2)
                {
                    return ex2;
                }

                return null;
            }, true);

            shouldUpdateStats = true;
            lblChCount.DataContext = null;
            lblChCount.DataContext = mangaChapters;

            if (ex != null)
            {
                MessageBox.Show(this,"Error occured while reading white ratio (convert step).\n" + ex.ToString());
            }
            else
            {
                MessageBox.Show(this, "Done!, Took:" + (DateTime.Now - start).ToString());
            }
        }

        private void mnuChSelectAll_Click(object sender, RoutedEventArgs e)
        {
            mangaChapters.ForEach((ch) => ch.IsChecked = true);
        }

        private void mnuChSelectNone_Click(object sender, RoutedEventArgs e)
        {
            mangaChapters.ForEach((ch) => ch.IsChecked = false);
        }

        private void mnuPgSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChapters.SelectedValue != null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                Chapter.Pages
                    .ForEach(p => p.IsChecked = true);
            }
        }

        private void mnuPgSelectNone_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChapters.SelectedValue != null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                Chapter.Pages
                    .ForEach(p => p.IsChecked = false);
            }
        }

        private void mnuDeletePg_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChapters.SelectedValue != null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                List<MangaPage> tempPages = Chapter.Pages.Where(p => p.IsChecked ).ToList();
                tempPages.ForEach(p => Chapter.Pages.Remove(p));
                Chapter.updateChapterStats();
            }
        }


        private List<string> getBindedRange()
        {
            List<string> chaptersInfo = new List<string>();   

            MangaChapter lastChapter = null;
            int lastChapterStart = -1;
            foreach (var _p in lstPrintPages.Items)
            {
                var page  = (PrintPage)_p;
                var Faces = new[] { page?.Front, page?.Back };
                foreach (var Face in Faces)
                {
                    var Sides = new[] { Face.Left, Face.Right };
                    if (Face.IsRTL) Sides = new[] { Face.Right, Face.Left };
                    
                    foreach (var Side in Sides)
                    {
                        if (Side.MangaPageSource?.Chapter != null)
                        {
                            var ch = Side.MangaPageSource.Chapter;
                            if ((ch == null && lastChapter != null) || !ch.isEqual(lastChapter))
                            {
                                chaptersInfo.Add(
                                    string.Format(HTMLItem, lastChapter?.Name + string.Format(" [{0}-{1}]", lastChapterStart, Face.FaceNumber-1), lastChapter?.ParentName)
                                );
                                lastChapter = ch;
                                lastChapterStart = Face.FaceNumber;
                            }
                        }
                    }
                }
            }

            // Add unclosed chapter:
            if (lastChapter != null)
            {
                chaptersInfo.Add(
                    string.Format(HTMLItem, lastChapter?.Name + string.Format(" [{0}-{1}]", lastChapterStart, lstPrintPages.Items.Count *2), lastChapter?.ParentName)
                );
            }

            if (chaptersInfo.Count > 0)
            {
                chaptersInfo.RemoveAt(0); // The first element is null chapter
            }

            return chaptersInfo;
        }

        const string HTMLItem = "<li><span>{0}</span><br><span style='color: dimgray;'>{1}</span></li>";
        private void mnuExportTOC_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChaptersBinding.ItemsSource != null && ((List<MangaChapter>)lstFileChaptersBinding.ItemsSource).Count == 0)
            {
                MessageBox.Show(this,"Please bind at least one chapter!");
                return;
            }


            bool bindedOnly = true;

            resetDlgSaveName();
            dlgSave.Title = "Export Table of content as PDF";
            dlgSave.Filter = "PDF |*.pdf";
            if (dlgSave.ShowDialog() == true)
            {
                FileInfo fi = new FileInfo(dlgSave.FileName);

                string StartHTML = string.Format("[{0}] <h1>{1}</h1>"+ "<ol>", DateTime.Now.ToShortDateString(), fi.Name) ;
                
                const string EndHTML = "</ol>";

                var list = bindedOnly ?
                    getBindedRange()
                    :
                     mangaChapters.Select(ch => string.Format(HTMLItem, ch.Name, ch.ParentName)).ToList();


                try
                {
                    PdfDocument pdf = PdfGenerator.GeneratePdf(
                    StartHTML +
                    string.Join("\n", list) +
                    EndHTML
                    , PageSize.A4);
                    pdf.Save(fi.FullName);
                    System.Diagnostics.Process.Start(fi.FullName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,"Error occured while saving TOC pdf.\n" + ex.ToString());
                }
            }
        }

        private void mnuChInsertUp_Click(object sender, RoutedEventArgs e)
        {
            List<MangaChapter> myChecked = mangaChapters.Where(c => c.IsChecked).Reverse().ToList();

            // Remove only if found 
            ListBoxAction<MangaChapter>(lstFileChapters, (ch) =>
            {
                // If  not chose a checked
                if (myChecked.IndexOf(ch) > -1)
                    return;

                myChecked.ForEach(c => mangaChapters.Remove(c));
                int index = mangaChapters.IndexOf(ch);
                myChecked.ForEach(c => mangaChapters.Insert(index, c));
            });
        }

        private void mnuChInsertDown_Click(object sender, RoutedEventArgs e)
        {
            List<MangaChapter> myChecked = mangaChapters.Where(c => c.IsChecked).Reverse().ToList();

            // Remove only if found
            ListBoxAction<MangaChapter>(lstFileChapters, (ch) =>
            {
                // If  not chose a checked
                if (myChecked.IndexOf(ch) > -1)
                    return;

                myChecked.ForEach(c => mangaChapters.Remove(c));
                int index = mangaChapters.IndexOf(ch);
                myChecked.ForEach(c => mangaChapters.Insert(index+1, c));
            });
        }

        private void mnuQuickDelete_Click(object sender, RoutedEventArgs e)
        {
            bool addFirstLast3 = MessageBox.Show("Add 3 First/Last pages?", "Smart Delete", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes;

            dlgBluredImageListActions dlg = new dlgBluredImageListActions();
            dlg.CustomTitle = "Smart Delete pages";
            ObservableCollection<ActionMangaPage<bool>> pagesToInspect = new ObservableCollection<ActionMangaPage<bool>>();
            int i = 0;
            mangaChapters.ForEach(ch =>
            {
                i = 0;
                ch.Pages.ForEach(_p =>
                {
                    ActionMangaPage<bool> p = new ActionMangaPage<bool>() { Page = _p, Result = false };

                    // Add first/lest 3
                    if (addFirstLast3 && (i < 3 || i + 3 >= ch.Pages.Count))
                    {
                        pagesToInspect.Add(p);
                    }
                    else if (p.Page.WhiteBlackRatio < CoreConf.I.Common_Alerts_InkFillLow || p.Page.WhiteBlackRatio > CoreConf.I.Common_Alerts_InkFillHigh)
                    {
                        pagesToInspect.Add(p);
                    }
                    else if (p.Page.AspectRatio < CoreConf.I.Common_Alerts_TooVertical)
                    {
                        pagesToInspect.Add(p);
                    }
                    else if (p.Page.IsChecked || p.Page.Chapter.IsChecked)
                    {
                        pagesToInspect.Add(p);
                    }
                    i++;
                });
            });
            dlg.Pages = pagesToInspect;

            if (dlg.ShowDialog() ?? false)
            {
                pagesToInspect
                    .Where(p => p.Result==true)
                    .ForEach(p => { 
                        p.Page.Chapter.Pages.Remove(p.Page); 
                        p.Page.Chapter.updateChapterStats(); 
                    });
            }

            
        }

        private void mnuSmartDeleteInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this,">> Smart delete includes choises:\n" +
                "+ (Optional) First and last 3 pages of each chapter\n" +
                "+ Checked pages\n" +
                "+ Checked chapters (all pages)\n" +
                "\n>> Smart delete includes found problems (if analyzed before):\n" +
                "* 🔳 InkFill% > " + CoreConf.I.Common_Alerts_InkFillHigh.Get() + "\n" +
                "* 🔳 InkFill% < " + CoreConf.I.Common_Alerts_InkFillLow.Get() + "\n" +
                "* ➗ TooVertical < " + CoreConf.I.Common_Alerts_TooVertical.Get() + "\n"
                );
        }

        bool _config_open = false;
        private void btnConfigMgr_Click(object sender, RoutedEventArgs e)
        {
            if (_config_open)
            {
                MessageBox.Show("Config dialog already open, can use one at a time.");
                return;
            }

            _config_open = true;
            dlgConfigMngr dlg = new dlgConfigMngr();
            dlg.Owner = this;
            dlg.Closed += Config_Dlg_Closed;
            dlg.Show();
        }

        private void Config_Dlg_Closed(object sender, EventArgs e)
        {
            _config_open = false;
        }

        private void mnuToOmit_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChapters.SelectedValue != null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                Chapter.Pages
                    .Where(p => p.IsChecked)
                    .ForEach(p => p.Effects.IsOmited = !p.Effects.IsOmited);
                Chapter.updateChapterStats();
            }
        }

        private void mnCrop_Click(object sender, RoutedEventArgs e)
        {
            Core.MangaPage page = lstFilePages.SelectedValue as Core.MangaPage;
            if (page != null)
            {
                Dialogs.dlgBluredImageFastCrop dlgImage = 
                    new Dialogs.dlgBluredImageFastCrop(page,  "File: " + page.Name);
                dlgImage.ShowDialog();
            }
        }

        private void mnuDummy_Click(object sender, RoutedEventArgs e)
        {
            if (lstFileChapters.SelectedValue != null)
            {
                MangaChapter Chapter = (MangaChapter)lstFileChapters.SelectedValue;
                Core.MangaPage page = lstFilePages.SelectedValue as Core.MangaPage;
                int index = 0;
                if (page != null)
                    index = Chapter.Pages.IndexOf(page);
                

                Chapter.Pages.Insert(Math.Max(0, index), new MangaPage()
                {
                    AspectRatio = 1f,
                    Chapter = Chapter,
                    ChildIndexStart = -1,
                    ChildIndexEnd = -1,
                    Effects = new PageEffects() { VirtualPath = MagickImaging.DummyImages.White200Sqr },
                    IsDouble = false,
                    ImagePath = "virtual_path",
                    WhiteBlackRatio = 1,
                    Name = "Dummy Full White"
                }); ;
                Chapter.updateChapterStats();
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            SelectablePrintPage p = (SelectablePrintPage)(((System.Windows.FrameworkElement)sender).DataContext);

            bool bookletRTL = (rbBookRTL.IsChecked ?? false);

            PrintFace newPage = new PrintFace()
            {
                BatchPaperNumber = p.Front.BatchPaperNumber,
                IsRTL = bookletRTL,
                FaceNumber = p.Front.FaceNumber,
                PrintFaceType = FaceType.SINGLES,

                Left = bookletRTL ? p.Back.Left: p.Front.Left,
                Right = !bookletRTL ? p.Back.Left : p.Front.Left,
            };

            

            var faces = new PrintFace[] { newPage };
            var sides = new PrintSide[] {  };

            var page = new PhysicalPageInfo(((JPage)cbPageSize.SelectedItem), CoreConf.I.Templates_PaddingPrcnt);

            var b = (new DuplexTemplates()).BuildFace(faces, sides,
                    page.singlePageWidth, page.singlePageHeight,
                    page.paddingPx, cbKeepColors.IsChecked ?? false, cbIncludeParent.IsChecked ?? false);


            Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(null,
                    "Left face of page: " + p.PageNumber, b);
            dlgImage.ShowDialog();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            SelectablePrintPage p = (SelectablePrintPage)(((System.Windows.FrameworkElement)sender).DataContext);

            bool bookletRTL = (rbBookRTL.IsChecked ?? false);

            PrintFace newPage = new PrintFace()
            {
                BatchPaperNumber = p.Front.BatchPaperNumber,
                IsRTL = bookletRTL,
                FaceNumber = p.Front.FaceNumber,
                PrintFaceType = FaceType.SINGLES,

                Right = bookletRTL ? p.Front.Right : p.Back.Right,
                Left = !bookletRTL ? p.Front.Right : p.Back.Right,
            };

            var faces = new PrintFace[] { newPage };
            var sides = new PrintSide[] { };

            var page = new PhysicalPageInfo(((JPage)cbPageSize.SelectedItem), CoreConf.I.Templates_PaddingPrcnt);

            var b = (new DuplexTemplates()).BuildFace(faces, sides,
                    page.singlePageWidth, page.singlePageHeight,
                    page.paddingPx, cbKeepColors.IsChecked ?? false, cbIncludeParent.IsChecked ?? false);


            Dialogs.dlgBluredImage dlgImage = new Dialogs.dlgBluredImage(null,
                    "Right face of page: " + p.PageNumber, b);
            dlgImage.ShowDialog();
        }

        private void MnuExport_Click(object sender, RoutedEventArgs e)
        {
            resetDlgSaveName();
            dlgSave.Title = "Export book as PDF";
            dlgSave.Filter = "PDF |*.pdf";
            if (dlgSave.ShowDialog() == true)
            {
                FileInfo fi = new FileInfo(dlgSave.FileName);
                PrintPage.lastFullExportMetadata = fi.Name.Replace(fi.Extension,"");
                int saveCounter = 0;

                var pageInfo = new PhysicalPageInfo(((JPage)cbPageSize.SelectedItem), CoreConf.I.Templates_PaddingPrcnt);

                int pW = pageInfo.singlePageWidth;
                int pH = pageInfo.singlePageHeight;
                int pad = pageInfo.paddingPx;

                int pagesCount = allPrintPages.Count;
                bool convertPdf = !(cbExportMinimal.IsChecked??false);
                bool keepColors = cbKeepColors.IsChecked ?? false;
                bool parentText = cbIncludeParent.IsChecked ?? false;
                List<string> filesToDelete = new List<string>();

                DateTime timeTemp;
                TimeSpan timeTemplates = TimeSpan.FromSeconds(0);
                TimeSpan timeAddPages = TimeSpan.FromSeconds(0);
                TimeSpan timeSavePdf = TimeSpan.FromSeconds(0);

                timeTemp = DateTime.Now;
                Exception ex = winWorking.waitForTask<Exception>(this, (updateFunc) =>
                {

                    ITemplateBuilder dt = new DuplexTemplates();
                    foreach (SelectablePrintPage page in allPrintPages)
                    {

                        try
                        {
                            updateFunc("[1/3] Export page " + page.PageNumber, (int)(100.0f * saveCounter / 2 / pagesCount));

                            var faces = new PrintFace[] { page.Front };
                            var sides = new PrintSide[] { page.Front.Right, page.Front.Left };

                            var b = dt.BuildFace(faces, sides, pW, pH, pad, keepColors, parentText);
                            var bName = System.IO.Path.Combine(
                                    fi.Directory.FullName,
                                    "_temp_" + String.Format("{0:000000000}", saveCounter++) + ".jpg"
                                    );
                            b.Save(bName);
                            filesToDelete.Add(bName);
                            b.Dispose();

                            faces = new PrintFace[] { page.Back };
                            sides = new PrintSide[] { page.Back.Right, page.Back.Left };

                            b = dt.BuildFace(faces, sides, pW, pH, pad, keepColors, parentText);
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
                timeTemplates = DateTime.Now - timeTemp;

                if (ex != null)
                {
                    MessageBox.Show(this,"Error occured while exporting pdf (image step).\n" + ex.ToString());
                }
                else
                {
                    if (convertPdf)
                    {
                        using (Core.MagickImaging pdfMagik = new MagickImaging())
                        {
                            timeTemp = DateTime.Now;
                            ex = winWorking.waitForTask<Exception>(this,(updateFunc) =>
                                {
                                    try
                                    {
                                        Action<int> onUpdateIndex = new Action<int>((index) =>
                                            {
                                                updateFunc("[2/3] Adding pdf page " + index, (int)(100.0f * index / filesToDelete.Count));
                                            });
                                        pdfMagik.MakeList(filesToDelete, fi.Directory.FullName,pageInfo.pageDepth, updateIndex: onUpdateIndex);
                                    }
                                    catch (Exception ex2)
                                    {
                                        return ex2;
                                    }

                                    return null;
                                }, true);
                            timeAddPages = DateTime.Now - timeTemp;

                            if (ex == null)
                            {
                                timeTemp = DateTime.Now;
                                ex = winWorking.waitForTask<Exception>(this,(updateFunc) =>
                                {
                                    try
                                    {
                                        updateFunc("[3/3] Saving pdf to file...",0);
                                        pdfMagik.SaveListToPdf(fi.FullName);
                                    }
                                    catch (Exception ex2)
                                    {
                                        return ex2;
                                    }

                                    return null;
                                }, false);
                                timeSavePdf = DateTime.Now - timeTemp;
                            }
                        }
                    }

                    if (ex != null)
                    {
                        MessageBox.Show(this,"Error occured while exporting pdf (convert step).\n" + ex.ToString());
                    }
                    else
                    {
                        if (convertPdf)
                        {
                            foreach (var f in filesToDelete)
                                File.Delete(f); 
                        }

                        string TimingInfo =
                            "1) Save pages  - " + timeTemplates.ToString() + "\n" +
                            "2) Add to PDF  - " + timeAddPages.ToString() + "\n" +
                            "3) PDF to File - " + timeSavePdf.ToString();
                        MessageBox.Show(this,
                            "Export done successfully!\n" +
                            "===============\n" +
                            TimingInfo
                            ,"Done");
                    }

                }

            }
        }

        #endregion

    }
}
