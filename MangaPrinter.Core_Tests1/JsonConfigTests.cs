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
    public class JsonConfigTests
    {
        [TestMethod()]
        public void NoSubObjects()
        {
            const string test_fullname = "window.font_size";

            JsonConfig conf = new JsonConfig();
            string json = conf.toJSON();
            float before = conf.Get<float>(test_fullname);

            Assert.AreNotEqual(json,"");

            conf.Update("Test1", new Dictionary<string, object>()
            {
                { "Update",1 },
                {test_fullname, 12 }
            });
            string json2 = conf.toJSON();
            float after = conf.Get<float>(test_fullname);

            Assert.AreNotEqual(before, after);

            conf.ResetToDefault(test_fullname);
            Assert.AreNotEqual(conf.Get<float>(test_fullname), after);
        }

       
    }
}