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
    /// Interaction logic for dlgChooseCutoffRatio.xaml
    /// </summary>
    public partial class dlgChooseCutoffRatio : Window
    {
        public dlgChooseCutoffRatio()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable<double> _x = Enumerable.Range(0, 10).Select<int, double>((x) => x * 1.0);
            IEnumerable<double> _y = _x.Select((x) => x * x);

            gRatio.Plot(_x,_y);

            IEnumerable<double> _x2 = new double[] { 1, 1 };
            IEnumerable<double> _y2 = new double[] { 0, 100 };

            gCutoff.Plot(_x2,_y2);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
