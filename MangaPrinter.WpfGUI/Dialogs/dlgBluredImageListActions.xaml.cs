using MangaPrinter.Core;
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

namespace MangaPrinter.WpfGUI.Dialogs
{

    /// <summary>
    /// Interaction logic for dlgBluredImageListAction.xaml
    /// </summary>
    public partial class dlgBluredImageListActions : Window
    {
        private string _imageUrl;
        private string _title;

        public dlgBluredImageListActions(string ImageUrl, string DialogTitle)
        {
            _imageUrl = ImageUrl;
            _title = DialogTitle;
            InitializeComponent();
        }

        MyImageBind myImage;
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
        
            slideZoom.Value = (100.0f *  finalHeight)/ myImage.Image.Height;
        }

        public static BitmapImage LoadBitmapWithoutLockingFile(string url)
        {
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/dee7cb68-aca3-402b-b159-2de933f933f1/disposing-a-wpf-image-or-bitmapimage-so-the-source-picture-file-can-be-modified?forum=wpf

            System.Windows.Media.Imaging.BitmapImage result = new System.Windows.Media.Imaging.BitmapImage();  // Create new BitmapImage  
            System.IO.Stream stream = new System.IO.MemoryStream();  // Create new MemoryStream  

            System.Drawing.Bitmap bitmap = MagickImaging.BitmapFromUrlExt(url);  // Create new Bitmap (System.Drawing.Bitmap) from the existing image file (albumArtSource set to its path name)  
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);  // Save the loaded Bitmap into the MemoryStream - Png format was the only one I tried that didn't cause an error (tried Jpg, Bmp, MemoryBmp)  
            bitmap.Dispose();  // Dispose bitmap so it releases the source image file 
            
            result.BeginInit();  // Begin the BitmapImage's initialisation  
            result.StreamSource = stream;  // Set the BitmapImage's StreamSource to the MemoryStream containing the image  
            result.EndInit();  // End the BitmapImage's initialisation  

            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = _title;

            BitmapImage bm = LoadBitmapWithoutLockingFile(_imageUrl);
            imgMain.DataContext = myImage = new MyImageBind() {
                Image = bm,
                BlurRadius = slideBlur.Value * maxBlur / 100,
                Zoom = 1f,
                Offset = new MyPointD() { X=0,Y=0}
            };

            resetZoom();
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
            myImage.Image = null;
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
