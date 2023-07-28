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

                makeTextualGraph(0);
            }
        }

        const int textualGraphRows = 17;
        void makeTextualGraph(int selectedIndex)
        {
            string result = "";

            int maxCount = InputBuckets.Select(b => b.count).Max();
            int minCount = InputBuckets.Select(b => b.count).Min();
            float countStep = 1.0f * (maxCount - minCount) / textualGraphRows;

            for (int row = 0; row < textualGraphRows; row++)
            {
                for (int bucket = 0; bucket < InputBuckets.Count; bucket++)
                {
                    result += InputBuckets[bucket].count > (textualGraphRows - row -1) * (countStep) ?
                                    "#" : "- ";
                    
                }
                result += "\n";
            }

            int countBefore = 0;
            int countAfter = 0;
            for (int bucket = 0; bucket < InputBuckets.Count; bucket++)
            {
                result += (selectedIndex == bucket) ? "^" : "- ";

                if (bucket < selectedIndex)
                {
                    countBefore += InputBuckets[bucket].count;
                }
                if (bucket > selectedIndex)
                {
                    countAfter += InputBuckets[bucket].count;
                }
            }

            x20x100.Text = result;
            txtValue.Text = Math.Round(InputBuckets[selectedIndex].value,2).ToString();
            txtValueCount.Text = InputBuckets[selectedIndex].count.ToString();
            txtCountAftr.Text = countAfter.ToString();
            txtCountBfr.Text = countBefore.ToString();

            

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (InputBuckets != null && InputBuckets.Count > 0)
            {
                BucketIndex = (int)sldCutoff.Value;

                IEnumerable<double> _x2 = new double[] { InputBuckets[BucketIndex].value, InputBuckets[BucketIndex].value };
                IEnumerable<double> _y2 = new double[] { 0, maxCount };

                gCutoff.Plot(_x2, _y2);

                makeTextualGraph(BucketIndex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(DialogResult ?? false)) // Make sure not left null
                DialogResult = false;
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
