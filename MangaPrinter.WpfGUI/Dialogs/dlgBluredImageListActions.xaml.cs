using MangaPrinter.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MangaPrinter.WpfGUI.Dialogs
{



    /// <summary>
    /// Interaction logic for dlgBluredImageListAction.xaml
    /// </summary>
    public partial class dlgBluredImageListActions : Window
    {

        public string CustomTitle {get;set;}

        public ObservableCollection<ActionMangaPage<bool>> Pages { get; set; }

        
        public dlgBluredImageListActions()
        {
            InitializeComponent();
        }

        MyImageBind myImage = null;
        public const double maxBlur = 40;

        void resetZoom()
        {
            float ratioBitmap = (float)myImage.Image.Width / (float)myImage.Image.Height;
            float ratioMax = (float)cnvsImage.ActualWidth / (float)cnvsImage.ActualHeight;

            int finalHeight = (int)cnvsImage.ActualHeight;
            if (ratioMax <= ratioBitmap)
            {
              finalHeight = (int)((float)cnvsImage.ActualWidth / ratioBitmap);
            }

            // for future:
            //      int finalWidth =  (int)cnvsImage.ActualWidth;
            //      ... else: finalWidth = (int)((float)cnvsImage.ActualHeight * ratioBitmap);
        
            myImage.Zoom =   finalHeight *1.0f / myImage.Image.Height;
        }

        void LoadImage(string imageUrl)
        {
            if (myImage != null)
                myImage.Image = null;

            // Reset blur on new picture
            if (!(cbKeepBlur.IsChecked ?? false))
            {
                slideBlur.Value = 100;
            }

            BitmapImage bm = dlgBluredImage.LoadBitmapWithoutLockingFile(imageUrl);
            imgMain.DataContext = myImage = new MyImageBind()
            {
                Image = bm,
                BlurRadius = slideBlur.Value * maxBlur / 100,
                Zoom = 1f,
                Offset = new MyPointD() { X = 0, Y = 0 }
            };

            resetZoom();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(CustomTitle))
                Title = CustomTitle;
            lstPages.ItemsSource = Pages;
        }

        bool inMove = false;
        Point startPointMouse, startPointWindow;
        private void imgMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            inMove = true;
            startPointMouse = Mouse.GetPosition(this);
            startPointWindow = new Point(Canvas.GetLeft(imgMain), Canvas.GetTop(imgMain));
        }

        private void cnvsImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (inMove)
            {
                Point currentMouse = Mouse.GetPosition(this);
                Canvas.SetLeft(imgMain, currentMouse.X - startPointMouse.X + startPointWindow.X);
                Canvas.SetTop(imgMain, currentMouse.Y - startPointMouse.Y + startPointWindow.Y);

            }
        }

        private void cnvsImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Point currentMouse = Mouse.GetPosition(cnvsImage);
            // only stop if leaving to outside (not back to image)
            if (currentMouse.X > 0 && currentMouse.X < cnvsImage.ActualWidth && currentMouse.Y > 0 && currentMouse.Y < cnvsImage.ActualHeight)
            { }
            else
                inMove = false;

        }

        private void slideBlur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (myImage != null)
                myImage.BlurRadius = slideBlur.Value * maxBlur / 100;
        }

        private void cnvsImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            slideZoom.Value += e.Delta / 10;
        }

        private void imgMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            inMove = false;
        }

        private void slideZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Zoom(slideZoom.Value);
        }

        private void btnRefit_Click(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(imgMain, 0);
            Canvas.SetTop(imgMain, 0);
            resetZoom();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (myImage != null)
                myImage.Image = null;
        }

        private void lstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPages.SelectedIndex > -1)
            {
                ActionMangaPage<bool> p = (ActionMangaPage<bool>)lstPages.SelectedItem;
                LoadImage(p.Page.ImagePath);
            }
        }

        private void btnKeep_Click(object sender, RoutedEventArgs e)
        {
            lstPages.Focus();
            if (lstPages.SelectedIndex > -1)
            {
                ActionMangaPage<bool> p = (ActionMangaPage<bool>)lstPages.SelectedItem;
                p.Result = false;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            lstPages.Focus();
            if (lstPages.SelectedIndex > -1)
            {
                ActionMangaPage<bool> p = (ActionMangaPage<bool>)lstPages.SelectedItem;
                LoadImage(p.Page.ImagePath);
                p.Result = true;
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void lstPages_KeyUp(object sender, KeyEventArgs e)
        {
            if (lstPages.SelectedIndex > -1)
            {
                ActionMangaPage<bool> p = (ActionMangaPage<bool>)lstPages.SelectedItem;

                if (e.Key == Key.Delete || e.Key == Key.D || e.Key == Key.Back)
                {
                    p.Result = true;
                }

                if (e.Key == Key.Enter)
                {
                    p.Result = !p.Result;
                }
            }
        }

        public void Zoom(double percent)
        {
            if (myImage != null)
            {
                myImage.Zoom = percent / 100;
            }
        }
    }

    
}
