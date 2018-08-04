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
        public static string ImagesExtensions =
            "*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.jpeg;*.tiff";



        public MangaPage getMangaPageFromPath(FileInfo fiImage, int cutoff)
        {
            MangaPage page = new MangaPage()
            {
                Name = Path.GetFileNameWithoutExtension(fiImage.Name),
                ImagePath = fiImage.FullName
            };

            using (FileStream file = new FileStream(page.ImagePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (Image jpeg = Image.FromStream(stream: file,
                                                                useEmbeddedColorManagement: false,
                                                                validateImageData: false))
                    {
                        page.IsDouble = jpeg.PhysicalDimension.Width > cutoff;
                    }
                }
                catch (OutOfMemoryException ex) { } // Not valid image or not supported
            }

            return page;
        }

        public List<MangaChapter> getChapters(DirectoryInfo di, bool subFodlers, int pageCutoff, bool RTL,
            Func<FileSystemInfo, object> orderFunc, Action<string, int> updateFunc = null)
        {
            List<MangaChapter> result = new List<MangaChapter>();

            if (di.Exists)
            {
                MangaChapter ch = new MangaChapter()
                {
                    Pages = new System.Collections.ObjectModel.ObservableCollection<MangaPage>(),
                    Name = di.Name,
                    IsRTL = RTL
                };

                foreach (FileInfo fi in di.EnumerateFiles("*.*").OrderBy(orderFunc))
                {
                    if (!ImagesExtensions.Contains(fi.Extension))
                        continue;

                    if (updateFunc != null)
                        updateFunc(fi.Directory.Name + "/" + fi.Name, 0);
                    try
                    {
                        ch.Pages.Add(getMangaPageFromPath(fi, pageCutoff));
                    }
                    catch (Exception ex) { Debug.Print(ex.ToString()); }
                }

                if (ch.Pages.Count > 0)
                {
                    result.Add(ch);
                }

                if (subFodlers)
                {
                    foreach (DirectoryInfo subdi in di.EnumerateDirectories().OrderBy(orderFunc))
                    {
                        List<MangaChapter> subdiChapters = getChapters(subdi, subFodlers, pageCutoff, RTL, orderFunc, updateFunc);
                        result.AddRange(subdiChapters);
                    }
                }
            }

            return result;
        }

        public List<MangaChapter> getChapters(string DirectoryPath, bool subFodlers, int pageCutoff, bool isRTL,
            Func<FileSystemInfo, object> orderFunc, Action<string, int> updateFunc = null)
        {
            return getChapters(new DirectoryInfo(DirectoryPath), subFodlers, pageCutoff, isRTL, orderFunc, updateFunc);
        }

        public List<MangaPage> importImages(string[] imagePaths, int pageCutoff,
            Func<FileSystemInfo, object> orderFunc, Action<string, int> updateFunc = null)
        {
            List<MangaPage> result = new List<MangaPage>();

            for (int pageIndex = 0; pageIndex < imagePaths.Length; pageIndex++)
            {
                FileInfo fi = new FileInfo(imagePaths[pageIndex]);

                if (ImagesExtensions.Contains(fi.Extension))
                {

                    if (updateFunc != null)
                        updateFunc(fi.Directory.Name + "/" + fi.Name, (int)(100.0f * pageIndex / imagePaths.Length));
                    try
                    {
                        result.Add(getMangaPageFromPath(fi, pageCutoff));
                    }
                    catch (Exception ex) { Debug.Print(ex.ToString()); }

                }
            };

            return result;
        }
    }
}
