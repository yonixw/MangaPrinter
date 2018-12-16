using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MangaPrinter.Core.ChapterBuilders
{
    public class DuplexBuilder : IBindBuilder
    {
      
        public List<PrintPage> Build(List<MangaChapter> chapters, bool addStartPage, bool addEndPage, int addAntiSpoiler = 0)
        {
            // For loop from 1 to end and add pages as necessary.
            // Double Template should have 0px between 2 pages.

            List<PrintPage> pc =  new List<PrintPage>() ;

            int sideCounter = 1;
            bool isFirst = true; // Starting page
            List<PrintFace> Faces = new List<PrintFace>();

            foreach (MangaChapter ch in chapters)
            {

                foreach (MangaPage p in ch.Pages)
                {

                    if (isFirst && !p.IsDouble)
                    {
                        PrintFace face = new PrintFace() { PrintFaceType = FaceType.SINGLES };
                        Faces.Add(face);

                        PrintSide side = new PrintSide()
                        {
                            SideNumber = sideCounter++,
                            SideType = SingleSideType.MANGA,
                            MangaPageSource = p,
                            MangaPageSourceType = SideMangaPageType.ALL
                        };

                        if (ch.IsRTL)
                            face.Right = side;
                        else
                            face.Left = side;

                        isFirst = false;

                    }
                    else if (isFirst && p.IsDouble)
                    {
                        PrintFace face = new PrintFace() { PrintFaceType = FaceType.DOUBLE };
                        Faces.Add(face);

                        PrintSide side = new PrintSide()
                        {
                            SideNumber = sideCounter++,
                            SideType = SingleSideType.MANGA,
                            MangaPageSource = p,
                            MangaPageSourceType = SideMangaPageType.ALL // only in booklet we need to know right\left
                        };

                        face.Left = face.Right = side;
                        sideCounter++;
                        isFirst = true;
                    }
                    else if (!isFirst && !p.IsDouble)
                    {
                        PrintFace face = Faces.Last();

                        PrintSide side = new PrintSide()
                        {
                            SideNumber = sideCounter++,
                            SideType = SingleSideType.MANGA,
                            MangaPageSource = p,
                            MangaPageSourceType = SideMangaPageType.ALL
                        };

                        if (ch.IsRTL)
                            face.Left = side;
                        else
                            face.Right = side;

                        isFirst = true;
                    }
                    else if (!isFirst && p.IsDouble)
                    {
                        // Add FILLER
                        PrintFace face = Faces.Last();

                        PrintSide side = new PrintSide()
                        {
                            SideNumber = sideCounter++,
                            SideType = SingleSideType.BEFORE_DOUBLE,
                        };

                        if (ch.IsRTL)
                            face.Left = side;
                        else
                            face.Right = side;

                        // Add Double
                        face = new PrintFace() { PrintFaceType = FaceType.DOUBLE };
                        Faces.Add(face);

                        side = new PrintSide()
                        {
                            SideNumber = sideCounter++,
                            SideType = SingleSideType.MANGA,
                            MangaPageSource = p,
                            MangaPageSourceType = SideMangaPageType.ALL // only in booklet we need to know right\left
                        };

                        face.Left = face.Right = side;
                        sideCounter++;
                        isFirst = true;
                    }
                }

                if (isFirst)
                {
                    // ignore empty side
                }
                else
                {
                    PrintFace face = Faces.Last();

                    PrintSide side = new PrintSide()
                    {
                        SideNumber = sideCounter++,
                        SideType = SingleSideType.MAKE_EVEN,
                    };

                    if (ch.IsRTL)
                        face.Left = side;
                    else
                        face.Right = side;
                }

            }
            //if (Faces.Count % 2 == 1)
            //{
            //    // Add a face for even faces to occupy entire double-sided pages.
            //    PrintFace face = new PrintFace() { PrintFaceType = FaceType.DOUBLE };
            //    Faces.Add(face);

            //    PrintSide side = new PrintSide()
            //    {
            //        SideNumber = sideCounter++,
            //        SideType = SingleSideType.MAKE_EVEN
            //    };

            //    face.Left = face.Right = side;
            //}

            int pageIndex = 1;
            for (int i=0;i<Faces.Count;i+=2)
            {
                PrintPage pp = new PrintPage() { PageNumber = pageIndex++,  Front = Faces[i], Back = Faces[i + 1] };
                pc.Add(pp);
            }

            return pc;
        }
    }
}
