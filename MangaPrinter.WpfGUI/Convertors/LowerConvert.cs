using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MangaPrinter.WpfGUI.Convertors
{
    class LowerConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float inParam = int.MaxValue;
            if (value != null && parameter != null  && float.TryParse((string)parameter,out inParam))
                return System.Convert.ToSingle(value) < inParam;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
