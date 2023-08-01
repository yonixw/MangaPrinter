using MangaPrinter.Conf;
using System;
using System.Drawing;

namespace MangaPrinter.Core.TemplateBuilders
{
    public class FlatTemplates : ITemplateBuilder
    {
        /* 
            *  Templates in duplex mode:
            *  ==================================
            *  
            *  Side Type:   I   O   M   B   E   A  O
            *  Single:      +   +   +   +   +   -  + 
            *  Double:      -   -   +   -   -   +  +
        */

        
        public FlatTemplates()
        {
            
        }

        public static Pen borderPen = new Pen(Color.Black, 4);
        public static Pen blackPen = new Pen(Color.Black, 4);
        public static Brush blackBrush = new SolidBrush(Color.Black);
        public static Font fontSide = new Font(new FontFamily(CoreConf.I.Templates_TextFont), 5, FontStyle.Bold); // size will be changed
        public static Font fontPageText = new Font(new FontFamily(CoreConf.I.Templates_TextFont), 5);

        public Bitmap BuildFace(PrintFace[] faces, PrintSide[] sides, int faceCount,  int spW, int spH, int padding, bool colors, bool parentText)
        {
            PrintFace face = faces[0];

            Bitmap result = null;
            if (face.Left == face.Right  ) // double
            {
                if (
                    face.Right.SideType != SingleSideType.MANGA &&
                    face.Right.SideType != SingleSideType.ANTI_SPOILER &&
                    face.Right.SideType != SingleSideType.OMITED
                    )
                    throw new Exception(
                        string.Format("Got type {0} in double in duplex. It's unexpected.", face.Right.SideType)
                        );

                result = TemplateDouble(face, faceCount, spW, spH, padding);

            }
            else
            {
                result = TemplateSingle(face, faceCount, spW, spH, padding, parentText);
            }

            if (result != null && !colors) result = GraphicsUtils.MakeGrayscale3(result);
            // TODO: dispose bitmaps?
            return result;
        }
        
        private static string GetFaceCountText(PrintFace face, int number, int delta = 0)
        {
            // Todo, total face numbers, total A-S count?
            string result = String.Format(CoreConf.I.Templates_MetaText_PageNum, 
               number);
            if (face.BatchPaperNumber > -1)
                result += " " + String.Format(CoreConf.I.Templates_MetaText_AntiSpoilerNum, face.BatchPaperNumber + 1);

            return TextUtils.PostProcess(result,false);
        }

        private string GetNamesMeta(PrintSide face)
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

        private void bindMetas(PrintFace face, int FaceCount, out string metaLeftTxt, out string metaRightTxt)
        {
            if (face.isBooklet)
            {
                // Face numbers trick explained here:
                // https://github.com/yonixw/MangaPrinter/issues/15#issuecomment-1660892656
                if (face.IsRTL) { 
                    if (face.isFront)
                    {
                        metaRightTxt = GetNamesMeta(face.Right);
                         metaLeftTxt =  GetFaceCountText(face,
                             (2 * FaceCount - face.FaceNumber) /2 +1 );
                    }
                    else
                    {
                         metaLeftTxt = GetNamesMeta(face.Left);
                         metaRightTxt = GetFaceCountText(face, face.FaceNumber /2  );
                    }
                }
                else
                {
                    if (face.isFront)
                    {
                        metaRightTxt = GetNamesMeta(face.Right);
                         metaLeftTxt = GetFaceCountText(face, face.FaceNumber/2 );
                    }
                    else
                    {
                        metaLeftTxt = GetNamesMeta(face.Left);
                         metaRightTxt = GetFaceCountText(face,
                             (2 * FaceCount - face.FaceNumber) /2 +1);
                    }
                }
            }
            else//duplex
            {
                metaLeftTxt = GetNamesMeta(face.IsRTL ? face.Right : face.Left);
                metaRightTxt = GetFaceCountText(face, face.FaceNumber);
            }
        }

        private Bitmap TemplateDouble(PrintFace face, int FaceCount, int spW, int spH, int padding)
        {
            int tmpW = spW * 2 + padding * 3;
            int tmpH = spH + padding * 2;

            int contentW = tmpW - padding * 2;
            int contentH = tmpH - padding * 2;

            int sideTextH = padding;

            bool isRTL = face.IsRTL;

            string metaLeftTxt, metaRightTxt;
            bindMetas(face, FaceCount, out metaLeftTxt, out metaRightTxt);

            Bitmap b = new Bitmap(tmpW, tmpH);
            using (Graphics g = Graphics.FromImage(b))
            {
                //int sideIndex = face.Right.SideNumber;

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

                GraphicsUtils.FontScaled metaRight  /*Face is one view of page*/ = GraphicsUtils.FindFontSizeByContent(
                    g, metaRightTxt, new Size((int)(tmpW * 0.5f), sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, metaRightTxt, metaRight, blackBrush,
                    new PointF(tmpW * 0.5f + tmpW * 0.5f * 0.5f, padding + contentH + padding / 2)
                );

                GraphicsUtils.FontScaled metaLeft = GraphicsUtils.FindFontSizeByContent(
                    g, metaLeftTxt, new Size((int)(tmpW * 0.5f), sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, metaLeftTxt, metaLeft, blackBrush,
                    new PointF(tmpW * 0.5f * 0.5f, padding + contentH + padding / 2)
                );

                Bitmap page = null;
                switch (face.Left.SideType)
                {
                    case SingleSideType.ANTI_SPOILER:
                        page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Render_AntiSpoiler, true),
                            contentH, contentW);
                        break;
                    case SingleSideType.OMITED:
                        page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Render_Omited, true),
                            contentH, contentW);
                        break;
                    case SingleSideType.MANGA:
                        page = GraphicsUtils.loadFileZoomedCentered(face.Left.MangaPageSource,
                            contentH, contentW);
                        break;
                }
                g.DrawImage(page, new Point(padding, padding));

