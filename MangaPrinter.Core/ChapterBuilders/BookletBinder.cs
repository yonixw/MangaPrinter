using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core.ChapterBuilders
{
    class BookletBinder : IBindBuilder
    {
        public List<PrintPage> Build(List<MangaChapter> ch, bool startPage, bool endPage, int antiSpoiler = 0)
        {
            List<PrintPage> result = new List<PrintPage>();

            List<PrintPage> duplexBase = new DuplexBuilder().Build(ch, startPage, endPage, 0);



            return result;
        }
    }
}
