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
            _sf.addOrUpdateSetting("Cat1", "subcat2", "double", 25.6);
            _sf.addOrUpdateSetting("Cat1", "subcat2", "float", 25.6f);
            _sf.addOrUpdateSetting("Cat1", "subcat2", "namx4", "String");
            _sf.addOrUpdateSetting("Cat1", "subcat2", "enum", MangaPrinter.Core.SingleSideType.ANTI_SPOILER);

            SettingFile _sf2 = SettingFile.DeSerialize(_sf.Serialize());

            Assert.AreNotEqual(
               null,
               _sf2.getSetting<MangaPrinter.Core.SingleSideType>("Cat1", "subcat2", "enum")?.value
           );
            Assert.AreEqual(
                _sf.getSetting<MangaPrinter.Core.SingleSideType>("Cat1", "subcat2", "enum")?.value,
                _sf2.getSetting<MangaPrinter.Core.SingleSideType>("Cat1", "subcat2", "enum")?.value
            );

            Assert.AreNotEqual(
               _sf2.getSetting<float>("Cat1", "subcat2", "float")?.value,
              _sf2.getSetting<double>("Cat1", "subcat2", "double")?.value
          );
            Assert.AreEqual(
              _sf2.getSetting<float>("Cat1", "subcat2", "float")?.value,
             (float)_sf2.getSetting<double>("Cat1", "subcat2", "double")?.value
         );
        }
    }
}