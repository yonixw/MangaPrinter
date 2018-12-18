using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core.TemplateBuilders
{
    public class DuplexTemplates
    {
        /* 
            *  Templates in duplex mode:
            *  ==================================
            *  
            *  Side Type:   I   O   M   B   E   A
            *  Single:      +   +   +   +   +   -
            *  Double:      -   -   +   -   -   +
        */

        Dictionary<SingleSideType, String> sideTextConsts = new Dictionary<SingleSideType, string>()
        {
            { SingleSideType.ANTI_SPOILER, "Anti Spoiler" },
            { SingleSideType.BEFORE_DOUBLE, "Filler\nBefore\nDouble" },
            { SingleSideType.INTRO, "Chapter start:\n{0}" },
            { SingleSideType.OUTRO, "Chapter end:\n{0}" },
            { SingleSideType.MAKE_EVEN, "Filler\nAfter\nChapter" },
        };

        public static Pen blackPen = new Pen(Color.Black, 4);
        public static Brush blackBrush = new SolidBrush(Color.Black);
        public static Font fontSide = new Font(new FontFamily("Arial"), 5, FontStyle.Bold); // size will be changed
        public static Font fontPageText = new Font(new FontFamily("Arial"), 5);

        public Bitmap BuildFace(PrintFace face, PrintFace nextFace, int spW, int spH, int padding)
        {
            if (face.Left == face.Right) // double
            {
                if (
                    face.Right.SideType != SingleSideType.MANGA &&
                    face.Right.SideType != SingleSideType.ANTI_SPOILER
                    )
                    throw new Exception(
                        string.Format("Got type {0} in double in duplex. It's unexpected.", face.Right.SideType)
                        );

                return TemplateDouble(face, nextFace, spW, spH, padding);

            }
            else
            {
                if (face.Right.SideType == SingleSideType.ANTI_SPOILER )
                    throw new Exception(
                        string.Format("Got type {0} in right single in duplex. It's unexpected.", face.Right.SideType)
                        );
                if (face.Right.SideType == SingleSideType.ANTI_SPOILER)
                    throw new Exception(
                        string.Format("Got type {0} in left single in duplex. It's unexpected.", face.Left.SideType)
                        );

                return null;
            }
        }

        private Bitmap TemplateDouble(PrintFace face, PrintFace nextFace, int spW, int spH, int padding)
        {
            int tmpW = spW * 2;
            int tmpH = spH;

            int contentW = tmpW - padding * 2;
            int contentH = tmpH - padding * 2;

            int sideTextW = contentW / 2;
            int sideTextH = padding;

            int arrowW = contentW;
            int arrowH = padding;

            bool isRTL = (face.Right.SideType == SingleSideType.MANGA) ?
                face.IsRTL : nextFace.IsRTL; // next face of anti spoiler must be from some chapter.

            Bitmap b = new Bitmap(tmpW, tmpH);
            using (Graphics g = Graphics.FromImage(b))
            {
                int sideIndex = face.Right.SideNumber;
                string side1 = "[" + sideIndex + "]";
                string side2 = "[" + (sideIndex + 1) + "]";
                Font side1Font = GraphicsUtils.FindFontSizeByContent(
                    g, side1, new Size(sideTextW, sideTextH), fontSide);
                Font side2Font = GraphicsUtils.FindFontSizeByContent(
                    g, side1, new Size(sideTextW, sideTextH), fontSide);

                if (isRTL)
                {
                    GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                        new Point(tmpW - padding, padding / 2),
                        new Point(padding, padding / 2),
                        padding);

                    g.DrawString(side1, side1Font, blackBrush, new PointF(tmpW / 2, padding + contentH));
                    g.DrawString(side2, side2Font, blackBrush, new PointF(padding, padding + contentH));
                }
                else
                {
                    GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                       new Point(padding, padding / 2),
                       new Point(tmpW - padding, padding / 2),
                       padding);

                    g.DrawString(side1, side1Font, blackBrush, new PointF(padding, padding + contentH));
                    g.DrawString(side2, side2Font, blackBrush, new PointF(tmpW / 2, padding + contentH));
                }

                Bitmap page = null;
                switch (face.Left.SideType)
                {
                    case SingleSideType.ANTI_SPOILER:
                        page = GraphicsUtils.createImageWithText(sideTextConsts[SingleSideType.ANTI_SPOILER],
                            contentH, contentW);
                        break;
                    case SingleSideType.MANGA:
                        page = GraphicsUtils.loadFileZoomedCentered(face.Left.MangaPageSource.ImagePath,
                            contentH, contentW);
                        break;
                }
                g.DrawImage(page, new Point(padding,padding));
                page.Dispose();

                // border
                g.DrawRectangle(blackPen, new Rectangle(padding, padding, contentW, contentH));
            }

            return b;
        }



    }
}
