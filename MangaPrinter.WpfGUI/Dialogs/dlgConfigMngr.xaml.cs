using MangaPrinter.Conf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        ObservableCollection<LoadFolderInfo> loadFolders = new ObservableCollection<LoadFolderInfo>();
        ObservableCollection<string> loadFoldersFiles = new ObservableCollection<string>();



        public void AddMany<T>(ObservableCollection<T> collection, IEnumerable<T> list) =>
                list.ToList().ForEach(collection.Add);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstLog.ItemsSource = changesLog;
            lstMetas.ItemsSource = configMetas;
            lstFolders.ItemsSource = loadFolders;
            txtFileName.Text = CoreConfLoader.ConfigSuffix;
            lstFolderFiles.ItemsSource = loadFoldersFiles;

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
            AddMany(changesLog, config.ConfigMessages);

            if (configMetas.Count == 0 && String.IsNullOrEmpty(txtSearch.Text))
            {
                configMetas.Clear();
                AddMany(configMetas, JsonConfig.GetConfigMetas().AsEnumerable());
            }


            if (CoreConfLoader.I.LoadFolders.Count > 0)
            {
                loadFolders.Clear();
                AddMany(loadFolders, CoreConfLoader.I.LoadFolders);
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
                        MessageBox.Show(this,"[OK] Valid.");
                    }
                    else
                    {
                        MessageBox.Show(this,"[ERR] Not valid!");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(this,"[ERR] Not valid!" + "\n---\n" + err.Message);
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

        LoadFolderInfo lastSelected = null;

        private void lstFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFolders.SelectedItem == null) return;
            lastSelected = (LoadFolderInfo)lstFolders.SelectedItem;

            lblSelectedFolder.Content = "Files of: \"" + lastSelected.FolderTemplate + "\"";

            loadFoldersFiles.Clear();

            if (Directory.Exists(lastSelected.FolderRealPath))
            {
                FileInfo[] _fi = new DirectoryInfo(lastSelected.FolderRealPath)
                    .GetFiles("*"+ CoreConfLoader.ConfigSuffix);

                _fi.Select((FileInfo f) => f.Name)
                    .ToList().ForEach((f) => loadFoldersFiles.Add(f));
            }
            
        }

        private void btnExportFile_Click(object sender, RoutedEventArgs e)
        {
            string error = null;
            if (lastSelected == null)
            {
                MessageBox.Show(this,"Please select folder");
                return;
            }

            string fileName = txtFileName.Text;
            string fullSavePath = System.IO.Path.Combine(lastSelected.FolderRealPath, fileName);

            if (lastSelected == null)
            {
                error = "Please select a folder";
            }
            else if (!Directory.Exists(lastSelected.FolderRealPath))
            {
                error = "Folder does not exist, please create it first";
            }
            else if (String.IsNullOrEmpty(fileName))
            {
                error = "Filename cannot be empty";
            }
            else if (!Regex.IsMatch(fileName, "^[a-zA-Z0-9\\-_\\.]*" + CoreConfLoader.ConfigSuffix + "$"))
            {
                error = "Filename must end with " + CoreConfLoader.ConfigSuffix + "\nAnd not include any special character";
            }


            if (String.IsNullOrEmpty(error))
            {
                if (File.Exists(fullSavePath))
                {
                    MessageBoxResult overwriteDlg =
                        MessageBox.Show(this,"File already exists, overwrite?", "Overwrite", MessageBoxButton.YesNo);
                    if (overwriteDlg == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                try
                {
                    File.WriteAllText(fullSavePath, CoreConfLoader.JsonConfigInstance.toJSON());
                    MessageBox.Show(this,"Exported successfully to:\n" + fullSavePath);
                }
                catch (Exception err)
                {
                    error = "Error exporting config, error=\n" + err.Message;
                }
            }

            if (!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(this,error);
            }


        }

        private void btnLoadFileAlone_Click(object sender, RoutedEventArgs e)
        {
            LoadConfigFromSelected(false);
        }


        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            LoadConfigFromSelected(true);
        }

        private void LoadConfigFromSelected(bool loadDefaultFiles)
        {
            string fileShortName = (string)lstFolderFiles.SelectedItem;

            if (lastSelected != null && fileShortName != null)
            {
                string fullPath = System.IO.Path.Combine(lastSelected.FolderRealPath, fileShortName);
                if (File.Exists(fullPath))
                    CoreConfLoader.I.loadFromPath(fullPath, loadDefaultFiles);

            }
        }

    }
}
