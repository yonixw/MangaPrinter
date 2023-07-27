using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public class CoreConfLoader
    {
        public static JsonConfig JsonConfigInstance = new JsonConfig();

        // To allow loading custom files
        public static CoreConfLoader I = new CoreConfLoader();


        private CoreConfLoader()
        {
            JsonConfig _config = new JsonConfig();

            LoadDefaultPlaces(_config, raiseEvent: true);

            JsonConfigInstance = _config;
        }

        public delegate void NotifyConf(JsonConfig newConfig);
        public static event NotifyConf onConfigFinishUpdate;

        public static void raiseChange(JsonConfig newConfig)
        {
            onConfigFinishUpdate?.Invoke(newConfig);
        }

        public void loadFromPath(string path, bool loadDefaultPlaces)
        {
            JsonConfig _config = new JsonConfig();
            if (loadDefaultPlaces)
            {
                LoadDefaultPlaces(_config, raiseEvent: false);
            }

            FileInfo _f = new FileInfo(Environment.ExpandEnvironmentVariables(path));
            if (_f.Exists)
            {
                string _txt = File.ReadAllText(_f.FullName);
                
                _config.UpdateJson(_f.FullName, _txt, raiseEvent: false);
                
                JsonConfigInstance = _config;
                
                raiseChange(_config);
            }
            // else keep the old config
        }

        public List<LoadFolderInfo> LoadFolders { get; set; } = new List<LoadFolderInfo>();

        public const string ConfigSuffix = "mpconfig.json";

        private void LoadDefaultPlaces(JsonConfig _config, bool raiseEvent = true)
        {
            string[] defaultPlaces = new[]
            {
                "%TEMP%",
                "%APPDATA%", // %APPDATA%   C:\\Users\\Username\\AppData\\Roaming
                ".",
                ".."
            };

            LoadFolders = new List<LoadFolderInfo>();

            for (int i = 0; i < defaultPlaces.Length; i++)
            {
                LoadFolders.Add(new LoadFolderInfo()
                {
                    FolderTemplate = defaultPlaces[i],
                    FolderRealPath = (new DirectoryInfo(Environment.ExpandEnvironmentVariables(defaultPlaces[i]))).FullName
                });
            }

            // Environment can point us to other locations...
            foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                if (env.Key.ToString().StartsWith("MANGAP_"))
                {
                    LoadFolders.Add(new LoadFolderInfo()
                    {
                        FolderTemplate = string.Format("{0}={1}", env.Key.ToString(), env.Value.ToString()),
                        FolderRealPath = Environment.ExpandEnvironmentVariables(env.Value.ToString())
                    });
                }
            }

            _config.ConfigMessages.Add("[INFO] Searching " + ConfigSuffix);

            foreach(LoadFolderInfo li in LoadFolders)
            {
                _config.ConfigMessages.Add(
                    String.Format("[INFO] Trying to load in: \"{0}\" ({1})",li.FolderTemplate, li.FolderRealPath));
                if (!Directory.Exists(li.FolderRealPath)) continue;


                // On default we load only "mpconfig.json" from all folders, for varations, load from the config manager
                FileInfo _f = new FileInfo(Path.Combine(li.FolderRealPath, ConfigSuffix));
                if (_f.Exists)
                {
                    try
                    {

                        string _txt = File.ReadAllText(_f.FullName);
                        _config.UpdateJson(_f.FullName, _txt, raiseEvent: false);
                    }
                    catch (Exception ex)
                    {
                        _config.ConfigMessages.Add(
                            String.Format("[ERROR] Loading file {0}, Err={1}", _f.FullName, ex.Message));
                    }
                }
            }

            if (raiseEvent)
            {
                JsonConfig.raiseConfigEvent(_config);
            }
        }
    }

    public class LoadFolderInfo
    {
        public string FolderTemplate { get; set; } = "";
        public string FolderRealPath { get; set; } = "";
    }
}
