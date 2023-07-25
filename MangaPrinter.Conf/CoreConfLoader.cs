﻿using System;
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
                _config.UpdateJson(_f.FullName, _txt, raiseEvent: true);

                JsonConfigInstance = _config;
            }
            // else keep the old config
        }

        private static void LoadDefaultPlaces(JsonConfig _config, bool raiseEvent = true)
        {
            string[] defaultPlaces = new[]
            {
                "%TEMP%\\config.json",
                "%APPDATA%\\config.json", // %APPDATA%   C:\\Users\\Username\\AppData\\Roaming
                "config.json"
            };

            for (int i = 0; i < defaultPlaces.Length; i++)
            {
                FileInfo _f = new FileInfo(Environment.ExpandEnvironmentVariables(defaultPlaces[i]));
                if (_f.Exists)
                {
                    string _txt = File.ReadAllText(_f.FullName);
                    _config.UpdateJson(_f.FullName, _txt, raiseEvent: false);
                }
            }

            // Environment can point us to other locations...
            foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                if (env.Key.ToString().StartsWith("MANGAP_"))
                {
                    FileInfo _f = new FileInfo(Environment.ExpandEnvironmentVariables(env.Value.ToString()));
                    if (_f.Exists)
                    {
                        string _txt = File.ReadAllText(_f.FullName);
                        _config.UpdateJson("env " + env.Key.ToString() + " points to " + _f.FullName, _txt, raiseEvent: false);
                    }
                }
            }

            if (raiseEvent)
            {
                JsonConfig.raiseConfigEvent(_config);
            }
        }
    }
}
