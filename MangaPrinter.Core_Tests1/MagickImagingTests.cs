using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaPrinter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MangaPrinter.Core.TemplateBuilders;

namespace MangaPrinter.Core.Tests
{
    [TestClass()]
    public class MagickImagingTests
    {
        [TestMethod()]
        public void WhitePercentageTest()
        {
            using (Bitmap b1 = MagickImaging.BitmapFromUrlExt("./44.33931_per_white.png"))
            {
                Assert.AreEqual("0.4433931", MagickImaging.WhiteRatio(b1).ToString("F7"));
            }

            using (Bitmap b1 = MagickImaging.BitmapFromUrlExt("./44.33931_per_white.png"))
            {
                using (Bitmap b2 = GraphicsUtils.MakeGrayscale3(b1))
                {
                    Assert.AreEqual("0.6915526", MagickImaging.WhiteRatio(b2).ToString("F7"));
                }
            }

            using (Bitmap b1 = MagickImaging.BitmapFromUrlExt("./44.33931_per_white.png"))
            {
                using (Bitmap b2 = GraphicsUtils.MakeBW1(b1)) // Hard threshold!!
                {
                    Assert.AreEqual("1.0000000", MagickImaging.WhiteRatio(b2).ToString("F7"));
                }
            }

            using (Bitmap b1 = MagickImaging.BitmapFromUrlExt("./44.33931_per_white_gray.png"))
            {
                using (Bitmap b2 = GraphicsUtils.MakeBW1(b1)) // Hard threshold!!
                {
                    Assert.AreEqual("1.0000000", MagickImaging.WhiteRatio(b2).ToString("F7"));
                }
            }

            using (Bitmap b1 = MagickImaging.BitmapFromUrlExt("./44.33931_per_white_gray.png"))
            {
                using (Bitmap b2 = GraphicsUtils.MakeGrayscale3(b1))
                {
                    Assert.AreEqual("0.6971297", MagickImaging.WhiteRatio(b2).ToString("F7"));
                }
            }
        }
    }
}