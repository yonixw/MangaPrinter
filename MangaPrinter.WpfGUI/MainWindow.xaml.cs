using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MangaPrinter.WpfGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtPageMaxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (!int.TryParse(txtPageMaxWidth.Text, out value))
            {
                MessageBox.Show("Can't convert " + txtPageMaxWidth.Text + " to integer, try again.");
                txtPageMaxWidth.Text = 900.ToString();
            }
        }

        public void myFunc(int go)
        {

        }
    }
}
