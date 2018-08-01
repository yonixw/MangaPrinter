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

                Func<System.IO.FileSystemInfo, object> orderFunc = (si) => si.CreationTime;
                if (rbByName.IsChecked ?? false)
                    orderFunc = (si) => si.Name;

                List<Core.MangaChapter> chapters = winWorking.waitForTask((updateFunc) =>
                {
                    return fileImporter.getChapters(DirPath, subFolders, cutoff, rtl, orderFunc, updateFunc);
                },
                isProgressKnwon: false);

                chapters.ForEach((cp) => { cp.Pages = Tools.ObservableFactory.ToList(cp.Pages).Cast<Core.MangaPage>().ToList(); });
                tvFiles.ItemsSource = Tools.ObservableFactory.ToList(chapters);

            }
        }

        private void tvFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvFiles.SelectedItem is Tools.Observable<Core.MangaPage>)
                tvFiles.ContextMenu = tvFiles.Resources["menuPage"] as System.Windows.Controls.ContextMenu;
            else
                tvFiles.ContextMenu = tvFiles.Resources["menuChapter"] as System.Windows.Controls.ContextMenu;
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void tvFiles_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //SO?592373
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;

                if (tvFiles.SelectedItem is Tools.Observable<Core.MangaPage>)
                    tvFiles.ContextMenu = tvFiles.Resources["menuPage"] as System.Windows.Controls.ContextMenu;
                else
                    tvFiles.ContextMenu = tvFiles.Resources["menuChapter"] as System.Windows.Controls.ContextMenu;


                tvFiles.ContextMenu.IsOpen = true;
            }
        }

        void TreeAction<T>(TreeView tree, Action<T> action) where T : class
        {
            T obj = tree.SelectedItem as T;
            if (obj != null)
            {
                action(obj);
            }
        }

        private void mnuToSingle_Click(object sender, RoutedEventArgs e)
        {
            TreeAction<Tools.Observable<Core.MangaPage>>(tvFiles, (ob) => ob.Act(page => page.IsDouble = false));
        }

        private void mnuToDouble_Click(object sender, RoutedEventArgs e)
        {
            TreeAction<Tools.Observable<Core.MangaPage>>(tvFiles, (ob) => ob.Act(page => page.IsDouble = true));
        }
    }
}
