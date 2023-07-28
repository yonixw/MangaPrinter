using MangaPrinter.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaPrinter.Core.TemplateBuilders
{
    class TextUtils
    {

        public static string BreakSingleLineWords(string text, int maxCharsInLine)
        {
            //based on https://stackoverflow.com/a/15303265/1997873

            maxCharsInLine = Math.Max(1, maxCharsInLine); // Sanity check

            string[] allWords = text.Split(' ');
            int currentLineLength = 0;
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < allWords.Length; i++)
            {
                if (currentLineLength + allWords[i].Length + 1 > maxCharsInLine)
                {
                    builder.AppendLine();
                    builder.Append(allWords[i]);
                    builder.Append(" ");
                    currentLineLength = allWords[i].Length + 1;
                }
                else
                {
                    builder.Append(allWords[i]);
                    builder.Append(" ");
                    currentLineLength += allWords[i].Length + 1;
                }
            }
            return builder.ToString();
        }

        public static string PostProcess (string txt, bool canBreakLines)
        {
            string result = txt;

            if (canBreakLines && CoreConf.I.Templates_AddSwName)
            {
                string SoftwareTxt = "MangaPrinter " + CoreConf.I.Info_GitVersion.Get().Split(' ')[0];
                result += "\n\n" + SoftwareTxt;
            }

            if (!canBreakLines && result.Length > CoreConf.I.Templates_MaxValueLength )
            {
                result = result.Substring(0, CoreConf.I.Templates_MaxValueLength) + "...";
            } 
            else if (canBreakLines && result.Length > CoreConf.I.Templates_MaxCharPerLine)
            {
                result = String.Join(Environment.NewLine,
                    result.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    .ToList()
                    .Select(s => BreakSingleLineWords(s, CoreConf.I.Templates_MaxCharPerLine))
                );
            }

            return result;
        }
    }
}
