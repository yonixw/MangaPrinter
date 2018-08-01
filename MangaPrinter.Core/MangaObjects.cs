using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MangaPrinter.Core
{
    public class MangaPage : ModelBaseWpf
    {
        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsDouble { get { return _baseGet(); } set { _baseSet(value); } }
        public string ImagePath { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public class MangaChapter : ModelBaseWpf
    {
        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public ObservableCollection<MangaPage> Pages { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsRTL { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public enum PageType
    {
        INTRO, OUTRO,
        SINGLE, DOUBLE,
        BEFORE_DOUBLE, MAKE_EVEN
    }

    public class MangaDoublPagePrint : ModelBaseWpf
    {
        public PageType  RSourceType { get { return _baseGet(); } set { _baseSet(value); } }
        public MangaPage RSourcePage { get { return _baseGet(); } set { _baseSet(value); } }
        public string RPageText { get { return _baseGet(); } set { _baseSet(value); } }

        public PageType  LSourceType { get { return _baseGet(); } set { _baseSet(value); } }
        public MangaPage LSourcePage { get { return _baseGet(); } set { _baseSet(value); } }
        public string LPageText { get { return _baseGet(); } set { _baseSet(value); } }
        
        public string Description { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public enum BindType
    {
        BOOKLET, DUPLEX
    }

    public class MangaChapterPrint : ModelBaseWpf
    {
        public ObservableCollection<MangaDoublPagePrint> PrintPages { get { return _baseGet(); } set { _baseSet(value); } }
        public BindType PrintBind { get { return _baseGet(); } set { _baseSet(value); } }
        public string Description { get { return _baseGet(); } set { _baseSet(value); } }
    }

}
