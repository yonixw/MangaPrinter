using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MangaPrinter.Core.TemplateBuilders
{
    public class GraphicsUtils
    {
        static Pen borderPen = new Pen(Color.Black, 4);

        //https://stackoverflow.com/questions/19674743/dynamically-resizing-font-to-fit-space-while-using-graphics-drawstring
        //This function checks the room size and your text and appropriate font for your text to fit in room
        //PreferedFont is the Font that you wish to apply
        //Room is your space in which your text should be in.
        //LongString is the string which it's bounds is more than room bounds.
        public static Font FindFontSizeByContent(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
            //you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }

        public static Bitmap createImageWithText(string drawText,
            int height, int width ,
            string fontName = "Arial" )
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.DrawRectangle(Pens.White, new Rectangle(0, 0, width, height));

            Font textFont = FindFontSizeByContent(g, drawText, b.Size, new Font(new FontFamily(fontName), 5)); // font size not imprtnt
            g.DrawString(drawText, textFont, Brushes.Black, 0, 0);
            g.DrawRectangle(borderPen, new Rectangle(0, 0, width, height));

            return b;
        }

        public static Bitmap loadFileZoomedCentered(string path, int newHeight, int newWidth)
        {
            try
            {
                return sameAspectResize( (Bitmap)Image.FromFile(path), newHeight, newWidth);
                // no stream because it has to stay open: https://stackoverflow.com/a/1053123/1997873
            }
            catch (OutOfMemoryException ex)
            {
                MessageBox.Show("Can't load image file:\n" + path);

            }
            return null;
        }

        // Put scaled image (content) inside bigger image, centered.
        public static Bitmap sameAspectResize(Bitmap image, int targetWidth, int targetHeight)
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
                Bitmap imageResized = bitmapResize(image, finalWidth, finalHeight, targetWidth, targetHeight);
                image.Dispose();
                return imageResized;
            }
            else
            {
                return image;
            }
        }

        // Put scaled image (content) inside bigger image, centered.
        public static Bitmap bitmapResize(Bitmap image, int contentWidth, int contentHeight,
            int imageWidth, int imageHeight)
        {
            float scale = Math.Min(1.0f * contentWidth / image.Width, 1.0f * contentHeight / image.Height);
            var fillBrush = new SolidBrush(Color.White);
           

            var bmp = new Bitmap(imageWidth,imageHeight);
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
                graph.DrawRectangle(borderPen, new Rectangle(0, 0, imageWidth, imageHeight));
            }

            image.Dispose();
            return bmp;
        }

    }
}
