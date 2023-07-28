using MangaPrinter.Conf;
using MangaPrinter.Core;
using MangaPrinter.Core.TemplateBuilders;
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
using System.Windows.Threading;

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
        public double maxBlur = 40;

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
            {
                if (myImage.OriginalImage != null )
                {
                    myImage.OriginalImage.Dispose();
                    myImage.OriginalImage = null;
                }
                myImage.Image = null;
            }

            // Reset blur on new picture
            if (!(cbKeepBlur.IsChecked ?? false))
            {
                slideBlur.Value = CoreConf.I.Common_PreviewBlurPrcnt;
            }


            System.Drawing.Bitmap bm = MagickImaging.BitmapFromUrlExt(imageUrl);
            imgMain.DataContext = myImage = new MyImageBind()
            {
                OriginalImage = bm,
                Image = dlgBluredImage.Bitmap2BitmapImage(
                        GraphicsUtils.sameAspectResize(bm,
                        (int)(1.0f * bm.Width * (1.0f * this.Height / bm.Height)), 
                        (int)this.Height, reuse: true)
                ),
                BlurRadius = slideBlur.Value * maxBlur / 100,
                Zoom = 1f,
                Offset = new MyPointD() { X = 0, Y = 0 }
            };

            resetZoom();
        }

        DispatcherTimer debounceTimer =
           new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = false };



        bool winLoaded = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(CustomTitle))
                Title = CustomTitle;
            lstPages.ItemsSource = Pages;

            if (CoreConfLoader.JsonConfigInstance != null)
            {
                ConfigChanged(CoreConfLoader.JsonConfigInstance);
            }
            CoreConfLoader.onConfigFinishUpdate += ConfigChanged;


            debounceTimer.Tick += DebounceTimer_Tick;

            winLoaded = true;
        }


        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            // Blur is relative to real image dimentions.
            // To make it feel the same, we will resize image based on form size, 
            // But will multiply with zoom to allow the user to read details he zoomed on...

            debounceTimer.Stop();
            debounceTimer.IsEnabled = false;

            if (
                myImage != null &&
                myImage.OriginalImage != null &&
                (int)(this.Width * myImage.Zoom) > 0 &&
                (int)(this.Height * myImage.Zoom) > 0
                )
            {
                imgMain.DataContext = null;
                myImage.Image = dlgBluredImage.Bitmap2BitmapImage(
                            GraphicsUtils.sameAspectResize(myImage.OriginalImage,
                            (int)( myImage.Zoom *1.0f * myImage.OriginalImage.Width * (1.0f* this.Height / myImage.OriginalImage.Height)), 
                            (int)(myImage.Zoom * this.Height ), reuse: true)
                );
                imgMain.DataContext = myImage;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Reset
            debounceTimer.IsEnabled = true;
            debounceTimer.Stop();

            // Debounce again
            debounceTimer.Start();
        }

        void ConfigChanged(JsonConfig config)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(() => ConfigChanged(config));
                return;
            }

            maxBlur = CoreConf.I.Common_MaxPreviewBlurRadius;
            slideBlur.Value = CoreConf.I.Common_PreviewBlurPrcnt;
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
            {
                if (myImage.OriginalImage != null)
                {
                    myImage.OriginalImage.Dispose();
                    myImage.OriginalImage = null;
                }
                myImage.Image = null;
            }
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
                //LoadImage(p.Page.ImagePath);
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
