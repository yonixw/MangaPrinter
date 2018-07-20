using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaPrinter.Core
{
    public class MangaPage
    {
        public string Name { get; set; }
        public bool IsDouble { get; set; }
        public string ImagePath { get; set; }
    }

    public class MangaChapter
    {
        public string Name { get; set; }
        public List<MangaPage> Pages { get; set; }
        public bool isRTL { get; set; }
    }

    public class MangaDoublPagePrint
    {
        public enum PageType
        {
            INTRO, OUTRO,
            SINGLE, DOUBLE,
            BEFORE_DOUBLE, MAKE_EVEN
        }

        public PageType  RSourceType { get; set; }
        public MangaPage RSourcePage { get; set; }
        public string RPageText;

        public PageType  LSourceType { get; set; }
        public MangaPage LSourcePage { get; set; }
        public string LPageText;

        public string Description { get; set; }
    }

    public class MangaChapterPrint
    {
        public enum BindType
        {
            BOOKLET, DUPLEX
        }

        public List<MangaDoublPagePrint> PrintPages { get; set; }
        public BindType PrintBind { get; set; }
        public string Description { get; set; }
    }

}
