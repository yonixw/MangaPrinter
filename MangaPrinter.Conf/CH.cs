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
    }

    public class CastHelpers
    {
        public static Dictionary<Tuple<Type,Type>, Func<object, object>> Casters = 
            new Dictionary<Tuple<Type, Type>, Func<object, object>>()
        {                
            { new Tuple<Type,Type>(typeof(int),typeof(float)), (O)=>Convert.ToSingle(O) }
        };

        
    }


}
