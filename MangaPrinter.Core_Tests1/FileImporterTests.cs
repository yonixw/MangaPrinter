using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaPrinter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core.Tests
{
    [TestClass()]
    public class FileImporterTests
    {
        [TestMethod()]
        public void pad0AllNumbersTest()
        {
            Assert.AreEqual("ch. 0000105", FileImporter.pad0AllNumbers("ch. 105"));
            Assert.AreEqual("chpater-0000002", FileImporter.pad0AllNumbers("chpater-2"));
            Assert.AreEqual("0000002", FileImporter.pad0AllNumbers("2"));
            Assert.AreEqual("0000020", FileImporter.pad0AllNumbers("20"));
            Assert.AreEqual("0000205", FileImporter.pad0AllNumbers("205"));


            // Decimal values not counted as pad since it will make 8.5 as ordered as 805
            Assert.AreEqual("ch 0000106.6", FileImporter.pad0AllNumbers("ch 106.6"));
            Assert.AreEqual("0000002.1", FileImporter.pad0AllNumbers("2.1"));
            Assert.AreEqual("0000002.22", FileImporter.pad0AllNumbers("2.22"));
            Assert.AreEqual("0000020.233", FileImporter.pad0AllNumbers("20.233"));

            // Always have leading 0, even if len=padCount, just add padCount (7 today)
            Assert.AreEqual("chapter.0000002.45.66", FileImporter.pad0AllNumbers("chapter.2.45.66"));

            Assert.AreEqual("chapter.0000002.45", FileImporter.pad0AllNumbers("chapter.2.45"));
            Assert.AreEqual("chapter.0000002.45.66777", FileImporter.pad0AllNumbers("chapter.2.45.66777"));
            Assert.AreEqual("chapter.0000002.45.667.665.5656.69656.63696677", FileImporter.pad0AllNumbers("chapter.2.45.667.665.5656.69656.63696677"));

        }

        [TestMethod()]
        public void pad0AllNumbersForCompareOnlyTest()
        {
            Assert.AreEqual("00000105;", FileImporter.pad0AllNumbersForCompareOnly("ch. 105"));
            Assert.AreEqual("00000002;", FileImporter.pad0AllNumbersForCompareOnly("chpater-2"));
            Assert.AreEqual("00000002;", FileImporter.pad0AllNumbersForCompareOnly("2"));
            Assert.AreEqual("00000020;", FileImporter.pad0AllNumbersForCompareOnly("20"));
            Assert.AreEqual("00000205;", FileImporter.pad0AllNumbersForCompareOnly("205"));


            // Decimal values not counted as pad since it will make 8.5 as ordered as 805
            Assert.AreEqual("00000106.6;", FileImporter.pad0AllNumbersForCompareOnly("ch 106.6"));
            Assert.AreEqual("00000002.1;", FileImporter.pad0AllNumbersForCompareOnly("2.1"));
            Assert.AreEqual("00000002.22;", FileImporter.pad0AllNumbersForCompareOnly("2.22"));
            Assert.AreEqual("00000020.233;", FileImporter.pad0AllNumbersForCompareOnly("20.233"));

            // Always have leading 0, even if len=padCount, just add padCount (7 today)
            Assert.AreEqual("00000002.45.66;", FileImporter.pad0AllNumbersForCompareOnly("chapter.2.45.66"));

            Assert.AreEqual("00000002.45;", FileImporter.pad0AllNumbersForCompareOnly("chapter.2.45"));
            Assert.AreEqual("00000002.45.66777;", FileImporter.pad0AllNumbersForCompareOnly("chapter.2.45.66777"));
            Assert.AreEqual("00000002.45.667.665.5656.69656.63696677;", FileImporter.pad0AllNumbersForCompareOnly("chapter.2.45.667.665.5656.69656.63696677"));

            Assert.AreEqual("00000024;0000024;", FileImporter.pad0AllNumbersForCompareOnly("Chapter 24 Ch.024"));

            
        }
    }
}