using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core
{
    public class MagickImaging
    {
        public static void Convert(List<string> images, string pdfPath, string tempFolder, uint MemoryMBLimit = 500)
        {
            if (images == null) return;

            ImageMagick.ResourceLimits.Memory = MemoryMBLimit * 1024 * 1024;

            // When reading PDF : 
            //     MagickNET.SetGhostscriptDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //     https://github.com/dlemstra/Magick.NET/tree/master/Documentation#ghostscript
            //     get files from extracting installer (64 and 32 bit) from official ghostscript site 

            MagickNET.SetTempDirectory(tempFolder);

            using (var combined = new MagickImageCollection())
            {
                foreach (string imagepath in images)
                {
                    ImageMagick.MagickImage img = new MagickImage(imagepath);
                    combined.Add(img);
                }
                combined.Write(pdfPath);
            }

            
        }
    }
}
