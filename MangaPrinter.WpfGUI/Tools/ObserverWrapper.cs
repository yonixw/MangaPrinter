using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.WpfGUI.Tools
{
    public class Observable<T> : DynamicObject, INotifyPropertyChanged
    {
        T data;

        Dictionary<string, PropertyInfo> myProperties = new Dictionary<string, PropertyInfo>();

        public Observable(T original)
        {
            data = original;
            inflateProperties();
        }

        void inflateProperties()
        {
            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (pi.MemberType != MemberTypes.Property)
                    continue;

                if (pi.GetMethod.IsPrivate && pi.SetMethod.IsPrivate)
                    continue;

                myProperties.Add(pi.Name, pi);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (myProperties.ContainsKey(binder.Name))
            {
                result = myProperties[binder.Name].GetValue(data);
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (myProperties.ContainsKey(binder.Name))
            {
                myProperties[binder.Name].SetValue(data, value);
                NotifyChange(binder.Name);
                return true;
            }
            return false;
        }

        public F Act<F>(Func<T,F> func )
        {
            return func((dynamic)this);
        }

        public void Act(Action<T> func)
        {
            func((dynamic)this);
        }

        public static explicit operator T(Observable<T> observable)
        {
            return (dynamic)observable;
        }

        #region Update

        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache =
            new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyChange<T>(Expression<Func<T>> propertySelector)
        {
            var myName = GetMemberName(propertySelector);

            if (!string.IsNullOrEmpty(myName))
                NotifyChange(myName);
        }

        protected virtual void NotifyChange(string propertyName)
        {
            if (_argsCache != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache.Add(propertyName, new PropertyChangedEventArgs(propertyName));

                NotifyChange(_argsCache[propertyName]);
            }
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression =
                    (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string GetMemberName(
                UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression =
                    (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand)
                        .Member.Name;
        }

        private void NotifyChange(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

    }

    public class ObservableFactory
    {
        public static ObservableCollection<dynamic> ToList<T>(IEnumerable<T> orginalList)
        {
            if (orginalList == null) return null;

            ObservableCollection<dynamic> newList = new ObservableCollection<dynamic>();
            foreach( T value in orginalList)
            {
                newList.Add((dynamic)new Observable<T>(value));
            }

            return newList;
        }
    }
}
