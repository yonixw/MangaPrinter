using ImageMagick;
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

        public void MakeList(List<string> images, string tempFolder, uint MemoryMBLimit = 500, Action<int> updateIndex = null)
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
                collection.Add(img);

                updateIndex?.Invoke(counter);
                counter = counter + 1;
            }          
            
        }

        public void SaveListToPdf(string pdfPath)
        {
            collection?.Write(pdfPath);
        }

        public static Bitmap BitmapFromUrlExt(string path)
        {
            Bitmap result = null;
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
            return result;
        }
    }
}
