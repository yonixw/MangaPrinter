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

        public MangaChapter Chapter { get { return _baseGet(); } set { _baseSet(value); } }
        public int ChildIndexStart { get { return _baseGet(); } set { _baseSet(value); } }
        public int ChildIndexEnd { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public class MangaChapter : ModelBaseWpf
    {
        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public ObservableCollection<MangaPage> Pages { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsRTL { get { return _baseGet(); } set { _baseSet(value); } }

        public MangaChapter()
        {
            Pages = new ObservableCollection<MangaPage>();
            Pages.CollectionChanged += Pages_CollectionChanged;
        }

        public bool autoUpdateMeta = true; // set to false when adding many pages at once.
        private void Pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            updateMeta();
        }

        public void updateMeta()
        {
            if (autoUpdateMeta)
            {
                int index = 1;
                foreach (MangaPage page in Pages)
                {
                    page.Chapter = this;
                    page.ChildIndexStart =  page.ChildIndexEnd = index++;
                    if (page.IsDouble)
                        page.ChildIndexEnd = index++;
                }
            }
        }
    }


    // Page (Physical) has 2 Faces that has 2 Sides.

    public enum SingleSideType
    {
        INTRO, OUTRO,
        MANGA,
        BEFORE_DOUBLE, MAKE_EVEN,
        ANTI_SPOILER
    }

    public enum SideMangaPageType
    {
        LEFT, RIGHT // For cutting original image file.
    }

    public enum FaceType
    {
        SINGLES, DOUBLE
    }

    public class PrintSide
    {
        SingleSideType SideType;
        int SideNumber;

        // If Manga page:
        MangaPage MangaPageSource;
        SideMangaPageType MangaPageSourceType;
    }

    public class PrintFace
    {
        int FaceNumber;
        FaceType PrintFaceType;

        PrintSide Left;
        PrintSide Right;
        PrintSide Double; // If needed, ignores PrintSide type.

        public void GetImage()
        {

        }
    }

    public class PrintPage
    {
        PrintFace Front;
        PrintFace Back;
    }

    public enum BindType
    {
        BOOKLET, DUPLEX
    }

    public class PrintChapter : ModelBaseWpf
    {
        BindType ChapterType;
        ObservableCollection<PrintPage> Pages { get { return _baseGet(); } set { _baseSet(value); } }
        MangaChapter SourceChapter;
    }


}
