using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MangaPrinter.Core
{

    

    public interface IBindBuilder
    {
        List<PrintPage> Build(List<MangaChapter> ch, bool startPage, bool endPage, int antiSpoiler = 0, bool parentFolder = true);
    }

    public class BookletChapterBuilder : IBindBuilder
    {


        List<PrintPage> IBindBuilder.Build(List<MangaChapter> ch, bool startPage, bool endPage, int antiSpoiler,bool parentFolder)
        {
            // looping from both end\start sides .... so middle can have some blanks
            // No way to save doubles... splitted into 2 faces singles

            throw new NotImplementedException();
        }
    }

    

   
}
