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
        List<PrintFace> Build(MangaChapter ch);
    }

    public abstract class ChapterBuilderBase
    {
        MangaChapter baseChapter;
        public ChapterBuilderBase(MangaChapter ch) { baseChapter = ch; }
    }

    public class DuplexChapterBuilder : ChapterBuilderBase, IChapterBuilder
    {
        public DuplexChapterBuilder(MangaChapter ch) : base(ch) { }

        public void addCustomPage(SingleSideType pageType)
        {

        }

        public void addDoubleMangaPage(SingleSideType fillPageType)
        {

        }

        public void addMangaPage(SideMangaPageType mangaSide)
        {

        }

        public void Build()
        {

        }
    }
}
