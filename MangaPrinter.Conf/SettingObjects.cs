using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    class SettingOptionList<T>
    {
        public List<T> Options { get; set; } = new List<T>();

        public int Selected { get; set; } = -1;

        public SettingOptionList(List<T> list, int selected = -1)
        {
            Options = list;
            Selected = selected;
        }

        public T GetValue(T notfound = default(T))
        {
            if (Selected < 0 || Selected > Options.Count - 1)
            {
                return notfound;
            }
            return Options[Selected];
        }

        public bool isSelectedIndx(int _selected)
        {
            if (Selected < 0 || Selected > Options.Count - 1)
            {
                return false;
            }
            return Selected == _selected;
        }

        public bool isSelectedValue(T value)
        {
            if (Selected < 0 || Selected > Options.Count - 1)
            {
                return false;
            }
            return Options.IndexOf(value) == Selected;
        }
    }
}
