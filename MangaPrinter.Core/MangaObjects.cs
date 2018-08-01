﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaPrinter.Core
{
    public class MangaPage : ModelBase
    {
        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsDouble { get { return _baseGet(); } set { _baseSet(value); } }
        public string ImagePath { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public class MangaChapter : ModelBase
    {
         public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public IList<MangaPage> Pages { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsRTL { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public enum PageType
    {
        INTRO, OUTRO,
        SINGLE, DOUBLE,
        BEFORE_DOUBLE, MAKE_EVEN
    }

    public class MangaDoublPagePrint : ModelBase
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

    public class MangaChapterPrint : ModelBase
    {
        public IList<MangaDoublPagePrint> PrintPages { get { return _baseGet(); } set { _baseSet(value); } }
        public BindType PrintBind { get { return _baseGet(); } set { _baseSet(value); } }
        public string Description { get { return _baseGet(); } set { _baseSet(value); } }
    }

}
