using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MangaPrinter.WpfGUI
{

    public class Property<T>
    {
        private T data;

        INotifyPropertyChanged myParent;
        string myName;

        public Property(Property<T> p)
        {

        }

        public Property(INotifyPropertyChanged Parent, string Name) {
            myName = Name;
            myParent = Parent;
        }

        public static implicit operator T(Property<T> p)
        {
            return p.data;
        }

        public void Update(T newValue) { data = newValue; }
    }
   
    public class PropertyManagers : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string PropName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(PropName));
            }
        }
 
    }

    public class C1 : PropertyManagers
    {
        Property<int> myInt = new Property<int>(this, "myInt");
    }

    
}
