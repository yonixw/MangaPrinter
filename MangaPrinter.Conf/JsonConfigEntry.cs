using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public struct JsonRawConfigEntry
    {
        public string Value;
        public string ValueSource;
    }

    public class JsonConfigEntry
    {
        public string FullName { get; set; }
        public string Description { get; set; } = "(no info)";
        public ConfigEntryTypes EntryType { get; set; }
        public string DefaultValue { get; set; }
        public bool Deprecated { get; set; } = false;
        public string Value { get; set; } // After populated
        public string ValueSource { get; set; } // After populated: ENV, file, default
        public Func<string, bool>[] Verifiers { get; set; } = new Func<string, bool>[] { };

        public int asInt()
        {
            return Int32.Parse(Value);
        }

        public float asFloat()
        {
            return float.Parse(Value);
        }

        public string asString()
        {
            return Value;
        }
    }

    public class JE : JsonConfigEntry { }

    public enum ConfigEntryTypes
    {
        Text, // default
        LimitedText,

        WholePositiveNumber, WholeNumber,
        Number, PositiveNumber, 
        TrueOrFalse, 
        
    }

    public static class ConfigEntryTypesExtensions // https://stackoverflow.com/a/75765455/1997873
    {
        private static Dictionary<ConfigEntryTypes, Type> NetTypes =
            new Dictionary<ConfigEntryTypes, Type>
            {
                {ConfigEntryTypes.WholePositiveNumber, typeof(int)},
                {ConfigEntryTypes.WholeNumber, typeof(int)},
                {ConfigEntryTypes.Number, typeof(float)},
                {ConfigEntryTypes.PositiveNumber, typeof(float)},
                {ConfigEntryTypes.TrueOrFalse, typeof(bool)},
                {ConfigEntryTypes.LimitedText, typeof(string)},
                {ConfigEntryTypes.Text, typeof(string)}
            };

        private static Dictionary<ConfigEntryTypes, string> Descriptions =
             new Dictionary<ConfigEntryTypes, string>
             {
                { ConfigEntryTypes.WholePositiveNumber,
                        "Number bigger than -1, no fractions."},
                { ConfigEntryTypes.WholeNumber,
                     "Number, no fractions." },
                { ConfigEntryTypes.Number,"Number, including fractions" },
                { ConfigEntryTypes.PositiveNumber,"Number, including fractions, bigger or equal to 0" },
                { ConfigEntryTypes.TrueOrFalse,"Can only be true (yes) or false (no)" },
                { ConfigEntryTypes.LimitedText,"Text containing only english letters, numbers and some symbols" },
                { ConfigEntryTypes.Text,"Type any text" },
             };

        private static Dictionary<ConfigEntryTypes, Func<string, bool>> DefaultVerifiers =
            new Dictionary<ConfigEntryTypes, Func<string, bool>>
            {
                { ConfigEntryTypes.WholePositiveNumber,
                        (str) => int.TryParse(str, out _) && int.Parse(str) > -1 },
                { ConfigEntryTypes.WholeNumber, 
                    (str) => int.TryParse(str, out _) },
                { ConfigEntryTypes.Number, 
                    (str) => float.TryParse(str, out _)},
                { ConfigEntryTypes.PositiveNumber, 
                    (str) => float.TryParse(str, out _) && float.Parse(str) >=0  },
                { ConfigEntryTypes.TrueOrFalse, 
                    (str) => bool.TryParse(str,out _) },
                { ConfigEntryTypes.LimitedText,
                    (str) => !new Regex("[^a-zA-Z0-9\\ \\.\\-_]").Match(str).Success },
                { ConfigEntryTypes.Text,
                    (str)=>true },
            };

        public static string Description(this ConfigEntryTypes value)
        {
            return Descriptions[value];
        }

        public static Type NetType(this ConfigEntryTypes value)
        {
            return NetTypes[value];
        }

        public static Func<string, bool> DefaultVerifier(this ConfigEntryTypes value)
        {
            return DefaultVerifiers[value];
        }
    }


    public class JsonConfigValueManager
    {
        Dictionary<string, JsonRawConfigEntry> _allRawEnteries = null;
        Dictionary<string, JsonConfigEntry> _allEntries = null;
        string _PREFIX = null;

        public JsonConfigValueManager(
            string PREFIX,
            Dictionary<string, JsonRawConfigEntry> allRawEnteries,
            Dictionary<string, JsonConfigEntry> allEntries
            )
        {
            this._PREFIX = PREFIX;
            this._allRawEnteries = allRawEnteries;
            this._allEntries = allEntries;
        }

        ConfigEntryTypes[] textTypesArr = // additional actions like convert '\\ n'
           new ConfigEntryTypes[] { 
             ConfigEntryTypes.Text, ConfigEntryTypes.LimitedText 
        };

        public void ConfigValR<T>(
            T t,
            string name,
            JsonConfigEntry entry)
        {
            t.GetType().GetProperty(name).SetValue(t,ConfigVal(name, entry));
        }

        public JsonConfigEntry ConfigVal(
            string name,
            JsonConfigEntry entry)
        {
            entry.FullName = (_PREFIX + "." + name)
                .Replace("....", ".").Replace("...", ".").Replace("..",".");

            if (entry.Deprecated)
                entry.Description = "(@Deprecated) " + entry.Description;

            if (entry.Verifiers.Length == 0)
            {
                entry.Verifiers = JsonCommonVerifiers.asList(entry.EntryType.DefaultVerifier());
            }

            if (_allRawEnteries.ContainsKey(entry.FullName))
            {
                JsonRawConfigEntry rawEntry = _allRawEnteries[entry.FullName];

                bool isValid = true;
                for (int i = 0; i < entry.Verifiers.Length; i++)
                {
                    isValid = isValid && entry.Verifiers[i](rawEntry.Value);
                }

                if (isValid)
                {
                    entry.Value = rawEntry.Value;
                    entry.ValueSource = rawEntry.ValueSource;
                }
                else
                {
                    entry.Value = entry.DefaultValue;
                    entry.ValueSource = "Default Value, (Not valid: '" + rawEntry.Value + "' from " + rawEntry.ValueSource + " )";
                }
            }
            else
            {
                entry.Value = entry.DefaultValue;
                entry.ValueSource = "(Default Value)";
                _allEntries.Add(entry.FullName, entry);
            }

            
            if (textTypesArr.Contains(entry.EntryType))
            {
                entry.Value = entry.Value
                    .Replace("\\t", "\t")
                    .Replace("\\r", "\r")
                    .Replace("\\n", "\n");
            }

            return entry;
        }
    }

    public class JsonCommonVerifiers
    {
        public static Func<string, bool>[] asList(params Func<string, bool>[] funcs)
        {
            return funcs;
        }

        public static Func<string,bool> inListBuilder(string[] list)
        {
            string[] _list = list;
            return (str) => list.Contains(str);
        }
    }

    public class JCV: JsonCommonVerifiers { } // short alias
}
