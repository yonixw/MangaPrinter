using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MangaPrinter.Conf
{
   

    public class JMeta
    {
        public string Description { get; set; } = "(no info)";
        public Type EntryType { get; set; } // Runtime gatekeeper
        public object JSONDefault { get; set; } = null;
        public bool Deprecated { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public Func<object, bool> Verifier { get; set; } = null;


        internal void Init(object _jsonDefault, string _desc,
            Func<object, bool> _verifier, bool _deprecated, bool _readonly)
        {
            JSONDefault = _jsonDefault;
            EntryType = _jsonDefault.GetType();
            ReadOnly = _readonly;

            Description = _desc;
            Deprecated = _deprecated;
            if (_verifier != null)
                Verifier = _verifier;
        }

        internal JMeta() { }

        public JMeta(object _jsonDefault, string _desc, 
            Func<object, bool> _verifier = null, bool _deprecated = false, bool _readonly = false)
        {
            Init(_jsonDefault, _desc, _verifier, _deprecated, _readonly);
        }

        public JMeta(object _jsonDefault, string[] _desc,
            Func<object, bool> _verifier = null, bool _deprecated = false, bool _readonly = false)
        {
            // For easy multiline desc
            Init(_jsonDefault, String.Join( "\n", _desc), _verifier, _deprecated, _readonly);
        }

        public T GetAsType<T>(object value)
        {
            if (value != null)
            {
                if (value.GetType().Equals(EntryType))
                    return (T)value;

                // For int and floats... which is js numbers
                object convertTry = JsonConfig.myTypeConverters(JsonConfig.myConverters(value), EntryType);
                if (convertTry.GetType().Equals(EntryType))
                    return (T)convertTry;


            }
            return (T)JSONDefault;

        }

        public bool Valid(object value = null)
        {
            if (Verifier == null)
                return true;

            if (value != null)
            {
                value = JsonConfig.myConverters(value);

                if (EntryType.Equals(value.GetType()))
                    return Verifier(value);
            }
            
            return Verifier(JSONDefault);
        }
    }

    public interface IFullname
    {
        void setFullname(string fullname);
    }

    public class JMetaT<T> : JMeta, IFullname
    {
        public JMetaT<T> Listify(CHList<T> list)
        {
            if (list == null || list.GetOptions() == null || list.GetOptions().Count == 0)
                throw new Exception("Cannot listify!");

            Func<object, bool> _wrapper = (O) => CH.L<T>(O, (T) => list.inListP(T));
            this.Verifier = _wrapper;

            this.Description += "\nAllowed Values: " + String.Join(", ", list.GetOptions().Select(s => s.ToString()));

            this.JSONDefault = list.GetOptions()[0];

            return this;
        }

        public JMetaT(T _jsonDefault, string[] _desc,
            Func<T, bool> _verifier = null, bool _deprecated = false, bool _readonly = false)
        {
            Func<object, bool> _wrapper = (O) => CH.L<T>(O, (T) => _verifier(T));

            base.Init(
                _jsonDefault,
                String.Join("\n", _desc),
                _verifier == null ? null : _wrapper,
                _deprecated, _readonly);
        }

        public JMetaT(T _jsonDefault, string _desc,
            Func<T, bool> _verifier = null, bool _deprecated = false, bool _readonly = false)
        {
            Func<object, bool> _wrapper = (O) => CH.L<T>(O, (T) => _verifier(T));
            base.Init(_jsonDefault, _desc, _verifier == null ? null : _wrapper, _deprecated, _readonly);
        }

        private string _my_fullname = ""; // To be filled from the outside

        public void setFullname(string _fullname)
        {
            _my_fullname = _fullname;
        }

        public T Get(JsonConfig config = null)
        {
            if (_my_fullname == "")
            {
                return default(T);
            }

            if (config != null)
            {
                return config.Get<T>(_my_fullname);
            }
            else
            {
                return CoreConfLoader.JsonConfigInstance.Get<T>(_my_fullname);
            }
        }

        public static implicit operator T(JMetaT<T> t) => t.Get();
    }


    public class JsonConfig
    {
        [JsonIgnore]
        public Dictionary<string, JMeta> configs_meta = null;

        [JsonIgnore]
        public Dictionary<string, List<string>> config_source = new Dictionary<string, List<string>>();

        public Dictionary<string, object> config_values = new Dictionary<string, object>();

        [JsonIgnore]
        public List<string> ConfigMessages = new List<string>() { "[INFO] Config Init with defaults at " + DateTime.Now }; // Info, warn and errors

        public JsonConfig()
        {
            configs_meta = GetConfigMetas();
            Init();
        }

        void Init()
        {
            List<string> _keys = configs_meta.Keys.ToList();
            for (int i = 0; i < _keys.Count; i++)
            {
                string fullname = _keys[i];
                JMeta _meta = configs_meta[fullname];
                config_values.Add(fullname, _meta.JSONDefault);
                config_source.Add(fullname, new List<string>() { "(Default Value)" });
            }
        }

        public static object myConverters (object o)
        {
            if (o == null) return null;

            Type t = o.GetType();
            if (t == typeof(Int64))
                return Convert.ToInt32(o);
            else if (t == typeof(Double))
                return Convert.ToSingle(o);
            else
                return o;
        }

        public static object myTypeConverters(object o, Type target)
        {
            if (o == null) return null;

            Type t = o.GetType();
            if (t == typeof(Int32) && target == typeof(Single))
                return Convert.ToSingle(o);
            else if (t == typeof(Single) && target == typeof(Int32))
                return Convert.ToInt32(Math.Floor((Single)o));
            else
                return o;
        }

        public void Update(string sourceName, Dictionary<string, object> data, bool raiseEvent = true)
        {
            ConfigMessages.Add("[INFO] Load config from " + sourceName);

            List<string> _keys = data.Keys.ToList();
            for (int i = 0; i < _keys.Count; i++)
            {
                string fullname = _keys[i];
                if (configs_meta.ContainsKey(fullname))
                {
                    JMeta _meta = configs_meta[fullname];
                    object value = data[fullname];

                    bool isValid = _meta.Valid(value);
                    if (isValid && !_meta.ReadOnly)
                    {
                        config_values[fullname] = value;
                    }
                    config_source[fullname].Add(String.Format("Source={0}, Valid={1}", sourceName, isValid));
                }
            }

            if (
                (new CoreConf()).Info_ConfigVersionMajor.Get(this) < CoreConf.CURR_CONFIG_MAJOR_VERSION
               )
            {
                ConfigMessages.Add("[WARN] Config version has major update, from " + sourceName);
            }

            if (raiseEvent)
                raiseConfigEvent(this);
        }

        public static void raiseConfigEvent(JsonConfig newConfig)
        {
            CoreConfLoader.raiseChange(newConfig);
        }

        public T Get<T>(string fullname)
        {
            if (configs_meta.ContainsKey(fullname))
            {
                return configs_meta[fullname].GetAsType<T>(config_values[fullname]);
            }
            else 
                throw new Exception("Cannot find config named: " + fullname);
        }

        public List<string> Log(string fullname)
        {
            if (config_source.ContainsKey(fullname))
            {
                return config_source[fullname];
            }
            else
                throw new Exception("Cannot find config named: " + fullname);
        }

        public void ResetToDefault(string fullname, bool raiseEvent)
        {
            if (configs_meta.ContainsKey(fullname))
            {
                ConfigMessages.Add("[INFO] Reset to default: " + fullname);

                config_values[fullname] = configs_meta[fullname].JSONDefault;

                if (raiseEvent) raiseConfigEvent(this);
            }
        }

        public string toJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(config_values, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All // Auto is minimal added data
            });
        }

        public void UpdateJson(string sourceName, string json, bool raiseEvent = true)
        {
            // todo: enforce in each type that MangaPrinter.Conf exists.. to avoid type injections

            Update(sourceName,
                Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All // Auto is minimal added data
            }),
                raiseEvent
                );
        }

        public static string NameToJsonName(string name)
        {
            // A_BbCc_Dd -> a.bb_cc.dd
            name = name.Replace("_", ".");
            name = Regex.Replace(name, "([^A-Z\\.])([A-Z])", "$1_$2"); // camel case to underscore
            name = name.ToLower();
            return name;
        }

        public static Dictionary<string, JMeta> GetConfigMetas()
        {
            Dictionary<string, JMeta> _config_metas = new Dictionary<string, JMeta>();

            PropertyInfo[] fields = CoreConf.I.GetType().GetProperties();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].PropertyType.BaseType.Equals(typeof(JMeta)))
                {
                    string fullname = NameToJsonName(fields[i].Name);
                    object value = myConverters( fields[i].GetValue(CoreConf.I) );

                    _config_metas.Add(fullname, (JMeta)value);

                    IFullname _ifn = (IFullname)value;
                    _ifn.setFullname(fullname);
                }
            }
            return _config_metas;
        }

    }

    

   
}
