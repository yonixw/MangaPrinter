using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaPrinter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MangaPrinter.Core.Tests
{
    public class Utils
    {
        public static MangaChapter MockChapter(bool isRTL, bool[] isDouble)
        {
            MangaChapter ch = new MangaChapter() { IsRTL = isRTL, Pages = new ObservableCollection<MangaPage>() };
            foreach (bool _b in isDouble)
            {
                ch.Pages.Add(new MangaPage() { IsDouble = _b });
            }
            return ch;
        }

        public const string NO_ERROR = "ok";
        public static bool isErorr(string error)
        {
            return error != NO_ERROR;
        }

        public static string VerifySide(MangaChapter ch, PrintSide ps, VerifySide vs)
        {
            if (vs.type != ps.SideType)
                return "Side type mismatch";
            if (vs.type == SingleSideType.MANGA && ps.MangaPageSource.ImagePath != ch.Pages[vs.MangaPageSourceIndex].ImagePath)
                return "Manga page mismatch";

            return NO_ERROR;
        }

        public static string VerifyFace(MangaChapter ch, PrintFace pf, VerifyFace vf)
        {
            string error = "";

            if (isErorr(error = VerifySide(ch, pf.Left, vf.Left)))
                return "[LEFT-SIDE] " + error;
            if (isErorr(error = VerifySide(ch, pf.Right, vf.Right)))
                return "[RIGHT-SIDE] " + error;

            return NO_ERROR;
        }

        public static string VerifyPage(MangaChapter ch, PrintPage pp, VerifyPage vp)
        {
            string error = "";

            if (isErorr(error = VerifyFace(ch, pp.Front, vp.Front)))
                return "[FRONT-FACE] " + error;
            if (isErorr(error = VerifyFace(ch, pp.Back, vp.Back)))
                return "[BACK-FACE] " + error;

            return NO_ERROR;
        }

        public static string VerifyChapter(MangaChapter ch, PrintChapter pch, VerifyPage[] verifyPages)
        {
            string error = "";

            if (pch.Pages.Count != verifyPages.Length)
                return "Pages count mismatch";

            for (int i = 0; i < verifyPages.Length; i++)
            {
                PrintPage pp = pch.Pages[i];
                VerifyPage vp = verifyPages[i];


                if (isErorr(error = VerifyPage(ch, pp, vp)))
                    return "[Page-" + i + "] " + error;
            }

            return NO_ERROR;
        }
    }

    public class VerifySide
    {
        public SingleSideType type;
        public int MangaPageSourceIndex; // if from manga.
    }

    public class VerifyFace
    {
        public VerifySide Left;
        public VerifySide Right;
    }

    public class VerifyPage
    {
        public VerifyFace Front;
        public VerifyFace Back;
    }


}
