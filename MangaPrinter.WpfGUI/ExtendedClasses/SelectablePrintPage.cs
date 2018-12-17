using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.WpfGUI.ExtendedClasses
{
    class SelectablePrintPage : MangaPrinter.Core.PrintPage
    {
        public bool Selected { get { return _baseGet(); } set { _baseSet(value); } }
    }
}
