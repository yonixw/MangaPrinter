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

        private CoreSettings() { }

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
    }

    public class SettingItemBase
    {
    }

    public class SettingItem<T> : SettingItemBase
    {
        T value = default(T);
        Dictionary<string, T> variations = new Dictionary<string, T>();

        public SettingItem(T _value) {
            value = _value;
        }
    }

    public class SettingCategory
    {
        public string Name { get; set; }
        public Dictionary<string, SettingSubCategory> SubCategories { get; set; } = new Dictionary<string, SettingSubCategory>();
    }

    public class SettingSubCategory
    {
        public string Name { get; set; }
        public Dictionary<string, SettingItemBase> Settings { get; set; } = new Dictionary<string, SettingItemBase>();
    }


    public class SettingFile
    {
        public Dictionary<string, SettingCategory> Categories { get; set; } = new Dictionary<string, SettingCategory>();

        public void addOrUpdateSetting<T>(string cat, string subcat, string name, T value)
        {
            SettingItemBase item = new SettingItem<T>(value);
            if(!Categories.ContainsKey(cat))
            {
                Categories.Add(cat, new SettingCategory());
            }

            if (!Categories[cat].SubCategories.ContainsKey(subcat))
            {
                Categories[cat].SubCategories.Add(subcat, new SettingSubCategory());
            }

            if (!Categories[cat].SubCategories[subcat].Settings.ContainsKey(name))
            {
                Categories[cat].SubCategories[subcat].Settings.Add(name, item);
            }
            else
            {
                Categories[cat].SubCategories[subcat].Settings[name] = item;
            }
        }

        public SettingItemBase getSettingBase(string cat, string subcat, string name)
        {
            if (!Categories.ContainsKey(cat)) return null;
            if (!Categories[cat].SubCategories.ContainsKey(subcat)) return null;
            if (!Categories[cat].SubCategories[subcat].Settings.ContainsKey(name)) return null;
            return Categories[cat].SubCategories[subcat].Settings[name];
        }

        public SettingItem<T> getSetting<T>(string cat, string subcat, string name)
        {
            if (!Categories.ContainsKey(cat)) return null;
            if (!Categories[cat].SubCategories.ContainsKey(subcat)) return null;
            if (!Categories[cat].SubCategories[subcat].Settings.ContainsKey(name)) return null;
            return Categories[cat].SubCategories[subcat].Settings[name] as SettingItem<T>;
        }

        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

    }
}


/*
 * 
 https://webcache.googleusercontent.com/search?q=cache:lHoxCxZaAKEJ:https://social.msdn.microsoft.com/Forums/vstudio/en-US/ad571139-7fff-4bfd-ba2f-8410b1b69fa4/xml-deserialize-works-with-net-452-breaks-with-46-xmlns-was-not-expected%3Fforum%3Dnetfxbcl+&cd=2&hl=iw&ct=clnk&gl=il

 string filename = @"C:\CARES_TFS\FIPS140\Cares\Cares.Test\TestData\FFM Sync\FFMStateAidCat38.xml";
 var xmlReader = new XmlTextReader(new FileStream(filename, FileMode.Open));
 var xmlserializer = new XmlSerializer(typeof(AccountTransferRequestPayloadType));
 var obj = xmlserializer.Deserialize(xmlReader) as AccountTransferRequestPayloadType;

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://at.dsh.cms.gov/extension/1.0")]
[System.Xml.Serialization.XmlRoot("AccountTransferRequest", IsNullable = false, Namespace = "http://at.dsh.cms.gov/exchange/1.0")]
public partial class AccountTransferRequestPayloadType : ComplexObjectType { ... }

*/
