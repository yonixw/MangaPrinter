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
            var bookletRtl = true;
            var outputDuplex = String.Join(" / ",new[] {
                "L>D,M",
                "L>S,M,M",
                "L>S,M,E",
                "L>S,M,M"
                });
            var outputBooklet = String.Join(" / ", new[] {
                "L>S,M,M",
                "L>S,M,M",
                "L>S,M,M",
                "L>S,E,M"
                });

            Utils.TestResultDuplex(input, outputDuplex, false, false, 0);
            Utils.TestResultBooklet(bookletRtl,input, outputBooklet, false, false, 0);
        }


        [TestMethod()]
        public void xSimpleNoDoubleEven()
        {
            var input = "L1";
            var output = "L>S,M,E / R>S,E,E";


            Utils.TestResultBooklet(false,input, output, false, false, 0);
        }


        [TestMethod()]
        public void xSimpleIntroOutroEvenAntiRTL()
        {
            var input = "R2,1,1,1,1  /  R1,1";
            var output = "R>D,A / R>S,B,I / R>D,M / R>S,M,M / R>S,M,M / R>S,E,O / R>D,A / R>S,M,I / R>S,O,M / R>D,A";
           

            Utils.TestResultBooklet(false,input, output, true, true, 3  /* => 6 faces*/ );
        }

      
    }
}