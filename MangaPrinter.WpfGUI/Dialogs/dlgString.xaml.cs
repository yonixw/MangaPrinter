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
    /// Interaction logic for dlgString.xaml
    /// </summary>
    public partial class dlgString : Window
    {
        public string Caption { get; set; }
        public string StringData { get; set; }

        public dlgString()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblDescription.Content = Caption;
            txtData.Text = StringData;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            StringData = txtData.Text;
            DialogResult = true;
        }

    }
}
