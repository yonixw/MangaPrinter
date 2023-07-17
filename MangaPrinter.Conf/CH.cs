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

    public class CastHelpers
    {
        public static Dictionary<Tuple<Type,Type>, Func<object, object>> Casters = 
            new Dictionary<Tuple<Type, Type>, Func<object, object>>()
        {                
            { new Tuple<Type,Type>(typeof(Int32),typeof(float)), (O)=>Convert.ToSingle(O) },

        };
    }

    public class PreferInt32Converter : JsonConverter 
    {
        // https://stackoverflow.com/a/44010307/1997873
        //https://stackoverflow.com/a/9444519/1997873

        public override bool CanConvert(Type objectType)
        {
            // may want to be less concrete here
            return objectType == typeof(Dictionary<string, object>);
        }

        public override bool CanWrite
        {
            // we only want to read (de-serialize)
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // again, very concrete
            Dictionary<string, object> result = new Dictionary<string, object>();
            reader.Read();

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value as string;
                reader.Read();

                object value;
                if (reader.TokenType == JsonToken.Integer)
                    value = Convert.ToInt32(reader.Value);      // convert to Int32 instead of Int64
                else
                    value = serializer.Deserialize(reader);     // let the serializer handle all other cases
                result.Add(propertyName, value);
                reader.Read();
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // since CanWrite returns false, we don't need to implement this
            throw new NotImplementedException();
        }
    }


}
