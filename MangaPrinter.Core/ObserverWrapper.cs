using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core
{

    public class ModelBaseShared 
    {
        protected Dictionary<string, object> myData = new Dictionary<string, object>();

        protected dynamic _baseGet([CallerMemberName] string propName = "")
        {
            if (!myData.ContainsKey(propName))
                return null;
            return myData[propName];
        }

        protected virtual void _baseSet<T>(T value, [CallerMemberName] string propName = "")
        {
            myData[propName] = value;
        }

        // public long Yoni { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public class ModelBasedWinform : ModelBaseShared
    {

    }


    public class ModelBaseWpf : ModelBaseShared, INotifyPropertyChanged
    {
        protected override void _baseSet<T>(T value, [CallerMemberName] string propName = "")
        {
            myData[propName] = value;
            NotifyChange(propName);
        }


        #region Update

        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache =
            new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyChange(string propertyName)
        {
            if (_argsCache != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache.Add(propertyName, new PropertyChangedEventArgs(propertyName));

                NotifyChange(_argsCache[propertyName]);
            }
        }
        private void NotifyChange(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion
    }
}
