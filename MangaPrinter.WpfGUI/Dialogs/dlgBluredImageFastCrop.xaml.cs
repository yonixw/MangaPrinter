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
    /// <summary>
    /// Interaction logic for dlgBluredImageFastCrop.xaml
    /// </summary>
    public partial class dlgBluredImageFastCrop : Window
    {
        private string _title;
        private MangaPage _page;

        public dlgBluredImageFastCrop( MangaPage Page, string DialogTitle)
        {
            _title = DialogTitle;
            _page = Page;
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

            slideZoom.Value = (100.0f * finalHeight) / myImage.Image.Height;
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

            System.Drawing.Bitmap bm = MagickImaging.BitmapFromUrlExt(_page);
            imgMain.DataContext = myImage = new MyImageBind()
            {
                Image = dlgBluredImage.Bitmap2BitmapImage(
                        GraphicsUtils.sameAspectResize(
                        bm,
                        (int)(1.0f * bm.Width * (1.0f * cnvsImage.RenderSize.Height / bm.Height)),
                        (int)cnvsImage.RenderSize.Height)
                ),
                BlurRadius = slideBlur.Value * maxBlur / 100,
                Zoom = 1f,
                Offset = new MyPointD() { X = 0, Y = 0 }
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
                imgMain.DataContext = null;
                System.Drawing.Bitmap bm = MagickImaging.BitmapFromUrlExt(_page);
                myImage.Image = dlgBluredImage.Bitmap2BitmapImage(
                            GraphicsUtils.sameAspectResize(
                            bm,
                            //GraphicsUtils.bitmapCrop(myImage.OriginalImage,_pageEffects, reuse: true),
                            (int)(myImage.Zoom * 1.0f * bm.Width * (1.0f * cnvsImage.RenderSize.Height / bm.Height)),
                            (int)(cnvsImage.RenderSize.Height * myImage.Zoom), reuse: true)
                );
                imgMain.DataContext = myImage;

                resetZoom();
            }


        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DebounceRender();
        }

        private void DebounceRender()
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
        }

        private void slideCrop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(txtCropPrcnt != null)
                txtCropPrcnt.Text = Math.Round(slideCrop.Value,2).ToString() + "%";
        }

        private void btnTop_Click(object sender, RoutedEventArgs e)
        {
            _page.Effects.CropTop = (float)Math.Round(slideCrop.Value, 2);
            DebounceRender();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            _page.Effects.CropRight = (float)Math.Round(slideCrop.Value, 2);
            DebounceRender();
        }

        private void btnBottom_Click(object sender, RoutedEventArgs e)
        {
            _page.Effects.CropBottom = (float)Math.Round(slideCrop.Value, 2);
            DebounceRender();
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            _page.Effects.CropLeft = (float)Math.Round(slideCrop.Value, 2);
            DebounceRender();
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            _page.Effects.CropTop = 
            _page.Effects.CropRight = 
            _page.Effects.CropBottom =
            _page.Effects.CropLeft = (float)Math.Round(slideCrop.Value, 2);
            DebounceRender();
        }

        private void slideBright_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _page.Effects.Brightness = (float)slideBright.Value;
            DebounceRender();
        }

        private void slideContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _page.Effects.Contrast = (float)slideContrast.Value;
            DebounceRender();
        }

        private void slideGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _page.Effects.Gamma = (float)slideGamma.Value;
            DebounceRender();
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