                if (CoreConf.I.Templates_ShowBorders)
                    g.DrawRectangle(borderPen, new Rectangle(padding, padding, contentW, contentH));

                page.Dispose();
            }

            return b;
        }

        private Bitmap TemplateSingle(PrintFace face, int faceCount, int spW, int spH, int padding, bool parentText)
        {
            int tmpW = spW * 2 + padding *3;
            int tmpH = spH + padding * 2;

            int pageW = (tmpW - padding * 3) /2; // one padding between pages
            int pageH = tmpH - padding * 2;

            int contentH = pageH;
            int sideTextH = padding;

            bool isRTL = face.IsRTL;

            string metaLeftTxt, metaRightTxt;
            bindMetas(face, faceCount, out metaLeftTxt, out metaRightTxt);

            Bitmap b = new Bitmap(tmpW, tmpH);
            using (Graphics g = Graphics.FromImage(b))
            {
                if (CoreConf.I.Templates_ShowDirectionArrows)
                {
                    // RightSide
                    if (face.Right.MangaPageSource != null)
                    {
                        if (face.Right.MangaPageSource.Chapter.IsRTL)
                        {
                            GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                               new Point(tmpW - padding, padding / 2),
                               new Point(tmpW/2 - padding, padding / 2),
                               padding);
                        }
                        else
                        {
                            GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                               new Point(tmpW / 2 - padding, padding / 2),
                               new Point(tmpW - padding, padding / 2),
                                padding);
                        }
                    }

                    // LeftSide
                    if (face.Left.MangaPageSource != null)
                    {
                        if (face.Left.MangaPageSource.Chapter.IsRTL)
                        {
                            GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                               new Point(tmpW/2 - padding, padding / 2),
                               new Point(padding, padding / 2),
                               padding);
                        }
                        else
                        {
                            GraphicsUtils.DrawArrowHeadRow(g, blackPen,
                              new Point(padding, padding / 2),
                              new Point(tmpW/2 - padding, padding / 2),
                              padding);
                        }
                    }



                }


                GraphicsUtils.FontScaled metaRight  = GraphicsUtils.FindFontSizeByContent(
                    g, metaRightTxt, new Size((int)(tmpW * 0.5f), sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, metaRightTxt, metaRight, blackBrush,
                    new PointF(tmpW * 0.5f + tmpW * 0.5f * 0.5f, padding + contentH + padding / 2)
                );

                GraphicsUtils.FontScaled metaLeft = GraphicsUtils.FindFontSizeByContent(
                    g, metaLeftTxt, new Size((int)(tmpW * 0.5f), sideTextH), fontSide
                );
                GraphicsUtils.DrawTextCenterd(g, metaLeftTxt, metaLeft, blackBrush,
                    new PointF(tmpW * 0.5f * 0.5f, padding + contentH + padding / 2)
                );

                if (CoreConf.I.Templates_Render_AddGutter)
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
                    DrawSide(pageW + padding/2, pageH, g, face.Right, new Point(padding + pageW + padding/2 -1 /*avoid 1 pixel line*/, padding), parentText);

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
                        TextUtils.PostProcess(String.Format(CoreConf.I.Templates_Render_Intro, chapterName),true),
                      pageH, pageW);
                    break;
                case SingleSideType.OUTRO:
                    page = GraphicsUtils.createImageWithText(
                        TextUtils.PostProcess(String.Format(CoreConf.I.Templates_Render_Outro, chapterName), true),
                      pageH, pageW);
                    break;
                case SingleSideType.BEFORE_DOUBLE:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Render_BeforeDouble, true),
                      pageH, pageW);
                    break;
                case SingleSideType.MAKE_EVEN:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Render_MakeEven, true),
                       pageH, pageW);
                    break;
                case SingleSideType.OMITED:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Render_Omited, true),
                       pageH, pageW);
                    break;
                case SingleSideType.ANTI_SPOILER:
                    page = GraphicsUtils.createImageWithText(TextUtils.PostProcess(CoreConf.I.Templates_Render_AntiSpoiler, true),
                       pageH, pageW);
                    break;
                case SingleSideType.MANGA:
                    page = GraphicsUtils.loadFileZoomedCentered(side.MangaPageSource,
                       pageH, pageW, side.DoubleSourceType);
                    break;
            }
            g.DrawImage(page, pagePlace);
            page.Dispose();
        }
    }
}
