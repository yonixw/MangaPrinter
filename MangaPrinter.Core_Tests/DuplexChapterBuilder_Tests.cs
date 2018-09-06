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
    [TestClass()]
    public class DuplexChapterBuilder_Tests
    {
        
        [TestMethod()]
        public void DuplexTest1_SimpleDouble_EvenPages_RTL()
        {
            MangaChapter ch = Utils.MockChapter(true, new bool[]
            {
                false, false, true
            });

            PrintChapter pc = new ChapterBuilders.DuplexChapterBuilder().Build(ch);

            string error = Utils.VerifyChapter(ch, pc, new VerifyPage[] {
                new VerifyPage()
                {
                    Front = new VerifyFace()
                    {
                        Right = new VerifySide() {type = SingleSideType.MANGA, MangaPageSourceIndex = 0},
                         Left = new VerifySide() { type = SingleSideType.MANGA, MangaPageSourceIndex=1},   
                    },
                     Back = new VerifyFace()
                    {
                         Right = new VerifySide() {type = SingleSideType.MANGA, MangaPageSourceIndex = 2},
                         Left = new VerifySide() { type = SingleSideType.MANGA, MangaPageSourceIndex=2},   
                    }
                }
            });

            if (Utils.isErorr(error))
                Assert.Fail(error);
        }

        [TestMethod()]
        public void DuplexTest1_SimpleDouble_EvenPages_LTR()
        {
            MangaChapter ch = Utils.MockChapter(false, new bool[]
            {
                false, false, true
            });

            PrintChapter pc = new ChapterBuilders.DuplexChapterBuilder().Build(ch);

            string error = Utils.VerifyChapter(ch, pc, new VerifyPage[] {
                new VerifyPage()
                {
                    Front = new VerifyFace()
                    {
                        Right = new VerifySide() {type = SingleSideType.MANGA, MangaPageSourceIndex = 1},
                         Left = new VerifySide() { type = SingleSideType.MANGA, MangaPageSourceIndex=0},
                    },
                     Back = new VerifyFace()
                    {
                         Right = new VerifySide() {type = SingleSideType.MANGA, MangaPageSourceIndex = 2},
                         Left = new VerifySide() { type = SingleSideType.MANGA, MangaPageSourceIndex=2},
                    }
                }
            });

            if (Utils.isErorr(error))
                Assert.Fail(error);
        }
    } 
}