using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MangaPrinter.Core
{
    public class BinderTools
    {
        public PrintChapter BindChapter(MangaChapter input, BindType bindType)
        {
            PrintChapter result = new PrintChapter() { ChapterType = bindType, SourceChapter = input, Pages = new ObservableCollection<PrintPage>() };

            return result;
        }
    }

    public interface IChapterBuilder
    {
        PrintChapter Build(MangaChapter ch);
    }

    public class BookletChapterBuilder : IChapterBuilder
    {
        public PrintChapter Build(MangaChapter ch)
        {
            // looping from both end\start sides .... so middle can have some blanks
            // No way to save doubles... become 2 signals

            return null;
        }
    }

    

   
}
