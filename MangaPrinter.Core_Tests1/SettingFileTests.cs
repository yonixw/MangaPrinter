using Microsoft.VisualStudio.TestTools.UnitTesting;
using MangaPrinter.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf.Tests
{
    [TestClass()]
    public class SettingFileTests
    {
        [TestMethod()]
        public void SerializeTest()
        {
            SettingFile _sf = new SettingFile();
            _sf.addOrUpdateSetting("Cat1", "subcat2", "nam1", true);
            _sf.addOrUpdateSetting("Cat1", "subcat2", "namx1", 123);
            _sf.addOrUpdateSetting("Cat1", "subcat2", "namx2", 25.6);
            _sf.addOrUpdateSetting("Cat1", "subcat2", "namx2", 25.6f);
            _sf.addOrUpdateSetting("Cat1", "subcat2", "namx2", "String");
            _sf.addOrUpdateSetting("Cat1", "subcat2", "enum", MangaPrinter.Core.SingleSideType.ANTI_SPOILER);
            string result = _sf.Serialize();
            Console.WriteLine(result);
        }
    }
}