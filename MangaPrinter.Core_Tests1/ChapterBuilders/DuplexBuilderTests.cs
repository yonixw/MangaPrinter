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
    }
}