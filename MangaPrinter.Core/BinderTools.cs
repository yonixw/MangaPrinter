using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MangaPrinter.Core
{

    public class BookletOptions
    {
        public bool isBookletRTL = true;
        public MangaPage bookletCoverFirst = null;
        public MangaPage bookletCoverLast = null;
    }

    public interface IBindBuilder
    {
        List<PrintPage> Build(List<MangaChapter> ch, bool startPage, bool endPage,
            int antiSpoiler = 0, BookletOptions bookletOption = null);
    }    


    public interface ITemplateBuilder
    {
        System.Drawing.Bitmap BuildFace(
            PrintFace[] faces, PrintSide[] sides, int spW, int spH, int padding, bool colors, bool parentText);
    }
   
}
