using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MangaPrinter.Core.TemplateBuilders
{
    class GraphicsUtils
    {
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

        public static Bitmap createTextPage(string name,
            int height, int width ,
            string drawText, string fontName = "Arial" )
        {
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.DrawRectangle(Pens.White, new Rectangle(0, 0, width, height));

            Font textFont = FindFontSizeByContent(g, drawText, b.Size, new Font(new FontFamily(fontName), 5)); // font size not imprtnt
            g.DrawString(drawText, textFont, Brushes.Black, 0, height / 2 /*Y Pivot in middle*/);

            return b;
        }

        public static Bitmap loadFileZoomed(string path, int newHeight, int newWidth)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return (Bitmap)Image.FromStream(stream: file,
                                                                useEmbeddedColorManagement: false,
                                                                validateImageData: false);
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show("Can't load image file:\n" + path);

                }
            }
            return null;
        }

        public static Bitmap sameAspectResize(Bitmap image, int maxWidth, int maxHeight)
        {
            if (maxHeight > 0 && maxWidth > 0)
            {
                int width = image.Width;
                int height = image.Height;
                float ratioBitmap = (float)width / (float)height;
                float ratioMax = (float)maxWidth / (float)maxHeight;

                int finalWidth = maxWidth;
                int finalHeight = maxHeight;
                if (ratioMax > ratioBitmap)
                {
                    finalWidth = (int)((float)maxHeight * ratioBitmap);
                }
                else
                {
                    finalHeight = (int)((float)maxWidth / ratioBitmap);
                }
                image = bitmapResize(image, finalWidth, finalHeight);
                return image;
            }
            else
            {
                return image;
            }
        }

        public static Bitmap bitmapResize(Bitmap image, int width, int height)
        {
            float scale = Math.Min(width / image.Width, height / image.Height);
            var brush = new SolidBrush(Color.Pink);

            var bmp = new Bitmap((int)width, (int)height);
            using (var graph = Graphics.FromImage(bmp))
            {

                // uncomment for higher quality output
                //graph.InterpolationMode = InterpolationMode.High;
                //graph.CompositingQuality = CompositingQuality.HighQuality;
                //graph.SmoothingMode = SmoothingMode.AntiAlias;

                var scaleWidth = (int)(image.Width * scale);
                var scaleHeight = (int)(image.Height * scale);

                graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
                graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
            }

            return bmp;
        }

    }
}
