﻿using ImageMagick;
using MangaPrinter.Conf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MangaPrinter.Core.TemplateBuilders
{
    public class GraphicsUtils
    {

        public struct FontScaled
        {
            public SizeF size;
            public Font font;
        }

        //https://stackoverflow.com/questions/19674743/dynamically-resizing-font-to-fit-space-while-using-graphics-drawstring
        //This function checks the room size and your text and appropriate font for your text to fit in room
        //PreferedFont is the Font that you wish to apply
        //Room is your space in which your text should be in.
        //LongString is the string which it's bounds is more than room bounds.
        public static FontScaled FindFontSizeByContent(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
            if (String.IsNullOrEmpty(longString)) longString = "(missing)";

            //you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new FontScaled() {
                font = new Font(PreferedFont.FontFamily, ScaleFontSize),
                size = new SizeF(ScaleRatio * RealSize.Width, ScaleRatio * RealSize.Height)
            };
        }

        public static void DrawTextCenterd(Graphics g, string text, FontScaled fs, Brush brush, PointF centerPoint)
        {
            g.DrawString(text, fs.font, brush, new PointF(centerPoint.X - fs.size.Width /2, centerPoint.Y - fs.size.Height / 2));
        }


        public static Bitmap createImageWithText(string drawText,
            int height, int width,
            string fontName = "Arial")
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));

            Font textFont = FindFontSizeByContent(g, drawText, b.Size, new Font(new FontFamily(fontName), 5)).font; // font size not imprtnt
            g.DrawString(drawText, textFont, Brushes.Black, 0, 0);

            return b;
        }

        public static Bitmap loadFileZoomedCentered(MangaPage page, int newHeight, int newWidth, DoubleSoure doubleSoure = DoubleSoure.ALL)
        {
            try
            {
                Bitmap fullFile = MagickImaging.BitmapFromUrlExt(page);
                if(doubleSoure != DoubleSoure.ALL)
                {
                    if (doubleSoure == DoubleSoure.LEFT)
                    {
                        return sameAspectResize(
                            bitmapCrop(fullFile, new PageEffects() { CropRight = 50 })
                            , newWidth, newHeight);
                    }
                    else if (doubleSoure == DoubleSoure.RIGHT)
                    {
                        return sameAspectResize(
                           bitmapCrop(fullFile, new PageEffects() { CropLeft = 50 })
                           , newWidth, newHeight);
                    }
                }
                else
                {
                    return sameAspectResize(fullFile, newWidth, newHeight);
                }

                
                // no stream because it has to stay open: https://stackoverflow.com/a/1053123/1997873
            }
            catch (OutOfMemoryException ex)
            {
                MessageBox.Show("Can't load image file:\n" + page.ImagePath + "\n" + ex.ToString());

            }
            return null;
        }

        // Put scaled image (content) inside bigger image, centered.
        public static Bitmap sameAspectResize(Bitmap image, int targetWidth, int targetHeight, bool reuse = false)
        {
            if (targetHeight > 0 && targetWidth > 0)
            {
                int width = image.Width;
                int height = image.Height;
                float ratioBitmap = (float)width / (float)height;
                float ratioMax = (float)targetWidth / (float)targetHeight;

                int finalWidth = targetWidth;
                int finalHeight = targetHeight;
                if (ratioMax > ratioBitmap)
                {
                    finalWidth = (int)((float)targetHeight * ratioBitmap);
                }
                else
                {
                    finalHeight = (int)((float)targetWidth / ratioBitmap);
                }
                Bitmap imageResized = bitmapResize(image, finalWidth, finalHeight, targetWidth, targetHeight, reuse);
                
                if (!reuse)
                    image.Dispose();

                return imageResized;
            }
            else
            {
                return image;
            }
        }

        public static Bitmap bmpJoinHorizon(Bitmap left, Bitmap right)
        {
            int newW = left.Width + right.Width;
            int newH = Math.Max(left.Height, right.Height);
            var bmp = new Bitmap( newW, newH );

            using (var graph = Graphics.FromImage(bmp))
            {
                // uncomment for higher quality output
                graph.InterpolationMode = InterpolationMode.High;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                
                var fillBrush = new SolidBrush(Color.White);
                graph.FillRectangle(fillBrush, new RectangleF(0, 0, newW, newH));

                graph.DrawImage(left, 0, 0);
                graph.DrawImage(right, left.Width, 0);
            }

            return bmp;
        }

        public static void bmpMirrotHorizon(Bitmap src)
        {
            src.RotateFlip(RotateFlipType.RotateNoneFlipY); // around y = horizontal filp
        }


        public static Bitmap bitmapLighting(Bitmap originalImage, PageEffects _pe, bool reuse = false)
        {
            // https://stackoverflow.com/a/15408608/1997873

            Bitmap adjustedImage = new Bitmap(originalImage.Width,originalImage.Height);
            float brightness = _pe.Brightness; 
            float contrast = _pe.Contrast; 
            float gamma = _pe.Gamma; 

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
            new float[] {contrast, 0, 0, 0, 0}, // scale red
            new float[] {0, contrast, 0, 0, 0}, // scale green
            new float[] {0, 0, contrast, 0, 0}, // scale blue
            new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
            new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, originalImage.Width, originalImage.Height,
                GraphicsUnit.Pixel, imageAttributes);

            if (!reuse)
                originalImage.Dispose();

            return adjustedImage;
        }

        public static Bitmap bitmapCrop(Bitmap image, PageEffects _pe, bool reuse = false)
        {
            if (image != null)
            {
                int W = image.Width;
                int H = image.Height;
                int T = (int)Math.Floor(image.Height * 0.01f * _pe.CropTop);
                int R = (int)Math.Floor(image.Width * 0.01f * _pe.CropRight);
                int B = (int)Math.Floor(image.Height * 0.01f * _pe.CropBottom);
                int L = (int)Math.Floor(image.Width * 0.01f * _pe.CropLeft);
                var fillBrush = new SolidBrush(Color.White);

                int newW = image.Width - R - L;
                int newH = image.Height - T - B;

                var bmp = new Bitmap(newW,newH);

                using (var graph = Graphics.FromImage(bmp))
                {
                    // uncomment for higher quality output
                    graph.InterpolationMode = InterpolationMode.High;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;

                    graph.FillRectangle(fillBrush, new RectangleF(0, 0, newW, newH));
                    graph.DrawImage(image,
                        0,0,
                        new RectangleF(L,T,newW,newH),
                        GraphicsUnit.Pixel);
                }

                if (!reuse)
                    image.Dispose();
                return bmp;

            }
            else
                return image;
        }


        // Put scaled image (content) inside bigger image, centered.
        public static Bitmap bitmapResize(Bitmap image, int contentWidth, int contentHeight,
            int imageWidth, int imageHeight, bool reuse = false)
        {
            float scale = Math.Min(1.0f * contentWidth / image.Width, 1.0f * contentHeight / image.Height);
            var fillBrush = new SolidBrush(Color.White);


            var bmp = new Bitmap(imageWidth, imageHeight);
            using (var graph = Graphics.FromImage(bmp))
            {

                // uncomment for higher quality output
                graph.InterpolationMode = InterpolationMode.High;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;

                var scaleWidth = (int)(image.Width * scale);
                var scaleHeight = (int)(image.Height * scale);

                graph.FillRectangle(fillBrush, new RectangleF(0, 0, imageWidth, imageHeight));
                graph.DrawImage(image,
                    ((int)imageWidth - scaleWidth) / 2,
                    ((int)imageHeight - scaleHeight) / 2,
                    scaleWidth,
                    scaleHeight);

                if (CoreConf.I.Common_RenderImageBorder.Get())
                {
                    graph.DrawRectangle(Pens.Black,
                        ((int)imageWidth - scaleWidth) / 2,
                        ((int)imageHeight - scaleHeight) / 2,
                        scaleWidth,
                        scaleHeight
                    );
                }

                if (CoreConf.I.Common_RenderPaintAreaBorder.Get())
                {
                    graph.DrawRectangle(Pens.Red,
                       0,0, imageWidth, imageHeight
                    );
                }
                
            }

            if(!reuse)
                image.Dispose();
            return bmp;
        }

        // Draw an arrowhead at the given point
        // in the normalizede direction <nx, ny>.
        public static void DrawArrowhead(Graphics gr, Pen pen,
            PointF p, float nx, float ny, float length)
        {
            float ax = length * (-ny - nx);
            float ay = length * (nx - ny);
            PointF[] points =
            {
                new PointF(p.X + ax, p.Y + ay),
                p,
                new PointF(p.X - ay, p.Y + ax)
            };
            gr.DrawLines(pen, points);
        }

        // Draw arrow heads or tails for the
        // segment from p1 to p2.
        public static void DrawArrow(Graphics gr, Pen pen, PointF p1, PointF p2,
            float headLenth)
        //http://csharphelper.com/blog/2014/12/draw-lines-with-arrowheads-in-c/
        //http://web.archive.org/web/20190216001430/http://csharphelper.com/blog/2014/12/draw-lines-with-arrowheads-in-c/

        {
            // Draw the shaft.
            gr.DrawLine(pen, p1, p2);

            // Find the arrow shaft unit vector.
            float vx = p2.X - p1.X;
            float vy = p2.Y - p1.Y;
            float dist = (float)Math.Sqrt(vx * vx + vy * vy);
            vx /= dist;
            vy /= dist;

            // Draw the start.
            gr.FillEllipse(new SolidBrush(pen.Color), new Rectangle(
                    new Point((int)p1.X, (int)p1.Y),
                    new Size((int)pen.Width, (int)pen.Width)));

            // Draw the end.
            DrawArrowhead(gr, pen, p2, vx, vy, headLenth);
        }


        public static void DrawArrowHeadRow(Graphics g, Pen pen, PointF p1, PointF p2,
            float headLength)
        {
            float vx = p2.X - p1.X;
            float vy = p2.Y - p1.Y;
            float dist = (float)Math.Sqrt(vx * vx + vy * vy);
            vx /= dist;
            vy /= dist;

            int count = (int)Math.Floor(dist / headLength);
            float x = p1.X;
            float y = p1.Y;
            for (int i=1;i<count;i++)
            {
                DrawArrowhead(g,pen,new PointF(x + vx*i* headLength, y + vy *i* headLength),vx,vy, headLength/2);
            }
        }

        public static Bitmap MakeBW1(Bitmap Original)
        {
            // on linux, use MakeGrayscale3
            return Original.Clone(
                new Rectangle(0, 0, Original.Width, Original.Height),
                System.Drawing.Imaging.PixelFormat.Format1bppIndexed
            );
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format16bppRgb555);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, original.Width, original.Height));
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public Bitmap rotateImage(Bitmap original)
        {
            //  https://stackoverflow.com/questions/2225363/c-sharp-rotate-bitmap-90-degrees
            throw new Exception("Yoni: Not implemented");

            // Was planned when I thout about mirroring for booklet, but instead did 2 metas
            //      that will replace place if booklet

            // But for future
            // Maybe add as option in PageEffects? Bright/Contrast also for ?

            //PageEffects p = new PageEffects();

            // Use:
            // original.RotateFlip( ... 90 ... 180 etc ... )
        }
    }

    

}
