using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public class ChapterAndFilesConfigSection
    {
        // Import
        public JsonConfigEntry ImportSortType { get; set; }
        public JsonConfigEntry ImportSmartSort { get; set; }
        public JsonConfigEntry ImportSubfolders { get; set; }
        public JsonConfigEntry ImportReadingDirections { get; set; }

        // Binding
        public JsonConfigEntry BindingType { get; set; }
        public JsonConfigEntry AddStartPage { get; set; }
        public JsonConfigEntry AddEndPage { get; set; }
        public JsonConfigEntry AddAntiSpoiler { get; set; }
        public JsonConfigEntry AntiSpoilerBatchCount { get; set; }


        private ChapterAndFilesConfigSection() { }
        internal ChapterAndFilesConfigSection(
            Dictionary<string, JsonRawConfigEntry> allRawEnteries,
            Dictionary<string, JsonConfigEntry> allEnteries
            )
        {
            JsonConfigValueManager v = new JsonConfigValueManager("Import", allRawEnteries, allEnteries);

            v.ConfigValR(this, nameof(ImportSortType), new JE() {
                EntryType = ConfigEntryTypes.Text,
                DefaultValue = "by_name",
                Description =
                    "When importing folder and files, how to sort them inside chapters." +
                    "Options are: \"by_name\",\"by_create_date\" ",
                Verifiers = JCV.asList(JCV.inListBuilder(new[] { "by_name", "by_create_date" }))
             });

            v.ConfigValR(this, nameof(ImportSmartSort), new JE() {
                EntryType = ConfigEntryTypes.TrueOrFalse,
                DefaultValue = "true",
                Description =
                    "If using sort by name, then sort numbers better in windows. because it will put 2.jpg after 11.jpg."
            });

            v.ConfigValR(this, nameof(ImportSubfolders), new JE() {
                EntryType = ConfigEntryTypes.TrueOrFalse,
                DefaultValue = "true",
                Description =
                    "Include subfolders (recursive) on import",
            });
        }
    }
}
