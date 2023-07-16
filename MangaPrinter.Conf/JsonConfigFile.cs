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
            if (EntryType.Equals(typeof(T)))
            {
                if (value != null && value.GetType().Equals(EntryType))
                {
                    return (T)value;
                }
                else
                {
                    return (T)JSONDefault;
                }
            }
            return default(T);
        }

        public bool Valid<T>(object value = null)
        {
            if (Verifier == null)
                return true;

            if (EntryType.Equals(typeof(T)))
            {
                if (value != null && value.GetType().Equals(EntryType))
                {
                    return Verifier((T)value);
                }
                else
                {
                    return Verifier((T)JSONDefault);
                }
            }
            
            return false;
        }
    }


    public class JsonConfig
    {
        [JsonIgnore]
        public Dictionary<string, JMeta> configs_meta = new Dictionary<string, JMeta>()
        {
            { "propa.prob.x.z", new JMeta(123, "Lonnnngg descriptions!") }
        };

        

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
