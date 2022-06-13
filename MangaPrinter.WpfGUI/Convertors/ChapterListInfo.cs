using MangaPrinter.WpfGUI.ExtendedClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MangaPrinter.WpfGUI.Convertors
{
    class ChapterListInfo : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            const string strFormat = "({0}/{1}) Selected // {2} ➗TooVertical // {3} 🔳EmptyInk% ";
            if (value == null) return
                string.Format(strFormat, 0, 0, 0, 0);

                BindingList <SelectableMangaChapter> observableCollection 
                    = value as BindingList<SelectableMangaChapter>;
                return string.Format(strFormat,
                    observableCollection.Where(ch => ch.IsChecked).Count(),
                    observableCollection.Count,
                    observableCollection.Where(ch => ch.MinRatio < 0.33f).Count(),
                    observableCollection.Where(ch => ch.MinWhiteRatio < 0.10f).Count()
                    );
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }
        }
    }

