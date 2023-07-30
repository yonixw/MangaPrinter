using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core.ChapterBuilders
{
    public class BookletBinder : IBindBuilder
    {
        public List<PrintPage> Build(List<MangaChapter> ch, bool startPage, bool endPage, int antiSpoiler = 0,
                bool isBookletRTL = true)
        {
            List<PrintPage> duplexBase = new DuplexBuilder().Build(ch, startPage, endPage, 0);

            List<PrintPage> result = new List<PrintPage>();
            
            PrintSide setSide(PrintFace f, bool right, PrintSide side)
            {
                if (right) f.Right = side;
                else f.Left = side;
                return side;
            }


            for (int i = 0; i < duplexBase.Count *2 ; i++) // going down
            {
                bool goingDown = i < duplexBase.Count;

                PrintPage tPage;
                if (goingDown)
                {
                    tPage = new PrintPage()
                    {
                        PageNumber = 0,
                        Front = new PrintFace()
                        {
                            FaceNumber = i / 2, 
                            IsRTL =isBookletRTL,
                            PrintFaceType=FaceType.SINGLES
                            
                        },
                        Back = new PrintFace()
                        {
                            FaceNumber = i / 2,
                            IsRTL = isBookletRTL,
                            PrintFaceType = FaceType.SINGLES
                        },
                    };
                }
                else {
                    tPage = result[duplexBase.Count * 2 - 1 - i]; // last i is first page again=0
                }

                // 2xFace -> 1top,1bottom
                PrintFace sourceFace = i%2==0? duplexBase[i/2].Front : duplexBase[i/2].Back;
                bool isSourceDouble = sourceFace.Left == sourceFace.Right;
                bool isSourceRTL = sourceFace.IsRTL;

                PrintSide topSide = setSide(goingDown?  tPage.Front : tPage.Back, 
                    isBookletRTL,  // opposite in going up
                    (isSourceRTL ? sourceFace.Right : sourceFace.Left));
                if (isSourceDouble && 
                    (topSide.SideType==SingleSideType.MANGA || topSide.SideType == SingleSideType.ANTI_SPOILER) )
                {
                    topSide.DoubleSourceType = isSourceRTL ? DoubleSoure.RIGHT : DoubleSoure.LEFT;
                }

                PrintSide bottomSide = setSide(goingDown ? tPage.Back : tPage.Front,
                    isBookletRTL,  // opposite in going up
                    (isSourceRTL ? sourceFace.Right : sourceFace.Left));
                if (isSourceDouble &&
                    (bottomSide.SideType == SingleSideType.MANGA || bottomSide.SideType == SingleSideType.ANTI_SPOILER))
                {
                    bottomSide.DoubleSourceType = isSourceRTL ? DoubleSoure.RIGHT : DoubleSoure.LEFT;
                }

                if (goingDown)
                    result.Add(tPage);
            }

            return result;
        }

    }
}
