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
        public void BuildTest()
        {
            string input, output;

            input = "R2,1,1,1 x R1,1";
            output = "D,M x S,M,M x S,M,E x S,M,M";

            Utils.TestResult(input, output, false, false, 0);
        }
    }
}