using MangaPrinter.Core;
using MangaPrinter.WpfGUI.Utils;
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
    /// Interaction logic for dlgChooseCutoffRatio.xaml
    /// </summary>
    public partial class dlgChooseCutoffRatio : Window
    {
        public dlgChooseCutoffRatio()
        {
            InitializeComponent();
        }

        public List<BucketInfo> InputBuckets;
        public int BucketIndex { get; set; }

        private int maxCount = 1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (InputBuckets != null && InputBuckets.Count > 0)
            {
                maxCount = InputBuckets.Max((b) => b.count);

                gRatio.Plot(
                    InputBuckets.Select((b)=>b.value),
                    InputBuckets.Select((b)=>b.count)
                    );
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (InputBuckets != null && InputBuckets.Count > 0)
            {
                BucketIndex = (int)sldCutoff.Value;
                IEnumerable<double> _x2 = new double[] { InputBuckets[BucketIndex].value, InputBuckets[BucketIndex].value };
                IEnumerable<double> _y2 = new double[] { 0, maxCount };

                gCutoff.Plot(_x2, _y2);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        
    }
}
