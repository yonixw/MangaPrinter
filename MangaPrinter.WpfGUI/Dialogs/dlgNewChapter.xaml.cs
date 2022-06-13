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
    public partial class dlgNewChapter : Window
    {
        public string Name { get; set; }
        public string Folder { get; set; }

        public dlgNewChapter()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Closes dialog: DialogResult = false;
            txtName.Text = Name;
            txtFolder.Text = Folder;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Name = txtName.Text;
            Folder = txtFolder.Text;
            DialogResult = true;
        }

    }
}
