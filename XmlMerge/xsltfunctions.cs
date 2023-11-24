using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace custom
{
     class TrxerFunctions
    {
        public static string RemoveAssemblyName(string asm)
        {
            if (asm.IndexOf(',') > 0)
            {
                return asm.Substring(0, asm.IndexOf(','));
            }
            else
            {
                return asm;
            }
        }
        public static string RemoveNamespace(string asm)
        {
            if (asm.IndexOf(',') > 0)
            {
                int coma = asm.IndexOf(',');
                return asm.Substring(coma + 2, asm.Length - coma - 2);
            }
            return asm;
        }
        public static string GetShortDateTime(string time)
        {
            if (string.IsNullOrEmpty(time))
            {
                return string.Empty;
            }

            return DateTime.Parse(time).ToString();
        }

        private static string ToExtactTime(double ms)
        {
            if (ms < 1000)
                return ms + " ms";

            if (ms >= 1000 && ms < 60000)
                return string.Format("{0:0.00} seconds", TimeSpan.FromMilliseconds(ms).TotalSeconds);

            if (ms >= 60000 && ms < 3600000)
                return string.Format("{0:0.00} minutes", TimeSpan.FromMilliseconds(ms).TotalMinutes);

            return string.Format("{0:0.00} hours", TimeSpan.FromMilliseconds(ms).TotalHours);
        }

        public static string ToExactTimeDefinition(string duration)
        {
            if (string.IsNullOrEmpty(duration))
            {
                return string.Empty;
            }

            return ToExtactTime(TimeSpan.Parse(duration).TotalMilliseconds);
        }

        public static string ToExactTimeDefinition(string start, string finish)
        {
            TimeSpan datetime = DateTime.Parse(finish) - DateTime.Parse(start);
            return ToExtactTime(datetime.TotalMilliseconds);
        }

        public static string CurrentDateTime()
        {
            return DateTime.Now.ToString();
        }

        public static string ExtractImageUrl(string text)
        {
           

             Match match = Regex.Match(text, "('|\")([^\\s]+(\\.(?i)(jpg|png|gif|bmp)))('|\")",
               RegexOptions.IgnoreCase | RegexOptions.NonBacktracking);


            if (match.Success)
            {
                return match.Value.Replace("\'", string.Empty).Replace("\"", string.Empty).Replace("\\", "\\\\");
            }
            return string.Empty;
        }

    }
}
