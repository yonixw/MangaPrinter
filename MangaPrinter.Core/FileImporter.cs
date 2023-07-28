using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ImageMagick;
using System.Text.RegularExpressions;

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
        public static string BitmapSupportedImagesExtensions =
            "*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.jpeg;*.tiff;";
        public static string MagickSupportedImagesExtentions =
            "*.webp;";

        //https://blog.filestack.com/thoughts-and-knowledge/complete-image-file-extension-list/
        //http://archive.is/ml1sM
        public static string AllImageExtensions =
            ".jpg;.jpeg;.jpe;jif;.jfif;.jfi;.png;.gif;.webp;.tiff;.tif;" +
            ".psd;.raw;.arw;.cr2;.nrw;.k25;.bmp;.dib;.heif;.heic;.ind;.indd;.indt;" +
            ".jp2;.j2k;.jpf;.jpx;.jpm;.mj2;.svg;.svgz;.ai;.eps;.pdf;";

        static bool checkFileSupported(FileInfo file, List<FileImporterError> errorPages)
        {
            string lowerExt = file.Extension.ToLower();
            bool supported = BitmapSupportedImagesExtensions.Contains(lowerExt) || MagickSupportedImagesExtentions.Contains(lowerExt);
            if (!supported && AllImageExtensions.Contains(lowerExt)) {
                errorPages?.Add(new FileImporterError(file: file, reason: "Image extention '" + lowerExt + "' not supported"));
            }
            return supported;
        }

        const string numberPattern = "(?:^|[^\\.0-9]|[^0-9]\\.)([0-9]+)((?:\\.[0-9]+)*)";
        static Regex fixNumberRegex = new Regex(numberPattern,RegexOptions.Compiled);
        public static string pad0AllNumbers(string input)
        {
            string result = input;
            int resultAddonsOffset = 0;
            try
            {
                foreach (Match match in fixNumberRegex.Matches(input))
                {
                    var g1 = match.Groups[1]; // main number
                    var g2 = match.Groups.Count > 2 ? match.Groups[2] : null; // all after decimal
                    int newPosition = g1.Index + resultAddonsOffset;
                    int actualLength = g1.Length + (g2?.Length ?? 0);
                    int oldResultLen = result.Length;
                    result =
                        (newPosition > 0 ? result.Substring(0, newPosition ) : "")
                        + g1.Value.PadLeft(7, '0') + (g2?.Value ?? "")
                        + (newPosition + actualLength > input.Length ? "" : result.Substring(newPosition + actualLength));
                    resultAddonsOffset += result.Length - oldResultLen;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
            }
            return result;
        }

        public static string pad0AllNumbersForCompareOnly(string input)
        {
            string result = "0";
            int resultAddonsOffset = 0;
            try
            {
                foreach (Match match in fixNumberRegex.Matches(input))
                {
                    var g1 = match.Groups[1]; // main number
                    var g2 = match.Groups.Count > 2 ? match.Groups[2] : null; // all after decimal
                    result += g1.Value.PadLeft(7, '0') + (g2?.Value ?? "") + ";";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
            }
            return result;
        }

        public MangaPage getMangaPageFromPath(FileInfo fiImage, float cutoff, List<FileImporterError> errorPages)
        {
            MangaPage resultPage = null;

            using (FileStream file = new FileStream(fiImage.FullName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    MangaPage page = new MangaPage()
                    {
                        Name = Path.GetFileNameWithoutExtension(fiImage.Name),
                        AspectRatio = MangaPage.MinRatio, // Must be > 0, o.w. Infinity!
                        WhiteBlackRatio = 0.5f, // No too much, no too little until scan.
                        ImagePath = fiImage.FullName,
                        IsDouble = false, Effects = new PageEffects()
                    };
                    if (MagickSupportedImagesExtentions.Contains(fiImage.Extension.ToLower()))
                    {
                        using (MagickImage webP = new MagickImage(file))
                        {
                            page.AspectRatio =
                                webP.Height > 0 ?
                                1.0f * webP.Width  / webP.Height :
                                MangaPage.MinRatio;
                            page.IsDouble = page.AspectRatio >= cutoff;
                        }

                    }
                    else
                    {
                        // We dont use BitmapFromUrlExt() because we can read only metadata for simple types
                        using (Image jpeg = Image.FromStream(stream: file,
                                                                    useEmbeddedColorManagement: false,
                                                                    validateImageData: false))
                        {
                            page.AspectRatio =
                                jpeg.PhysicalDimension.Height > 0 ?
                                1.0f * jpeg.PhysicalDimension.Width / jpeg.PhysicalDimension.Height :
                                MangaPage.MinRatio;
                            page.IsDouble = page.AspectRatio >= cutoff;
                        }
                    }
                    resultPage = page;
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

            return resultPage;
        }

        public List<MangaChapter> getChapters(DirectoryInfo di, bool subFodlers, float pageCutoff, bool RTL, 
            Func<FileSystemInfo, object> orderFunc, List<FileImporterError> errorPages, Action<string, int> updateFunc = null)
        {
            List<MangaChapter> result = new List<MangaChapter>();

            if (di.Exists)
            {
                MangaChapter ch = new MangaChapter()
                {
                    Name = di.Name, ParentName = ( di.Parent?.Name ?? ""),
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
                        var page = getMangaPageFromPath(fi, pageCutoff, errorPages);
                        if (page != null)
                            ch.Pages.Add(page);
                    }
                    catch (Exception ex) { Debug.Print(ex.ToString()); }
                }

                ch.autoPageNumbering = true;
                ch.updateChapterStats();

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
                        var page = getMangaPageFromPath(fi, pageCutoff, errorPages);
                        if (page != null)
                            result.Add(page);
                    }
                    catch (Exception ex) { Debug.Print(ex.ToString()); }

                }
            };

            return result;
        }
    }
}
