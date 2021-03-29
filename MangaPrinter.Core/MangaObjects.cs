using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MangaPrinter.Core
{
    public class MangaPage : ModelBaseWpf
    {
        public const float MinRatio = 0.05f;
        public const float MaxRatio = 20f;

        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsDouble { get { return _baseGet(); } set { _baseSet(value); } }
        public float AspectRatio { get { return _baseGet(); } set { _baseSet(value); } }
        public float WhiteBlackRatio { get { return _baseGet(); } set { _baseSet(value); } }
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


        public bool autoPageNumbering = true; // set to false when adding many pages at once.
        private void Pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            updatePageNumber();
            NotifyChange("MinRatio"); 
            // NotifyChange("MinWhiteRatio"); // Will not work since we update after inserting to collection
        }

        public void PublicNotifyChange(string name)
        {
            NotifyChange(name);
        }

        public float MinRatio
        {
            get
            {
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Min(p=>p.AspectRatio) : MangaPage.MaxRatio;
            }
        }

        public float MinWhiteRatio
        {
            get
            {
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Min(p => p.WhiteBlackRatio) : 1.00f;
            }
        }

        public float MaxRatio
        {
            get
            {
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Max(p => p.AspectRatio) : MangaPage.MinRatio;
            }
        }

        public void updatePageNumber()
        {
            if (autoPageNumbering)
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

        public static T Extend<T>(MangaChapter input) where T : MangaChapter, new()
        {
            T result = new T();
            result.autoPageNumbering = input.autoPageNumbering;
            result.IsRTL = input.IsRTL;
            result.Name = input.Name;
            result.Pages = input.Pages;

            return result;
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
        ALL, // only in booklet we need to know right\left
        LEFT, RIGHT // For cutting original image file.
    }

    public enum FaceType
    {
        SINGLES, /*Only in duplex: */DOUBLE
    }

    public class PrintSide : ModelBaseWpf
    {
        public SingleSideType SideType { get { return _baseGet(); } set { _baseSet(value); } }
        public int SideNumber { get { return _baseGet(); } set { _baseSet(value); } }

        // If Manga page:
        public MangaPage MangaPageSource { get { return _baseGet(); } set { _baseSet(value); } }
        public SideMangaPageType MangaPageSourceType { get { return _baseGet(); } set { _baseSet(value); } }

        public override string ToString()
        {
            string result = string.Format("({0}) {1}",SideNumber,SideType);
            if (SideType == SingleSideType.MANGA)
                if (MangaPageSource.IsDouble)
                    result += string.Format(" (p{0}-{1})", MangaPageSource.ChildIndexStart, MangaPageSource.ChildIndexEnd);
                else
                    result += string.Format(" (p{0})", MangaPageSource.ChildIndexStart);
            return result;
        }
    }

    public class PrintFace : ModelBaseWpf
    {
        public int FaceNumber { get { return _baseGet(); } set { _baseSet(value); } }
        public int BatchPaperNumber { get { return _baseGet(); } set { _baseSet(value); } } // For anti-spoiler batches
        public FaceType PrintFaceType { get { return _baseGet(); } set { _baseSet(value); } }

        public bool IsRTL { get { return _baseGet(); } set { _baseSet(value); } } // for template + side index
        public PrintSide Left { get { return _baseGet(); } set { _baseSet(value); } }
        public PrintSide Right { get { return _baseGet(); } set { _baseSet(value); } }
       
        // You get the page to print from this class
    }

    public class PrintPage : ModelBaseWpf
    {
        public int PageNumber { get { return _baseGet(); } set { _baseSet(value); } }
        public PrintFace Front { get { return _baseGet(); } set { _baseSet(value); } }
        public PrintFace Back { get { return _baseGet(); } set { _baseSet(value); } }

        public static T Extend<T>(PrintPage input) where T:PrintPage, new()
        {
            T result = new T();
            result.PageNumber = input.PageNumber;
            result.Front = input.Front;
            result.Back = input.Back;
            return result;
        }
    }

   
    


}
