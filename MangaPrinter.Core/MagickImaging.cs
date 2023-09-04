using ImageMagick;
using MangaPrinter.Core.TemplateBuilders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core
{
    public class MagickImaging : IDisposable
    {
        MagickImageCollection collection;

        public void Dispose()
        {
            collection?.Dispose();
        }

        public void MakeList(List<string> images, string tempFolder, int dpi, uint MemoryMBLimit = 500, Action<int> updateIndex = null)
        {
            if (images == null) return;

            ImageMagick.ResourceLimits.Memory = MemoryMBLimit * 1024 * 1024;

            // When reading PDF : 
            //     MagickNET.SetGhostscriptDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //     https://github.com/dlemstra/Magick.NET/tree/master/Documentation#ghostscript
            //     get files from extracting installer (64 and 32 bit) from official ghostscript site 

            MagickNET.SetTempDirectory(tempFolder);

            collection = new MagickImageCollection();
            int counter = 0;
            foreach (string imagepath in images)
            {
                ImageMagick.MagickImage img = new MagickImage(imagepath);
                img.Density = new Density(dpi);
                collection.Add(img);

                updateIndex?.Invoke(counter);
                counter = counter + 1;
            }          
            
        }

        public void SaveListToPdf(string pdfPath)
        {
            collection?.Write(pdfPath);
        }

        public class DummyImages
        {
            public const string White200Sqr = "/dummy/white";
        }

        public static void setDefaultRes(Bitmap b)
        {
            using (Bitmap def = new Bitmap(10,10))
            {
                b.SetResolution(def.HorizontalResolution, def.VerticalResolution); 
            }
        }

        public static Bitmap BitmapFromUrlExt(MangaPage page)
        {
            Bitmap result = null;

            if (page.Effects != null && !String.IsNullOrEmpty(page.Effects.VirtualPath))
            {
                if (page.Effects.VirtualPath == DummyImages.White200Sqr)
                {
                    result = new Bitmap(200, 200);
                    using (Graphics g = Graphics.FromImage(result))
                    {
                        g.FillRectangle(Brushes.White, new RectangleF(0, 0, 200, 200));
                    }
                    return result;
                }

                throw new Exception("Unkown dummy: " + page.Effects.VirtualPath);
            }

            string path = page.ImagePath;
            string extLower = Path.GetExtension(path).ToLower();
            if (FileImporter.BitmapSupportedImagesExtensions.Contains(extLower))
            {
                result = (Bitmap)Bitmap.FromFile(path);
            }
            else if (FileImporter.MagickSupportedImagesExtentions.Contains(extLower))
            {
                using (MagickImage img = new MagickImage(path))
                {
                    result = img.ToBitmap();
                }
            }

            setDefaultRes(result);

            if (page.Effects != null)
            {
                if (
                    page.Effects.CropBottom > 0 ||
                    page.Effects.CropTop > 0 ||
                    page.Effects.CropLeft > 0 ||
                    page.Effects.CropRight > 0 
                    )
                    result = GraphicsUtils.bitmapCrop(result, page.Effects, reuse: false);

                if(page.Effects.Brightness != 1.0f ||
                    page.Effects.Contrast != 1.0f ||
                    page.Effects.Gamma != 1.0f )
                    result = GraphicsUtils.bitmapLighting(result, page.Effects, reuse: false);
            }

            return result;
        }

        public static float WhiteRatio(Bitmap bmp)
        {
            //https://stackoverflow.com/a/21406312/1997873
            int whiteColor = 0;
            int blackColor = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color color = bmp.GetPixel(x, y);
                    if (color.ToArgb() == Color.White.ToArgb())
                    {
                        whiteColor++;
                    }
                }
            }
            return whiteColor > 0?  (whiteColor * 1f) / (bmp.Width*bmp.Height) : 0;
        }
    }
}
