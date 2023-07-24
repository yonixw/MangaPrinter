using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public class CoreConf
    {
        public static CoreConf I = new CoreConf(); // To other point here, and get hard types

        /* ===============  Information             =============== */

        public JMetaT<string> Info_GitVersion { get; } = new JMetaT<string>("",
            "Version of the software on build time", _readonly: true
        );

        public const int CURR_CONFIG_MAJOR_VERSION  = 1; // Breaking!    (change, delete)
        public const int CURR_CONFIG_MINOR_VERSION = 1; // Not breaking (add only)

        public JMetaT<int> Info_ConfigVersionMajor { get; } = new JMetaT<int>(CURR_CONFIG_MAJOR_VERSION,
           "Major Version of the config, which changes on breaking changes", _readonly: true
        );

        public JMetaT<int> Info_ConfigVersionMinor { get; } = new JMetaT<int>(CURR_CONFIG_MINOR_VERSION,
           "Minor Version of the config, which changes when new config options are added", _readonly: true
        );

        /* ===============  Common                 =============== */

        public JMetaT<float> Common_PreviewBlurPrcnt { get; } = new JMetaT<float>(100,
            "Default Preview Blur Percent, from 0 (none) to 100",
            (P) => P >= 0 && P <= 100
        );

        public JMetaT<int> Common_MaxPreviewBlurRadius { get; } = new JMetaT<int>(40,
            "Default Preview Blur radius (pixels) bigger than 10",
            (N) => N >= 10
        );

        /* ===============  Window                 =============== */

        public JMetaT<string> Window_StartMode { get; } = new JMetaT<string>("centered",
            "Start position mode of the main GUI window. Can be 'centered' or 'fixed' ",
            (T) => CH.inListP(T, "centered", "fixed")
        );

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

        public JMetaT<string> Chapters_SortImportBy { get; } = new JMetaT<string>("by_name",
            "How to sort files and folders: by_name or by_create_date",
            (T) => CH.inListP(T, "by_name", "by_create_date")
        );

        public JMetaT<bool> Chapters_SmartNumberImport { get; } = new JMetaT<bool>(true,
            new[] {
                "Smart sort numbers in file names, otherwise 2.jpg comes before 11.jpg",
                "Only if import sort is by_name"
            }
        );

        public JMetaT<bool> Chapters_ImportSubfolders { get; } = new JMetaT<bool>(true,
            "Import subfolders recursively"
        );

        public JMetaT<string> Chapters_ChapDir { get; } = new JMetaT<string>("rtl",
            "Default direction of new chapters. Japanese stuff is rtl (right-to-left) usually.",
            (T) => CH.inListP(T, "rtl", "ltr")
        );

        public JMetaT<string> Chapters_ImportDir { get; } = new JMetaT<string>("",
           "Default location to point to when importing. Good if you want to point to HakuNeko folder, Empty for OS default",
           (T) => T != null
       );

        public JMetaT<string> Chapters_SmartScan { get; } = new JMetaT<string>("only3",
           "Default mode for smart scan, still will need user accepting the dialog",
           (T) => CH.inListP(T, "only3", "all", "none")
        );

        /* ===============  Binding  Common                   =============== */

        public JMetaT<string> Binding_Type { get; } = new JMetaT<string>("duplex",
           "Binding method type",
           (T) => CH.inListP(T, "duplex", "booklet_single", "booklet_print")
        );

        public JMetaT<bool> Binding_AddStartPage { get; } = new JMetaT<bool>(true,
           "Add page before every chapter with text"
        );

        public JMetaT<bool> Binding_AddEndPage { get; } = new JMetaT<bool>(true,
          "Add page after every chapter with text"
        );

        public JMetaT<int> Binding_AniSpoilerBatch { get; } = new JMetaT<int>(25,
           "How many pages to print between Anti-spoiler pages. 0=No anti spoiler at all",
           (N) => N >= 0
         );

        public JMetaT<bool> Binding_AddParentFolder { get; } = new JMetaT<bool>(true,
           "Works when the folder structure is Name>Chapter000>Page000.jpg, and you import top folder."
        );

        public JMetaT<bool> Binding_KeepColors { get; } = new JMetaT<bool>(false,
           "Don't convert images to black and white. Some printers can't print those files because they become too big."
        );

        public JMetaT<bool> Binding_ExportImages { get; } = new JMetaT<bool>(false,
           "Don't combine exported images into PDF"
        );

        public JMetaT<List<JPage>> Binding_PageSizeList { get; } = new JMetaT<List<JPage>>(
            new List<JPage>() { 
                new JPage() { name="REPLACE_ME", WidthPixels=1, HeightPixels=1 } 
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

        public JMetaT<JPadding> Templates_PaddingPrcnt { get; } = new JMetaT<JPadding>(new JPadding(),
          "Padding of the 4 outer direction, in precent, >= 0",
          (P) => P != null && 
                P.Bottom >= 0 && P.Top >= 0 && P.Left >= 0 && P.Right >= 0 &&
                P.Bottom <= 100 && P.Top <= 100 && P.Left <= 100 && P.Right <= 100
        );

        public JMetaT<int> Templates_MaxCharPerLine { get; } = new JMetaT<int>(80,
           "Max character before breaking into new line, >= 0",
           (N) => N >= 0
         );

        public JMetaT<int> Templates_MaxValueLength { get; } = new JMetaT<int>(80,
           "Max character before clipping any value, >= 0",
           (N) => N >= 0
         );

        public JMetaT<string> Templates_TextFont { get; } = new JMetaT<string>("Arial",
           "Font to use on templates, might crash if not installed on OS", CH.stringy
        );

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


        /* ===============  Templates -> Duplex   =============== */

        public JMetaT<int> Templates_Duplex_GutterPrcnt { get; } = new JMetaT<int>(25,
           "Space between 2 pages in the middle, in precent, unless double page, >= 0",
           (P) => P >= 0 && P <= 100
         );

        public JMetaT<string> Templates_Duplex_Intro { get; } = new JMetaT<string>("Chapter start:\n{0}",
           "Text template to use on chapter start", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_Outro { get; } = new JMetaT<string>("Chapter end:\n{0}",
           "Text template to use on chapter end", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_BeforeDouble { get; } = new JMetaT<string>("Filler Before Double",
           "Text template to use when adjusting double-page", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_MakeEven { get; } = new JMetaT<string>("Filler After Chapter",
           "Text template to use when adjusting chapter even length", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_AntiSpoiler { get; } = new JMetaT<string>("Anti Spoiler",
           "Text template to use for anti spoiler pages", CH.stringy
        );


    }
}
