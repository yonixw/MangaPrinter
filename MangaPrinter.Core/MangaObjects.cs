using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MangaPrinter.Core
{
   
    public class PageEffects : ModelBaseWpf
    {
        public bool IsOmited { get { return _baseGet(); } set { _baseSet(value); } } 
        public string VirtualPath { get { return _baseGet(); } set { _baseSet(value); } }

        public float CropTop { get { return _baseGet(); } set { _baseSet(value); } }
        public float CropRight { get { return _baseGet(); } set { _baseSet(value); } }
        public float CropBottom { get { return _baseGet(); } set { _baseSet(value); } }
        public float CropLeft { get { return _baseGet(); } set { _baseSet(value); } }

        public PageEffects()
        {
            IsOmited = false;
            VirtualPath = "";
            CropTop = 0;
            CropRight = 0;
            CropBottom = 0;
            CropLeft = 0;
        }
    }

    public class MangaPage : ModelBaseWpf
    {
        public const float MinRatio = 0.05f;
        public const float MaxRatio = 20f;

        public bool IsChecked { get { return _baseGet(failover: false); } set { _baseSet(value); } } 

        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public bool IsDouble { get { return _baseGet(); } set { _baseSet(value); } }
        public PageEffects Effects { get { return _baseGet(); } set { _baseSet(value); } }
        public float AspectRatio { get { return _baseGet(); } set { _baseSet(value); } }
        public float WhiteBlackRatio { get { return _baseGet(); } set { _baseSet(value); } }
        public string ImagePath { get { return _baseGet(); } set { _baseSet(value); } }

        public MangaChapter Chapter { get { return _baseGet(); } set { _baseSet(value); } }
        public int ChildIndexStart { get { return _baseGet(); } set { _baseSet(value); } }
        public int ChildIndexEnd { get { return _baseGet(); } set { _baseSet(value); } }
    }

    public class ActionMangaPage<R> : ModelBaseWpf
    {
        public MangaPage Page { get { return _baseGet(); } set { _baseSet(value); } }

        public R Result { get { return _baseGet(); } set { _baseSet(value); } }
    }


    public class MangaChapter : ModelBaseWpf
    {
        public bool IsChecked { get { return _baseGet(failover: false); } set { _baseSet(value); } }
        public string Name { get { return _baseGet(); } set { _baseSet(value); } }
        public string ParentName { get { return _baseGet(""); } set { _baseSet(value); } }
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
           updateChapterStats();
           
            // NotifyChange("MinWhiteRatio"); // Will not work since we update after inserting to collection
        }

        public void updateChapterStats()
        {
            updatePageNumber();
            NotifyChange("MinRatio");
            NotifyChange("MaxRatio");
            NotifyChange("MinWhiteRatio");
            NotifyChange("MaxWhiteRatio");
            NotifyChange("CalculatedPageCount");
            NotifyChange("Pages");
        }

        public int CalculatedPageCount
        {
            get
            {
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Last().ChildIndexEnd : 0;
            }
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
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Min(p => p.WhiteBlackRatio) : 0.5f;
            }
        }

        public float MaxWhiteRatio
        {
            get
            {
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Max(p => p.WhiteBlackRatio) : 0.5f;
            }
        }

        public float MaxRatio
        {
            get
            {
                return ((Pages?.Count() ?? 0) > 0) ? Pages.Max(p => p.AspectRatio) : MangaPage.MinRatio;
            }
        }

        private void updatePageNumber()
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

        int MyGetHashCode()
        {
            return string.Format("{0}|{1}|{2}", Name, ParentName, Pages.Count).GetHashCode();
        }

        public bool isEqual(MangaChapter other)
        {
            if (other == null) return false;
            return this.MyGetHashCode() == other.MyGetHashCode();
        }



        public static T Extend<T>(MangaChapter input) where T : MangaChapter, new()
        {
            T result = new T();
            result.autoPageNumbering = input.autoPageNumbering;
            result.IsRTL = input.IsRTL;
            result.Name = input.Name;
            result.ParentName = input.ParentName;
            result.Pages = input.Pages;

            return result;
        }
    }


    // Page (Physical) has 2 Faces that has 2 Sides.

    public enum SingleSideType
    {
        INTRO=0, OUTRO,
        MANGA,
        BEFORE_DOUBLE, MAKE_EVEN,
        ANTI_SPOILER,
        OMITED,

        LAST
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
        public static string lastFullExportMetadata = "(Filename)";

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
