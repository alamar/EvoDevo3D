using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EvoDevo3D.Support
{
    public static class CommonUtil
    {
        public static List<T> Copy<T>(this List<T> list)
        {
            return list.GetRange(0, list.Count);
        }

        /// <summary>
        /// Encodes an argument for passing into a program
        /// </summary>
        /// <param name="original">The value that should be received by the program</param>
        /// <returns>The value which needs to be passed to the program for the original value 
        /// to come through</returns>
        public static string EncodeAsParameter(this string original)
        {
            if( string.IsNullOrEmpty(original))
                return original;
            string value = Regex.Replace(original, @"(\\*)" + "\"", @"$1\$0");
            value = Regex.Replace(value, @"^(.*\s.*?)(\\*)$", "\"$1$2$2\"");
            return value;
        }
    }
}
