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
    /// Interaction logic for dlgEmptyInk.xaml
    /// </summary>
    public partial class dlgTOC : Window
    {
       public bool isQuick { get; set; }

        public dlgTOC()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rbQucik.IsChecked = true;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            isQuick = rbQucik.IsChecked ?? false;
            DialogResult = !rbSkip.IsChecked;
        }

    }
}
