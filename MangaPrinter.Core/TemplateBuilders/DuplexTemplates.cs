using MangaPrinter.Conf;
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
            *  Side Type:   I   O   M   B   E   A  O
            *  Single:      +   +   +   +   +   -  + 
            *  Double:      -   -   +   -   -   +  +
        */

        
        public DuplexTemplates()
        {
            
        }


        public static Pen borderPen = new Pen(Color.Black, 4);
        public static Pen blackPen = new Pen(Color.Black, 4);
        public static Brush blackBrush = new SolidBrush(Color.Black);
        public static Font fontSide = new Font(new FontFamily(CoreConf.I.Templates_TextFont), 5, FontStyle.Bold); // size will be changed
        public static Font fontPageText = new Font(new FontFamily(CoreConf.I.Templates_TextFont), 5);

        public Bitmap BuildFace(PrintFace face,  int spW, int spH, int padding, bool colors, bool parentText)
        {
            Bitmap result = null;
            if (face.Left == face.Right) // double
            {
                if (
                    face.Right.SideType != SingleSideType.MANGA &&
                    face.Right.SideType != SingleSideType.ANTI_SPOILER &&
                    face.Right.SideType != SingleSideType.OMMITED
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
            // Todo, total face numbers, total A-S count?
            string result = String.Format(CoreConf.I.Templates_MetaText_PageNum, face.FaceNumber);
            if (face.BatchPaperNumber > -1)
                result += " " + String.Format(CoreConf.I.Templates_MetaText_AntiSpoilerNum, face.BatchPaperNumber + 1);

            return TextUtils.PostProcess(result,false);
        }

        private string GetMeta(PrintSide face)
        {
            if (face.MangaPageSource == null)
            {
                return PrintPage.lastFullExportMetadata;
            }
            string chName = face.MangaPageSource.Chapter.Name;
            string chFolder = face.MangaPageSource.Chapter.ParentName;

            string meta = string.Format(CoreConf.I.Templates_MetaText_Structure, 
                chFolder, chName, PrintPage.lastFullExportMetadata);

            return TextUtils.PostProcess(meta,false);
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

                if (CoreConf.I.Templates_ShowDirectionArrows)
                {
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
                }

                GraphicsUtils.FontScaled faceCountFS  /*Face count became PrintPagecount*/ = GraphicsUtils.FindFontSizeByContent(
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
                        page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Duplex_AntiSpoiler,true),
                            contentH, contentW);
                        break;
                    case SingleSideType.OMMITED:
                        page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Ommited, true),
                            contentH, contentW);
                        break;
                    case SingleSideType.MANGA:
                        page = GraphicsUtils.loadFileZoomedCentered(face.Left.MangaPageSource.ImagePath,
                            contentH, contentW);
                        break;
                }
                g.DrawImage(page, new Point(padding, padding));

                if (CoreConf.I.Templates_ShowBorders)
                    g.DrawRectangle(borderPen, new Rectangle(padding,padding, contentW, contentH));

                page.Dispose();
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

                if (CoreConf.I.Templates_ShowDirectionArrows)
                {
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
                }


                GraphicsUtils.FontScaled faceCountFS /*Face count became PrintPagecount*/ = GraphicsUtils.FindFontSizeByContent(
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

                if (CoreConf.I.Templates_Duplex_AddGutter)
                {
                    DrawSide(pageW, pageH, g, face.Left, new Point(padding, padding), parentText);
                    DrawSide(pageW, pageH, g, face.Right, new Point(padding * 2 + pageW, padding), parentText);

                    if (CoreConf.I.Templates_ShowBorders)
                    {
                        g.DrawRectangle(borderPen, new Rectangle(padding, padding, pageW, pageH));
                        g.DrawRectangle(borderPen, new Rectangle(padding * 2 + pageW, padding, pageW, pageH));
                    }
                }
                else
                {
                    DrawSide(pageW + padding/2, pageH, g, face.Left, new Point(padding, padding), parentText);
                    DrawSide(pageW + padding/2, pageH, g, face.Right, new Point(padding + pageW + padding/2, padding), parentText);

                    if (CoreConf.I.Templates_ShowBorders)
                        g.DrawRectangle(borderPen, new Rectangle(padding, padding, pageW*2+padding, pageH));
                }
            }

            return b;
        }

        private void DrawSide(int pageW, int pageH, Graphics g,  PrintSide side, Point pagePlace, bool parentName)
        {
            Bitmap page = null;
            string chapterName = "(Missing Ch. Name)";
            if (side.SideType == SingleSideType.INTRO || side.SideType == SingleSideType.OUTRO)
            {
                chapterName = (parentName ? side.MangaPageSource.Chapter.ParentName + '\n' : "") +
                side.MangaPageSource.Chapter.Name;
            }
                
            switch (side.SideType)
            {
                case SingleSideType.INTRO:
                    page = GraphicsUtils.createImageWithText(
                        TextUtils.PostProcess(String.Format(CoreConf.I.Templates_Duplex_Intro, chapterName),true),
                      pageH, pageW);
                    break;
                case SingleSideType.OUTRO:
                    page = GraphicsUtils.createImageWithText(
                        TextUtils.PostProcess(String.Format(CoreConf.I.Templates_Duplex_Outro, chapterName), true),
                      pageH, pageW);
                    break;
                case SingleSideType.BEFORE_DOUBLE:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Duplex_BeforeDouble, true),
                      pageH, pageW);
                    break;
                case SingleSideType.MAKE_EVEN:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Duplex_MakeEven, true),
                       pageH, pageW);
                    break;
                case SingleSideType.OMMITED:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Ommited, true),
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
