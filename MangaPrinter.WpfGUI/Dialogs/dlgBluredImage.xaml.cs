using MangaPrinter.Conf;
using MangaPrinter.Core;
using MangaPrinter.Core.TemplateBuilders;
using System;
using System.Collections.Generic;
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

    public class MyPointD : ModelBaseWpf
    {
        public double X { get { return _baseGet(); } set { _baseSet(value); } }
        public double Y { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public class MyImageBind : ModelBaseWpf
    {
        public BitmapImage Image { get { return _baseGet(); } set { _baseSet(value); } }
        public double BlurRadius { get { return _baseGet(); } set { _baseSet(value); } }
        public double Zoom { get { return _baseGet(); } set { _baseSet(value); } }
        public MyPointD Offset { get { return _baseGet(); } set { _baseSet(value); } }
    }

    /// <summary>
    /// Interaction logic for dlgBluredImage.xaml
    /// </summary>
    public partial class dlgBluredImage : Window
    {
        private System.Drawing.Bitmap _custom;
        private MangaPage _page;
        private string _title;

        public dlgBluredImage(MangaPage page, string DialogTitle, System.Drawing.Bitmap custom = null)
        {
            _page = page;
            _title = DialogTitle;
            _custom = custom; // cases like front/back after build duplex face
            InitializeComponent();
        }

        MyImageBind myImage;
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
        
            slideZoom.Value = (100.0f *  finalHeight)/ myImage.Image.Height;
        }

        public static BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/dee7cb68-aca3-402b-b159-2de933f933f1/disposing-a-wpf-image-or-bitmapimage-so-the-source-picture-file-can-be-modified?forum=wpf

            System.Windows.Media.Imaging.BitmapImage result = new System.Windows.Media.Imaging.BitmapImage();  // Create new BitmapImage  
            System.IO.Stream stream = new System.IO.MemoryStream();  // Create new MemoryStream  

            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);  // Save the loaded Bitmap into the MemoryStream - Png format was the only one I tried that didn't cause an error (tried Jpg, Bmp, MemoryBmp)  
            bitmap.Dispose();  // Dispose bitmap so it releases the source image file 
            
            result.BeginInit();  // Begin the BitmapImage's initialisation  
            result.StreamSource = stream;  // Set the BitmapImage's StreamSource to the MemoryStream containing the image  
            result.EndInit();  // End the BitmapImage's initialisation  

            return result;
        }


        bool winLoaded = false;
        DispatcherTimer debounceTimer = 
            new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = false };


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = _title;

            if (CoreConfLoader.JsonConfigInstance != null)
            {
                ConfigChanged(CoreConfLoader.JsonConfigInstance);
            }
            CoreConfLoader.onConfigFinishUpdate += ConfigChanged;

            System.Drawing.Bitmap bm =
                _custom != null ? _custom : MagickImaging.BitmapFromUrlExt(_page);
            imgMain.DataContext = myImage = new MyImageBind() {
                Image = Bitmap2BitmapImage(
                        GraphicsUtils.sameAspectResize(bm, 
                        (int)(1.0f * bm.Width * (1.0f * cnvsImage.RenderSize.Height / bm.Height)), 
                        (int)cnvsImage.RenderSize.Height, reuse: _custom != null)
                ),
                BlurRadius = slideBlur.Value * maxBlur / 100,
                Zoom = 1f,
                Offset = new MyPointD() { X=0,Y=0}
            };

            debounceTimer.Tick += DebounceTimer_Tick;

            resetZoom();

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
                (int)(this.Width * myImage.Zoom) > 0 &&
                (int)(this.Height * myImage.Zoom) > 0
                )
            {
                System.Drawing.Bitmap bm =
                     _custom != null ? _custom : MagickImaging.BitmapFromUrlExt(_page);
                imgMain.DataContext = null;
                myImage.Image = Bitmap2BitmapImage(
                            GraphicsUtils.sameAspectResize(bm,
                            (int)( myImage.Zoom* 1.0f * bm.Width * (1.0f * cnvsImage.RenderSize.Height / bm.Height)),
                            (int)(cnvsImage.RenderSize.Height * myImage.Zoom), reuse: _custom != null)
                );
                imgMain.DataContext = myImage;

                resetZoom();
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
                myImage.Image = null;
            }
            if (_custom != null)
            {
                _custom.Dispose();
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
