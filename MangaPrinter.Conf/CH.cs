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

        public static bool inListP<T>(T item, params T[] arr )
        {
            return Array.IndexOf(arr, item) > -1;
        }

        public static bool inList<T>(T item, T[] arr)
        {
            return Array.IndexOf(arr, item) > -1;
        }

        public static Func<string, bool> stringy = (string s) => !String.IsNullOrEmpty(s);
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
