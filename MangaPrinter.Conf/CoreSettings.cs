using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public class CoreSettings
    {
        public static CoreSettings Instance = new CoreSettings();

        public SettingFile allSettings = new SettingFile();

        private CoreSettings() { }

        void initSettingStructure()
        {
            allSettings.addOrUpdateSetting("Chapter and files", "Import", "Sort type", 
                new SettingOptionList<string>(new[] { "By Creation Date", "By Name" }.ToList(),1),
                "When importing folder and files, how to sort them inside chapters."
                );
            allSettings.addOrUpdateSetting("Chapter and files", "Import", "Smart 0-9 Sort", true,
                "If using sort by name, then sort numbers better in windows. because usally 2.jpg comes after 11.jpg."
                );
            allSettings.addOrUpdateSetting("Chapter and files", "Import", "Include subfolders",true,
                "Include all subfolders on import"
                );
            allSettings.addOrUpdateSetting("Chapter and files", "Import", "Import direction",
                new SettingOptionList<string>(new[] { "RTL (↼)", "LTR (⇀)" }.ToList(), 0),
                "When importing, what direction should new chapters be"
                );
            // ----------------
            allSettings.addOrUpdateSetting("Bindings", "General", "Binding type",
                new SettingOptionList<string>(new[] { "Duplex" }.ToList(), 0),
                "Binding type to prepare."
                );
            allSettings.addOrUpdateSetting("Bindings", "Chapter Pages", "Add start pages",true,
               "Add start page before each chapter"
               );
            allSettings.addOrUpdateSetting("Bindings", "Chapter Pages", "Add end pages", true,
               "Add end page after each chapter"
               );
            allSettings.addOrUpdateSetting("Bindings", "Chapter Pages", "Anti-Spoilers pages", true,
               "Add pages to hide content of chapters. Good to hide spoilers while stapling or cutting pages"
               );
            allSettings.addOrUpdateSetting("Bindings", "Chapter Pages", "Anti-Spoilers batch page count", 0,
               "How much pages to put between spoiler pages, usually the same page count your stapler or scissors can handle."
               );
        }


        // ------ Git Version Banner

        private string _programVersion = "???";

        public void setProgramVersion(string versionString)
        {
            _programVersion = string.Format("\n\n[MangaPrinter {0}]\n\n", versionString);
        }

        // ------- Template Text

        private Dictionary<int, String> _sideTextConsts = new Dictionary<int, string>()
        {
            { 1, "Anti Spoiler" },
            { 2, "Filler Before Double" },
            { 3, "Chapter start:\n{0}" },
            { 4, "Chapter end:\n{0}" },
            { 5, "Filler After Chapter" },
        };

        public string getSideTextConsts(int type)
        {
            return _sideTextConsts[type] + _programVersion;
        }

        // Todo:
        // Build your up to date config
        //    For each setting, choose preview to show (multiple? tabs?)
        // Config state: Found at X, not found, Saved succesully to X, Error saving..., Error loading...
        // If config exists:
        //    Recursive go through and update from it.
        //    If deprecated add deprecated flag
        // For each type: 
        //     (Bool, Int ... SingleSideType ...)
        //     Add editor/viewer
    }



   
}

