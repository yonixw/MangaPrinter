using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        private string _programVersion = "???";
        public DuplexTemplates(string versionString)
        {
            _programVersion = string.Format("\n\n[MangaPrinter {0}]\n\n", versionString );
        }

        private Dictionary<SingleSideType, String> _sideTextConsts = new Dictionary<SingleSideType, string>()
        {
            { SingleSideType.ANTI_SPOILER, "Anti Spoiler" },
            { SingleSideType.BEFORE_DOUBLE, "Filler Before Double" },
            { SingleSideType.INTRO, "Chapter start:\n{0}" },
            { SingleSideType.OUTRO, "Chapter end:\n{0}" },
            { SingleSideType.MAKE_EVEN, "Filler After Chapter" },
        };

        public void setTextTemplate(int type, string text)
        {
            if (type > -1 && type < (int)SingleSideType.LAST)
            {
                if (_sideTextConsts.ContainsKey((SingleSideType)type))
                {
                    _sideTextConsts[(SingleSideType)type] = text;
                }
            }
        }

        public string sideTextConsts(SingleSideType type)
        {
            return _sideTextConsts[type] + _programVersion;
        }

        public static Pen blackPen = new Pen(Color.Black, 4);
        public static Brush blackBrush = new SolidBrush(Color.Black);
        public static Font fontSide = new Font(new FontFamily("Arial"), 5, FontStyle.Bold); // size will be changed
        public static Font fontPageText = new Font(new FontFamily("Arial"), 5);

        public Bitmap BuildFace(PrintFace face,  int spW, int spH, int padding, bool colors, bool parentText)
        {
            Bitmap result = null;
            if (face.Left == face.Right) // double
            {
                if (
                    face.Right.SideType != SingleSideType.MANGA &&
                    face.Right.SideType != SingleSideType.ANTI_SPOILER
                    )
                    throw new Exception(
                        string.Format("Got type {0} in double in duplex. It's unexpected.", face.Right.SideType)
                        );

                result = TemplateDouble(face, spW, spH, padding);

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

                result = TemplateSingle(face, spW, spH, padding, parentText);
            }

            if (result != null && !colors) result = GraphicsUtils.MakeGrayscale3(result);
            // TODO: dispose bitmaps?
            return result;
        }
        private static string GetFaceCountText(PrintFace face)
        {
            string result = "Page No. " + face.FaceNumber;
            if (face.BatchPaperNumber > -1)
                result += " (AntiSpoiler " + (face.BatchPaperNumber + 1) + ")";
            return result;
        }

        private string GetMeta(PrintSide face)
        {
            if (face.MangaPageSource == null)
            {
                return PrintPage.lastFullExportMetadata;
            }
            string chName = face.MangaPageSource.Chapter.Name;
            string chFolder = face.MangaPageSource.Chapter.ParentName;
            string meta = string.Format("{0} > {1} > {2}", chFolder, chName, PrintPage.lastFullExportMetadata);
            return meta;
        }

        private Bitmap TemplateDouble(PrintFace face, int spW, int spH, int padding)
        {
            int tmpW = spW * 2 + padding * 3;
            int tmpH = spH + padding * 2;

            int contentW = tmpW - padding * 2;
            int contentH = tmpH - padding * 2;

            int sideTextH = padding;

            bool isRTL = face.IsRTL;

            string metaInfo = GetMeta(isRTL ? face.Right : face.Left);

            Bitmap b = new Bitmap(tmpW, tmpH);
            using (Graphics g = Graphics.FromImage(b))
            {
                int sideIndex = face.Right.SideNumber;
                string FaceCountText = GetFaceCountText(face);

                if (isRTL)
                {
                    GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                        new Point(tmpW - padding, padding / 2),
                        new Point(padding, padding / 2),
                        padding);
                }
                else
                {
                    GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                       new Point(padding, padding / 2),
                       new Point(tmpW - padding, padding / 2),
                       padding);
                }

                GraphicsUtils.FontScaled faceCountFS = GraphicsUtils.FindFontSizeByContent(
                    g, FaceCountText, new Size(tmpW / 4, sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, FaceCountText, faceCountFS, blackBrush,
                    new PointF(tmpW * 0.75f + tmpW * 0.25f * 0.5f, padding + contentH + padding / 2)
                );

                GraphicsUtils.FontScaled metaFS = GraphicsUtils.FindFontSizeByContent(
                    g, metaInfo, new Size((int)(tmpW * 0.75f), sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, metaInfo, metaFS, blackBrush,
                    new PointF(tmpW * 0.75f * 0.5f, padding + contentH + padding / 2)
                );

                Bitmap page = null;
                switch (face.Left.SideType)
                {
                    case SingleSideType.ANTI_SPOILER:
                        page = GraphicsUtils.createImageWithText(sideTextConsts(SingleSideType.ANTI_SPOILER),
                            contentH, contentW);
                        break;
                    case SingleSideType.MANGA:
                        page = GraphicsUtils.loadFileZoomedCentered(face.Left.MangaPageSource.ImagePath,
                            contentH, contentW);
                        break;
                }
                g.DrawImage(page, new Point(padding, padding));
                page.Dispose();

                // border
                //g.DrawRectangle(blackPen, new Rectangle(padding, padding, contentW, contentH));
            }

            return b;
        }

        private Bitmap TemplateSingle(PrintFace face, int spW, int spH, int padding, bool parentText)
        {
            int tmpW = spW * 2 + padding *3;
            int tmpH = spH + padding * 2;

            int pageW = (tmpW - padding * 3) /2; // one padding between pages
            int pageH = tmpH - padding * 2;

            int contentH = pageH;
            int sideTextH = padding;

            bool isRTL = face.IsRTL;
            string metaInfo = GetMeta(isRTL ? face.Right : face.Left);

            Bitmap b = new Bitmap(tmpW, tmpH);
            using (Graphics g = Graphics.FromImage(b))
            {
                string FaceCountText = GetFaceCountText(face);

                if (isRTL)
                {
                    GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                        new Point(tmpW - padding, padding / 2),
                        new Point(padding, padding / 2),
                        padding);
                }
                else
                {
                    GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                       new Point(padding, padding / 2),
                       new Point(tmpW - padding, padding / 2),
                       padding);
                }


                GraphicsUtils.FontScaled faceCountFS = GraphicsUtils.FindFontSizeByContent(
                    g, FaceCountText, new Size(tmpW / 4, sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, FaceCountText, faceCountFS, blackBrush,
                    new PointF(tmpW * 0.75f + tmpW * 0.25f * 0.5f, padding + contentH + padding / 2)
                );

                GraphicsUtils.FontScaled metaFS = GraphicsUtils.FindFontSizeByContent(
                    g, metaInfo, new Size((int)(tmpW * 0.75f), sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, metaInfo, metaFS, blackBrush,
                    new PointF(tmpW * 0.75f * 0.5f, padding + contentH + padding / 2)
                );


                DrawSide(pageW, pageH, g, face.Left, new Point(padding, padding), parentText);
                DrawSide(pageW, pageH, g, face.Right, new Point(padding*2 + pageW, padding), parentText);

                // border
                //g.DrawRectangle(blackPen, new Rectangle(padding, padding, pageW, pageH));
            }

            return b;
        }

        private void DrawSide(int pageW, int pageH, Graphics g,  PrintSide side, Point pagePlace, bool parentName)
        {
            Bitmap page = null;
            string chapterName = "<init ch name>";
            if (side.SideType == SingleSideType.INTRO || side.SideType == SingleSideType.OUTRO)
            {
                chapterName = (parentName ? side.MangaPageSource.Chapter.ParentName + '\n' : "") +
                side.MangaPageSource.Chapter.Name;
            }
                
            switch (side.SideType)
            {
                case SingleSideType.INTRO:
                    page = GraphicsUtils.createImageWithText(sideTextConsts(SingleSideType.INTRO)
                        .Replace("{0}", chapterName),
                      pageH, pageW);
                    break;
                case SingleSideType.OUTRO:
                    page = GraphicsUtils.createImageWithText(sideTextConsts(SingleSideType.OUTRO)
                        .Replace("{0}", chapterName),
                      pageH, pageW);
                    break;
                case SingleSideType.BEFORE_DOUBLE:
                    page = GraphicsUtils.createImageWithText(sideTextConsts(SingleSideType.BEFORE_DOUBLE),
                      pageH, pageW);
                    break;
                case SingleSideType.MAKE_EVEN:
                    page = GraphicsUtils.createImageWithText(sideTextConsts(SingleSideType.MAKE_EVEN),
                       pageH, pageW);
                    break;
                case SingleSideType.MANGA:
                    page = GraphicsUtils.loadFileZoomedCentered(side.MangaPageSource.ImagePath,
                       pageH, pageW);
                    break;
            }
            g.DrawImage(page, pagePlace);
            page.Dispose();
        }
    }
}
