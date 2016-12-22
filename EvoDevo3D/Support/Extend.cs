using System.Collections.Generic;

namespace EvoDevo4
{
    public static class CommonUtil
    {
        public static List<T> Copy<T>(this List<T> list)
        {
            return list.GetRange(0, list.Count);
        }
    }
}
