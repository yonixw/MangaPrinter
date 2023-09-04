using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaPrinter.Core.ChapterBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core.ChapterBuilders_Tests
{
    [TestClass()]
    public class BookletBuilderTests
    {
         //  Copied from utils.cs in same project:
         //  ======================================================================

         // Input format:
         //   [R]TL or [L]TR
         //   R1,2,1,1,2,1,1,1,1  /  L2,2,1,1,1  /  ....

         // Output format(Per face Single - 3 items, Double 2 items)
         //       * MUST have faces for both sides (full pages)
         //       * Front side first, Left face first -> LTR (a->b) a,b while RTL (a->b) b,a
         //   R>S,M,B  /  R>D,M  /  R>S,M,M  /  R>S,M,E  /  ....


        [TestMethod()]
        public void xMakeEvenSimple()
        {
            var input = "L2,1,1,1  /  L1,1";
            var bookletRtl = new BookletOptions() { isBookletRTL = false};
            var outputDuplex = String.Join(" / ",new[] {
                "L>D,M",
                "L>S,M,M",
                "L>S,M,E",
                "L>S,M,M"
                });
            var outputBooklet = String.Join(" / ", new[] {
                "L>S,E,E",

                "L>S,M,M",
                "L>S,M,M",
                "L>S,M,E",
                "L>S,M,M",

                "L>S,E,E",
                });

            Utils.TestResultDuplex(input, outputDuplex, false, false, 0);
            Console.WriteLine("--");
            Utils.TestResultBooklet(bookletRtl,input, outputBooklet, false, false, 0);
        }


        [TestMethod()]
        public void xSimpleNoDoubleEven()
        {
            var input = "L1";
            var output = "L>S,M,E / R>S,E,E";
            var bookletRtl = new BookletOptions() { isBookletRTL = false };

            var outputBooklet = String.Join(" / ", new[] {
                "L>S,E,E",

                "L>S,M,E",
                "L>S,E,E",

                "L>S,E,E",
                });


            Utils.TestResultDuplex(input, output, false, false, 0);
            Console.WriteLine("--");
            Utils.TestResultBooklet(bookletRtl, input, outputBooklet, false, false, 0);
        }

        [TestMethod()]
        public void xSimpleNoDoubleEvenCover()
        {
            var input = "L1";
            var output = "L>S,M,E / R>S,E,E";
            var bookletRtl = new BookletOptions() {
                isBookletRTL = false,
                bookletCoverFirst = new MangaPage()
                
            };

            var outputBooklet = String.Join(" / ", new[] {
                "L>S,M,E",

                "L>S,M,E",
                "L>S,E,E",

                "L>S,E,E",
                });


            Utils.TestResultDuplex(input, output, false, false, 0);
            Console.WriteLine("--");
            Utils.TestResultBooklet(bookletRtl, input, outputBooklet, false, false, 0);
        }

        [TestMethod()]
        public void xDoubleCoverAndAS()
        {
            var input = "L1";
            var output = "L>D,A / L>S,M,E / L>D,A / L>D,A";
            var bookletRtl = new BookletOptions()
            {
                isBookletRTL = false,
                bookletCoverFirst = new MangaPage(),
                bookletCoverLast = new MangaPage()
            };

            var outputBooklet = String.Join(" / ", new[] {
                "L>S,M,M",

                "L>S,M,E",
                "L>S,E,E",

                "L>S,A,A",
                });


            Utils.TestResultDuplex(input, output, false, false, 100);
            Console.WriteLine("--");
            Utils.TestResultBooklet(bookletRtl, input, outputBooklet, false, false, 100);
        }

        [TestMethod()]
        public void xSimpleNoDoubleEvenCoverEvenNoAddAS()
        {
            var input = "L1/L1/L1/L1/L1";
            var output = "L>S,M,E / L>S,M,E / L>S,M,E / L>S,M,E/ L>S,M,E / R>S,E,E";
            var bookletRtl = new BookletOptions()
            {
                isBookletRTL = false,
                bookletCoverFirst = new MangaPage()

            };

            var outputBooklet = String.Join(" / ", new[] {
                "L>S,M,A",

                "L>S,M,E",
                "L>S,E,E",

                "L>S,A,A",
                "L>S,M,E",

                "L>S,A,A",
                "L>S,E,M",

                "L>S,M,E",
                "L>S,E,M",

                "L>S,A,A", // last not adding another one
                });


            Utils.TestResultDuplex(input, output, false, false, 0);
            Console.WriteLine("--");
            Utils.TestResultBooklet(bookletRtl, input, outputBooklet, false, false, 1);
        }


        [TestMethod()]
        public void xSimpleIntroOutroEvenAntiRTL()
        {
            var input = "R2,1,1,1,1  /  R1,1";
            var outputWithAS = "R>D,A / R>S,B,I / R>D,M / R>S,M,M / R>S,M,M /" +
                " R>S,E,O / R>D,A / R>S,M,I / R>S,O,M / R>D,A";
            var outputWithoutAS = "R>S,B,I / R>D,M / R>S,M,M / R>S,M,M / " +
                "R>S,E,O / R>S,M,I / R>S,O,M / R>S,E,E";

            var bookletRtl = new BookletOptions() { isBookletRTL = true };
            var outputBooklet = String.Join(" / ", new[] {
                "R>S,A,A",

                "R>S,E,I",
                "R>S,E,B",

                "R>S,O,M",
                "R>S,M,M",

                "R>S,M,M",
                "R>S,I,M",

                "R>S,A,A",

                "R>S,E,M",
                "R>S,O,M",

                "R>S,A,A",
                "R>S,A,A",
                });

            Utils.TestResultDuplex(input, outputWithAS, true, true, 3  /* => 6 faces*/ );
            Utils.TestResultDuplex(input, outputWithoutAS, true, true, 0 );
            Console.WriteLine("--");
            Utils.TestResultBooklet(bookletRtl, input, outputBooklet, true, true, 3  /* => 6 faces*/);
        }

      
    }
}