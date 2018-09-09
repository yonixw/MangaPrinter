using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using MangaPrinter.Core;

namespace MangaPrinter.WpfGUI
{
    public class PrintSideLabelConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PrintSide obj = value as PrintSide;
            if (obj != null)
            {
                TextBlock result = new TextBlock();

                result.Inlines.Add(new Bold(new Run(string.Format("{0}",obj.SideNumber))));

                Brush color = Brushes.Black;
                switch(obj.SideType)
                {
                    case SingleSideType.MANGA:
                        color = Brushes.Orange;
                        break;
                    case SingleSideType.INTRO:
                    case SingleSideType.OUTRO:
                        color = Brushes.Red;
                        break;
                    case SingleSideType.MAKE_EVEN:
                    case SingleSideType.BEFORE_DOUBLE:
                        color = Brushes.Blue;
                        break;
                    case SingleSideType.ANTI_SPOILER:
                        color = Brushes.Purple;
                        break;
                }
                result.Inlines.Add(new Run(string.Format(" {0}", obj.SideType)) { Foreground = color });

                if (obj.SideType == SingleSideType.MANGA)
                    if (obj.MangaPageSource.IsDouble)
                        result.Inlines.Add(new Italic(new Run(
                                string.Format(" (p{0}-{1})", obj.MangaPageSource.ChildIndexStart, obj.MangaPageSource.ChildIndexEnd)
                            )));
                    else
                        result.Inlines.Add(new Italic(new Run(
                                string.Format(" (p{0})", obj.MangaPageSource.ChildIndexStart)
                            )));

                return result;
            }
            else
            {
                return new TextBlock() { Text = "Error converting null" };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
