using MangaPrinter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.WpfGUI.Utils
{
    public class BucketInfo {
        public int index = 0;
        public double value = 0;
        public int count = 0;
        public List<string> bucketPagesDesc = new List<string>();
        public List<MangaPage> bucketPages = new List<MangaPage>();
    };
}