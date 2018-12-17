using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.WpfGUI.ExtendedClasses
{
    class SelectableMangaChapter : MangaPrinter.Core.MangaChapter
    {
        public bool Selected { get { return _baseGet(); } set { _baseSet(value); } }

        
    }
}
