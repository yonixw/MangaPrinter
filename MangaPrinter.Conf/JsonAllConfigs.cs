using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Conf
{
    class JsonAllConfigs
    {
        public Dictionary<string, JMeta> configs_meta = new Dictionary<string, JMeta>()
        {
            { "window.start_mode", new JMeta("centered", "Start position mode of the main GUI window. Can be 'centered' or 'fixed' ",
                    (O)=>CH.L<string>(O,(T)=> CH.inListP(T,"centered","fixed"))
             )},
            { "window.start_pos", new JMeta(new JPoint(), "Start position of the main GUI window. If mode='fixed'.") /* X,Y can be minus*/},
            { "window.size", new JMeta(new JSize(), "Start size of the main GUI window. dimentions must be >0",
                (O)=> CH.L<JSize>(O,(S)=>S.Height >0 && S.Width >0)
            )},
            { "window.font_size", new JMeta(15.5f, "Start font size for GUI window. must be >0, can be fraction",
                (O)=> CH.L<float>(O,(F)=>F>0.0f )
            )},
        };
    }
}
