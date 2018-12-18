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
        [TestMethod()]
        public void MakeEvenSimple()
        {
            var input = "L2,1,1,1 / L1,1";
            var output = "D,M / S,M,M / S,M,E / S,M,M";

            Utils.TestResult(input, output, false, false, 0);
        }

        [TestMethod()]
        public void DoubleDoubleSimple()
        {
            var input = "L1,2,1,2";
            var output = "S,M,B/D,M/S,M,B/D,M";

            Utils.TestResult(input, output, false, false, 0);
        }

        [TestMethod()]
        public void DoubleDoubleSimpleRTL()
        {
            var input = "R1,2,1,2";
            var output = "S,B,M/D,M/S,B,M/D,M";

            Utils.TestResult(input, output, false, false, 0);
        }

        [TestMethod()]
        public void SimpleIntroOutroRTL()
        {
            var input = "L2,1,1,1 / L1,1";
            var output = "s,I,b / D,M / S,M,M / S,M,O / S,I,M / S,M,O";

            Utils.TestResult(input, output, true, true, 0);
        }

        [TestMethod()]
        public void SimpleIntroOutroEvenRTL()
        {
            var input = "L2,1,1,1,1 / L1,1";
            var output = "s,I,b / D,M / S,M,M / S,M,M / S,O,E / S,I,M / S,M,O / D,E";

            Utils.TestResult(input, output, true, true, 0);
        }
    }
}