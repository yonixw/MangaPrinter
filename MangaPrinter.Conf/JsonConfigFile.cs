using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MangaPrinter.Conf
{
    public class CoreSettings
    {
        public static JsonConfig Instance = null;

        private CoreSettings()
        {
            // todo: load file, env or default
            Instance = new JsonConfig();
        }
    }

    public class JMeta
    {
        public string Description { get; set; } = "(no info)";
        public Type EntryType { get; set; } // Runtime gatekeeper
        public object JSONDefault { get; set; } = null;
        public bool Deprecated { get; set; } = false;
        public Func<object, bool> Verifier { get; set; } = null;


        private void Init(object _jsonDefault, string _desc, Func<object, bool> _verifier, bool _deprecated)
        {
            JSONDefault = _jsonDefault;
            EntryType = _jsonDefault.GetType();

            Description = _desc;
            Deprecated = _deprecated;
            if (_verifier != null)
                Verifier = _verifier;
        }

        public JMeta(object _jsonDefault, string _desc, 
            Func<object, bool> _verifier = null, bool _deprecated = false)
        {
            Init(_jsonDefault, _desc, _verifier, _deprecated);
        }

        public JMeta(object _jsonDefault, string[] _desc,
            Func<object, bool> _verifier = null, bool _deprecated = false)
        {
            // For easy multiline desc
            Init(_jsonDefault, String.Join( "\n", _desc), _verifier, _deprecated);
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


    public class JsonConfig
    {
        [JsonIgnore]
        public Dictionary<string, JMeta> configs_meta = null;

        [JsonIgnore]
        public Dictionary<string, List<string>> config_source = new Dictionary<string, List<string>>();

        public Dictionary<string, object> config_values = new Dictionary<string, object>();


        public JsonConfig()
        {
            configs_meta = new JsonAllConfigs().configs_meta;
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
                    if (isValid)
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
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // Auto is minimal added data
            });
        }


        public string getSideTextConsts(int x) { return "TODO!!!" + x; }

        public string setProgramVersion(string x) { return "TODO!!!2" + x; }
    }

   
}
