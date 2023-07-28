using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{

    class CH // Config Helpers
    {
        public static bool L<T>(object _O, Func<T,bool> _F)
        {
            // Cast helper
            return _F((T)_O);
        }


        public static Func<string, bool> stringy = (string s) => !String.IsNullOrEmpty(s);
    }

    public class CHList<T>
    {
        List<T> _options = null;

        public CHList(params T[] options)
        {
            _options = options.ToList();
        }

        public bool inListP(T item)
        {
            return _options.FindIndex(e=>e.Equals(item)) > -1;
        }

        public List<T> GetOptions()
        {
            return _options;
        }

    }

    public class JsonEditHelpers
    {
        public static object FromJsonValue(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<object>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All // Auto is minimal added data
            });
        }

        public static string ToJsonValue(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }
    
   


}
