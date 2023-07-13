using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MangaPrinter.Conf
{
    public class CoreSettings
    {
        public static JsonConfig Instance = null;

        private CoreSettings()
        {
            // todo: load file, env or default
            Instance = new JsonConfig();
        }
    }

    public class JsonConfig
    {
        Dictionary<string, JsonConfigEntry> allEnteries { get; set; } =
                new Dictionary<string, JsonConfigEntry>();

        // Config Sections:
        public ChapterAndFilesConfigSection ChapterAndFiles { get; set; }

        // Config Init:

        public JsonConfig(
            Dictionary<string, JsonRawConfigEntry> allRawEnteries = null,
            List<Func<Dictionary<string, JsonRawConfigEntry>,bool>> FullConfigVerifiers = null )
        {
            if (allRawEnteries == null)
                allRawEnteries = new Dictionary<string, JsonRawConfigEntry>();
            if (FullConfigVerifiers == null)
                FullConfigVerifiers = new List<Func<Dictionary<string, JsonRawConfigEntry>, bool>>();

            ChapterAndFiles = new ChapterAndFilesConfigSection(allRawEnteries, allEnteries);
        }

        public string toJSON()
        {
            List<string> allKeys = allEnteries.Keys.ToList();
            Dictionary<string, string> allValues = new Dictionary<string, string>();

            for (int i = 0; i < allKeys.Count; i++)
            {
                allValues.Add(allEnteries[allKeys[i]].FullName, allEnteries[allKeys[i]].Value);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(allValues, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // Auto is minimal added data
            });
        }

        //static public SettingFile DeSerialize(string value)
        //{
        //    return Newtonsoft.Json.JsonConvert.DeserializeObject<SettingFile>(value, new JsonSerializerSettings
        //    {
        //        TypeNameHandling = TypeNameHandling.Auto // Auto is minimal added data
        //    });
        //}

        public string getSideTextConsts(int x) { return "TODO!!!" + x; }

        public string setProgramVersion(string x) { return "TODO!!!2" + x; }
    }

   
}
