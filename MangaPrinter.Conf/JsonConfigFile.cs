using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MangaPrinter.Conf
{
    public class CoreSettingsLoader
    {
        public static JsonConfig JsonConfigInstance = new JsonConfig();

        private CoreSettingsLoader()
        {
            // todo: load file, env or default
            JsonConfigInstance = new JsonConfig();
        }
    }

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

        public T Get<T>(object value = null)
        {
            if (value != null)
            {
                Tuple<Type, Type> myTypes = new Tuple<Type, Type>(value.GetType(), EntryType);

                if (value.GetType().Equals(EntryType))
                    return (T)value;
               else if (CastHelpers.Casters.ContainsKey(myTypes))
                    return (T)CastHelpers.Casters[myTypes](value);
            }
            return (T)JSONDefault;

        }

        public bool Valid(object value = null)
        {
            if (Verifier == null)
                return true;

            if (value != null)
            {
                Tuple<Type, Type> myTypes = new Tuple<Type, Type>(value.GetType(), EntryType);

                if (EntryType.Equals(value.GetType()))
                    return Verifier(value);
                else if (CastHelpers.Casters.ContainsKey(myTypes))
                    return Verifier(CastHelpers.Casters[myTypes](value));
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
                return CoreSettingsLoader.JsonConfigInstance.Get<T>(_my_fullname);
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

        public void Update(string sourceName, Dictionary<string, object> data)
        {
            List<string> _keys = data.Keys.ToList();
            for (int i = 0; i < _keys.Count; i++)
            {
                string fullname = _keys[i];
                if (configs_meta.ContainsKey(fullname))
                {
                    JMeta _meta = configs_meta[fullname];
                    bool isValid = _meta.Valid(data[fullname]);
                    if (isValid && !_meta.ReadOnly)
                    {
                        config_values[fullname] = data[fullname];
                    }
                    config_source[fullname].Add(String.Format("Source={0}, Valid={1}", sourceName, isValid));
                }
            }
        }

        public T Get<T>(string fullname)
        {
            if (configs_meta.ContainsKey(fullname))
            {
                return configs_meta[fullname].Get<T>(config_values[fullname]);
            }
            else 
                throw new Exception("Cannot find config named: " + fullname);
        }

        public void ResetToDefault(string fullname)
        {
            if (configs_meta.ContainsKey(fullname))
            {
                config_values[fullname] = configs_meta[fullname].JSONDefault;
            }
            else
                throw new Exception("Cannot find config named: " + fullname);
        }

        public string toJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(config_values, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // Auto is minimal added data
            });
        }

        public void UpdateJson(string sourceName, string json)
        {
            Update(sourceName,
                Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>() { new PreferInt32Converter()},
                TypeNameHandling = TypeNameHandling.Auto // Auto is minimal added data
            }));
        }

        public static string NameToJsonName(string name)
        {
            // A_BbCc_Dd -> a.bb_cc.dd
            name = name.Replace("_", ".");
            name = Regex.Replace(name, "([^A-Z\\.])([A-Z])", "$1_$2"); // camel case to underscore
            name = name.ToLower();
            return name;
        }

        public Dictionary<string, JMeta> GetConfigMetas()
        {
            Dictionary<string, JMeta> _config_metas = new Dictionary<string, JMeta>();

            FieldInfo[] fields = CoreConf.I.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType.BaseType.Equals(typeof(JMeta)))
                {
                    string fullname = NameToJsonName(fields[i].Name);
                    object value = fields[i].GetValue(CoreConf.I);

                    _config_metas.Add(fullname, (JMeta)value);

                    IFullname _ifn = (IFullname)value;
                    _ifn.setFullname(fullname);
                }
            }
            return _config_metas;
        }

    }

   
}
