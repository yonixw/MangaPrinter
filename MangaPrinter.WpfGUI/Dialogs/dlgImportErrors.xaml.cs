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
    /// Interaction logic for dlgImportErrors.xaml
    /// </summary>
    public partial class dlgImportErrors : Window
    {
        public List<FileImporterError> DataErrors;

        public dlgImportErrors()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstErrorTitle.ItemsSource = DataErrors ?? new List<FileImporterError>();
        }

        private void LstErrorTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileImporterError error = lstErrorTitle.SelectedItem as FileImporterError;
            if (error != null)
            {
                string detatil = "File:\n " + ((error.fileObj?.FullName) ?? "<No file>") + "\n";
                detatil += "Reason:\n " + ((error.reasonString!="") ? error.reasonString : "<No reason>") + "\n";
                detatil += "Exception:\n " + (error.exObject?.ToString() ?? "<No Exception>") + "\n";

                TextRange textRange = new TextRange(rtbErrorDetails.Document.ContentStart, rtbErrorDetails.Document.ContentEnd);
                textRange.Text =  detatil;
            }
        }
    }
}
