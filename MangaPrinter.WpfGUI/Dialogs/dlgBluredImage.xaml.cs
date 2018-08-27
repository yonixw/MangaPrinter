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
    /// Interaction logic for dlgBluredImage.xaml
    /// </summary>
    public partial class dlgBluredImage : Window
    {
        public dlgBluredImage()
        {
            InitializeComponent();
        }

        MyImageBind myImage;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage bm = new BitmapImage(new Uri
                (@"C:\Users\Yoni\source\repos\MangaPrinter\MangaPrinter.WpfGUI\Icons\1Page.png", UriKind.Absolute));
            
            imgMain.DataContext = myImage = new MyImageBind() { Image = bm, BlurRadius = 10 };
            Zoom(slideZoom.Value);
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
                myImage.BlurRadius = slideBlur.Value * 40 / 100;
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

        public void Zoom(double percent)
        {
            if (myImage != null)
            {
                double _z =  percent / 100;

                imgMain.Width = myImage.Image.Width * _z;
                imgMain.Height = myImage.Image.Height * _z;
            }
        }
    }

    public class MyImageBind : ModelBaseWpf
    {

        public BitmapImage Image { get { return _baseGet(); } set { _baseSet(value); } }
        public double BlurRadius { get { return _baseGet(); } set { _baseSet(value); } }
    }
}
