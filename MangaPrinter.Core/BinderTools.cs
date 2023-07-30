using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MangaPrinter.Core
{

    

    public interface IBindBuilder
    {
        List<PrintPage> Build(List<MangaChapter> ch, bool startPage, bool endPage,
            int antiSpoiler = 0, bool isBookletRTL = true);
    }    

   
}
