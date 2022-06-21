using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core
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

        private Dictionary<SingleSideType, String> _sideTextConsts = new Dictionary<SingleSideType, string>()
        {
            { SingleSideType.ANTI_SPOILER, "Anti Spoiler" },
            { SingleSideType.BEFORE_DOUBLE, "Filler Before Double" },
            { SingleSideType.INTRO, "Chapter start:\n{0}" },
            { SingleSideType.OUTRO, "Chapter end:\n{0}" },
            { SingleSideType.MAKE_EVEN, "Filler After Chapter" },
        };

        public string getSideTextConsts(SingleSideType type)
        {
            return _sideTextConsts[type] + _programVersion;
        }
    }
}
