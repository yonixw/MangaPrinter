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

           
            bool isFirst = true; // Starting page
            List<PrintFace> Faces = new List<PrintFace>();

            foreach (MangaChapter ch in chapters)
            {
                MangaPage SinglePageNULL = new MangaPage() { IsDouble = false, Chapter = ch }; 

                if (addStartPage)
                    HandleFace( ref isFirst, Faces, ch, SinglePageNULL, SingleSideType.INTRO);

                foreach (MangaPage p in ch.Pages)
                {
                    HandleFace( ref isFirst, Faces, ch, p);
                }

                if (addEndPage)
                    HandleFace( ref isFirst, Faces, ch, SinglePageNULL, SingleSideType.OUTRO);

                if (isFirst)
                {
                    // ignore empty side
                }
                else
                {
                    PrintFace face = Faces.Last();

                    PrintSide side = new PrintSide()
                    {
                        SideType = SingleSideType.MAKE_EVEN,
                    };

                    if (ch.IsRTL)
                        face.Left = side;
                    else
                        face.Right = side;

                    isFirst = true;
                }

            }

            // ----------- Anti Spoiler
            if (addAntiSpoiler > 1)
            {
                int faceIndex = 0;
                while (faceIndex < Faces.Count)
                {
                    PrintSide s = new PrintSide() { SideType = SingleSideType.ANTI_SPOILER };
                    PrintFace f = new PrintFace() { PrintFaceType = FaceType.DOUBLE , IsRTL = true}; // RTL not important
                    f.Left = f.Right = s;
                    Faces.Insert(faceIndex, f);
                    faceIndex += addAntiSpoiler * 2;
                }
            }

            if (Faces.Count % 2 == 1)
            {
                // Add a face for even faces to occupy entire double-sided pages.
                PrintFace face = new PrintFace() { PrintFaceType = FaceType.DOUBLE ,IsRTL = true };
                Faces.Add(face);

                PrintSide side = new PrintSide()
                {
                    SideType = SingleSideType.MAKE_EVEN
                };

                face.Left = face.Right = side;
            }

            int pageIndex = 1;
            int sideCounter = 1;
            for (int i=0;i<Faces.Count;i+=2)
            {
                for (int j=0;j<2;j++)
                {
                    if (Faces[i + j].Right == Faces[i + j].Left)
                    {
                        Faces[i + j].Right.SideNumber = sideCounter++;
                        sideCounter++;
                    }
                    else
                    {
                        if (Faces[i + j].IsRTL )
                        {
                            Faces[i+j].Right.SideNumber = sideCounter++;
                            Faces[i + j].Left.SideNumber = sideCounter++;
                        }
                        else
                        {
                            Faces[i + j].Left.SideNumber = sideCounter++;
                            Faces[i + j].Right.SideNumber = sideCounter++;
                        }
                    }
                }
                PrintPage pp = new PrintPage() { PageNumber = pageIndex++,  Front = Faces[i], Back = Faces[i + 1] };
                pc.Add(pp);
            }

            return pc;
        }

        private static void HandleFace( ref bool isFirst, List<PrintFace> Faces, MangaChapter ch, MangaPage p, SingleSideType sideType = SingleSideType.MANGA)
        {
            if (isFirst && !p.IsDouble)
            {
                PrintFace face = new PrintFace() { PrintFaceType = FaceType.SINGLES , IsRTL = ch.IsRTL};
                Faces.Add(face);

                PrintSide side = null;
                if (sideType == SingleSideType.MANGA)
                {
                    side = new PrintSide()
                    {
                        
                        SideType = SingleSideType.MANGA,
                        MangaPageSource = p,
                        MangaPageSourceType = SideMangaPageType.ALL
                    };
                }
                else
                {
                    side = new PrintSide()
                    {
                        MangaPageSource = p,
                        SideType = sideType,
                    };
                }

                if (ch.IsRTL)
                    face.Right = side;
                else
                    face.Left = side;

                isFirst = false;

            }
            else if (isFirst && p.IsDouble)
            {
                PrintFace face = new PrintFace() { PrintFaceType = FaceType.DOUBLE, IsRTL = ch.IsRTL };
                Faces.Add(face);

                PrintSide side = new PrintSide()
                {
                    
                    SideType = SingleSideType.MANGA,
                    MangaPageSource = p,
                    MangaPageSourceType = SideMangaPageType.ALL // only in booklet we need to know right\left
                };

                face.Left = face.Right = side;
               
                isFirst = true;
            }
            else if (!isFirst && !p.IsDouble)
            {
                PrintFace face = Faces.Last();

                PrintSide side = null;
                if (sideType == SingleSideType.MANGA)
                {
                    side = new PrintSide()
                    {
                       
                        SideType = SingleSideType.MANGA,
                        MangaPageSource = p,
                        MangaPageSourceType = SideMangaPageType.ALL
                    };
                }
                else
                {
                    side = new PrintSide()
                    {
                        SideType = sideType,
                        MangaPageSource = p,
                    };
                }

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

                    SideType = SingleSideType.BEFORE_DOUBLE,
                };

                if (ch.IsRTL)
                    face.Left = side;
                else
                    face.Right = side;

                // Add Double
                face = new PrintFace() { PrintFaceType = FaceType.DOUBLE, IsRTL = ch.IsRTL };
                Faces.Add(face);

                side = new PrintSide()
                {

                    SideType = SingleSideType.MANGA,
                    MangaPageSource = p,
                    MangaPageSourceType = SideMangaPageType.ALL // only in booklet we need to know right\left
                };

                face.Left = face.Right = side;
                isFirst = true;
            }
        }
    }
}
