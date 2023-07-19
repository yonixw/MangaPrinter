using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    class ConfigClasses
    {
    }

    public class JPoint
    {
        public int X { get; set;  } = 0;

        public int Y { get; set; } = 0;
    }

    public class JSize
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
    }

    public class JPage
    {
        public string name { get; set; } = "";
        public int WidthPixels { get; set; } = 1;
        public int HeightPixels { get; set; } = 1;
    }

    public class JPadding
    {
        public int Top { get; set; } = 0;
        public int Bottom { get; set; } = 0;
        public int Left { get; set; } = 0;
        public int Right { get; set; } = 0;


    }

}
