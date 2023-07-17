using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    public class CoreConf
    {
        public static CoreConf I = new CoreConf(); // To other point here, and get hard types

        /* ===============  Information             =============== */

        public JMetaT<string> Info_GitVersion = new JMetaT<string>("",
            "Version of the software on build time", _readonly: true
        );

        /* ===============  Window                 =============== */

        public JMetaT<string> Window_StartMode = new JMetaT<string>("centered",
            "Start position mode of the main GUI window. Can be 'centered' or 'fixed' ",
            (T) => CH.inListP(T, "centered", "fixed")
        );

        public JMetaT<JPoint> Window_StartPos = new JMetaT<JPoint>(new JPoint(),
            "Start position of the main GUI window. If mode='fixed'."
        );

        public JMetaT<JSize> Window_StartSize = new JMetaT<JSize>(new JSize(),
          "Start size of the main GUI window. dimentions must be >0",
          (S) => S.Height > 0 && S.Width > 0
        );

        public JMetaT<float> Window_StartFontSize = new JMetaT<float>(15.5f,
          "Start font size for GUI window. must be >0, can be fraction",
          (F) => F > 0.0f
        );


        /* ===============  Templates -> Duplex -> Pages     =============== */

        public JMetaT<string> Templates_Duplex_Intro = new JMetaT<string>("Chapter start:\n{0}",
           "Text template to use on chapter start", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_Outro = new JMetaT<string>("Chapter end:\n{0}",
           "Text template to use on chapter end", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_BeforeDouble = new JMetaT<string>("Filler Before Double",
           "Text template to use when adjusting double-page", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_MakeEven = new JMetaT<string>("Filler After Chapter",
           "Text template to use when adjusting chapter even length", CH.stringy
        );

        public JMetaT<string> Templates_Duplex_AntiSpoiler = new JMetaT<string>("Anti Spoiler",
           "Text template to use for anti spoiler pages", CH.stringy
        );


    }
}
