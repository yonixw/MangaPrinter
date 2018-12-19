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
        public static void Convert(List<string> images, string pdfPath, string tempFolder)
        {
            if (images == null) return;

            // When reading PDF : 
            //     MagickNET.SetGhostscriptDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //     https://github.com/dlemstra/Magick.NET/tree/master/Documentation#ghostscript
            //     get files from extracting installer (64 and 32 bit) from official ghostscript site 

            MagickNET.SetTempDirectory(tempFolder);

            var combined = new MagickImageCollection();
            foreach (string imagepath in images)
            {
                combined.Add(imagepath);
            }
            combined.Write(pdfPath);
        }
    }
}
