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
    public class DuplexBuilderTests
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
        public void MakeEvenSimple()
        {
            var input = "L2,1,1,1  /  L1,1";
            var output = "L>D,M / L>S,M,M / L>S,M,E / L>S,M,M";

            Utils.TestResultDuplex(input, output, false, false, 0);
        }

        [TestMethod()]
        public void DoubleDoubleSimple()
        {
            var input = "L1,2,1,2";
            var output = "L>S,M,B / L>D,M / L>S,M,B / L>D,M";

            Utils.TestResultDuplex(input, output, false, false, 0);
        }

        [TestMethod()]
        public void DoubleDoubleSimpleRTL()
        {
            var input = "R1,2,1,2";
            var output = "R>S,B,M / R>D,M / R>S,B,M / R>D,M";

            Utils.TestResultDuplex(input, output, false, false, 0);
        }

        [TestMethod()]
        public void SimpleIntroOutro()
        {
            var input = "L2,1,1,1  /  L1,1";
            var output = "L>S,I,B / L>D,M / L>S,M,M / L>S,M,O / L>S,I,M / L>S,M,O";

            Utils.TestResultDuplex(input, output, true, true, 0);
        }

        [TestMethod()]
        public void SimpleIntroOutroEven()
        {
            var input = "L2,1,1,1,1  /  L1,1";
            var output = "L>S,I,B / L>D,M / L>S,M,M / L>S,M,M / L>S,O,E / L>S,I,M / L>S,M,O / R>S,E,E";

            Utils.TestResultDuplex(input, output, true, true, 0);
        }

        [TestMethod()]
        public void SimpleIntroOutroEvenRTL()
        {
            var input = "R2,1,1,1,1  /  R1,1";
            var output = "R>S,B,I / R>D,M / R>S,M,M / R>S,M,M / R>S,E,O / R>S,M,I / R>S,O,M / R>S,E,E";

            Utils.TestResultDuplex(input, output, true, true, 0);
        }

        [TestMethod()]
        public void SimpleIntroEvenRTL()
        {
            var input = "R2,1,1,1,1  /  R1,1";
            var output = "R>S,B,I / R>D,M / R>S,M,M / R>S,M,M / R>S,M,I / R>S,E,M";

            Utils.TestResultDuplex(input, output, true, false, 0);
        }

        [TestMethod()]
        public void SimpleDouble_EvenPages_RTL()
        {
            var input = "R1,1,2";
            var output = "R>S,M,M / R>D,M";

            Utils.TestResultDuplex(input, output, false,false, 0);
        }


        [TestMethod()]
        public void SimpleNoDoubleEven()
        {
            var input = "L1";
            var output = "L>S,M,E / R>S,E,E";


            Utils.TestResultDuplex(input, output, false, false, 0);
        }


        [TestMethod()]
        public void SimpleIntroOutroEvenAntiRTL()
        {
            var input = "R2,1,1,1,1  /  R1,1";
            var output = "R>D,A / R>S,B,I / R>D,M / R>S,M,M / R>S,M,M / R>S,E,O / R>D,A / R>S,M,I / R>S,O,M / R>D,A";
           

            Utils.TestResultDuplex(input, output, true, true, 3  /* => 6 faces*/ );
        }

        [TestMethod()]
        public void SimpleIntroOutroEvenAntiRTL2()
        {
            var input = "R2,1,1,1,1  /  R1,1,1";
            var output = "R>D,A / R>S,B,I / R>D,M / R>S,M,M / R>S,M,M / R>S,E,O / R>D,A / R>S,M,I / R>S,M,M / R>S,E,O / R>D,A / R>S,E,E";


            Utils.TestResultDuplex(input, output, true, true, 3  /* => 6 faces*/ );
        }


        [TestMethod()]
        public void AntiSpoilerUsingCorrectRTL()
        {
            var input = "R2,1,2,1,1 / L1,2,1,2 / R2,1,1,2";
            var output = 
                "R>D,A / R>S,B,I / R>D,M / R>S,B,M / R>D,M / R>S,M,M / R>D,A / R>S,E,O / L>S,I,M / L>D,M / L>S,M,B / L>D,M / L>D,A / L>S,O,E / R>S,B,I / R>D,M / R>S,M,M / R>D,M / R>D,A / R>S,E,O / R>D,A / R>S,E,E";

            // Notes:
            // * First anti-spolier takes from face 2
            // * Middle anti-spoiler takes from 1 page before (so it still left!)
            // * Last anti spoiler takes again, from 1 before so need to match last episode RTL.

            Utils.TestResultDuplex(input, output, true, true, 3  /* => 6 faces*/ );
        }


        [TestMethod()]
        public void AntiSpoilerUsingCorrectRTL2()
        {
            var input = "L1,2 / R1,1";
            var output =
                "L>D,A / L>S,I,M / L>D,M / L>S,O,E / L>D,A / R>S,M,I / R>S,O,M / R>D,A";

            // Notes:
            // * First anti-spolier takes from face 2
            // * Middle anti-spoiler takes from 1 page before (so it still left!)
            // * Last anti spoiler takes again, from 1 before so need to match last episode RTL.


            Utils.TestResultDuplex(input, output, true, true, 2  /* => 4 faces*/ );
        }
    }
}