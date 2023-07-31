using System.Collections.Generic;

namespace MangaPrinter.Core.ChapterBuilders
{
    public class BookletBinder : IBindBuilder
    {
        public List<PrintPage> Build(List<MangaChapter> ch, bool startPage, bool endPage, int antiSpoiler = 0,
               BookletOptions bookletOptions = null)
        {
            BookletOptions _O = bookletOptions ?? new BookletOptions();

            List<PrintPage> duplexBase = new DuplexBuilder().Build(ch, startPage, endPage, 0);

            List<PrintFace> faceResults = new List<PrintFace>();

            PrintSide setSide(PrintFace f, bool right, PrintSide side)
            {
                if (right) f.Right = side;
                else f.Left = side;
                return side;
            }

            for (int i = 0; i < duplexBase.Count * 2; i++) // going down
            {
                bool goingDown = i < duplexBase.Count;

                PrintFace Front, Back;

                if (goingDown)
                {
                    Front = new PrintFace()
                    {
                        FaceNumber = i / 2,
                        IsRTL = _O.isBookletRTL,
                        PrintFaceType = FaceType.SINGLES

                    };
                    Back = new PrintFace()
                    {
                        FaceNumber = i / 2,
                        IsRTL = _O.isBookletRTL,
                        PrintFaceType = FaceType.SINGLES
                    };
                }
                else
                {
                    // last i is first page again=0
                    Front = faceResults[((2*duplexBase.Count-i)-1)*2 ];
                    Back = faceResults[((2 * duplexBase.Count - i) - 1) * 2 +1];
                }

                // 2xFace -> 1top,1bottom
                PrintFace sourceFace = i % 2 == 0 ? duplexBase[i / 2].Front : duplexBase[i / 2].Back;
                bool isSourceDouble = sourceFace.Left == sourceFace.Right;
                bool isSourceRTL = sourceFace.IsRTL;

                if (isSourceDouble && sourceFace.Right.SideType==SingleSideType.MANGA)
                {
                    // Seperate objects because double are the same 
                    sourceFace.Left = new PrintSide(sourceFace.Left);
                    sourceFace.Right = new PrintSide(sourceFace.Right);
                }

                PrintSide topSide = setSide(goingDown ? Front : Back,
                    goingDown ? _O.isBookletRTL : !_O.isBookletRTL,  // opposite in going up
                    (isSourceRTL ? sourceFace.Right : sourceFace.Left));
                if (isSourceDouble &&
                    (topSide.SideType == SingleSideType.MANGA || 
                    topSide.SideType == SingleSideType.ANTI_SPOILER ||
                    topSide.SideType == SingleSideType.OMITED))
                {
                    topSide.DoubleSourceType = isSourceRTL ? DoubleSoure.RIGHT : DoubleSoure.LEFT;
                }

                PrintSide bottomSide = setSide(goingDown ? Back: Front,
                     goingDown ? _O.isBookletRTL : !_O.isBookletRTL,  // opposite in going up
                    (!isSourceRTL ? sourceFace.Right : sourceFace.Left));
                if (isSourceDouble &&
                   (bottomSide.SideType == SingleSideType.MANGA ||
                   bottomSide.SideType == SingleSideType.ANTI_SPOILER ||
                   bottomSide.SideType == SingleSideType.OMITED))
                {
                    bottomSide.DoubleSourceType = !isSourceRTL ? DoubleSoure.RIGHT : DoubleSoure.LEFT;
                }

                if (goingDown)
                {
                    faceResults.Add(Front);
                    faceResults.Add(Back);
                }
            }

            int antiSpoilerAdded = 0;
            if (antiSpoiler > 0)
            {
                int batchCounter = 0;
                int arrayLength = faceResults.Count; // make it constant
                for (int i = 0; i < arrayLength; i++)
                {
                    faceResults[i].BatchPaperNumber = batchCounter;
                    if (i % 2 == (antiSpoilerAdded % 2)) {
                        if (batchCounter < antiSpoiler)
                            batchCounter++;
                        else
                        {
                            antiSpoilerAdded++;
                            batchCounter = 0;
                            faceResults.Insert(i, new PrintFace()
                            {
                                IsRTL = _O.isBookletRTL,
                                Left = new PrintSide() { 
                                    SideType=SingleSideType.ANTI_SPOILER,
                                    DoubleSourceType=DoubleSoure.LEFT
                                },
                                Right = new PrintSide()
                                {
                                    SideType = SingleSideType.ANTI_SPOILER,
                                    DoubleSourceType = DoubleSoure.RIGHT
                                },
                                BatchPaperNumber = 0,
                                PrintFaceType=FaceType.SINGLES
                            });
                        }
                    }

                }
            }

            void setupExtraFaceSide(PrintFace _f, bool right, MangaPage cover)
            {
                if (cover != null)
                {
                    setSide(_f, right, new PrintSide()
                    {
                        DoubleSourceType = DoubleSoure.ALL, // We don't support spliting double on cover
                        MangaPageSource = cover,
                        SideType = SingleSideType.MANGA
                    });
                }
                else if (antiSpoiler > 0)
                {
                    setSide(_f, right, new PrintSide()
                    {
                        DoubleSourceType = DoubleSoure.ALL,
                        SideType = SingleSideType.ANTI_SPOILER
                    });
                }
                else
                {
                    setSide(_f, right, new PrintSide()
                    {
                        DoubleSourceType = DoubleSoure.ALL,
                        SideType = SingleSideType.MAKE_EVEN
                    });
                }
            }

            PrintFace extraFaceBefore = new PrintFace()
            {
                IsRTL = _O.isBookletRTL,
                PrintFaceType = FaceType.SINGLES,
                BatchPaperNumber = 0
            };

            setupExtraFaceSide(extraFaceBefore, _O.isBookletRTL, _O.bookletCoverFirst);
            setupExtraFaceSide(extraFaceBefore, !_O.isBookletRTL, _O.bookletCoverLast);

            faceResults.Insert(0,extraFaceBefore);

            PrintFace extraFaceAfter = new PrintFace()
            {
                IsRTL = _O.isBookletRTL,
                PrintFaceType = FaceType.SINGLES,
                BatchPaperNumber = antiSpoiler > 0 ? antiSpoilerAdded+1 : 0
            };

            setupExtraFaceSide(extraFaceAfter, _O.isBookletRTL, null);
            setupExtraFaceSide(extraFaceAfter, !_O.isBookletRTL, null);

            faceResults.Add(extraFaceAfter);

            if (antiSpoiler > 0 && antiSpoilerAdded %2 ==1)
            {
                PrintFace extraFaceAS = new PrintFace()
                {
                    IsRTL = _O.isBookletRTL,
                    PrintFaceType = FaceType.SINGLES,
                    BatchPaperNumber = antiSpoiler > 0 ? antiSpoilerAdded+2 : 0
                };

                setupExtraFaceSide(extraFaceAS, _O.isBookletRTL, null);
                setupExtraFaceSide(extraFaceAS, !_O.isBookletRTL, null);

                faceResults.Add(extraFaceAS);
            }

            if (faceResults.Count %2 != 0)
            {
                throw new System.Exception("Unexpected faces count! " + faceResults.Count);
            }

            int sideCounter = 0;

            void setSideCount(bool bRTL, int faceID )
            {
                (bRTL ? (faceResults[faceID].Right) : (faceResults[faceID].Left)).SideNumber
                    = sideCounter;
                (bRTL ? (faceResults[faceID].Left) : (faceResults[faceID].Right)).SideNumber
                    = faceResults.Count * 2 - sideCounter;

                sideCounter++;
            }

            List<PrintPage> result = new List<PrintPage>();
            for (int i = 0; i < faceResults.Count; i +=2)
            {
                setSideCount(bookletOptions.isBookletRTL, i);
                setSideCount(bookletOptions.isBookletRTL, i+1);
                faceResults[i].FaceNumber = i;
                faceResults[i+1].FaceNumber = i+1;

                result.Add(new PrintPage()
                {
                    Front = faceResults[i],
                    Back = faceResults[i + 1],
                    PageNumber = i/2
                });
            }

            return result;
           
        }

    }
}
