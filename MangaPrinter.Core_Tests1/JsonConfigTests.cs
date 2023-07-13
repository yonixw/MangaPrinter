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
            JsonConfig conf = new JsonConfig();
            string json = conf.toJSON();
            Assert.AreEqual(json.Split('{').Length,2);
        }

       
    }
}