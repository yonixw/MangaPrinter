using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MangaPrinter.Core
{
    public class FileImporterError
    {
        public FileInfo fileObj =null;
        public Exception exObject = null;
        public string reasonString = "";

        public override string ToString()
        {
            return (reasonString == "") ? "Empty reason" : reasonString;
        }

        public FileImporterError(FileInfo file=null, Exception ex=null, string reason="")
        {
            fileObj = file;
            ex = exObject;
            reasonString = reason;
        }
    }

    public class FileImporter
    {
        public static string SupportedImagesExtensions =
            "*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.jpeg;*.tiff";

        //https://blog.filestack.com/thoughts-and-knowledge/complete-image-file-extension-list/
        //http://archive.is/ml1sM
        public static string AllImageExtensions =
            ".jpg;.jpeg;.jpe;jif;.jfif;.jfi;.png;.gif;.webp;.tiff;.tif;" +
            ".psd;.raw;.arw;.cr2;.nrw;.k25;.bmp;.dib;.heif;.heic;.ind;.indd;.indt;" +
            ".jp2;.j2k;.jpf;.jpx;.jpm;.mj2;.svg;.svgz;.ai;.eps;.pdf;";

        static bool checkFileSupported(FileInfo file, List<FileImporterError> errorPages)
        {
            string lowerExt = file.Extension.ToLower();
            bool supported = SupportedImagesExtensions.Contains(lowerExt);
            if (!supported && AllImageExtensions.Contains(lowerExt)) {
                errorPages?.Add(new FileImporterError(file: file, reason: "Image extention '" + lowerExt + "' not supported"));
            }
            return supported;
        }

        public MangaPage getMangaPageFromPath(FileInfo fiImage, float cutoff, List<FileImporterError> errorPages)
        {
            MangaPage page = new MangaPage()
            {
                Name = Path.GetFileNameWithoutExtension(fiImage.Name),
                ImagePath = fiImage.FullName,
                IsDouble = false
            };

            using (FileStream file = new FileStream(page.ImagePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (Image jpeg = Image.FromStream(stream: file,
                                                                useEmbeddedColorManagement: false,
                                                                validateImageData: false))
                    {
                        page.AspectRatio =
                            jpeg.PhysicalDimension.Height > 0 ?
                            jpeg.PhysicalDimension.Width / jpeg.PhysicalDimension.Height :
                            0;
                        page.IsDouble = page.AspectRatio >= cutoff;
                    }
                }
                catch (OutOfMemoryException ex) {
                    // Not valid image or not supported
                    errorPages?.Add(new FileImporterError(file: fiImage, ex: ex, reason: "Can't load image file. Invalid Format?"));
                } 
                catch (Exception ex)
                {
                    errorPages?.Add(new FileImporterError(file: fiImage, ex: ex, reason: "Can't load image file."));
                }
            }

            return page;
        }

        public List<MangaChapter> getChapters(DirectoryInfo di, bool subFodlers, float pageCutoff, bool RTL, 
            Func<FileSystemInfo, object> orderFunc, List<FileImporterError> errorPages, Action<string, int> updateFunc = null)
        {
            List<MangaChapter> result = new List<MangaChapter>();

            if (di.Exists)
            {
                MangaChapter ch = new MangaChapter()
                {
                    Name = di.Name,
                    IsRTL = RTL,
                    autoPageNumbering = false
                };

                foreach (FileInfo fi in di.EnumerateFiles("*.*").OrderBy(orderFunc))
                {
                    if (!checkFileSupported(fi, errorPages))
                        continue;

                    updateFunc?.Invoke(fi.Directory.Name + "/" + fi.Name, 0);
                    try
                    {
                        ch.Pages.Add(getMangaPageFromPath(fi, pageCutoff,errorPages));
                    }
                    catch (Exception ex) { Debug.Print(ex.ToString()); }
                }

                ch.autoPageNumbering = true;
                ch.updatePageNumber();

                if (ch.Pages.Count > 0)
                {
                    result.Add(ch);
                }


                if (subFodlers)
                {
                    foreach (DirectoryInfo subdi in di.EnumerateDirectories().OrderBy(orderFunc))
                    {
                        List<MangaChapter> subdiChapters = getChapters(subdi, subFodlers, pageCutoff, RTL, orderFunc, errorPages, updateFunc);
                        result.AddRange(subdiChapters);
                    }
                }
            }

            return result;
        }

        public List<MangaChapter> getChapters(string DirectoryPath, bool subFodlers, float pageCutoff, bool isRTL, 
            Func<FileSystemInfo, object> orderFunc, List<FileImporterError> errorPages, Action<string, int> updateFunc = null)
        {
            return getChapters(new DirectoryInfo(DirectoryPath), subFodlers, pageCutoff, isRTL,  orderFunc, errorPages, updateFunc);
        }

        public List<MangaPage> importImages(string[] imagePaths, float pageCutoff,
            Func<FileSystemInfo, object> orderFunc, List<FileImporterError> errorPages, Action<string, int> updateFunc = null)
        {
            List<FileSystemInfo> files = new List<FileSystemInfo>();
            List<MangaPage> result = new List<MangaPage>();


            foreach (string path in imagePaths)
            {
                FileInfo fi = new FileInfo(path);
                if (checkFileSupported(fi, errorPages))
                {
                    files.Add(fi);
                }
            }

            files = files.OrderBy(orderFunc).ToList();

            int pageCount = files.Count();
            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                FileInfo fi = new FileInfo(imagePaths[pageIndex]);

                if (checkFileSupported(fi, errorPages))
                {

                    updateFunc?.Invoke(fi.Directory.Name + "/" + fi.Name, (int)(100.0f * pageIndex / pageCount));
                    try
                    {
                        result.Add(getMangaPageFromPath(fi, pageCutoff, errorPages));
                    }
                    catch (Exception ex) { Debug.Print(ex.ToString()); }

                }
            };

            return result;
        }
    }
}
