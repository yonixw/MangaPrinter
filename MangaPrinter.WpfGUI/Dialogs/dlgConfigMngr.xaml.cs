using MangaPrinter.Conf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for dlgConfigMngr.xaml
    /// </summary>
    public partial class dlgConfigMngr : Window
    {
        public dlgConfigMngr()
        {
            InitializeComponent();
        }

        ObservableCollection<string> changesLog = new ObservableCollection<string>();
        ObservableCollection<KeyValuePair<string,JMeta>> configMetas = new ObservableCollection<KeyValuePair<string, JMeta>>();

        

        public void AddMany<T>(ObservableCollection<T> collection, IEnumerable<T> list) =>
                list.ToList().ForEach(collection.Add);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstLog.ItemsSource = changesLog;
            lstMetas.ItemsSource = configMetas;

            if (CoreConfLoader.JsonConfigInstance != null)
            {
                ConfigChanged(CoreConfLoader.JsonConfigInstance);
            }
            CoreConfLoader.onConfigFinishUpdate += ConfigChanged;
        }

        void ConfigChanged(JsonConfig config)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(() => ConfigChanged(config));
                return;
            }

            changesLog.Clear();
            AddMany(changesLog, CoreConfLoader.JsonConfigInstance.ConfigMessages);

            if (configMetas.Count == 0 && String.IsNullOrEmpty(txtSearch.Text))
            {
                configMetas.Clear();
                AddMany(configMetas, JsonConfig.GetConfigMetas().AsEnumerable());
            }
        }

        private void btnCpLogs_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CoreConfLoader.JsonConfigInstance.ConfigMessages.Aggregate((a, b) => a + Environment.NewLine + b));
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool emptySearch = String.IsNullOrEmpty(txtSearch.Text);
            configMetas.Clear();
            if (!emptySearch)
                AddMany(configMetas, JsonConfig.GetConfigMetas().AsEnumerable().Where((kv)=> kv.Key.IndexOf(txtSearch.Text) > -1));
            else
                AddMany(configMetas, JsonConfig.GetConfigMetas().AsEnumerable());
        }

        string lastSelectionFullName = null;
        private void lstMetas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMetas.Items.Count == 0) return;
            KeyValuePair<string, JMeta>? _kvj = (KeyValuePair<string, JMeta>)lstMetas.SelectedItem;
            if (_kvj != null && _kvj.HasValue)
            {
                txtSlctMeta.Text = _kvj.Value.Key;
                lastSelectionFullName = _kvj.Value.Key;

                rtbJValue.Document.Blocks.Clear();
                rtbJValue.Document.Blocks.Add(new Paragraph(new Run(
                    JsonEditHelpers.ToJsonValue(CoreConfLoader.JsonConfigInstance.Get<object>(lastSelectionFullName))
                )));

                lstItemLog.ItemsSource = null;
                lstItemLog.ItemsSource = CoreConfLoader.JsonConfigInstance.Log(_kvj.Value.Key);
            }
        }

        private void btnCurrent_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, JMeta>? _kvj = (KeyValuePair<string, JMeta>)lstMetas.SelectedItem;
            if (_kvj != null && _kvj.HasValue && !String.IsNullOrEmpty(lastSelectionFullName))
            {
                rtbJValue.Document.Blocks.Clear();
                rtbJValue.Document.Blocks.Add(new Paragraph(new Run(
                    JsonEditHelpers.ToJsonValue(CoreConfLoader.JsonConfigInstance.Get<object>(lastSelectionFullName))
                )));

                //rtbJValue.AppendText();
            }
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, JMeta>? _kvj = (KeyValuePair<string, JMeta>)lstMetas.SelectedItem;
            if (_kvj != null && _kvj.HasValue && !String.IsNullOrEmpty(lastSelectionFullName))
            {
                rtbJValue.Document.Blocks.Clear();
                rtbJValue.Document.Blocks.Add(new Paragraph(new Run(
                    JsonEditHelpers.ToJsonValue(_kvj.Value.Value.JSONDefault)
                )));
            }
        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, JMeta>? _kvj = (KeyValuePair<string, JMeta>)lstMetas.SelectedItem;
            if (_kvj != null && _kvj.HasValue && !String.IsNullOrEmpty(lastSelectionFullName))
            {
                TextRange textRange = new TextRange(
                    rtbJValue.Document.ContentStart,
                    rtbJValue.Document.ContentEnd
                );

                try
                {
                    if ( _kvj.Value.Value.Valid(JsonEditHelpers.FromJsonValue(textRange.Text))
                    )
                    {
                        MessageBox.Show("[OK] Valid.");
                    }
                    else
                    {
                        MessageBox.Show("[ERR] Not valid!");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("[ERR] Not valid!" + "\n---\n" + err.Message);
                }                
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, JMeta>? _kvj = (KeyValuePair<string, JMeta>)lstMetas.SelectedItem;
            if (_kvj != null && _kvj.HasValue && !String.IsNullOrEmpty(lastSelectionFullName))
            {
                TextRange textRange = new TextRange(
                    rtbJValue.Document.ContentStart,
                    rtbJValue.Document.ContentEnd
                );

                CoreConfLoader.JsonConfigInstance.Update(
                "Config Mngr - " + lastSelectionFullName,
                new Dictionary<string, object>() {
                    { lastSelectionFullName, JsonEditHelpers.FromJsonValue(textRange.Text) }
                },
                raiseEvent: true) ;

                lstItemLog.ItemsSource = null;
                lstItemLog.ItemsSource = CoreConfLoader.JsonConfigInstance.Log(_kvj.Value.Key);
            }
        }

      
    }
}
