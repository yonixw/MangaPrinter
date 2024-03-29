﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public class CoreConf
    {
        public static CoreConf I = new CoreConf(); // To other point here, and get hard types

        /* ===============  Information             =============== */

        public JMetaT<string> Info_GitVersion { get; } = new JMetaT<string>(
            Properties.Resources.GitInfo.Replace("\"", "").Split(';')[0], // remove surrounding qoutes and ignore commit msg
            "Version of the software on build time", _readonly: true
        );



        public const int CURR_CONFIG_MAJOR_VERSION = 1; // Breaking!    (change, delete)
        public const int CURR_CONFIG_MINOR_VERSION = 1; // Not breaking (add only)

        public JMetaT<int> Info_ConfigVersionMajor { get; } = new JMetaT<int>(CURR_CONFIG_MAJOR_VERSION,
           "Major Version of the config, which changes on breaking changes"
        );

        public JMetaT<int> Info_ConfigVersionMinor { get; } = new JMetaT<int>(CURR_CONFIG_MINOR_VERSION,
           "Minor Version of the config, which changes when new config options are added"
        );

        public JMetaT<bool> Info_IsNotWindows { get; } = new JMetaT<bool>(Process.GetProcessesByName("winlogon").Length == 0,
          "Is the program running under linux", _readonly: true
       );

        /* ===============  Common                 =============== */

        public JMetaT<float> Common_PreviewBlurPrcnt { get; } = new JMetaT<float>(100,
            "Default Preview Blur Percent, from 0 (none) to 100",
            (P) => P >= 0 && P <= 100
        );

        public JMetaT<bool> Common_RenderImageBorder { get; } = new JMetaT<bool>(false,
           "Add black border for any image, good for troubleshooting import"
        );

        public JMetaT<bool> Common_RenderPaintAreaBorder { get; } = new JMetaT<bool>(false,
           "Add red border for painting area, good for troubleshooting import"
        );

        public JMetaT<int> Common_MaxPreviewBlurRadius { get; } = new JMetaT<int>(50,
            "Default Preview Blur radius (pixels) bigger than 10",
            (N) => N >= 10
        );

        public JMetaT<float> Common_Alerts_InkFillLow { get; } = new JMetaT<float>(0.1f, // xaml converters expect float
           "InkFill low threshold, 0 (all ink) to 1 (no ink, blank page)",
           (P) => P >= 0 && P <= 1.1f
        );

        public JMetaT<float> Common_Alerts_InkFillHigh { get; } = new JMetaT<float>(0.9f, // xaml converters expect float
          "InkFill high threshold, 0 (all ink) to 1 (no ink, blank page)",
          (P) => P >= 0 && P <= 1.1f
       );

        public JMetaT<float> Common_Alerts_TooVertical { get; } = new JMetaT<float>(0.6f, // xaml converters expect float
          "Too vertical alert, value of (Width/Height)",
          (P) => P >= 0
       );

        public JMetaT<float> Common_Alerts_PageCountLow { get; } = new JMetaT<float>(10f, // xaml converters expect float
          "Chapter pages, low threshold",
          (P) => P >= 0 
       );

        public JMetaT<float> Common_Alerts_PageCountHigh1 { get; } = new JMetaT<float>(25f, // xaml converters expect float
          "Chapter pages, high but expected threshold",
          (P) => P >= 0
       );

        public JMetaT<float> Common_Alerts_PageCountHigh2 { get; } = new JMetaT<float>(35f, // xaml converters expect float
          "Chapter pages, high but unexpected threshold",
          (P) => P >= 0
       );

        /* ===============  Window                 =============== */

        public JMetaT<string> Window_StartMode { get; } = new JMetaT<string>("",
            "Start position mode of the main GUI window."
        ).Listify(new CHList<string>("centered", "fixed"));

        public JMetaT<JSize> Window_LocationSize { get; } = new JMetaT<JSize>(new JSize() { X=0,Y=0, Width=1000, Height=900},
            "Start position and size of the main GUI window. Position only if mode='fixed'.",
            (S) => S.Height > 0 && S.Width > 0
        );

        // Window_FontSize / Window_FontName
        //      - Won't do - No way to change all at once... need to change each componenet

        /* ===============  Chapter Step                     =============== */

        public JMetaT<float> Chapters_DblPgRatioCuttof { get; } = new JMetaT<float>(1.15f,
         "Ratio of width/height to decide if an image is a double page, makes sense for >1.0",
         (F) => F > 0.0f
       );

        public JMetaT<string> Chapters_SortImportBy { get; } = new JMetaT<string>("",
            "How to sort files and folders"
        ).Listify(new CHList<string>("by_name", "by_create_date"));

        public JMetaT<bool> Chapters_SmartNumberImport { get; } = new JMetaT<bool>(true,
            new[] {
                "Smart sort numbers in file names, otherwise 2.jpg comes before 11.jpg",
                "Only if import sort is by_name"
            }
        );

        public JMetaT<bool> Chapters_ImportSubfolders { get; } = new JMetaT<bool>(true,
            "Import subfolders recursively"
        );

        public JMetaT<string> Chapters_ChapDir { get; } = new JMetaT<string>("",
            "Default direction of new chapters. Japanese stuff is rtl (right-to-left) usually."
        ).Listify(new CHList<string>("rtl", "ltr"));

        public JMetaT<string> Chapters_ImportDir { get; } = new JMetaT<string>(@"%USERPROFILE%\Documents\Mangas\",
           "Default location to point to when importing. Good if you want to point to HakuNeko folder, Empty for OS default",
           (T) => T != null
        );

        /* ===============  Binding  Common                   =============== */

        public JMetaT<string> Binding_Type { get; } = new JMetaT<string>("",
           "Binding method type"
        ).Listify(new CHList<string>("duplex", "booklet"));

        public JMetaT<bool> Binding_AddStartPage { get; } = new JMetaT<bool>(true,
           "Add page before every chapter with text"
        );

        public JMetaT<bool> Binding_AddEndPage { get; } = new JMetaT<bool>(true,
          "Add page after every chapter with text"
        );

        public JMetaT<int> Binding_AntiSpoilerBatch { get; } = new JMetaT<int>(26,
           "How many pages to print between Anti-spoiler pages. 0=No anti spoiler at all",
           (N) => N >= 0
         );

        public JMetaT<bool> Binding_AddParentFolder { get; } = new JMetaT<bool>(true,
           new[]{"Add parent folder to chapter name","" +
               "Works when the folder structure is Name>Chapter000>Page000.jpg, and you import top folder." }
        );

        public JMetaT<bool> Binding_SkipGrayscale { get; } = new JMetaT<bool>(false,
           "Don't convert images to black and white. Some printers can't print those files because they become too big."
        );

        public JMetaT<bool> Binding_SkipPDF { get; } = new JMetaT<bool>(false,
           "Don't combine exported images into PDF"
        );

        public JMetaT<bool> Binding_Booklet_Cover { get; } = new JMetaT<bool>(false,
           "Keep first/last pages as cover"
        );

        public JMetaT<bool> Binding_Booklet_ExportSingles { get; } = new JMetaT<bool>(false,
           "Export singles for booklet"
        );

        public JMetaT<bool> Binding_Booklet_MirrorSnd { get; } = new JMetaT<bool>(false,
           "Mirror every 2nd page"
        );

        public JMetaT<string> Binding_Booklet_RTL { get; } = new JMetaT<string>("",
           "Booklet binding direction"
       ).Listify(new CHList<string>("rtl","ltr"));

        public JMetaT<List<JPage>> Binding_PageSizeList { get; } = new JMetaT<List<JPage>>(
            new List<JPage>() { 
                new JPage() { Name="Tailored A4 150DPI", HeightPixels=1266, WidthPixels=1648, TargetDensity=150 },
                new JPage() { Name="A4 150DPI", HeightPixels=1240, WidthPixels=1754, TargetDensity=150 },
                new JPage() { Name="A4 300DPI", HeightPixels=2480, WidthPixels=3508, TargetDensity=300 },
                new JPage() { Name="Tailored A3 150DPI Landscape", HeightPixels=824, WidthPixels=633, TargetDensity=150 },
                new JPage() { Name="A3 150DPI Landscape", HeightPixels=877, WidthPixels=620, TargetDensity=150 },
            },
           "List of available pages size, first is default",
           (L) => L != null && L.Count > 0 && L.TrueForAll(P => P.HeightPixels > 0 && P.WidthPixels > 0)
        );


        /* ===============  Templates -> Shared     =============== */

        public JMetaT<bool> Templates_ShowDirectionArrows { get; } = new JMetaT<bool>(true,
           "Show reading direction arrows in each page, on the top paddings"
        );

        public JMetaT<bool> Templates_ShowBorders { get; } = new JMetaT<bool>(true,
           "Black border around each page/image"
        );

        public JMetaT<bool> Templates_AddSwName { get; } = new JMetaT<bool>(true,
           "Add software name to text templates"
        );

        public JMetaT<float> Templates_PaddingPrcnt { get; } = new JMetaT<float>(2.2f,
          "Padding of the 4 outer direction, in precent, >= 0, around the main content",
          (F) => F > 0.0
        );

        public JMetaT<int> Templates_MaxCharPerLine { get; } = new JMetaT<int>(80,
           "Max character before breaking into new line, >= 0",
           (N) => N >= 0
         );

        public JMetaT<int> Templates_MaxValueLength { get; } = new JMetaT<int>(70,
           "Max character before clipping any value, If can't break into lines, >= 0",
           (N) => N >= 0
         );

        public JMetaT<string> Templates_TextFont { get; } = new JMetaT<string>("Arial",
           "Font to use on templates, might crash if not installed on OS", CH.stringy
        );

        // TODO: every list of options, add to description

        // TODO: list of all available meta replace !PNAME! (chapter, parent, page number?, batch number A-S?)

        public JMetaT<string> Templates_MetaText_Structure { get; } = new JMetaT<string>("{0} > {1} > {2}",
          "How to write the structure of the folders/chapters", CH.stringy
        );

        public JMetaT<string> Templates_MetaText_PageNum { get; } = new JMetaT<string>("Page {0}",
          "How to write the page number", CH.stringy
        );

        public JMetaT<string> Templates_MetaText_AntiSpoilerNum { get; } = new JMetaT<string>("(Anti Spoiler {0})",
          "Number of pages relative to its anti spoiler batch", CH.stringy
        );


        /* ===============  Templates (Duplex oriented)  =============== */

        public JMetaT<string> Templates_Render_Omited { get; } = new JMetaT<string>("(Manually Omited)",
           "Text template to use when ommiting files", CH.stringy
        );

        public JMetaT<bool> Templates_Render_AddGutter { get; } = new JMetaT<bool>(true,
           "Add a padding between two singles"
        );

        public JMetaT<string> Templates_Render_Intro { get; } = new JMetaT<string>("Chapter start:\n{0}",
           "Text template to use on chapter start", CH.stringy
        );

        public JMetaT<string> Templates_Render_Outro { get; } = new JMetaT<string>("Chapter end:\n{0}",
           "Text template to use on chapter end", CH.stringy
        );

        public JMetaT<string> Templates_Render_BeforeDouble { get; } = new JMetaT<string>("Filler Before Double",
           "Text template to use when adjusting double-page", CH.stringy
        );

        public JMetaT<string> Templates_Render_MakeEven { get; } = new JMetaT<string>("Filler After Chapter",
           "Text template to use when adjusting chapter even length", CH.stringy
        );

        public JMetaT<string> Templates_Render_AntiSpoiler { get; } = new JMetaT<string>("Anti Spoiler",
           "Text template to use for anti spoiler pages", CH.stringy
        );


    }
}
