using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaPrinter.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaPrinter.Core.ChapterBuilders_Tests
{
    class Utils
    {
        //Input format:
        //  [R]TL or [L]TR
        //  R1,2,1,1,2,1,1,1,1 / L2,2,1,1,1 / ....

        static Dictionary<string, MangaPrinter.Core.FaceType> FaceConsts = new Dictionary<string, Core.FaceType>()
        {
            { "S", Core.FaceType.SINGLES},
            { "D", Core.FaceType.DOUBLE},

        };
        static Dictionary<string, MangaPrinter.Core.SingleSideType> SideConsts = new Dictionary<string, Core.SingleSideType>()
        {
            { "I", Core.SingleSideType.INTRO},
            { "O", Core.SingleSideType.OUTRO},
            { "M", Core.SingleSideType.MANGA},
            { "B", Core.SingleSideType.BEFORE_DOUBLE},
            { "E", Core.SingleSideType.MAKE_EVEN},
            { "A", Core.SingleSideType.ANTI_SPOILER},

        };
        //Output format(Per face Single - 3 items, Double 2 items)
        //      * MUST have faces for both sides (full pages)
        //      * Front side first, Left face first -> LTR (a->b) a,b while RTL (a->b) b,a
        //  S,M,B / D,M / S,M,M / S,M,E / ....

        public static void TestResult(string inputMangaChapters, string outputPrintFaces,
            bool startPage, bool endPage, int antiSpoiler=0)
        {
            // ------------ MOCK INPUT --------------

            List<MangaChapter> allChapters = new List<MangaChapter>();
            string [] iCs = inputMangaChapters.Replace(" ", "").Split('/');
            foreach(string iC in iCs)
            {
                MangaChapter mc = new MangaChapter();
                mc.Pages = new System.Collections.ObjectModel.ObservableCollection<MangaPage>();
                mc.IsRTL = (iC[0] == 'R');

                string[] pages = iC.Substring(1).Split(',');
                foreach (string stringpage in pages)
                {
                    MangaPage mp = new MangaPage() { IsOmmited = false};
                    mp.IsDouble = (stringpage == "1") ? false : true;
                    mc.Pages.Add(mp);
                }

                allChapters.Add(mc);
            }

            // ------------ REAL OUTPUT --------------

            List<PrintPage> resultPages = 
                (new Core.ChapterBuilders.DuplexBuilder()).Build(allChapters, startPage, endPage, antiSpoiler);

            List<PrintFace>
                resultFaces = resultPages.SelectMany<PrintPage, PrintFace>((p) => new[] { p.Front, p.Back }).ToList();

            List<string> quickLookArr = new List<string>();
            foreach (PrintFace face in resultFaces) {
                string isRTL = (face.IsRTL ? "R" : "L" ) + ">";
                if (face.PrintFaceType == FaceType.SINGLES)
                {
                    quickLookArr.Add(isRTL + "S," + reverseSide(face.Left.SideType) + "," + reverseSide(face.Right.SideType));
                }
                else
                {
                    quickLookArr.Add(isRTL + "D," + reverseSide(face.Left.SideType) );
                }
            }

            // ------------ COMPARE --------------

            string resultOutputString = string.Join("/", quickLookArr).ToUpper();
            string testOutputString = outputPrintFaces.Replace(" ", "").ToUpper();
            Console.WriteLine(resultOutputString);

            Assert.AreEqual(testOutputString, resultOutputString);
        }

        static string reverseSide(SingleSideType type)
        {
            return SideConsts.FirstOrDefault(x => x.Value == type).Key;
        }
    }
}
