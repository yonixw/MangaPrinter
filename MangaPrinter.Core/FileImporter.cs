using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MangaPrinter.Core
{
    public class FileImporter
    {
        public MangaPage getMangaPageFromPath(FileInfo fiImage, int cutoff)
        {
            MangaPage page = new MangaPage()
            {
                Name = Path.GetFileNameWithoutExtension(fiImage.Name),
                ImagePath = fiImage.FullName
            };

            using (FileStream file = new FileStream(page.ImagePath, FileMode.Open, FileAccess.Read))
            {
                using (Image jpeg = Image.FromStream(stream: file,
                                                    useEmbeddedColorManagement: false,
                                                    validateImageData: false))
                {
                    //float width = jpeg.PhysicalDimension.Width;
                    //float height = jpeg.PhysicalDimension.Height;
                    //float hresolution = jpeg.HorizontalResolution;
                    //float vresolution = jpeg.VerticalResolution;
                    page.IsDouble = jpeg.PhysicalDimension.Width > cutoff;
                }
            }

            return page;
        }

        public List<MangaChapter> getChapters(DirectoryInfo di, bool subFodlers, int pageCutoff, bool RTL,
            Action<string, int> updateFunc = null)
        {
            List<MangaChapter> result = new List<MangaChapter>();

            if (di.Exists)
            {
                MangaChapter ch = new MangaChapter()
                {
                    Pages = new List<MangaPage>(),
                    Name = di.Name,
                    IsRTL = RTL
                };

                foreach (FileInfo fi in di.EnumerateFiles("*.jpg"))
                {
                    if (updateFunc != null)
                        updateFunc(fi.Directory.Name + "/" + fi.Name, 0);
                    try
                    {
                        ch.Pages.Add(getMangaPageFromPath(fi, pageCutoff));
                    } catch (Exception ex) { Debug.Print(ex.ToString()); }
                }

                if (ch.Pages.Count > 0)
                    result.Add(ch);

                if (subFodlers)
                {
                    foreach (DirectoryInfo subdi in di.EnumerateDirectories())
                    {
                        List<MangaChapter> subdiChapters = getChapters(subdi, subFodlers, pageCutoff, RTL);
                        result.AddRange(subdiChapters);
                    }
                }
            }

            return result;
        }

        public List<MangaChapter> getChapters(string DirectoryPath, bool subFodlers, int pageCutoff, bool isRTL,
            Action<string,int> updateFunc = null)
        {
            return getChapters(new DirectoryInfo(DirectoryPath), subFodlers, pageCutoff, isRTL, updateFunc);
        }

    }
}
